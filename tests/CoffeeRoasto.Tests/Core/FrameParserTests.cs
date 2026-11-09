using System.Globalization;
using CoffeeRoasto.Hardware;

namespace CoffeeRoasto.Tests.Core;

public class FrameParserTests
{
    [Fact]
    public void A_data_frame_gives_the_three_temperatures()
    {
        ParsedLine line = FrameParser.Parse("@37.11A38.67B150.00C");

        Assert.Equal(LineKind.Telemetry, line.Kind);
        Assert.Equal(37.11, line.Telemetry.Bean, 9);
        Assert.Equal(38.67, line.Telemetry.Drum, 9);
        Assert.Equal(150.0, line.Telemetry.Reference, 9);
    }

    [Fact]
    public void A_frame_without_reference_reads_zero()
    {
        Assert.Equal(0, FrameParser.Parse("@37.11A38.67B0C").Telemetry.Reference);
    }

    [Theory]
    [InlineData("X", RoasterEvent.RoastingFinished)]
    [InlineData("V", RoasterEvent.HeatingFinished)]
    [InlineData("Y", RoasterEvent.BurnerOn)]
    [InlineData("Z", RoasterEvent.BurnerOff)]
    public void A_single_letter_is_an_event(string text, RoasterEvent expected)
    {
        ParsedLine line = FrameParser.Parse(text);

        Assert.Equal(LineKind.Event, line.Kind);
        Assert.Equal(expected, line.Event);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("hello")]
    [InlineData("@xA1B2C")]
    [InlineData("A1B2C@3")]
    public void Noise_and_damaged_frames_are_ignored(string? text)
    {
        Assert.Equal(LineKind.Ignored, FrameParser.Parse(text).Kind);
    }

    [Fact]
    public void A_long_field_is_still_a_frame()
    {
        // The old app stored the index in an sbyte, which overflowed after 127 characters.
        Assert.Equal(LineKind.Telemetry, FrameParser.Parse("@" + new string('1', 200) + "A1B2C").Kind);
    }

    [Theory]
    [InlineData("id-ID")]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    public void The_result_does_not_depend_on_the_culture_of_the_pc(string culture)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            Telemetry t = FrameParser.Parse("@37.11A38.67B150.00C").Telemetry;

            Assert.Equal(37.11, t.Bean, 9);
            Assert.Equal(150.0, t.Reference, 9);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
