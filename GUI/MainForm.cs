using RBFNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GUI
{
    public partial class MainForm : Form, IView
    {
        private const string STR_Expected = "Expected";
        private const string STR_Output = "Output";
        private const string STR_Error = "Error";

        public List<DataPoint> DataPoints { get; set; }
        private NeuralNetworkModel model;

        public MainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Load += MainForm_Load;
        }

        private Series CreateSeries(string name)
        {
            Series expectedSeries = new Series(name);
            expectedSeries.ChartType = SeriesChartType.Line;
            expectedSeries.XValueType = ChartValueType.Double;
            expectedSeries.BorderWidth = 5;
            expectedSeries.ToolTip = "Курс. 1: #VALY \nДата: 19#VALX";
            return expectedSeries;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            chart.Series.Clear();
            Series expectedSeries = CreateSeries(STR_Expected);
            Series outputSeries = CreateSeries(STR_Output);
            //Series errorSeries = new Series(STR_Error);
            //errorSeries.ChartType = SeriesChartType.Line;
            //errorSeries.BorderWidth = 5;

            //Axis XA = chart.ChartAreas[0].AxisX;
            //XA.MajorGrid.Enabled = false;
            //XA.LabelStyle.Format = "MMM";
            //XA.IntervalType = DateTimeIntervalType.Months;
            //XA.Interval = 1;
            //chart.ChartAreas[0].AxisY.Interval = 1;

            chart.Series.Add(expectedSeries);
            chart.Series.Add(outputSeries);
            //chart.Series.Add(errorSeries);
            await LoadData();
        }

        private async Task LoadData()
        {
            var dataset = new Dataset();
            Assembly assembly = Assembly.GetExecutingAssembly();

            //var datasetStream = assembly.GetManifestResourceStream("GUI.Resources.trainTemp.csv");


            var datasetStream = assembly.GetManifestResourceStream("GUI.Resources.ALL.csv");
            await dataset.LoadTemperature(new StreamReader(datasetStream));
            var ds = new Dataset();
            var dsStream = assembly.GetManifestResourceStream("GUI.Resources.RSMOSCOW-test.csv");
            await ds.LoadTemperature(new StreamReader(dsStream));
            model = new NeuralNetworkModel(dataset, ds);

        }

        private void BtnTrain_Click(object sender, EventArgs e)
        {            
            Cursor = Cursors.WaitCursor;
            tbTrainError.Text = model.Train().ToString();
            Cursor = Cursors.Default;
        }

        private void BtnPredict_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbTrainError.Text))
                return;
            DataPoints = model.Predict();
            var months = new List<string> {
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec" };
            int m = -1;
            int k = 0;
            double error = 0;
            double rightAnswers = 0;
            DataPoints.ForEach(dp =>
            {
                if (dp.Expected > 0 && dp.Output > 0)
                {
                    chart.Series[STR_Expected].Points.AddXY(dp.X, dp.Expected);
                    chart.Series[STR_Output].Points.AddXY(dp.X, dp.Output);

                    error += dp.Error;
                    if (dp.Expected - dp.Output <= 2)
                        rightAnswers++;
                    }
            }

            );

            tbPredictError.Text = Math.Sqrt((error / (DataPoints.Count - 1))).ToString();
            tbRightPredictions.Text = $"{rightAnswers} of {DataPoints.Count} == {Math.Round((rightAnswers * 100 / DataPoints.Count), 1)}%";
        }

        private void chart_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}