using System.Windows.Forms.DataVisualization.Charting;
using CoffeeRoasto_GUI.Hardware;

namespace CoffeeRoasto_GUI.UI
{
    /// <summary>The temperature chart with its three series (bean, drum, reference).</summary>
    internal sealed class TemperatureChart
    {
        private const string BeanSeries = "Bean Temperature";
        private const string DrumSeries = "Drum Temperature";
        private const string ReferenceSeries = "Reference Temperature";

        private readonly Chart _chart;

        public TemperatureChart(Chart chart)
        {
            _chart = chart;
        }

        /// <summary>Draws a flat starting line so the chart is not empty before the first reading.</summary>
        public void ShowPlaceholder()
        {
            _chart.Series[BeanSeries].Points.AddXY(1, 1);
            _chart.Series[DrumSeries].Points.AddXY(1, 1);
        }

        /// <summary>Clears the bean and drum series only (used when connecting).</summary>
        public void ClearMeasured()
        {
            _chart.Series[BeanSeries].Points.Clear();
            _chart.Series[DrumSeries].Points.Clear();
        }

        public void ClearAll()
        {
            ClearMeasured();
            _chart.Series[ReferenceSeries].Points.Clear();
        }

        public void Add(string time, Telemetry reading)
        {
            _chart.Series[BeanSeries].Points.AddXY(time, reading.Bean);
            _chart.Series[DrumSeries].Points.AddXY(time, reading.Drum);
            _chart.Series[ReferenceSeries].Points.AddXY(time, reading.Reference);
        }
    }
}
