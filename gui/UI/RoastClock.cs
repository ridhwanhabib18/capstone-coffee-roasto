using System;
using System.Diagnostics;

namespace CoffeeRoasto_GUI.UI
{
    /// <summary>The time axis of the chart: elapsed time since connect or since a mode was started.</summary>
    internal sealed class RoastClock
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan? _frozen;

        public void Restart()
        {
            _frozen = null;
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        /// <summary>The elapsed time as mm:ss.</summary>
        public string Label
        {
            get { return (_frozen ?? _stopwatch.Elapsed).ToString("mm':'ss"); }
        }

        /// <summary>
        /// Fixes the clock at a given time. Only used by tools/screenshots to draw a whole roast
        /// without waiting for it; the application itself never calls this.
        /// </summary>
        internal void FreezeAt(TimeSpan elapsed)
        {
            _frozen = elapsed;
        }
    }
}
