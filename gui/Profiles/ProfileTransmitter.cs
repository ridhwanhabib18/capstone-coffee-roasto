using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
using CoffeeRoasto_GUI.Hardware;

namespace CoffeeRoasto_GUI.Profiles
{
    /// <summary>One line to send to the firmware.</summary>
    internal struct ProfileChunk
    {
        public ProfileChunk(string text, bool isFullGroup)
        {
            Text = text;
            IsFullGroup = isFullGroup;
        }

        public string Text { get; }

        /// <summary>True for a group of ten values; the sender then waits for the firmware to use them.</summary>
        public bool IsFullGroup { get; }
    }

    /// <summary>
    /// Sends a profile to the firmware in groups of ten values, 40 seconds apart
    /// (ten set points last about 40 s: one every ~4 s). Runs on a background thread.
    /// </summary>
    internal sealed class ProfileTransmitter
    {
        public const int GroupSize = 10;
        public static readonly TimeSpan GroupDelay = TimeSpan.FromSeconds(40);

        private readonly RoasterLink _link;
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        public ProfileTransmitter(RoasterLink link)
        {
            _link = link;
            _worker.DoWork += (sender, e) => Send((IReadOnlyList<string>)e.Argument);
            _worker.RunWorkerCompleted += OnCompleted;
        }

        /// <summary>
        /// Splits the values into lines: nine values with a comma, a tenth without one.
        /// A shorter last group keeps its trailing comma, which the firmware tolerates.
        /// </summary>
        public static IEnumerable<ProfileChunk> BuildChunks(IReadOnlyList<string> values)
        {
            string payload = "";
            for (int i = 0; i < values.Count; i++)
            {
                if ((i + 1) % GroupSize == 0)
                {
                    yield return new ProfileChunk(payload + values[i], true);
                    payload = "";
                }
                else
                {
                    payload += values[i] + ",";
                }
            }

            if (payload != "")
            {
                yield return new ProfileChunk(payload, false);
            }
        }

        /// <summary>Starts sending. Throws InvalidOperationException when a transfer is already running.</summary>
        public void Start(DataTable table)
        {
            var values = new List<string>(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                values.Add(row[ProfileReader.BeanTemperatureColumn].ToString());
            }
            _worker.RunWorkerAsync(values);
        }

        private void Send(IReadOnlyList<string> values)
        {
            foreach (ProfileChunk chunk in BuildChunks(values))
            {
                _link.SendLine(chunk.Text);
                if (chunk.IsFullGroup)
                {
                    Thread.Sleep(GroupDelay);
                }
            }
        }

        private static void OnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.WriteLine(e.Error != null ? "profile transfer failed: " + e.Error.Message : "profile transfer finished");
        }
    }
}
