// Draws screenshots of the real Coffee Roasto window without a roaster.
//
// The window and its view model are created exactly as in the application and fed the lines that the firmware
// simulation produced (tests/firmware-sim/golden/scenario12.txt: a whole roast following a recorded profile in a
// closed loop). Only the serial port is missing, so the connected state is set directly and the lamps of the
// running mode are lit the way the button commands would after a successful send.
//
// usage: Screenshots <trace.txt> <profile.xlsx> <output-dir>
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CoffeeRoasto.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace CoffeeRoasto.Tools;

internal static class Program
{
    private sealed record TraceEvent(long Millis, string Kind, string Text);

    private static MainWindow _window = null!;
    private static MainViewModel _vm = null!;
    private static string _out = "";
    private static string _frames = "";
    private static int _frameNumber;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: Screenshots <trace.txt> <profile.xlsx> <output-dir>");
            return 2;
        }

        _out = Path.GetFullPath(args[2]);
        _frames = Path.Combine(_out, "_frames");
        Directory.CreateDirectory(_frames);
        foreach (string old in Directory.GetFiles(_frames, "*.png")) File.Delete(old);

        List<TraceEvent> trace = ReadTrace(args[0]);

        var app = new App();
        app.InitializeComponent();
        _vm = new MainViewModel { Post = action => action() };
        _window = new MainWindow(_vm) { WindowBackdropType = WindowBackdropType.None };
        _window.Show();

        Shot("01-start");

        _vm.Ports.Add("COM3");
        _vm.SelectedPort = "COM3";
        _vm.IsConnected = true;
        Shot("02-connected");

        _vm.PickProfileFile = () => Path.GetFullPath(args[1]);
        _vm.BrowseProfileCommand.Execute(null);
        Shot("03-profile-loaded");

        Replay(trace);
        return 0;
    }

    private static void Replay(List<TraceEvent> trace)
    {
        long start = trace.First(e => e.Kind == "frame").Millis;
        long now = start;
        _vm.ElapsedProvider = () => TimeSpan.FromMilliseconds(now - start);

        // PID mode started by the operator, recording on.
        _vm.PidLamp = _vm.DrumLamp = _vm.BlowerLamp = _vm.ValveLamp = true;
        _vm.StartRecordingCommand.Execute(null);

        int frames = 0;
        long lastMinute = -1;
        foreach (TraceEvent e in trace)
        {
            now = e.Millis;
            _vm.HandleLine(e.Text);
            if (e.Kind != "frame") continue;

            frames++;
            if (frames % 20 == 0) SaveFrame();

            long minute = (e.Millis - start) / 60000;
            if (minute != lastMinute && (minute == 5 || minute == 15))
            {
                Shot("04-pid-running-" + minute.ToString("00") + "min");
                lastMinute = minute;
            }
        }

        _vm.StopRecordingCommand.Execute(null);
        _vm.PidLamp = false;
        SaveFrame();
        Shot("05-roast-finished");

        _window.ApplyTheme(ApplicationTheme.Dark);
        Settle(TimeSpan.FromMilliseconds(1500));
        Shot("06-dark-theme");
    }

    private static List<TraceEvent> ReadTrace(string path)
    {
        var list = new List<TraceEvent>();
        foreach (string line in File.ReadAllLines(path))
        {
            // t=12345 frame @150.00A160.00B150.00C   |   t=12345 send Y
            if (!line.StartsWith("t=")) continue;
            int space = line.IndexOf(' ');
            long millis = long.Parse(line.AsSpan(2, space - 2), CultureInfo.InvariantCulture);
            string rest = line[(space + 1)..];

            if (rest.StartsWith("frame "))
            {
                list.Add(new TraceEvent(millis, "frame", rest[6..]));
            }
            else if (rest is "send Y" or "send Z" or "send X")
            {
                list.Add(new TraceEvent(millis, rest[5..], rest[5..]));
            }
        }
        return list;
    }

    // ---- Pictures ----------------------------------------------------------------------------------

    /// <summary>Lets the dispatcher run for a while, so colour transitions of the theme can finish.</summary>
    private static void Settle(TimeSpan time)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer(DispatcherPriority.Background, _window.Dispatcher) { Interval = time };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            frame.Continue = false;
        };
        timer.Start();
        Dispatcher.PushFrame(frame);
    }

    private static BitmapSource Render()
    {
        // Let the bindings, the layout and the chart finish before the picture is taken.
        _window.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        // Mica is off for the picture, so the window needs a solid background that follows the theme.
        _window.Background = (Brush)_window.FindResource("ApplicationBackgroundBrush");
        _window.UpdateLayout();

        const double scale = 1.5;
        int width = (int)(_window.ActualWidth * scale);
        int height = (int)(_window.ActualHeight * scale);
        var bitmap = new RenderTargetBitmap(width, height, 96 * scale, 96 * scale, PixelFormats.Pbgra32);
        bitmap.Render(_window);
        return bitmap;
    }

    private static void Save(BitmapSource bitmap, string path)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using FileStream stream = File.Create(path);
        encoder.Save(stream);
    }

    private static void Shot(string name)
    {
        Save(Render(), Path.Combine(_out, name + ".png"));
        Console.WriteLine("saved " + name + ".png");
    }

    private static void SaveFrame()
    {
        Save(Render(), Path.Combine(_frames, $"frame{_frameNumber++:000}.png"));
    }
}
