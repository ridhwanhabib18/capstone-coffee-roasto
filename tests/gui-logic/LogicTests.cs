// Plain console tests for the GUI classes that have no screen: FrameParser, ProfileTransmitter.BuildChunks and
// RecordingSession. No test framework is used (no extra packages); run.ps1 compiles and runs this file.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CoffeeRoasto_GUI.Hardware;
using CoffeeRoasto_GUI.Profiles;
using CoffeeRoasto_GUI.Recording;

internal static class LogicTests
{
    private static int _checks;
    private static int _failures;

    private static void Check(bool ok, string name)
    {
        _checks++;
        if (!ok)
        {
            _failures++;
            Console.WriteLine("FAIL  " + name);
        }
    }

    private static bool Near(double a, double b)
    {
        return Math.Abs(a - b) < 1e-9;
    }

    // The sendtohardware loop of the original Form1.cs, reduced to the lines it wrote.
    private static List<string> LegacyChunks(List<string> values)
    {
        var lines = new List<string>();
        var payload = "";
        for (var i = 0; i < values.Count; i++)
        {
            if ((i + 1) % 10 == 0)
            {
                payload += values[i];
                lines.Add(payload);
                payload = "";
            }
            else
            {
                payload += values[i] + ",";
            }
        }
        if (payload != "")
        {
            lines.Add(payload);
        }
        return lines;
    }

    private static void FrameParserTests()
    {
        // The firmware prints a dot; the app expects an Indonesian culture (see docs/en/serial-protocol.md).
        CultureInfo.CurrentCulture = new CultureInfo("id-ID");

        var frame = FrameParser.Parse("@37.11A38.67B150.00C");
        Check(frame.Kind == LineKind.Telemetry, "frame is telemetry");
        Check(Near(frame.Telemetry.Bean, 37.11) && Near(frame.Telemetry.Drum, 38.67) && Near(frame.Telemetry.Reference, 150.0), "frame values");
        Check(Near(FrameParser.Parse("@37.11A38.67B0C").Telemetry.Reference, 0), "frame without reference");

        Check(FrameParser.Parse("X").Event == RoasterEvent.RoastingFinished, "event X");
        Check(FrameParser.Parse("V").Event == RoasterEvent.HeatingFinished, "event V");
        Check(FrameParser.Parse("Y").Event == RoasterEvent.BurnerOn, "event Y");
        Check(FrameParser.Parse("Z").Event == RoasterEvent.BurnerOff, "event Z");

        Check(FrameParser.Parse("").Kind == LineKind.Ignored, "empty line is ignored");
        Check(FrameParser.Parse(null).Kind == LineKind.Ignored, "null line is ignored");
        Check(FrameParser.Parse("hello").Kind == LineKind.Ignored, "noise is ignored");
        Check(FrameParser.Parse("@xA1B2C").Kind == LineKind.Ignored, "damaged number is ignored");
        Check(FrameParser.Parse("A1B2C@3").Kind == LineKind.Ignored, "markers out of order are ignored");
        Check(FrameParser.Parse("@" + new string('1', 200) + "A1B2C").Kind == LineKind.Telemetry, "long field (the old sbyte index overflowed)");
    }

    private static void ProfileChunkTests()
    {
        for (int n = 0; n <= 45; n++)
        {
            var values = Enumerable.Range(1, n).Select(i => (100 + i).ToString()).ToList();
            var chunks = ProfileTransmitter.BuildChunks(values).Select(c => c.Text).ToList();
            Check(chunks.SequenceEqual(LegacyChunks(values)), "chunks match the original algorithm, n=" + n);
        }

        var groups = ProfileTransmitter.BuildChunks(Enumerable.Range(1, 23).Select(i => i.ToString()).ToList()).ToList();
        Check(groups.Count == 3, "23 values make 3 lines");
        Check(groups[0].IsFullGroup && groups[1].IsFullGroup && !groups[2].IsFullGroup, "only full groups are followed by the 40 s wait");
        Check(groups[0].Text == "1,2,3,4,5,6,7,8,9,10", "a full group has no trailing comma");
        Check(groups[2].Text == "21,22,23,", "the short last group keeps its trailing comma");
        Check(ProfileTransmitter.GroupDelay == TimeSpan.FromSeconds(40), "group delay is 40 s");
    }

    private static void RecordingTests()
    {
        var session = new RecordingSession();
        session.Add(1, 2);
        Check(session.Samples.Count == 0, "nothing is recorded before START");

        session.Start();
        session.Add(150.5, 160.25);
        session.Add(151.0, 161.0);
        Check(session.IsRecording && session.Samples.Count == 2, "samples are recorded after START");
        Check(Near(session.Samples[0].beantemperature, 150.5) && Near(session.Samples[0].drumtemperature, 160.25), "sample values");

        session.Stop();
        session.Add(152, 162);
        Check(!session.IsRecording && session.Samples.Count == 2, "nothing is recorded after STOP");

        session.Start();
        Check(session.Samples.Count == 0, "START discards the previous recording");
    }

    private static int Main()
    {
        FrameParserTests();
        ProfileChunkTests();
        RecordingTests();

        Console.WriteLine(_failures == 0
            ? "ALL PASS (" + _checks + " checks)"
            : _failures + " of " + _checks + " checks FAILED");
        return _failures == 0 ? 0 : 1;
    }
}
