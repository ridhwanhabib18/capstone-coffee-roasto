namespace CoffeeRoasto.ViewModels;

/// <summary>The points of the chart: elapsed seconds with the bean, drum and reference temperature.</summary>
public sealed class TemperatureHistory
{
    public List<double> Seconds { get; } = new();
    public List<double> Bean { get; } = new();
    public List<double> Drum { get; } = new();
    public List<double> Reference { get; } = new();

    /// <summary>Raised after a point is added or the history is cleared.</summary>
    public event Action? Changed;

    public void Add(double seconds, double bean, double drum, double reference)
    {
        Seconds.Add(seconds);
        Bean.Add(bean);
        Drum.Add(drum);
        Reference.Add(reference);
        Changed?.Invoke();
    }

    public void Clear()
    {
        Seconds.Clear();
        Bean.Clear();
        Drum.Clear();
        Reference.Clear();
        Changed?.Invoke();
    }
}
