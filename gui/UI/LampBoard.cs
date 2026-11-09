using System.Collections.Generic;
using System.Drawing;

namespace CoffeeRoasto_GUI.UI
{
    internal enum Lamp
    {
        Burner,
        MotorDrum,
        BlowerDrum,
        Cooler,
        MotorValve,
        RecordData,
        HeatingMode,
        AutoMode,
        PidMode
    }

    /// <summary>All pilot lamps of the screen, addressed by name instead of by control.</summary>
    internal sealed class LampBoard
    {
        private static readonly Color OnColor = Palette.LampOn;
        private static readonly Color OffColor = Palette.LampOff;

        private readonly IDictionary<Lamp, PilotLamp> _lamps;

        public LampBoard(IDictionary<Lamp, PilotLamp> lamps)
        {
            _lamps = lamps;
        }

        public void On(params Lamp[] lamps)
        {
            Set(lamps, OnColor);
        }

        public void Off(params Lamp[] lamps)
        {
            Set(lamps, OffColor);
        }

        public void AllOff()
        {
            foreach (PilotLamp lamp in _lamps.Values)
            {
                lamp.BackColor = OffColor;
            }
        }

        private void Set(IEnumerable<Lamp> lamps, Color color)
        {
            foreach (Lamp lamp in lamps)
            {
                _lamps[lamp].BackColor = color;
            }
        }
    }
}
