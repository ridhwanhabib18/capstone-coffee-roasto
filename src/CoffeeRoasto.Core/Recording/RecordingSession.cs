namespace CoffeeRoasto.Recording;

/// <summary>One recorded sample. The property names are the column headers of the table and the export.</summary>
public sealed class Coffee
{
    public double BeanTemperature { get; init; }
    public double DrumTemperature { get; init; }
    public double ReferenceTemperature { get; init; }
    public double ElapsedSeconds { get; init; }
}

/// <summary>Collects temperatures while the operator is recording.</summary>
public sealed class RecordingSession
{
    private readonly List<Coffee> _samples = new();

    public bool IsRecording { get; private set; }

    /// <summary>The recorded samples, ready to be used as a data source.</summary>
    public IReadOnlyList<Coffee> Samples => _samples;

    /// <summary>Starts a new recording and discards the previous one.</summary>
    public void Start()
    {
        _samples.Clear();
        IsRecording = true;
    }

    public void Stop() => IsRecording = false;

    public void Add(double bean, double drum, double reference, double elapsedSeconds)
    {
        if (IsRecording)
        {
            _samples.Add(new Coffee
            {
                BeanTemperature = bean,
                DrumTemperature = drum,
                ReferenceTemperature = reference,
                ElapsedSeconds = elapsedSeconds
            });
        }
    }
}
