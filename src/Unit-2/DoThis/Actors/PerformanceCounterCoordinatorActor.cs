using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class PerformanceCounterCoordinatorActor : ReceiveActor
    {
        public class Watch
        {
            public CounterType Counter { get; }

            public Watch(CounterType counter)
            {
                Counter = counter;
            }
        }

        public class UnWatch
        {
            public CounterType Counter { get; }

            public UnWatch(CounterType counter)
            {
                Counter = counter;
            }
        }

        private static readonly Dictionary<CounterType, Func<PerformanceCounter>> CounterGenerators = new Dictionary
            <CounterType, Func<PerformanceCounter>>
            {
                {
                    CounterType.Cpu,
                    () => new PerformanceCounter(categoryName: "Processor", counterName: "% Processor Time", instanceName: "_Total", readOnly: true)
                },
                {CounterType.Memory, () => new PerformanceCounter(categoryName: "Memory", counterName: "% Committed Bytes In Use", readOnly: true)},
                {
                    CounterType.Disk,
                    () => new PerformanceCounter(categoryName: "LogicalDisk", counterName: "% Disk Time", instanceName: "_Total", readOnly: true)
                }
            };

        private static readonly Dictionary<CounterType, Func<Series>> CounterSeries = new Dictionary<CounterType, Func<Series>>
        {
            {CounterType.Cpu, () => new Series(CounterType.Cpu.ToString()) {ChartType = SeriesChartType.SplineArea, Color = Color.DarkGreen}},
            {CounterType.Memory, () => new Series(CounterType.Memory.ToString()) {ChartType = SeriesChartType.FastLine, Color = Color.MediumBlue}},
            {CounterType.Disk, () => new Series(CounterType.Disk.ToString()) {ChartType = SeriesChartType.SplineArea, Color = Color.DarkRed}},
        };

        private readonly IActorRef _chartingActor;
        private readonly Dictionary<CounterType, IActorRef> _counterActors;

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor) : this(chartingActor, new Dictionary<CounterType, IActorRef>())
        {

        }

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor, Dictionary<CounterType, IActorRef> counterActors)
        {
            _chartingActor = chartingActor;
            _counterActors = counterActors;
            Receive<Watch>(watch =>
                           {
                               if (!_counterActors.ContainsKey(watch.Counter))
                               {
                                   var counterActor =
                                       Context.ActorOf(
                                           Props.Create(() => new PerformanceCounterActor(watch.Counter.ToString(), CounterGenerators[watch.Counter])));
                                   _counterActors[watch.Counter] = counterActor;
                               }

                               _chartingActor.Tell(new ChartingActor.AddSeries(CounterSeries[watch.Counter]()));
                               _counterActors[watch.Counter].Tell(new SubscribeCounter(watch.Counter, _chartingActor));
                           });

            Receive<UnWatch>(unwatch =>
                             {
                                 if (!_counterActors.ContainsKey(unwatch.Counter))
                                 {
                                     return;
                                 }

                                 _counterActors[unwatch.Counter].Tell(new UnsubscribeCounter(unwatch.Counter, _chartingActor));
                                 _chartingActor.Tell(new ChartingActor.RemoveSeries(unwatch.Counter.ToString()));
                             });
        }
    }
}
