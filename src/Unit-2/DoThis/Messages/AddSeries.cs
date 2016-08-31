using System.Windows.Forms.DataVisualization.Charting;

namespace ChartApp.Messages
{
    public class AddSeries
    {
        public Series Series { get; }

        public AddSeries(Series series)
        {
            Series = series;
        }
    }
}