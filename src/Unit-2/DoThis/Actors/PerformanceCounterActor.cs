using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;
using ChartApp.Messages;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor : UntypedActor
    {
        private readonly string _seriesName;
        private readonly Func<PerformanceCounter> _performanceCounterGenerator;
        private readonly HashSet<IActorRef> _subscriptions;
        private readonly Cancelable _cancelPublishing;
        private PerformanceCounter _counter;

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator )
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = performanceCounterGenerator;
            _subscriptions = new HashSet<IActorRef>();
            _cancelPublishing = new Cancelable(Context.System.Scheduler);
        }

        protected override void PreStart()
        {
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(initialDelay: TimeSpan.FromMilliseconds(250),
                                                            interval: TimeSpan.FromMilliseconds(250),
                                                            receiver: Self,
                                                            message: new GatherMetrics(),
                                                            sender: Self,
                                                            cancelable: _cancelPublishing);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel(false);
                _counter.Dispose();
            }
            catch { }
            finally
            {
                base.PostStop();
            }
        }

        protected override void OnReceive(object message)
        {
            if (message is GatherMetrics)
            {
                var metric = new Metric(_seriesName, _counter.NextValue());
                foreach (var subscription in _subscriptions)
                {
                    subscription.Tell(metric);
                }
            }
            else if (message is SubscribeCounter)
            {
                var sc = (SubscribeCounter)message;
                _subscriptions.Add(sc.Subscriber);
            }
            else if (message is UnsubscribeCounter)
            {
                var uc = (UnsubscribeCounter)message;
                _subscriptions.Remove(uc.Subscriber);
            }
        }
    }
}
