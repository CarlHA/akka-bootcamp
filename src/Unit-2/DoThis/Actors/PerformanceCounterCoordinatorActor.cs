using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;
using ChartApp.Messages;

namespace ChartApp.Actors
{
    public class PerformanceCounterCoordinatorActor : ReceiveActor
    {
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

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor)
        {
            _chartingActor = chartingActor;
            _counterActors = new Dictionary<CounterType, IActorRef>();
            Receive<Watch>(watch => HandleWatch(watch));
            Receive<UnWatch>(unwatch =>HandleUnwatch(unwatch));
        }

        private void HandleUnwatch(UnWatch unwatch)
        {
            if (!_counterActors.ContainsKey(unwatch.Counter))
            {
                return;
            }

            _counterActors[unwatch.Counter].Tell(new UnsubscribeCounter(unwatch.Counter, _chartingActor));
            _chartingActor.Tell(new RemoveSeries(unwatch.Counter.ToString()));
        }

        private void HandleWatch(Watch watch)
        {
            if (!_counterActors.ContainsKey(watch.Counter))
            {
                var counterActor =
                    Context.ActorOf(
                        Props.Create(() => new PerformanceCounterActor(watch.Counter.ToString(), CounterGenerators[watch.Counter])));
                _counterActors[watch.Counter] = counterActor;
            }

            _chartingActor.Tell(new AddSeries(CounterSeries[watch.Counter]()));
            _counterActors[watch.Counter].Tell(new SubscribeCounter(watch.Counter, _chartingActor));
        }
    }
}
