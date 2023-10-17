using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;
using Akka.Util.Internal;
using ChartApp.Actors;

namespace ChartApp
{
    public partial class Main : Form
    {
        private IActorRef _chartActor;
        private readonly AtomicCounter _seriesCounter = new AtomicCounter(1);

        public Main()
        {
            InitializeComponent();
        }

        #region Initialization


        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
            var series = ChartDataHelper.RandomSeries(GetFakeSeriesName());
            var initializeChartDictionary = new Dictionary<string, Series>()
            {
                {
                    series.Name,
                    series
                }
            };
            _chartActor.Tell(new ChartingActor.InitializeChart(initializeChartDictionary));
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            _chartActor.Tell(PoisonPill.Instance);

            //shut down the ActorSystem
            Program.ChartActors.Terminate();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            var series = ChartDataHelper.RandomSeries(GetFakeSeriesName());
            _chartActor.Tell(new ChartingActor.AddSeries(series));
        }

        private string GetFakeSeriesName()
        {
            return $"FakeSeries{_seriesCounter.GetAndIncrement()}";
        }
    }
}
