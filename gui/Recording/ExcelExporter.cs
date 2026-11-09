using System;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace CoffeeRoasto_GUI.Recording
{
    /// <summary>Copies a data grid to a new Excel workbook. Needs Microsoft Excel on the PC.</summary>
    internal static class ExcelExporter
    {
        private const string SheetName = "Exported from gridview";

        public static void Export(DataGridView grid)
        {
            Excel._Application app = new Excel.Application();
            Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            app.Visible = true;

            Excel._Worksheet worksheet = workbook.ActiveSheet;
            worksheet.Name = SheetName;

            for (int column = 0; column < grid.Columns.Count; column++)
            {
                worksheet.Cells[1, column + 1] = grid.Columns[column].HeaderText;
            }

            int excelRow = 2;
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                for (int column = 0; column < grid.Columns.Count; column++)
                {
                    worksheet.Cells[excelRow, column + 1] = row.Cells[column].Value?.ToString();
                }
                excelRow++;
            }
        }
    }
}
