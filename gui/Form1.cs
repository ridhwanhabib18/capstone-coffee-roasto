using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using CoffeeRoasto_GUI.Hardware;
using CoffeeRoasto_GUI.Profiles;
using CoffeeRoasto_GUI.Recording;
using CoffeeRoasto_GUI.UI;

namespace CoffeeRoasto_GUI
{
    /// <summary>
    /// The only screen. This file holds the setup and the handling of data coming from the roaster;
    /// the button handlers are grouped in the Form1.*.cs files next to it.
    /// </summary>
    public partial class Form_CoffeeRoasto : Form
    {
        private const string TemperatureFormat = "{0}ºC ";
        private const string NoReferenceText = "....ºC";

        private RoasterLink _link;
        private LampBoard _lamps;
        private TemperatureChart _chart;
        private RoastClock _clock;
        private RecordingSession _recording;
        private ProfileTransmitter _transmitter;

        public Form_CoffeeRoasto()
        {
            InitializeComponent();
            ComposeParts();
        }

        /// <summary>Creates the helper objects the handlers work with.</summary>
        private void ComposeParts()
        {
            _link = new RoasterLink(serialPort1);
            _link.Error += message => MessageBox.Show(message);

            _lamps = new LampBoard(new Dictionary<Lamp, PilotLamp>
            {
                { Lamp.Burner, pilotLamp_Burner },
                { Lamp.MotorDrum, pilotLamp_MotorDrum },
                { Lamp.BlowerDrum, pilotLamp_BlowerDrum },
                { Lamp.Cooler, pilotLamp_Cooler },
                { Lamp.MotorValve, pilotLampMotorValve },
                { Lamp.RecordData, pilotLamp_RecordData },
                { Lamp.HeatingMode, pilotLamp_HeatingMode },
                { Lamp.AutoMode, pilotLamp_AutoMode },
                { Lamp.PidMode, pilotLamp_PIDMode }
            });

            _chart = new TemperatureChart(chart_Temperature);
            _clock = new RoastClock();
            _recording = new RecordingSession();
            _transmitter = new ProfileTransmitter(_link);
        }

        private void Form_CoffeeRoasto_Load(object sender, EventArgs e)
        {
            button_Connect.Enabled = true;
            button_Disconnect.Enabled = false;
            progressBar_PanelConnection.Value = 0;
            SetControlsEnabled(false);
            _lamps.AllOff();
            _chart.ShowPlaceholder();
        }

        private void Form_CoffeeRoasto_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _link.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        // ---- Incoming data ------------------------------------------------------------------------

        /// <summary>Runs on a serial-port thread; the work is handed to the UI thread.</summary>
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string line = _link.TryReadLine();
            if (line == null || !IsHandleCreated || IsDisposed)
            {
                return;
            }

            try
            {
                BeginInvoke(new Action(() => HandleLine(line)));
            }
            catch (InvalidOperationException)
            {
                // The window closed between the check and the call.
            }
        }

        private void HandleLine(string line)
        {
            ParsedLine parsed = FrameParser.Parse(line);
            switch (parsed.Kind)
            {
                case LineKind.Telemetry:
                    ShowTelemetry(parsed.Telemetry);
                    break;
                case LineKind.Event:
                    HandleEvent(parsed.Event);
                    break;
            }
        }

        private void ShowTelemetry(Telemetry reading)
        {
            _recording.Add(reading.Bean, reading.Drum);

            label_DataBeanTemperature.Text = string.Format(TemperatureFormat, reading.Bean);
            label_DataDrumTemperature.Text = string.Format(TemperatureFormat, reading.Drum);
            label_DataReferenceTemperature.Text = reading.Reference > 0
                ? string.Format("{0}ºC", reading.Reference)
                : NoReferenceText;

            _chart.Add(_clock.Label, reading);
        }

        private void HandleEvent(RoasterEvent roasterEvent)
        {
            switch (roasterEvent)
            {
                case RoasterEvent.RoastingFinished:
                    MessageBox.Show("Roasting Selesai");
                    _lamps.Off(Lamp.AutoMode, Lamp.BlowerDrum, Lamp.Burner);
                    _lamps.On(Lamp.MotorDrum, Lamp.Cooler);
                    break;

                case RoasterEvent.HeatingFinished:
                    MessageBox.Show("Heating Selesai");
                    _lamps.Off(Lamp.HeatingMode, Lamp.BlowerDrum, Lamp.Burner, Lamp.Cooler);
                    _lamps.On(Lamp.MotorDrum);
                    break;

                case RoasterEvent.BurnerOn:
                    _lamps.On(Lamp.Burner);
                    break;

                case RoasterEvent.BurnerOff:
                    _lamps.Off(Lamp.Burner);
                    break;
            }
        }
    }
}
