using ClosedXML.Excel;

namespace CoffeeRoasto.Recording;

/// <summary>Writes a recording to an .xlsx file. Microsoft Excel does not have to be installed.</summary>
public static class ExcelExporter
{
    public const string SheetName = "Roast";

    public static void Export(string path, IEnumerable<Coffee> samples)
    {
        using var workbook = new XLWorkbook();
        IXLWorksheet sheet = workbook.Worksheets.Add(SheetName);

        string[] headers = { "Time (s)", "Bean (°C)", "Drum (°C)", "Reference (°C)" };
        for (int column = 0; column < headers.Length; column++)
        {
            sheet.Cell(1, column + 1).Value = headers[column];
        }
        sheet.Row(1).Style.Font.Bold = true;

        int row = 2;
        foreach (Coffee sample in samples)
        {
            sheet.Cell(row, 1).Value = Math.Round(sample.ElapsedSeconds, 1);
            sheet.Cell(row, 2).Value = sample.BeanTemperature;
            sheet.Cell(row, 3).Value = sample.DrumTemperature;
            sheet.Cell(row, 4).Value = sample.ReferenceTemperature;
            row++;
        }

        sheet.Columns().AdjustToContents();
        workbook.SaveAs(path);
    }
}
