using System.Globalization;

namespace CoffeeRoasto.Hardware;

/// <summary>
/// Reads the lines sent by the firmware: a data frame "@bean A drum B reference C"
/// or a single-letter event (X, V, Y, Z). Pure logic, no UI and no serial port.
/// </summary>
public static class FrameParser
{
    /// <summary>
    /// The firmware prints a dot ("37.11"). The values are read with the invariant culture, so the result no longer
    /// depends on the regional settings of the PC (the v1 app needed an Indonesian culture for this).
    /// </summary>
    public static ParsedLine Parse(string? line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return ParsedLine.Ignored;
        }

        int start = line.IndexOf('@');
        int bean = line.IndexOf('A');
        int drum = line.IndexOf('B');
        int reference = line.IndexOf('C');

        if (start != -1 && bean != -1 && drum != -1 && reference != -1)
        {
            return TryParseFrame(line, start, bean, drum, reference);
        }

        if (line.Contains('X')) return ParsedLine.FromEvent(RoasterEvent.RoastingFinished);
        if (line.Contains('V')) return ParsedLine.FromEvent(RoasterEvent.HeatingFinished);
        if (line.Contains('Y')) return ParsedLine.FromEvent(RoasterEvent.BurnerOn);
        if (line.Contains('Z')) return ParsedLine.FromEvent(RoasterEvent.BurnerOff);

        return ParsedLine.Ignored;
    }

    private static ParsedLine TryParseFrame(string line, int start, int bean, int drum, int reference)
    {
        if (TryReadField(line, start, bean, out double beanValue)
            && TryReadField(line, bean, drum, out double drumValue)
            && TryReadField(line, drum, reference, out double referenceValue))
        {
            return ParsedLine.FromTelemetry(new Telemetry(beanValue, drumValue, referenceValue));
        }

        // A damaged frame is skipped; the next one arrives in a few seconds.
        return ParsedLine.Ignored;
    }

    /// <summary>Reads the text between two marker positions.</summary>
    private static bool TryReadField(string line, int markerBefore, int markerAfter, out double value)
    {
        value = 0;
        int length = markerAfter - markerBefore - 1;
        if (length < 0)
        {
            return false;
        }

        string text = line.Substring(markerBefore + 1, length);
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
