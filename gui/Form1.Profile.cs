using System;
using System.Data;
using System.Windows.Forms;
using CoffeeRoasto_GUI.Profiles;

namespace CoffeeRoasto_GUI
{
    // The reference profile for automatic roasting: choose a file and sheet, then send it.
    public partial class Form_CoffeeRoasto
    {
        private DataTableCollection _profileSheets;

        private void button_BrowseFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Filter = ProfileReader.FileDialogFilter })
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                textBox_FileName.Text = dialog.FileName;
                _profileSheets = ProfileReader.Read(dialog.FileName);

                comboBox_Sheet.Items.Clear();
                foreach (DataTable sheet in _profileSheets)
                {
                    comboBox_Sheet.Items.Add(sheet.TableName);
                }
            }
        }

        private void comboBox_Sheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView.DataSource = _profileSheets[comboBox_Sheet.SelectedItem.ToString()];
        }

        /// <summary>Sends the shown profile to the firmware. PID mode must already be on.</summary>
        private void button_bean_Click(object sender, EventArgs e)
        {
            try
            {
                _transmitter.Start((DataTable)dataGridView.DataSource);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}
