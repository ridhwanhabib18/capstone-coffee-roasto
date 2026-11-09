namespace CoffeeRoasto_GUI.Hardware
{
    /// <summary>One calibrated reading from the firmware, in degrees Celsius.</summary>
    internal struct Telemetry
    {
        public double Bean { get; set; }
        public double Drum { get; set; }
        public double Reference { get; set; }
    }

    /// <summary>Single-letter status lines sent by the firmware.</summary>
    internal enum RoasterEvent
    {
        RoastingFinished, // X
        HeatingFinished,  // V
        BurnerOn,         // Y
        BurnerOff         // Z
    }

    internal enum LineKind
    {
        Ignored,
        Telemetry,
        Event
    }

    /// <summary>The result of reading one line from the serial port.</summary>
    internal sealed class ParsedLine
    {
        public static readonly ParsedLine Ignored = new ParsedLine(LineKind.Ignored, default(Telemetry), default(RoasterEvent));

        private ParsedLine(LineKind kind, Telemetry telemetry, RoasterEvent roasterEvent)
        {
            Kind = kind;
            Telemetry = telemetry;
            Event = roasterEvent;
        }

        public LineKind Kind { get; }
        public Telemetry Telemetry { get; }
        public RoasterEvent Event { get; }

        public static ParsedLine FromTelemetry(Telemetry telemetry)
        {
            return new ParsedLine(LineKind.Telemetry, telemetry, default(RoasterEvent));
        }

        public static ParsedLine FromEvent(RoasterEvent roasterEvent)
        {
            return new ParsedLine(LineKind.Event, default(Telemetry), roasterEvent);
        }
    }
}
