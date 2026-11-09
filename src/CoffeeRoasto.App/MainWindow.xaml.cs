using System.Windows;
using CoffeeRoasto.Recording;
using CoffeeRoasto.Profiles;
using CoffeeRoasto.ViewModels;
using Microsoft.Win32;
using ScottPlot;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace CoffeeRoasto;

public partial class MainWindow : FluentWindow
{
    private readonly MainViewModel _viewModel;
    private readonly SnackbarService _snackbar = new();

    public MainWindow(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();

        _snackbar.SetSnackbarPresenter(SnackbarHost);
        viewModel.NotificationRaised += ShowNotification;
        viewModel.PickProfileFile = PickProfileFile;
        viewModel.PickExportPath = PickExportPath;
        viewModel.History.Changed += DrawChart;

        StylePlot();
        DrawChart();
        Closed += (_, _) => viewModel.Dispose();
    }

    // ---- Theme -------------------------------------------------------------------------------------

    /// <summary>Switches between the light and the dark Fluent theme.</summary>
    public void ApplyTheme(ApplicationTheme theme)
    {
        ApplicationThemeManager.Apply(theme);
        ThemeButton.Content = theme == ApplicationTheme.Dark ? "Light" : "Dark";
        ThemeButton.Icon = new SymbolIcon(theme == ApplicationTheme.Dark ? SymbolRegular.WeatherSunny24 : SymbolRegular.WeatherMoon24);
        StylePlot();
        Plot.Refresh();
    }

    private void OnThemeClick(object sender, RoutedEventArgs e)
    {
        ApplyTheme(ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark);
    }

    // ---- Chart -------------------------------------------------------------------------------------

    private static readonly ScottPlot.Color BeanColor = ScottPlot.Color.FromHex("#D13438");
    private static readonly ScottPlot.Color DrumColor = ScottPlot.Color.FromHex("#0F6CBD");
    private static readonly ScottPlot.Color ReferenceColor = ScottPlot.Color.FromHex("#107C10");

    /// <summary>The colours of the plot follow the theme of the window.</summary>
    private void StylePlot()
    {
        bool dark = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark;
        ScottPlot.Color surface = ScottPlot.Color.FromHex(dark ? "#2B2B2B" : "#FFFFFF");
        ScottPlot.Color text = ScottPlot.Color.FromHex(dark ? "#D6D6D6" : "#444444");
        ScottPlot.Color grid = ScottPlot.Color.FromHex(dark ? "#3C3C3C" : "#E8E8E8");

        Plot plot = Plot.Plot;
        plot.FigureBackground.Color = surface;
        plot.DataBackground.Color = surface;
        plot.Axes.Color(text);
        plot.Grid.MajorLineColor = grid;
        plot.Legend.BackgroundColor = surface;
        plot.Legend.FontColor = text;
        plot.Legend.OutlineColor = grid;
        plot.Axes.Left.Label.Text = "Temperature (°C)";
        plot.Axes.Bottom.Label.Text = "Time (mm:ss)";
        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic
        {
            LabelFormatter = seconds => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss")
        };
    }

    /// <summary>Redraws the three lines from the history of the view model.</summary>
    private void DrawChart()
    {
        TemperatureHistory h = _viewModel.History;
        Plot plot = Plot.Plot;
        plot.Clear();

        if (h.Seconds.Count > 0)
        {
            double[] seconds = h.Seconds.ToArray();

            var bean = plot.Add.ScatterLine(seconds, h.Bean.ToArray());
            bean.Color = BeanColor; bean.LineWidth = 2.5f; bean.LegendText = "Bean";

            var drum = plot.Add.ScatterLine(seconds, h.Drum.ToArray());
            drum.Color = DrumColor; drum.LineWidth = 2.5f; drum.LegendText = "Drum";

            var reference = plot.Add.ScatterLine(seconds, h.Reference.ToArray());
            reference.Color = ReferenceColor; reference.LineWidth = 2f; reference.LinePattern = LinePattern.Dashed;
            reference.LegendText = "Reference";

            plot.ShowLegend(Alignment.UpperLeft);
            plot.Axes.AutoScale();
        }
        else
        {
            plot.HideLegend();
            plot.Axes.SetLimits(0, 600, 0, 250);
        }

        Plot.Refresh();
    }

    // ---- Data table --------------------------------------------------------------------------------

    private static readonly Dictionary<string, string> ColumnTitles = new()
    {
        ["ElapsedSeconds"] = "Time (s)",
        ["BeanTemperature"] = "Bean (°C)",
        ["DrumTemperature"] = "Drum (°C)",
        ["ReferenceTemperature"] = "Reference (°C)",
        ["beantemperature"] = "Bean set point (°C)"
    };

    private void OnAutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
    {
        string name = e.PropertyName;
        if (ColumnTitles.TryGetValue(name, out string? title))
        {
            e.Column.Header = title;
        }
        if (e.Column is System.Windows.Controls.DataGridTextColumn text && e.PropertyType == typeof(double))
        {
            text.Binding.StringFormat = "0.0";
        }
        e.Column.Width = new System.Windows.Controls.DataGridLength(1, System.Windows.Controls.DataGridLengthUnitType.Star);
    }

    // ---- Dialogs and toasts ------------------------------------------------------------------------

    private string? PickProfileFile()
    {
        var dialog = new OpenFileDialog { Filter = ProfileReader.FileDialogFilter, Title = "Choose a reference profile" };
        return dialog.ShowDialog(this) == true ? dialog.FileName : null;
    }

    private string? PickExportPath(string suggestedName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Excel workbook (*.xlsx)|*.xlsx",
            FileName = suggestedName,
            Title = "Export the recording"
        };
        return dialog.ShowDialog(this) == true ? dialog.FileName : null;
    }

    private void ShowNotification(Notification notification)
    {
        (ControlAppearance appearance, SymbolRegular icon) = notification.Level switch
        {
            NotificationLevel.Success => (ControlAppearance.Success, SymbolRegular.CheckmarkCircle24),
            NotificationLevel.Warning => (ControlAppearance.Caution, SymbolRegular.Warning24),
            NotificationLevel.Error => (ControlAppearance.Danger, SymbolRegular.ErrorCircle24),
            _ => (ControlAppearance.Info, SymbolRegular.Info24)
        };

        _snackbar.Show(notification.Title, notification.Message, appearance, new SymbolIcon(icon), TimeSpan.FromSeconds(5));
    }
}
