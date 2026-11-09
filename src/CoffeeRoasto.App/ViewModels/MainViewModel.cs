using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoffeeRoasto.Hardware;
using CoffeeRoasto.Profiles;
using CoffeeRoasto.Recording;

namespace CoffeeRoasto.ViewModels;

public enum NotificationLevel { Info, Success, Warning, Error }

/// <summary>A short message for the operator; the window shows it as a toast.</summary>
public sealed record Notification(string Title, string Message, NotificationLevel Level);

/// <summary>
/// Everything the window shows and every action behind its buttons. It has no reference to any control:
/// the window binds to it, and the chart and the toasts listen to its events.
/// </summary>
public sealed partial class MainViewModel : ObservableObject, IDisposable
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    private const string NoValue = "—";

    private readonly RoasterLink _link;
    private readonly ProfileTransmitter _transmitter;
    private readonly RecordingSession _recording = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly Queue<(double Seconds, double Bean)> _recent = new();
    private DataTableCollection? _sheets;
    private DispatcherTimer? _clockTimer;

    public MainViewModel()
    {
        _link = new RoasterLink();
        _link.LineReceived += line => Post(() => HandleLine(line));
        _link.Error += message => Post(() => Notify("Serial port", message, NotificationLevel.Error));

        _transmitter = new ProfileTransmitter(_link);
        _transmitter.Finished += error => Post(() =>
        {
            if (error == null) Notify("Profile", "The whole profile was sent.", NotificationLevel.Success);
            else Notify("Profile", error, NotificationLevel.Error);
        });

        ElapsedProvider = () => _stopwatch.Elapsed;
        RefreshPorts();
    }

    // ---- Wiring for the window (and for tools that drive the view model without hardware) ----------

    /// <summary>The time of the chart axis. Tools replace it to draw a whole roast without waiting.</summary>
    public Func<TimeSpan> ElapsedProvider { get; set; }

    /// <summary>Runs an action on the UI thread. Defaults to the dispatcher of the application.</summary>
    public Action<Action> Post { get; set; } = action =>
    {
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher == null || dispatcher.CheckAccess()) action();
        else dispatcher.BeginInvoke(action);
    };

    /// <summary>Asks the operator for a profile file. Set by the window.</summary>
    public Func<string?>? PickProfileFile { get; set; }

    /// <summary>Asks the operator where to save an export; the argument is a suggested file name.</summary>
    public Func<string, string?>? PickExportPath { get; set; }

    /// <summary>Raised for every message that should reach the operator.</summary>
    public event Action<Notification>? NotificationRaised;

    public TemperatureHistory History { get; } = new();

    // ---- Connection --------------------------------------------------------------------------------

    public ObservableCollection<string> Ports { get; } = new();

    [ObservableProperty] private string? _selectedPort;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisconnected), nameof(ConnectionText))]
    private bool _isConnected;

    public bool IsDisconnected => !IsConnected;
    public string ConnectionText => IsConnected ? "Connected" : "Not connected";

    [RelayCommand]
    private void RefreshPorts()
    {
        string? keep = SelectedPort;
        Ports.Clear();
        foreach (string port in RoasterLink.AvailablePorts().OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            Ports.Add(port);
        }
        SelectedPort = keep != null && Ports.Contains(keep) ? keep : Ports.FirstOrDefault();
    }

    [RelayCommand]
    private void Connect()
    {
        if (string.IsNullOrWhiteSpace(SelectedPort))
        {
            Notify("Connection", "Choose a COM port first.", NotificationLevel.Warning);
            return;
        }

        try
        {
            RestartClock();
            _link.Open(SelectedPort);
            IsConnected = true;
            ClearMeasurements();
            Notify("Connection", $"Connected to {SelectedPort}.", NotificationLevel.Success);
        }
        catch (Exception ex)
        {
            Notify("Connection", ex.Message, NotificationLevel.Error);
        }
    }

    [RelayCommand]
    private void Disconnect()
    {
        try
        {
            _link.Close();
        }
        catch (Exception ex)
        {
            Notify("Connection", ex.Message, NotificationLevel.Error);
        }

        _clockTimer?.Stop();
        IsConnected = false;
        ClearMeasurements();
        ResetLamps();
        TableSource = null;
        TableTitle = "Data table";
    }

    // ---- Readings ----------------------------------------------------------------------------------

    [ObservableProperty] private string _beanText = NoValue;
    [ObservableProperty] private string _drumText = NoValue;
    [ObservableProperty] private string _referenceText = NoValue;
    [ObservableProperty] private string _deltaText = NoValue;
    [ObservableProperty] private string _rateText = NoValue;
    [ObservableProperty] private string _elapsedText = "00:00";

    private void ClearMeasurements()
    {
        History.Clear();
        _recent.Clear();
        BeanText = DrumText = ReferenceText = DeltaText = RateText = NoValue;
        ElapsedText = "00:00";
    }

    /// <summary>Handles one line received from the roaster. Must run on the UI thread.</summary>
    public void HandleLine(string line)
    {
        ParsedLine parsed = FrameParser.Parse(line);
        switch (parsed.Kind)
        {
            case LineKind.Telemetry:
                ShowTelemetry(parsed.Telemetry);
                break;
            case LineKind.Event:
                HandleEvent(parsed.Event);
                break;
        }
    }

    private void ShowTelemetry(Telemetry reading)
    {
        double seconds = ElapsedProvider().TotalSeconds;
        _recording.Add(reading.Bean, reading.Drum, reading.Reference, seconds);

        bool hasReference = reading.Reference > 0;
        BeanText = Format(reading.Bean);
        DrumText = Format(reading.Drum);
        ReferenceText = hasReference ? Format(reading.Reference) : NoValue;
        DeltaText = hasReference ? Signed(reading.Bean - reading.Reference) : NoValue;
        RateText = RateOfRise(seconds, reading.Bean);
        ElapsedText = Clock(seconds);

        History.Add(seconds, reading.Bean, reading.Drum, reading.Reference);
    }

    /// <summary>Bean temperature change in °C per minute over the last minute.</summary>
    private string RateOfRise(double seconds, double bean)
    {
        _recent.Enqueue((seconds, bean));
        while (_recent.Count > 1 && seconds - _recent.Peek().Seconds > 60)
        {
            _recent.Dequeue();
        }

        (double oldSeconds, double oldBean) = _recent.Peek();
        double span = seconds - oldSeconds;
        return span >= 8 ? Signed((bean - oldBean) / span * 60) : NoValue;
    }

    private static string Format(double value) => value.ToString("0.0", Invariant);
    private static string Signed(double value) => (value >= 0 ? "+" : "") + value.ToString("0.0", Invariant);
    private static string Clock(double seconds) => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss", Invariant);

    // ---- Lamps -------------------------------------------------------------------------------------

    [ObservableProperty] private bool _burnerLamp;
    [ObservableProperty] private bool _drumLamp;
    [ObservableProperty] private bool _blowerLamp;
    [ObservableProperty] private bool _coolerLamp;
    [ObservableProperty] private bool _valveLamp;
    [ObservableProperty] private bool _recordLamp;
    [ObservableProperty] private bool _heatingLamp;
    [ObservableProperty] private bool _autoLamp;
    [ObservableProperty] private bool _pidLamp;

    private void ResetLamps()
    {
        BurnerLamp = DrumLamp = BlowerLamp = CoolerLamp = ValveLamp = false;
        RecordLamp = HeatingLamp = AutoLamp = PidLamp = false;
    }

    private void HandleEvent(RoasterEvent roasterEvent)
    {
        switch (roasterEvent)
        {
            case RoasterEvent.RoastingFinished:
                Notify("Roasting finished", "The cooler is running; the drum keeps turning.", NotificationLevel.Success);
                AutoLamp = PidLamp = BlowerLamp = BurnerLamp = false;
                DrumLamp = CoolerLamp = true;
                break;

            case RoasterEvent.HeatingFinished:
                Notify("Heating finished", "The drum reached its warm-up time.", NotificationLevel.Success);
                HeatingLamp = BlowerLamp = BurnerLamp = CoolerLamp = false;
                DrumLamp = true;
                break;

            case RoasterEvent.BurnerOn:
                BurnerLamp = true;
                break;

            case RoasterEvent.BurnerOff:
                BurnerLamp = false;
                break;
        }
    }

    // ---- Manual control ----------------------------------------------------------------------------

    /// <summary>One button of the manual panel: the command it sends and the lamp it changes.</summary>
    [RelayCommand]
    private void Switch(string name)
    {
        switch (name)
        {
            case "BurnerOn": SwitchActuator(RoasterCommand.BurnerOn, v => BurnerLamp = v, true); break;
            case "BurnerOff": SwitchActuator(RoasterCommand.BurnerOff, v => BurnerLamp = v, false); break;
            case "DrumOn": SwitchActuator(RoasterCommand.DrumMotorOn, v => DrumLamp = v, true); break;
            case "DrumOff": SwitchActuator(RoasterCommand.DrumMotorOff, v => DrumLamp = v, false); break;
            case "BlowerOn": SwitchActuator(RoasterCommand.BlowerOn, v => BlowerLamp = v, true); break;
            case "BlowerOff": SwitchActuator(RoasterCommand.BlowerOff, v => BlowerLamp = v, false); break;
            case "CoolerOn": SwitchActuator(RoasterCommand.CoolerOn, v => CoolerLamp = v, true); break;
            case "CoolerOff": SwitchActuator(RoasterCommand.CoolerOff, v => CoolerLamp = v, false); break;
            case "ValveOpen": SwitchActuator(RoasterCommand.ValveOpen, v => ValveLamp = v, true); break;
            case "ValveClose": SwitchActuator(RoasterCommand.ValveClose, v => ValveLamp = v, false); break;
            default: throw new ArgumentException("Unknown actuator command: " + name, nameof(name));
        }
    }

    /// <summary>Sends the command and, only when it was sent, changes the lamp.</summary>
    private void SwitchActuator(string command, Action<bool> setLamp, bool on)
    {
        if (_link.Send(command))
        {
            setLamp(on);
        }
    }

    [ObservableProperty] private string _setpointText = "";

    /// <summary>Gives the PID a set point typed by the operator.</summary>
    [RelayCommand]
    private void SendSetpoint()
    {
        string text = SetpointText.Trim();
        if (!double.TryParse(text, NumberStyles.Float, Invariant, out double value) || value <= 0)
        {
            Notify("Set point", "Type a temperature in °C, for example 150.", NotificationLevel.Warning);
            return;
        }

        if (_link.Send(value.ToString(Invariant)))
        {
            SetpointText = "";
        }
    }

    // ---- The three automatic modes -----------------------------------------------------------------

    [RelayCommand]
    private void StartHeating()
    {
        if (!_link.Send(RoasterCommand.HeatingStart)) return;
        HeatingLamp = DrumLamp = BlowerLamp = ValveLamp = true;
        CoolerLamp = false;
    }

    [RelayCommand]
    private void StopHeating()
    {
        if (!_link.Send(RoasterCommand.Stop)) return;
        HeatingLamp = BlowerLamp = BurnerLamp = CoolerLamp = false;
        DrumLamp = ValveLamp = true;
    }

    [RelayCommand]
    private void StartAuto()
    {
        if (!_link.IsOpen) return;
        BeginRun();
        if (!_link.Send(RoasterCommand.AutoStart)) return;
        AutoLamp = DrumLamp = BlowerLamp = true;
        CoolerLamp = false;
    }

    [RelayCommand]
    private void StopAuto()
    {
        if (!_link.Send(RoasterCommand.Stop)) return;
        AutoLamp = BlowerLamp = BurnerLamp = CoolerLamp = ValveLamp = false;
        DrumLamp = true;
    }

    [RelayCommand]
    private void StartPid()
    {
        if (!_link.IsOpen) return;
        BeginRun();
        if (!_link.Send(RoasterCommand.PidStart)) return;
        PidLamp = DrumLamp = BlowerLamp = ValveLamp = true;
        CoolerLamp = false;
    }

    [RelayCommand]
    private void StopPid()
    {
        if (!_link.Send(RoasterCommand.Stop)) return;
        PidLamp = BlowerLamp = BurnerLamp = CoolerLamp = false;
        DrumLamp = ValveLamp = true;
    }

    /// <summary>A new run starts: an empty chart and the clock from zero.</summary>
    private void BeginRun()
    {
        ClearMeasurements();
        RestartClock();
    }

    // ---- Reference profile -------------------------------------------------------------------------

    public ObservableCollection<string> Sheets { get; } = new();

    [ObservableProperty] private string _profilePath = "";

    [ObservableProperty] private string? _selectedSheet;

    partial void OnSelectedSheetChanged(string? value)
    {
        if (value != null && _sheets != null && _sheets.Contains(value))
        {
            TableSource = _sheets[value]!.DefaultView;
            TableTitle = $"Profile · {value}";
        }
    }

    [RelayCommand]
    private void BrowseProfile()
    {
        string? path = PickProfileFile?.Invoke();
        if (path == null) return;

        try
        {
            _sheets = ProfileReader.Read(path);
            ProfilePath = path;
            Sheets.Clear();
            foreach (DataTable sheet in _sheets) Sheets.Add(sheet.TableName);
            SelectedSheet = Sheets.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Notify("Profile", ex.Message, NotificationLevel.Error);
        }
    }

    /// <summary>Sends the chosen sheet to the firmware. PID mode must already be on.</summary>
    [RelayCommand]
    private void SendProfile()
    {
        DataTable? sheet = SelectedSheet != null && _sheets != null && _sheets.Contains(SelectedSheet)
            ? _sheets[SelectedSheet]
            : null;
        if (sheet == null)
        {
            Notify("Profile", "Choose a profile file and a sheet first.", NotificationLevel.Warning);
            return;
        }

        try
        {
            _transmitter.Start(sheet);
            Notify("Profile", "Sending the profile: ten set points every 40 seconds.", NotificationLevel.Info);
        }
        catch (Exception ex)
        {
            Notify("Profile", ex.Message, NotificationLevel.Error);
        }
    }

    // ---- Recording ---------------------------------------------------------------------------------

    [ObservableProperty] private object? _tableSource;

    [ObservableProperty] private string _tableTitle = "Data table";

    [RelayCommand]
    private void StartRecording()
    {
        _recording.Start();
        TableSource = null;
        TableTitle = "Recording…";
        RecordLamp = true;
    }

    [RelayCommand]
    private void StopRecording()
    {
        _recording.Stop();
        TableSource = _recording.Samples.ToList();
        TableTitle = $"Recording · {_recording.Samples.Count} samples";
        RecordLamp = false;
    }

    [RelayCommand]
    private void ExportRecording()
    {
        if (_recording.Samples.Count == 0)
        {
            Notify("Export", "There is nothing recorded yet.", NotificationLevel.Warning);
            return;
        }

        string? path = PickExportPath?.Invoke($"coffee-roast-{DateTime.Now:yyyyMMdd-HHmm}.xlsx");
        if (path == null) return;

        try
        {
            ExcelExporter.Export(path, _recording.Samples);
            Notify("Export", $"Saved {Path.GetFileName(path)}.", NotificationLevel.Success);
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Notify("Export", ex.Message, NotificationLevel.Error);
        }
    }

    // ---- Helpers -----------------------------------------------------------------------------------

    private void RestartClock()
    {
        _stopwatch.Restart();
        if (_clockTimer == null)
        {
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += (_, _) => ElapsedText = Clock(ElapsedProvider().TotalSeconds);
        }
        _clockTimer.Start();
    }

    private void Notify(string title, string message, NotificationLevel level)
    {
        NotificationRaised?.Invoke(new Notification(title, message, level));
    }

    public void Dispose()
    {
        _clockTimer?.Stop();
        _link.Dispose();
    }
}
