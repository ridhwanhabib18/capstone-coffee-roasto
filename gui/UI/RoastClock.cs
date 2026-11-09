using System.Diagnostics;

namespace CoffeeRoasto_GUI.UI
{
    /// <summary>The time axis of the chart: elapsed time since connect or since a mode was started.</summary>
    internal sealed class RoastClock
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void Restart()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        /// <summary>The elapsed time as mm:ss.</summary>
        public string Label
        {
            get { return _stopwatch.Elapsed.ToString("mm':'ss"); }
        }
    }
}
