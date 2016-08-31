using Akka.Actor;
using ChartApp.Actors;

namespace ChartApp.Messages
{
    public class UnsubscribeCounter
    {
        public CounterType Counter { get; }
        public IActorRef Subscriber { get; }

        public UnsubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Counter = counter;
            Subscriber = subscriber;
        }
    }
}