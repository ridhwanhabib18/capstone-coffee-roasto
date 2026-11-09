using ClosedXML.Excel;
using CoffeeRoasto.Recording;

namespace CoffeeRoasto.Tests.Core;

public class RecordingTests
{
    [Fact]
    public void Nothing_is_recorded_before_start()
    {
        var session = new RecordingSession();
        session.Add(1, 2, 3, 4);

        Assert.Empty(session.Samples);
    }

    [Fact]
    public void Samples_are_recorded_between_start_and_stop()
    {
        var session = new RecordingSession();
        session.Start();
        session.Add(150.5, 160.25, 150, 4);
        session.Add(151.0, 161.0, 151, 8);
        session.Stop();
        session.Add(152, 162, 152, 12);

        Assert.False(session.IsRecording);
        Assert.Equal(2, session.Samples.Count);
        Assert.Equal(150.5, session.Samples[0].BeanTemperature);
        Assert.Equal(161.0, session.Samples[1].DrumTemperature);
    }

    [Fact]
    public void Start_discards_the_previous_recording()
    {
        var session = new RecordingSession();
        session.Start();
        session.Add(1, 2, 3, 4);
        session.Start();

        Assert.Empty(session.Samples);
    }

    [Fact]
    public void Export_writes_every_sample_including_the_last_one()
    {
        var session = new RecordingSession();
        session.Start();
        for (int i = 0; i < 5; i++) session.Add(100 + i, 110 + i, 100, i * 4);

        string path = Path.Combine(Path.GetTempPath(), $"roast-{Guid.NewGuid():N}.xlsx");
        try
        {
            ExcelExporter.Export(path, session.Samples);

            using var workbook = new XLWorkbook(path);
            IXLWorksheet sheet = workbook.Worksheet(ExcelExporter.SheetName);
            Assert.Equal("Bean (°C)", sheet.Cell(1, 2).GetString());
            Assert.Equal(6, sheet.LastRowUsed()!.RowNumber());   // header + 5 samples
            Assert.Equal(104, sheet.Cell(6, 2).GetDouble());
        }
        finally
        {
            File.Delete(path);
        }
    }
}
