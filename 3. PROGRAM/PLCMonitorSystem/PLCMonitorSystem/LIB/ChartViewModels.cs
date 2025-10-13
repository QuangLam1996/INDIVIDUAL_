using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCMonitorSystem
{
    class BarChartViewModel
    {
        public PlotModel MyModel { get; private set; }

        public BarChartViewModel(String title, List<double> values, List<string> axeLabels)
        {
            this.MyModel = new PlotModel { Title = title };

            var barItems = new List<BarItem>();
            double maxVal = 0;
            foreach (var v in values)
            {
                barItems.Add(new BarItem(v));
                if (v > maxVal)
                {
                    maxVal = v;
                }
            }

            var barSeries = new BarSeries
            {
                ItemsSource = barItems,
                LabelMargin = 5,
                LabelPlacement = LabelPlacement.Outside,
                TextColor = OxyColors.Blue,
                LabelFormatString = "{0}",
            };

            barSeries.FillColor = OxyColors.SkyBlue;

            MyModel.Series.Add(barSeries);

            MyModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                TickStyle = TickStyle.None,
                ItemsSource = axeLabels,
                IsZoomEnabled = false,
                IsPanEnabled = false
            }); ;

            var X = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Maximum = maxVal * 1.1,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            MyModel.Axes.Add(X);
        }
    }

    class LineChartViewModel1
    {
        public PlotModel MyModel { get; set; }

        public LineChartViewModel1(String xAxisName, String yAxisName, List<List<double>> values, List<List<string>> xAxisLabels)
        {
            this.MyModel = new PlotModel();

            List<List<DataPoint>> lstDataPoint = new List<List<DataPoint>>();
            double maxVal = 0;
            double minVal = double.MaxValue;
            for (int i = 0; i < values.Count; i++)
            {
                lstDataPoint.Add(new List<DataPoint>());
                for (int j = 0; j < values[i].Count; j++)
                {
                    var v = values[i][j];
                    lstDataPoint[i].Add(new DataPoint(j, v));
                    if (v > maxVal)
                    {
                        maxVal = v;
                    }
                    if (v < minVal)
                    {
                        minVal = v;
                    }
                }
            }
            for (int i = 0; i < values.Count; i++)
            {
                var lineSeries = new LineSeries
                {
                    ItemsSource = lstDataPoint[i],
                    MarkerType = MarkerType.Circle
                };
                switch (i)
                {
                    case 0:
                        lineSeries.MarkerFill = OxyColor.FromRgb(25, 25, 25);
                        break;
                    case 1:
                        lineSeries.MarkerFill = OxyColor.FromRgb(25, 100, 25);
                        break;
                    case 2:
                        lineSeries.MarkerFill = OxyColor.FromRgb(25, 100, 200);
                        break;
                    case 3:
                        lineSeries.MarkerFill = OxyColor.FromRgb(100, 100, 200);
                        break;
                    case 4:
                        lineSeries.MarkerFill = OxyColor.FromRgb(100, 10, 200);
                        break;
                    default:
                        lineSeries.MarkerFill = OxyColor.FromRgb(100, 150, 200);
                        break;
                }
                MyModel.Series.Add(lineSeries);
            }

            MyModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = xAxisName,
                TickStyle = TickStyle.None,
                ItemsSource = xAxisLabels,
                IsZoomEnabled = false,
                IsPanEnabled = false
            });

            var Y = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = yAxisName,
                Minimum = minVal * 0.9,
                Maximum = maxVal * 1.1,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            MyModel.Axes.Add(Y);
        }
    }
    class LineChartViewModel
    {
        public PlotModel MyModel { get; set; }

        public LineChartViewModel(String xAxisName, String yAxisName, List<double> values, List<string> xAxisLabels)
        {
            this.MyModel = new PlotModel();

            List<DataPoint> lstDataPoint = new List<DataPoint>();
            double maxVal = 0;
            double minVal = double.MaxValue;

            for (int j = 0; j < values.Count; j++)
            {
                var v = values[j];
                lstDataPoint.Add(new DataPoint(j, v));
                if (v > maxVal)
                {
                    maxVal = v;
                }
                if (v < minVal)
                {
                    minVal = v;
                }
            }

            var lineSeries = new LineSeries
            {
                ItemsSource = lstDataPoint,
                MarkerFill = OxyColor.FromRgb(100, 100, 100),
                MarkerType = MarkerType.Circle
            };

            MyModel.Series.Add(lineSeries);

            MyModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = xAxisName,
                TickStyle = TickStyle.None,
                ItemsSource = xAxisLabels,
                IsZoomEnabled = false,
                IsPanEnabled = false
            });
            var Y = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = yAxisName,
                Minimum = minVal * 0.9,
                Maximum = maxVal * 1.1,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            MyModel.Axes.Add(Y);
        }
    }
}
