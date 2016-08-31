using Akka.Actor;
using ChartApp.Actors;

namespace ChartApp.Messages
{
    public class SubscribeCounter
    {
        public CounterType Counter { get; }
        public IActorRef Subscriber { get; }

        public SubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Counter = counter;
            Subscriber = subscriber;
        }
    }
}