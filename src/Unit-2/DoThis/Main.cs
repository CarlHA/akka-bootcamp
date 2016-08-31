using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Akka.Actor;
using ChartApp.Actors;

namespace ChartApp
{
    public partial class Main : Form
    {
        private const string UiDispatcher = "akka.actor.synchronized-dispatcher";
        private IActorRef _chartActor;
        private IActorRef _coordinatorActor;
        private readonly Dictionary<CounterType, IActorRef> _toggleActors = new Dictionary<CounterType, IActorRef>();

        public Main()
        {
            InitializeComponent();
        }

        #region Initialization


        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart, PauseResumeButton)), "charting");
            _chartActor.Tell(new ChartingActor.InitializeChart(null));
            _coordinatorActor = Program.ChartActors.ActorOf(Props.Create(() => new PerformanceCounterCoordinatorActor(_chartActor)), "counters");
            _toggleActors[CounterType.Cpu] =
                Program.ChartActors.ActorOf(
                           Props.Create(() => new ButtonToggleActor(_coordinatorActor, CpuButton, CounterType.Cpu, false))
                                .WithDispatcher(UiDispatcher));
            _toggleActors[CounterType.Memory] =
                Program.ChartActors.ActorOf(
                           Props.Create(() => new ButtonToggleActor(_coordinatorActor, MemoryButton, CounterType.Memory, false))
                                .WithDispatcher(UiDispatcher));
            _toggleActors[CounterType.Disk] =
                Program.ChartActors.ActorOf(
                           Props.Create(() => new ButtonToggleActor(_coordinatorActor, DiskButton, CounterType.Disk, false))
                                .WithDispatcher(UiDispatcher));
            _toggleActors[CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            _chartActor.Tell(PoisonPill.Instance);

            //shut down the ActorSystem
            Program.ChartActors.Terminate();
        }

        #endregion

        private void CpuButton_Click(object sender, EventArgs e)
        {
            _toggleActors[CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
        }

        private void MemoryButton_Click(object sender, EventArgs e)
        {
            _toggleActors[CounterType.Memory].Tell(new ButtonToggleActor.Toggle());
        }

        private void DiskButton_Click(object sender, EventArgs e)
        {
            _toggleActors[CounterType.Disk].Tell(new ButtonToggleActor.Toggle());
        }

        private void PauseResumeButton_Click(object sender, EventArgs e)
        {
            _chartActor.Tell(new ChartingActor.TogglePause());
        }
    }
}
