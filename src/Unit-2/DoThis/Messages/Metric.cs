namespace ChartApp.Messages
{
    public class Metric
    {
        public string Series { get; }
        public float CounterValue { get; }

        public Metric(string series, float counterValue)
        {
            Series = series;
            CounterValue = counterValue;
        }
    }
}