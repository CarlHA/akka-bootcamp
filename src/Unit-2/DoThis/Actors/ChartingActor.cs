using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        public const int MaxPoints = 250;

        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; }
        }

        public class AddSeries
        {
            public Series Series { get; }

            public AddSeries(Series series)
            {
                Series = series;
            }
        }
        #endregion

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;
        private int xPosCounter = 0;

        public ChartingActor(Chart chart) : this(chart, new Dictionary<string, Series>())
        {
        }

        public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;

            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
            Receive<RemoveSeries>(rs => HandleRemoveSeries(rs));
            Receive<Metric>(m => HandleMetrics(m));
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                //swap the two series out
                _seriesIndex = ic.InitialSeries;
            }

            //delete any existing series
            _chart.Series.Clear();
            var area = _chart.ChartAreas[0];
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;
            SetChartBoundaries();
            //attempt to render the initial chart
            if (_seriesIndex.Any())
            {
                foreach (var series in _seriesIndex)
                {
                    //force both the chart and the internal index to use the same names
                    series.Value.Name = series.Key;
                    _chart.Series.Add(series.Value);
                }
            }

            SetChartBoundaries();
        }

        private void HandleAddSeries(AddSeries series)
        {
            if (!String.IsNullOrEmpty(series.Series.Name) && !_seriesIndex.ContainsKey(series.Series.Name))
            {
                _seriesIndex.Add(series.Series.Name, series.Series);
                _chart.Series.Add(series.Series);
                SetChartBoundaries();
            }
        }

        private void HandleRemoveSeries(RemoveSeries series)
        {
            if (!String.IsNullOrEmpty(series.SeriesName) && _seriesIndex.ContainsKey(series.SeriesName))
            {
                var seriesToRemove = _seriesIndex[series.SeriesName];
                _seriesIndex.Remove(series.SeriesName);
                _chart.Series.Remove(seriesToRemove);
                SetChartBoundaries();
            }
        }

        private void HandleMetrics(Metric metric)
        {
            if (!String.IsNullOrEmpty(metric.Series) && _seriesIndex.ContainsKey(metric.Series))
            {
                var series = _seriesIndex[metric.Series];
                if(series.Points == null)return;
                series.Points.AddXY(xPosCounter++, metric.CounterValue);
                while (series.Points.Count>MaxPoints)
                {
                    series.Points.RemoveAt(0);
                }

                SetChartBoundaries();
            }
        }

        #endregion

        public class RemoveSeries
        {
            public string SeriesName { get; }

            public RemoveSeries(string seriesName)
            {
                SeriesName = seriesName;
            }
        }

        private void SetChartBoundaries()
        {
            double maxAxisX, maxAxisY, minAxisX, minAxisY = 0.0d;
            var allPoints = _seriesIndex.Values.SelectMany(series => series.Points).ToList();
            var yValues = allPoints.SelectMany(point => point.YValues).ToList();
            maxAxisX = xPosCounter;
            minAxisX = xPosCounter - MaxPoints;
            maxAxisY = yValues.Count > 0 ? Math.Ceiling(yValues.Max()) : 1.0d;
            minAxisY = yValues.Count > 0 ? Math.Floor(yValues.Min()) : 0.0d;

            if (minAxisY == maxAxisY)
            {
                maxAxisY++;
            }

            var area = _chart.ChartAreas[0];
            area.AxisY.Minimum = minAxisY;
            area.AxisY.Maximum = Math.Max(1.0d, maxAxisY);
            if (allPoints.Count > 2)
            {
                area.AxisX.Minimum = minAxisX;
                area.AxisX.Maximum = maxAxisX;
            }
        }
    }
}
