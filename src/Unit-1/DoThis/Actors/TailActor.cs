using System.IO;
using System.Text;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{
    internal class TailActor : UntypedActor
    {
        private readonly string filePath;

        private readonly IActorRef reporterActor;

        private Stream fileStream;

        private StreamReader fileStreamReader;

        private FileObserver observer;

        public TailActor(IActorRef reporterActor, string filePath)
        {
            this.reporterActor = reporterActor;
            this.filePath = filePath;
        }

        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                var text = fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    reporterActor.Tell(text);
                }
            }
            else if (message is FileError)
            {
                var fe = (FileError)message;
                reporterActor.Tell($"Tail error: {fe.Reason}");
            }
            else if (message is InitialRead)
            {
                var ir = (InitialRead)message;
                reporterActor.Tell(ir.Text);
            }
        }

        protected override void PostStop()
        {
            observer.Dispose();
            fileStreamReader.Dispose();
        }

        protected override void PreStart()
        {
            observer = new FileObserver(Self, Path.GetFullPath(filePath));
            observer.Start();
            fileStream = new FileStream(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStreamReader = new StreamReader(fileStream, Encoding.UTF8);
            var text = fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(filePath, text));
        }
    }
}