using System.Data;
using System.IO;
using ExcelDataReader;

namespace CoffeeRoasto_GUI.Profiles
{
    /// <summary>Reads a reference temperature profile from an Excel file.</summary>
    internal static class ProfileReader
    {
        /// <summary>Header of the only column that is read from each sheet.</summary>
        public const string BeanTemperatureColumn = "beantemperature";

        public const string FileDialogFilter = "Excel 97-2003 Workbook|*.xls|Excel Workbook|*.xlsx";

        /// <summary>Returns one table per sheet, each holding only the bean temperature column.</summary>
        public static DataTableCollection Read(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
            {
                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true,
                        FilterColumn = (columnReader, columnIndex) =>
                            columnReader.GetString(columnIndex) == BeanTemperatureColumn
                    }
                });
                return result.Tables;
            }
        }
    }
}
