using System;
using System.Windows.Forms;
using CoffeeRoasto_GUI.Hardware;
using CoffeeRoasto_GUI.UI;

namespace CoffeeRoasto_GUI
{
    // The three automatic modes: heating, auto and PID. The firmware runs them; the screen
    // sends the start or stop command and sets the lamps to match.
    public partial class Form_CoffeeRoasto
    {
        // ---- Heating ------------------------------------------------------------------------------

        private void button_StartHeatingMode_Click(object sender, EventArgs e)
        {
            if (!_link.Send(RoasterCommand.HeatingStart))
            {
                return;
            }

            _lamps.On(Lamp.HeatingMode, Lamp.MotorDrum, Lamp.BlowerDrum, Lamp.MotorValve);
            _lamps.Off(Lamp.Cooler);
        }

        private void button_StopHeatingMode_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Roasting Selesai");
            if (!_link.Send(RoasterCommand.Stop))
            {
                return;
            }

            _lamps.Off(Lamp.HeatingMode, Lamp.BlowerDrum, Lamp.Burner, Lamp.Cooler);
            _lamps.On(Lamp.MotorDrum, Lamp.MotorValve);
        }

        // ---- Auto ---------------------------------------------------------------------------------

        private void button_StartAutoMode_Click(object sender, EventArgs e)
        {
            if (!_link.IsOpen)
            {
                return;
            }

            _chart.ClearAll();
            if (!_link.Send(RoasterCommand.AutoStart))
            {
                return;
            }

            _lamps.On(Lamp.AutoMode, Lamp.MotorDrum, Lamp.BlowerDrum);
            _lamps.Off(Lamp.Cooler);
            _clock.Restart();
        }

        private void button_StopAutoMode_Click(object sender, EventArgs e)
        {
            if (!_link.Send(RoasterCommand.Stop))
            {
                return;
            }

            _lamps.Off(Lamp.AutoMode, Lamp.BlowerDrum, Lamp.Burner, Lamp.Cooler, Lamp.MotorValve);
            _lamps.On(Lamp.MotorDrum);
        }

        // ---- PID ----------------------------------------------------------------------------------

        private void button_StartPID_Click(object sender, EventArgs e)
        {
            if (!_link.IsOpen)
            {
                return;
            }

            _chart.ClearAll();
            if (!_link.Send(RoasterCommand.PidStart))
            {
                return;
            }

            _lamps.On(Lamp.PidMode, Lamp.MotorDrum, Lamp.BlowerDrum, Lamp.MotorValve);
            _lamps.Off(Lamp.Cooler);
            _clock.Restart();
        }

        private void button_StopPID_Click(object sender, EventArgs e)
        {
            if (!_link.Send(RoasterCommand.Stop))
            {
                return;
            }

            _lamps.Off(Lamp.PidMode, Lamp.BlowerDrum, Lamp.Burner, Lamp.Cooler);
            _lamps.On(Lamp.MotorDrum, Lamp.MotorValve);
        }
    }
}
