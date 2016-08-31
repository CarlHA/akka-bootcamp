using ChartApp.Actors;

namespace ChartApp.Messages
{
    public class UnWatch
    {
        public CounterType Counter { get; }

        public UnWatch(CounterType counter)
        {
            Counter = counter;
        }
    }
}