using System.Windows.Forms;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ButtonToggleActor : UntypedActor
    {
        private readonly IActorRef _coordinatorActor;
        private readonly Button _myButton;
        private readonly CounterType _myCounterType;
        private bool _isToggledOn;

        public class Toggle
        {

        }

        public ButtonToggleActor(IActorRef coordinatorActor, Button myButton, CounterType myCounterType, bool isToggledOn = false)
        {
            _coordinatorActor = coordinatorActor;
            _myButton = myButton;
            _myCounterType = myCounterType;
            _isToggledOn = isToggledOn;
        }
        protected override void OnReceive(object message)
        {
            if (message is Toggle && _isToggledOn)
            {
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.UnWatch(_myCounterType));
                FlipToggle();
            }
            else if (message is Toggle && !_isToggledOn)
            {
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_myCounterType));
                FlipToggle();
            }
            else
            {
                Unhandled(message);
            }
        }

        private void FlipToggle()
        {
            _isToggledOn = !_isToggledOn;
            _myButton.Text = $"{_myCounterType.ToString().ToUpperInvariant()} ({(_isToggledOn ? "ON" : "OFF")})";
        }
    }
}