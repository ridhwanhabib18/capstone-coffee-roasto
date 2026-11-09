using System;
using CoffeeRoasto_GUI.Hardware;
using CoffeeRoasto_GUI.UI;

namespace CoffeeRoasto_GUI
{
    // Manual control of the actuators: one button sends one command and lights its lamp.
    public partial class Form_CoffeeRoasto
    {
        /// <summary>Sends the command and, only when it was sent, switches the lamp.</summary>
        private void SwitchActuator(string command, Lamp lamp, bool on)
        {
            if (!_link.Send(command))
            {
                return;
            }

            if (on)
            {
                _lamps.On(lamp);
            }
            else
            {
                _lamps.Off(lamp);
            }
        }

        private void button_ONBurner_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.BurnerOn, Lamp.Burner, true);
        }

        private void button_OFFBurner_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.BurnerOff, Lamp.Burner, false);
        }

        private void button_ONMotorDrum_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.DrumMotorOn, Lamp.MotorDrum, true);
        }

        private void button_OFFMotorDrum_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.DrumMotorOff, Lamp.MotorDrum, false);
        }

        private void button_ONBlowerDrum_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.BlowerOn, Lamp.BlowerDrum, true);
        }

        private void button_OFFBlowerDrum_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.BlowerOff, Lamp.BlowerDrum, false);
        }

        private void button_ONCooler_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.CoolerOn, Lamp.Cooler, true);
        }

        private void button_OFFCooler_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.CoolerOff, Lamp.Cooler, false);
        }

        private void button_ONMotorValve_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.ValveOpen, Lamp.MotorValve, true);
        }

        private void button_OFFMotorValve_Click(object sender, EventArgs e)
        {
            SwitchActuator(RoasterCommand.ValveClose, Lamp.MotorValve, false);
        }

        /// <summary>Gives the PID a set point typed by the operator.</summary>
        private void button_SENDInputTemperature_Click(object sender, EventArgs e)
        {
            if (_link.Send(textBox_InputTemp.Text))
            {
                textBox_InputTemp.Text = string.Empty;
            }
        }
    }
}
