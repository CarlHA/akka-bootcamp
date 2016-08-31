using ChartApp.Actors;

namespace ChartApp.Messages
{
    public class Watch
    {
        public CounterType Counter { get; }

        public Watch(CounterType counter)
        {
            Counter = counter;
        }
    }
}