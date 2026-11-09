using System.Windows;
using CoffeeRoasto.ViewModels;

namespace CoffeeRoasto;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = new MainWindow(new MainViewModel());
        MainWindow = window;
        window.Show();
    }
}
