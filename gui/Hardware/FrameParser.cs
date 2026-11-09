using System;

namespace CoffeeRoasto_GUI.Hardware
{
    /// <summary>
    /// Reads the lines sent by the firmware: a data frame "@bean A drum B reference C"
    /// or a single-letter event (X, V, Y, Z). Pure logic, no UI and no serial port.
    /// </summary>
    internal static class FrameParser
    {
        /// <summary>
        /// The values are divided by 100 on purpose: the firmware prints "37.11" and, on a PC with an
        /// Indonesian culture, Convert.ToDouble reads the dot as a thousands separator (3711).
        /// See "Decimal separator" in docs/en/serial-protocol.md before changing this.
        /// </summary>
        private const double CultureScale = 100.0;

        public static ParsedLine Parse(string line)
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

            if (line.IndexOf('X') != -1) return ParsedLine.FromEvent(RoasterEvent.RoastingFinished);
            if (line.IndexOf('V') != -1) return ParsedLine.FromEvent(RoasterEvent.HeatingFinished);
            if (line.IndexOf('Y') != -1) return ParsedLine.FromEvent(RoasterEvent.BurnerOn);
            if (line.IndexOf('Z') != -1) return ParsedLine.FromEvent(RoasterEvent.BurnerOff);

            return ParsedLine.Ignored;
        }

        private static ParsedLine TryParseFrame(string line, int start, int bean, int drum, int reference)
        {
            try
            {
                var telemetry = new Telemetry
                {
                    Bean = ReadField(line, start, bean),
                    Drum = ReadField(line, bean, drum),
                    Reference = ReadField(line, drum, reference)
                };
                return ParsedLine.FromTelemetry(telemetry);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is ArgumentOutOfRangeException)
            {
                // A damaged frame is skipped; the next one arrives in a few seconds.
                return ParsedLine.Ignored;
            }
        }

        /// <summary>Reads the text between two marker positions.</summary>
        private static double ReadField(string line, int markerBefore, int markerAfter)
        {
            string text = line.Substring(markerBefore + 1, markerAfter - markerBefore - 1);
            return Convert.ToDouble(text) / CultureScale;
        }
    }
}
