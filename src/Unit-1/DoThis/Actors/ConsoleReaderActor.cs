using System;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{
    internal class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";

        public const string StartCommand = "start";

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }
            else if (message is InputError)
            {
                Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell((InputError)message);
            }

            GetAndValidateInput();
        }

        private void DoPrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\n");
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (!String.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                Context.System.Terminate().RunSynchronously();
                return;
            }

            Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell(message);
        }
    }
}