using System.Collections.Generic;

namespace CoffeeRoasto_GUI.Recording
{
    /// <summary>Collects bean and drum temperatures while the operator is recording.</summary>
    internal sealed class RecordingSession
    {
        private readonly List<Coffee> _samples = new List<Coffee>();

        public bool IsRecording { get; private set; }

        /// <summary>The recorded samples, ready to be used as a data source.</summary>
        public IList<Coffee> Samples
        {
            get { return _samples; }
        }

        /// <summary>Starts a new recording and discards the previous one.</summary>
        public void Start()
        {
            _samples.Clear();
            IsRecording = true;
        }

        public void Stop()
        {
            IsRecording = false;
        }

        public void Add(double bean, double drum)
        {
            if (IsRecording)
            {
                _samples.Add(new Coffee { beantemperature = bean, drumtemperature = drum });
            }
        }
    }
}
