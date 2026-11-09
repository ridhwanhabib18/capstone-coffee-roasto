using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoffeeRoasto.Controls;

public partial class LampIndicator : UserControl
{
    public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
        nameof(IsOn), typeof(bool), typeof(LampIndicator),
        new PropertyMetadata(false, (d, _) => ((LampIndicator)d).UpdateLook()));

    private static readonly Brush OnBrush = Freeze(new SolidColorBrush(Color.FromRgb(0x22, 0xC5, 0x5E)));
    private static readonly Brush OffBrush = Freeze(new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF)));

    public LampIndicator()
    {
        InitializeComponent();
        UpdateLook();
    }

    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    private void UpdateLook()
    {
        Body.Fill = IsOn ? OnBrush : OffBrush;
        Halo.Opacity = IsOn ? 1 : 0;
    }

    private static Brush Freeze(Brush brush)
    {
        brush.Freeze();
        return brush;
    }
}
