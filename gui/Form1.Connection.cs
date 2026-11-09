using System;
using System.Windows.Forms;
using CoffeeRoasto_GUI.Hardware;

namespace CoffeeRoasto_GUI
{
    // Connect, disconnect and the enabled state of the controls.
    public partial class Form_CoffeeRoasto
    {
        /// <summary>The controls that only make sense while connected.</summary>
        private Control[] ConnectedControls()
        {
            return new Control[]
            {
                button_ONBurner, button_OFFBurner,
                button_ONMotorDrum, button_OFFMotorDrum,
                button_ONBlowerDrum, button_OFFBlowerDrum,
                button_ONCooler, button_OFFCooler,
                button_ONMotorValve, button_OFFMotorValve,
                textBox_InputTemp, button_SENDInputTemperature,
                button_StartRecordData, button_StopRecordData, button_SaveRecordData,
                textBox_FileName, button_BrowseFile, comboBox_Sheet,
                button_StartHeatingMode, button_StopHeatingMode,
                button_StartAutoMode, button_StopAutoMode, button_bean,
                button_StartPID, button_StopPID
            };
        }

        private void SetControlsEnabled(bool enabled)
        {
            foreach (Control control in ConnectedControls())
            {
                control.Enabled = enabled;
            }
        }

        private void comboBox_ComPort_DropDown(object sender, EventArgs e)
        {
            comboBox_ComPort.Items.Clear();
            comboBox_ComPort.Items.AddRange(RoasterLink.AvailablePorts());
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                _clock.Restart();
                _link.Open(comboBox_ComPort.Text);

                button_Connect.Enabled = false;
                button_Disconnect.Enabled = true;
                progressBar_PanelConnection.Value = 100;
                SetControlsEnabled(true);

                _chart.ClearMeasured();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            if (!_link.IsOpen)
            {
                return;
            }

            try
            {
                _clock.Restart();
                _link.Close();

                button_Connect.Enabled = true;
                button_Disconnect.Enabled = false;
                progressBar_PanelConnection.Value = 0;
                SetControlsEnabled(false);

                _chart.ClearAll();
                label_DataBeanTemperature.Text = NoReferenceText;
                label_DataDrumTemperature.Text = NoReferenceText;
                label_DataReferenceTemperature.Text = NoReferenceText;
                dataGridView.DataSource = null;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}
