using System.Data;
using System.Text;
using ExcelDataReader;

namespace CoffeeRoasto.Profiles;

/// <summary>Reads a reference temperature profile from an Excel file.</summary>
public static class ProfileReader
{
    /// <summary>Header of the only column that is read from each sheet.</summary>
    public const string BeanTemperatureColumn = "beantemperature";

    public const string FileDialogFilter = "Excel workbooks (*.xlsx;*.xls)|*.xlsx;*.xls";

    static ProfileReader()
    {
        // Old .xls files use code pages that .NET does not load by default.
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>Returns one table per sheet, each holding only the bean temperature column.</summary>
    public static DataTableCollection Read(string path)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true,
                FilterColumn = (columnReader, columnIndex) =>
                    columnReader.GetValue(columnIndex)?.ToString() == BeanTemperatureColumn
            }
        });
        return result.Tables;
    }
}
