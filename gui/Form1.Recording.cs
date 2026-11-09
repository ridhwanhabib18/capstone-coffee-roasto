using System;
using System.Windows.Forms;
using CoffeeRoasto_GUI.Recording;
using CoffeeRoasto_GUI.UI;

namespace CoffeeRoasto_GUI
{
    // Recording a run into the data table and exporting it to Excel.
    public partial class Form_CoffeeRoasto
    {
        private void button_StartRecordData_Click(object sender, EventArgs e)
        {
            _chart.ClearAll();
            _recording.Start();
            dataGridView.DataSource = null;

            _lamps.On(Lamp.RecordData);
            _clock.Restart();
        }

        private void button_StopRecordData_Click(object sender, EventArgs e)
        {
            _recording.Stop();
            dataGridView.DataSource = _recording.Samples;

            _lamps.Off(Lamp.RecordData);
        }

        private void button_ExportRecordData_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelExporter.Export(dataGridView);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}
