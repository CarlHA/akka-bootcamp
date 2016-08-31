namespace ChartApp.Messages
{
    public class RemoveSeries
    {
        public string SeriesName { get; }

        public RemoveSeries(string seriesName)
        {
            SeriesName = seriesName;
        }
    }
}