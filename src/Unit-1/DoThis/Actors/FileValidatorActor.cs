using System;
using System.IO;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{
    internal class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef consoleWriterActor;

        public FileValidatorActor(IActorRef consoleWriterActor)
        {
            this.consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (String.IsNullOrEmpty(msg))
            {
                consoleWriterActor.Tell(new NullInputError("Input was blank. Please try again.\n"));
                Sender.Tell(new ContinueProcessing());
            }
            else
            {
                var valid = IsFileUrl(msg);
                if (valid)
                {
                    consoleWriterActor.Tell(new InputSuccess($"Starting processing for {msg}"));
                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor")
                           .Tell(new StartTail(msg, consoleWriterActor));
                }
                else
                {
                    consoleWriterActor.Tell(new ValidationError($"{msg} is not an existing URI on disk"));
                    Sender.Tell(new ContinueProcessing());
                }
            }
        }

        private static bool IsFileUrl(string path)
        {
            return File.Exists(path);
        }
    }
}
