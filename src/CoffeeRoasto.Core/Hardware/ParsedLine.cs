namespace CoffeeRoasto.Hardware;

/// <summary>One calibrated reading from the firmware, in degrees Celsius.</summary>
public readonly record struct Telemetry(double Bean, double Drum, double Reference);

/// <summary>Single-letter status lines sent by the firmware.</summary>
public enum RoasterEvent
{
    RoastingFinished, // X
    HeatingFinished,  // V
    BurnerOn,         // Y
    BurnerOff         // Z
}

public enum LineKind
{
    Ignored,
    Telemetry,
    Event
}

/// <summary>The result of reading one line from the serial port.</summary>
public sealed class ParsedLine
{
    public static readonly ParsedLine Ignored = new(LineKind.Ignored, default, default);

    private ParsedLine(LineKind kind, Telemetry telemetry, RoasterEvent roasterEvent)
    {
        Kind = kind;
        Telemetry = telemetry;
        Event = roasterEvent;
    }

    public LineKind Kind { get; }
    public Telemetry Telemetry { get; }
    public RoasterEvent Event { get; }

    public static ParsedLine FromTelemetry(Telemetry telemetry) => new(LineKind.Telemetry, telemetry, default);

    public static ParsedLine FromEvent(RoasterEvent roasterEvent) => new(LineKind.Event, default, roasterEvent);
}
