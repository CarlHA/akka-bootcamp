using System;
using Akka.Actor;

namespace WinTail
{
    internal class TailCoordinatorActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = (StartTail)message;
                Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: x =>
                    {
                        if (x is ArithmeticException)
                        {
                            return Directive.Resume;
                        }
                        else if (x is NotSupportedException)
                        {
                            return Directive.Stop;
                        }
                        else
                        {
                            return Directive.Restart;
                        }
                    });
        }

        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }

            public string FilePath { get; }

            public IActorRef ReporterActor { get; }
        }

        public class StopTail
        {
            public string FilePath { get; }

            public StopTail(string filePath)
            {
                FilePath = filePath;
            }
        }
    }
}