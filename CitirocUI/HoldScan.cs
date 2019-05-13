using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Charting = System.Windows.Forms.DataVisualization.Charting;

namespace CitirocUI
{
    public partial class Citiroc : Form
    {
        private void button_holdScan_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_holdScan.IsBusy)
            {
                backgroundWorker_holdScan.CancelAsync();
                return;
            }
            else if (Firmware.readWord(100, usbDevId) == "00000000")
            {
                roundButton_connect.BackColor = Color.Gainsboro;
                roundButton_connect.ForeColor = Color.Black;
                roundButton_connectSmall.BackColor = Color.Gainsboro;
                roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff.png");
                connectStatus = -1;
                label_boardStatus.Text = "Board status\n" + "No board connected";
                button_loadFw.Visible = false;
                progressBar_loadFw.Visible = false;
                MessageBox.Show("No USB Devices found.", "Warning", MessageBoxButtons.OKCancel);
                return;
            }

            button_holdScan.Text = "Stop scan";

            int minCode = 0;
            int maxCode = 100;
            int step = 1;

            // Initialize min, max and step
            bool tryparse;
            tryparse = int.TryParse(textBox_stepCodeHoldScan.Text, out step);
            if (!tryparse) step = 0;
            if (step == 0) { step = 1; textBox_stepCodeHoldScan.Text = "1"; }
            tryparse = int.TryParse(textBox_minCodeHoldScan.Text, out minCode);
            if (!tryparse) minCode = 0;
            if (minCode <= 0) { minCode = 0; textBox_minCodeHoldScan.Text = "0"; }
            else if (minCode > 255) { minCode = 255; textBox_minCodeHoldScan.Text = "255"; }
            tryparse = int.TryParse(textBox_maxCodeHoldScan.Text, out maxCode);
            if (!tryparse) maxCode = 255;
            if (maxCode < step + minCode) { maxCode = step + minCode; textBox_maxCodeHoldScan.Text = (step + minCode).ToString(); }
            if (maxCode >= 255)
            {
                maxCode = 255; textBox_maxCodeHoldScan.Text = "255";
                if (minCode > maxCode - step) { minCode = maxCode - step; textBox_minCodeHoldScan.Text = (maxCode - step).ToString(); }
            }

            int nbAcq = 0;
            tryparse = int.TryParse(textBox_holdScanNbAcq.Text, out nbAcq);
            if (!tryparse) nbAcq = 100;
            textBox_holdScanNbAcq.Text = nbAcq.ToString();

            if (maxCode - minCode < step)
            {
                MessageBox.Show("The step size must be inferior to the difference between min and max code.");
                return;
            }

            holdScanLoadFile = textBox_holdScanSavePath.Text;
            granularity = Convert.ToInt32(textBox_granularity.Text);
            textBox_holdScanMaxGradient.Text = "10";

            if (!validPath(holdScanLoadFile)) { MessageBox.Show("The save path does not exist."); return; }

            // initialize chart
            chart_holdScan.Series.Clear();
            if (checkBox_showScatterPlot.Checked)
            {
                chart_holdScan.Series.Add("Scatter plot");
                chart_holdScan.Series["Scatter plot"].ChartType = Charting.SeriesChartType.Point;
                chart_holdScan.Series["Scatter plot"].MarkerStyle = Charting.MarkerStyle.Square;
            }
            chart_holdScan.Series.Add("Hold scan average");
            chart_holdScan.Series["Hold scan average"].ChartType = Charting.SeriesChartType.Line;
            chart_holdScan.Series["Hold scan average"].BorderWidth = 2;
            chart_holdScan.Series["Hold scan average"].Color = Color.Black;
            chart_holdScan.ChartAreas[0].AxisX.Minimum = 0;
            chart_holdScan.ChartAreas[0].AxisX.Maximum = maxCode + 1;
            chart_holdScan.ChartAreas[0].AxisY.IsStartedFromZero = false;

            // parameters to pass to the DoWork event of the background worker.
            object[] parameters = new object[] { minCode, maxCode, step, nbAcq };
            // start background worker
            backgroundWorker_holdScan.RunWorkerAsync(parameters);
        }

        private void backgroundWorker_holdScan_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] parameters = e.Argument as object[];
            int minCode = (int)parameters[0];
            int maxCode = (int)parameters[1];
            int step = (int)parameters[2];
            int nbAcq = (int)parameters[3];
            int FIFOAcqLength = 100;
            
            TextWriter tw = new StreamWriter(holdScanLoadFile);
            
            for (int delay = minCode; delay < maxCode + 1; delay += step)
            {
                Firmware.sendWord(30, IntToBin(delay, 8), usbDevId);

                int nbCycles = nbAcq / FIFOAcqLength;
                if (nbAcq % FIFOAcqLength != 0 || nbCycles == 0) nbCycles++; // add one cycle for the remainder or if nbCycle == 0 (ie nbAcq < 100)

                int[] chargeHG = new int[nbAcq];
                int[] chargeLG = new int[nbAcq];
                int[] points = new int[nbAcq];

                for (int cycle = 0; cycle < nbCycles; cycle++)
                {
                    int nbAcqInCycle = 0;
                    if (nbAcq >= FIFOAcqLength) nbAcqInCycle = FIFOAcqLength;
                    else if (nbAcq < FIFOAcqLength) nbAcqInCycle = nbAcq;

                    string strNbAcq = IntToBin(nbAcqInCycle, 8);
                    Firmware.sendWord(45, strNbAcq, usbDevId);

                    Firmware.sendWord(43, "10000000", usbDevId);

                    string rd4 = "00000000";

                    // bit 7 of subAdd 4 is 1 when the acquisitions are done
                    while (rd4.Substring(7, 1) == "0" && !backgroundWorker_holdScan.CancellationPending) { rd4 = Firmware.readWord(4, usbDevId); System.Threading.Thread.Sleep(10); }
                    if (backgroundWorker_holdScan.CancellationPending)
                    {
                        e.Cancel = true;
                        tw.Flush();
                        tw.Close();
                        return;
                    }

                    string subAdd22 = Firmware.readWord(22, usbDevId);

                    if (subAdd22 != "00000000") { cycle -= 1; continue; }

                    int nbData = (NbChannels + 1) * nbAcqInCycle;

                    byte[] FIFO20 = Firmware.readWord(20, nbData, usbDevId);
                    byte[] FIFO21 = Firmware.readWord(21, nbData, usbDevId);
                    byte[] FIFO23 = Firmware.readWord(23, nbData, usbDevId);
                    byte[] FIFO24 = Firmware.readWord(24, nbData, usbDevId);

                    byte[] FIFOHG = new byte[2 * nbData];
                    byte[] FIFOLG = new byte[2 * nbData];

                    for (int i = 0; i < nbData; i++)
                    {
                        FIFOHG[i * 2 + 1] = FIFO20[i];
                        FIFOHG[i * 2 + 0] = FIFO21[i];
                        FIFOLG[i * 2 + 1] = FIFO23[i];
                        FIFOLG[i * 2 + 0] = FIFO24[i];
                    }

                    BitArray bitArrayHG = new BitArray(FIFOHG);
                    BitArray bitArrayLG = new BitArray(FIFOLG);

                    int chn = (int)(numericUpDown_channelHoldScan.Value);

                    for (int i = 0; i < nbAcqInCycle; i++)
                    {
                        bool[] boolArrayDataHG = { bitArrayHG[i * 528 + chn * 16 + 0], bitArrayHG[i * 528 + chn * 16 + 1], bitArrayHG[i * 528 + chn * 16 + 2], bitArrayHG[i * 528 + chn * 16 + 3], bitArrayHG[i * 528 + chn * 16 + 4], bitArrayHG[i * 528 + chn * 16 + 5],
                            bitArrayHG[i * 528 + chn * 16 + 6], bitArrayHG[i * 528 + chn * 16 + 7], bitArrayHG[i * 528 + chn * 16 + 8], bitArrayHG[i * 528 + chn * 16 + 9], bitArrayHG[i * 528 + chn * 16 + 10], bitArrayHG[i * 528 + chn * 16 + 11] };
                        BitArray bitArrayDataHG = new BitArray(boolArrayDataHG);
                        int[] array = new int[1];
                        bitArrayDataHG.CopyTo(array, 0);
                        chargeHG[i + FIFOAcqLength * cycle] = array[0];

                        bool[] boolArrayDataLG = { bitArrayLG[i * 528 + chn * 16 + 0], bitArrayLG[i * 528 + chn * 16 + 1], bitArrayLG[i * 528 + chn * 16 + 2], bitArrayLG[i * 528 + chn * 16 + 3], bitArrayLG[i * 528 + chn * 16 + 4], bitArrayLG[i * 528 + chn * 16 + 5],
                            bitArrayLG[i * 528 + chn * 16 + 6], bitArrayLG[i * 528 + chn * 16 + 7], bitArrayLG[i * 528 + chn * 16 + 8], bitArrayLG[i * 528 + chn * 16 + 9], bitArrayLG[i * 528 + chn * 16 + 10], bitArrayLG[i * 528 + chn * 16 + 11] };
                        BitArray bitArrayDataLG = new BitArray(boolArrayDataLG);
                        bitArrayDataLG.CopyTo(array, 0);
                        chargeLG[i + FIFOAcqLength * cycle] = array[0];
                    }

                    Firmware.sendWord(43, "00000000", usbDevId);
                }

                if (checkBox_holdScanChoice.Checked) points = chargeLG;
                else points = chargeHG;

                string outdata = delay.ToString();
                for (int i = 0; i < points.Length; i++) outdata += " " + points[i];
                tw.WriteLine(outdata);
                
                object[] progressChangedParameters = { delay, points };
                backgroundWorker_holdScan.ReportProgress(0, progressChangedParameters);
            }
            tw.Flush();
            tw.Close();
        }

        private void backgroundWorker_holdScan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button_holdScan.Text = "Hold scan";
            Firmware.sendWord(43, "00000000", usbDevId);

            if (e.Cancelled) return;

            double[] x = new double[chart_holdScan.Series["Hold scan average"].Points.Count];
            double[] y = new double[chart_holdScan.Series["Hold scan average"].Points.Count];
            int index = 0;

            // getting back the data points from the chart
            foreach (Charting.DataPoint point in chart_holdScan.Series["Hold scan average"].Points)
            {
                x[index] = point.XValue;
                y[index] = point.YValues[0];
                index++;
            }

            // fit the data points with an order 6 polynomial
            double[] fitResult = Polyfit(x, y, 6);

            chart_holdScan.Series.Add("Fit");
            chart_holdScan.Series["Fit"].ChartType = Charting.SeriesChartType.Line;
            chart_holdScan.Series["Fit"].BorderDashStyle = Charting.ChartDashStyle.Dash;
            chart_holdScan.Series["Fit"].BorderWidth = 2;
            chart_holdScan.Series["Fit"].Color = Color.Gray;

            // plot the fit
            for (int i = (int)x.Min(); i < x.Max(); i++)
                chart_holdScan.Series["Fit"].Points.AddXY(i, fitResult[6] * i * i * i * i * i * i + fitResult[5] * i * i * i * i * i + fitResult[4] * i * i * i * i + fitResult[3] * i * i * i + fitResult[2] * i * i + fitResult[1] * i + fitResult[0]);

            // change color of scatter plot
            if (checkBox_showScatterPlot.Checked)
            {
                chart_holdScan.Series["Scatter plot"].MarkerSize = granularity * (int)Math.Ceiling((double)chart_holdScan.Height / (chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMaximum - chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMinimum));
                double minValue = Convert.ToDouble(textBox_holdScanMinGradient.Text);
                double maxValue = Convert.ToDouble(textBox_holdScanMaxGradient.Text) - minValue;
                for (int i = 0; i < chart_holdScan.Series["Scatter plot"].Points.Count; i++)
                {
                    double point = (Convert.ToDouble(chart_holdScan.Series["Scatter plot"].Points[i].ToolTip) - minValue) / maxValue;
                    if (point > 1) point = 1;
                    chart_holdScan.Series["Scatter plot"].Points[i].Color = HSL2RGB(0.67 - point * 0.67, 0.7, 0.7);
                }
            }
            
            // get the x coordinate of the y maximum and write it in the slow control delay textbox
            Charting.DataPoint dataPoint = chart_holdScan.Series["Fit"].Points.FindMaxByValue();
            textBox_delay.Text = ((int)dataPoint.XValue).ToString();
            MessageBox.Show("Optimal delay value has been found to be " + textBox_delay.Text + ". Delay parameter has been updated.");
            // Send the FPGA word 30 with updated value.
            Firmware.sendWord(30, IntToBin((int)dataPoint.XValue, 8), usbDevId);

            resetZoom(chart_holdScan);
        }

        private void backgroundWorker_holdScan_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] progressChangedParameters = e.UserState as object[];

            int delay = (int)progressChangedParameters[0];
            int[] points = (int[])progressChangedParameters[1];

            chart_holdScan.Series["Hold scan average"].Points.AddXY(delay, (double)(points.Sum()) / points.Length);
            
            double[] charge = new double[4096 / granularity];
            for (int i = 0; i < points.Length; i++) charge[points[i] / granularity]++;
            
            double maxValue = Math.Max(Math.Max(10, charge.Max()), Convert.ToInt32(textBox_holdScanMaxGradient.Text));
            textBox_holdScanMaxGradient.Text = maxValue.ToString();
            
            if (checkBox_showScatterPlot.Checked)
                for (int i = 0; i < charge.Length; i++)
                {
                    if (charge[i] > Convert.ToInt32(textBox_holdScanMinGradient.Text))
                    {
                        chart_holdScan.Series["Scatter plot"].Points.AddXY(delay, i * granularity);
                        chart_holdScan.Series["Scatter plot"].Points.Last().ToolTip = charge[i].ToString();
                        //chart_holdScan.Series["Scatter plot"].Points.Last().Color = HSL2RGB(0.67 - charge[i] / maxValue * 0.67, 0.7, 0.7);
                    }
                }
        }
        
        private void textBox_minCodeHoldScan_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox_minCodeHoldScan.Text) > Convert.ToInt32(textBox_maxCodeHoldScan.Text))
                    textBox_minCodeHoldScan.Text = textBox_maxCodeHoldScan.Text;
            }
            catch { textBox_minCodeHoldScan.Text = "0"; return; }
        }

        private void textBox_maxCodeHoldScan_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox_maxCodeHoldScan.Text) < Convert.ToInt32(textBox_minCodeHoldScan.Text))
                    textBox_maxCodeHoldScan.Text = textBox_minCodeHoldScan.Text;
                if (Convert.ToInt32(textBox_maxCodeHoldScan.Text) > 255)
                    textBox_maxCodeHoldScan.Text = "255";
            }
            catch { textBox_maxCodeHoldScan.Text = "255"; return; }
        }

        public static double[] Polyfit(double[] x, double[] y, int degree)
        {
            // Vandermonde matrix
            var v = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(x.Length, degree + 1);
            for (int i = 0; i < v.RowCount; i++)
                for (int j = 0; j <= degree; j++) v[i, j] = Math.Pow(x[i], j);

            var yv = new MathNet.Numerics.LinearAlgebra.Double.DenseVector(y).ToColumnMatrix();
            MathNet.Numerics.LinearAlgebra.Factorization.QR<double> qr = v.QR();
            // Math.Net doesn't have an "economy" QR, so:
            // cut R short to square upper triangle, then recompute Q
            var r = qr.R.SubMatrix(0, degree + 1, 0, degree + 1);
            var q = v.Multiply(r.Inverse());
            var p = r.Inverse().Multiply(q.TransposeThisAndMultiply(yv));
            return p.Column(0).ToArray();
        }
        
        private void button_holdScanSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog DataSDialog = new SaveFileDialog();
            DataSDialog.Title = "Specify Output file";
            DataSDialog.Filter = "All Files(*.*)|*.*";
            DataSDialog.RestoreDirectory = true;
            if (DataSDialog.ShowDialog() == DialogResult.OK) textBox_holdScanSavePath.Text = DataSDialog.FileName;
        }

        string holdScanLoadFile;
        private void button_loadHoldScan_Click(object sender, EventArgs e)
        {
            // Open dialog box
            OpenFileDialog DataLoadDialog = new OpenFileDialog();
            DataLoadDialog.Title = "Specify Data file";
            //DataLDialog.Filter = "TxT files|*.txt";
            DataLoadDialog.RestoreDirectory = true;

            if (DataLoadDialog.ShowDialog() == DialogResult.OK && DataLoadDialog.FileName != null)
            {
                holdScanLoadFile = DataLoadDialog.FileName;
                FileInfo info = new FileInfo(holdScanLoadFile);
                if (!info.Exists) { MessageBox.Show("The file does not exist."); return; }
                else LoadHoldScan();
            }
            else return;
        }

        int granularity = 2;
        private void LoadHoldScan()
        {
            if (holdScanLoadFile == null) return;
            FileInfo info = new FileInfo(holdScanLoadFile);
            if (!info.Exists) return;

            Label dataLoading = new Label();
            ProgressBar progress = new ProgressBar();
            if (info.Length > 1e6)
            {
                tabPage_holdScan.Controls.Add(dataLoading);
                tabPage_holdScan.Controls.Add(progress);
                dataLoading.Text = "Please wait while data are being loaded.";
                dataLoading.TextAlign = ContentAlignment.MiddleCenter;
                dataLoading.Size = new Size((int)(500 * scale), (int)(100 * scale));
                progress.Size = new Size((int)(400 * scale), (int)(20 * scale));
                dataLoading.BorderStyle = BorderStyle.FixedSingle;
                dataLoading.Location = new Point(tabPage_holdScan.Width / 2 - dataLoading.Width / 2, tabPage_holdScan.Height / 2);
                progress.Location = new Point(tabPage_holdScan.Width / 2 - progress.Width / 2, (int)(tabPage_holdScan.Height / 2 * 1.28));
                dataLoading.BringToFront();
                progress.BringToFront();
                Update();
            }

            ArrayList DataArrayList;

            if (holdScanLoadFile == null) return;
            else DataArrayList = ReadFileLine(holdScanLoadFile);

            int DataArrayListCount = DataArrayList.Count;

            if (DataArrayListCount <= 1) return;

            string[] DataArray = new string[DataArrayListCount];
            DataArrayList.CopyTo(DataArray);

            // initialize chart
            chart_holdScan.Series.Clear();
            if (checkBox_showScatterPlot.Checked)
            {
                chart_holdScan.Series.Add("Scatter plot");
                chart_holdScan.Series["Scatter plot"].ChartType = Charting.SeriesChartType.Point;
                chart_holdScan.Series["Scatter plot"].MarkerStyle = Charting.MarkerStyle.Square;
            }
            chart_holdScan.Series.Add("Hold scan average");
            chart_holdScan.Series["Hold scan average"].ChartType = Charting.SeriesChartType.Line;
            chart_holdScan.Series["Hold scan average"].BorderWidth = 2;
            chart_holdScan.Series["Hold scan average"].Color = Color.Black;
            chart_holdScan.ChartAreas[0].AxisX.Minimum = 0;
            chart_holdScan.ChartAreas[0].AxisY.IsStartedFromZero = false;

            granularity = Convert.ToInt32(textBox_granularity.Text);
            double maxValue = 1;

            for (int i = 0; i < DataArrayListCount; i++)
            {
                string[] DataSplit = DataArray[i].Split(' ');
                int delay = Convert.ToInt32(DataSplit[0]);
                double average = 0;

                double[] charge = new double[4096 / granularity];
                for (int ii = 1; ii < DataSplit.Length; ii++)
                {
                    charge[Convert.ToInt32(DataSplit[ii]) / granularity]++;
                    average += Convert.ToDouble(DataSplit[ii]);
                }

                chart_holdScan.Series["Hold scan average"].Points.AddXY(delay, average / (DataSplit.Length - 1));

                maxValue = Math.Max(charge.Max(), maxValue);
                label_holdScanMaxGradient.Text = "(" + maxValue.ToString() + ")";

                if (checkBox_showScatterPlot.Checked)
                    for (int ii = 0; ii < charge.Length; ii++)
                    {
                        if (charge[ii] > Convert.ToInt32(textBox_holdScanMinGradient.Text))
                        {
                            chart_holdScan.Series["Scatter plot"].Points.AddXY(delay, ii * granularity);
                            chart_holdScan.Series["Scatter plot"].Points.Last().ToolTip = charge[ii].ToString();
                            //chart_holdScan.Series["Scatter plot"].Points.Last().Color = HSL2RGB(0.67 - charge[ii] / maxValue * 0.67, 0.7, 0.7);
                        }
                    }

                progress.Value = i * 100 / DataArrayListCount;
            }

            double[] x = new double[chart_holdScan.Series["Hold scan average"].Points.Count];
            double[] y = new double[chart_holdScan.Series["Hold scan average"].Points.Count];
            int index = 0;

            // getting back the data points from the chart
            foreach (Charting.DataPoint point in chart_holdScan.Series["Hold scan average"].Points)
            {
                x[index] = point.XValue;
                y[index] = point.YValues[0];
                index++;
            }

            chart_holdScan.ChartAreas[0].AxisX.Maximum = x.Max() + 1;

            // fit the data points with an order 6 polynomial
            double[] fitResult = Polyfit(x, y, 6);

            chart_holdScan.Series.Add("Fit");
            chart_holdScan.Series["Fit"].ChartType = Charting.SeriesChartType.Line;
            chart_holdScan.Series["Fit"].BorderDashStyle = Charting.ChartDashStyle.Dash;
            chart_holdScan.Series["Fit"].BorderWidth = 2;
            chart_holdScan.Series["Fit"].Color = Color.Gray;

            // plot the fit
            for (int i = (int)x.Min(); i < x.Max(); i++)
                chart_holdScan.Series["Fit"].Points.AddXY(i, fitResult[6] * i * i * i * i * i * i + fitResult[5] * i * i * i * i * i + fitResult[4] * i * i * i * i + fitResult[3] * i * i * i + fitResult[2] * i * i + fitResult[1] * i + fitResult[0]);

            chart_holdScan.Update(); // need to update before accessing scaleview parameters
            if (checkBox_showScatterPlot.Checked)
            {
                // change color of scatter plot
                if (checkBox_showScatterPlot.Checked)
                {
                    chart_holdScan.Series["Scatter plot"].MarkerSize = granularity * (int)Math.Ceiling((double)chart_holdScan.Height / (chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMaximum - chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMinimum));
                    double minValue = Convert.ToDouble(textBox_holdScanMinGradient.Text);
                    maxValue = Convert.ToDouble(textBox_holdScanMaxGradient.Text) - minValue;
                    for (int i = 0; i < chart_holdScan.Series["Scatter plot"].Points.Count; i++)
                    {
                        double point = (Convert.ToDouble(chart_holdScan.Series["Scatter plot"].Points[i].ToolTip) - minValue) / maxValue;
                        if (point > 1) point = 1;
                        chart_holdScan.Series["Scatter plot"].Points[i].Color = HSL2RGB(0.67 - point * 0.67, 0.7, 0.7);
                    }
                }
            }
            
            resetZoom(chart_holdScan);
            progress.Dispose();
            dataLoading.Dispose();
        }

        private void button_scatterToPng_Click(object sender, EventArgs e)
        {
            if (holdScanLoadFile == null) { MessageBox.Show("No data to plot."); return; }
            FileInfo info = new FileInfo(holdScanLoadFile);
            if (!info.Exists) return;

            string imageSavePath = "";

            SaveFileDialog DataSDialog = new SaveFileDialog();
            DataSDialog.Title = "Specify Output file";
            DataSDialog.Filter = ".png (*.png)|*.png";
            DataSDialog.RestoreDirectory = true;
            if (DataSDialog.ShowDialog() == DialogResult.OK) imageSavePath = DataSDialog.FileName;
            else return;

            Label imageProcessing = new Label();
            ProgressBar progress = new ProgressBar();
            if (info.Length > 1e6)
            {
                tabPage_holdScan.Controls.Add(imageProcessing);
                tabPage_holdScan.Controls.Add(progress);
                imageProcessing.Text = "Please wait during image creation.";
                imageProcessing.TextAlign = ContentAlignment.MiddleCenter;
                imageProcessing.Size = new Size((int)(500 * scale), (int)(100 * scale));
                progress.Size = new Size((int)(400 * scale), (int)(20 * scale));
                imageProcessing.BorderStyle = BorderStyle.FixedSingle;
                imageProcessing.Location = new Point(tabPage_holdScan.Width / 2 - imageProcessing.Width / 2, tabPage_holdScan.Height / 2);
                progress.Location = new Point(tabPage_holdScan.Width / 2 - progress.Width / 2, (int)(tabPage_holdScan.Height / 2 * 1.28));
                imageProcessing.BringToFront();
                progress.BringToFront();
                Update();
            }

            ArrayList DataArrayList;

            if (holdScanLoadFile == null) return;
            else DataArrayList = ReadFileLine(holdScanLoadFile);

            int DataArrayListCount = DataArrayList.Count;

            if (DataArrayListCount <= 1) return;

            string[] DataArray = new string[DataArrayListCount];
            DataArrayList.CopyTo(DataArray);

            granularity = Convert.ToInt32(textBox_granularity.Text);
            int ptWidth = (4096 / granularity) / 255;
            Bitmap outputImage = new Bitmap(DataArrayListCount * ptWidth, 4096 / granularity);
            Graphics grph = Graphics.FromImage(outputImage);

            // create canvas
            grph.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(0,0), outputImage.Size));
            for (int i = 0; i < 4096; i += 200)
            {
                Rectangle rect = new Rectangle(0, i / granularity, outputImage.Width, 1);
                grph.FillRectangle(new SolidBrush(Color.Black), rect);
            }
            for (int i = 0; i < 255; i += 50)
            {
                Rectangle rect = new Rectangle(i * ptWidth, 0, 1, outputImage.Height);
                grph.FillRectangle(new SolidBrush(Color.Black), rect);
            }

            double minValue = Convert.ToDouble(textBox_holdScanMinGradient.Text);
            double maxValue = Convert.ToDouble(textBox_holdScanMaxGradient.Text) - minValue;

            // create scatter plot
            for (int i = 0; i < DataArrayListCount; i++)
            {
                string[] DataSplit = DataArray[i].Split(' ');
                int delay = Convert.ToInt32(DataSplit[0]);

                double[] charge = new double[4096 / granularity];
                for (int ii = 1; ii < DataSplit.Length; ii++)
                {
                    charge[Convert.ToInt32(DataSplit[ii]) / granularity]++;
                }
                                
                for (int ii = 0; ii < charge.Length; ii++)
                {
                    if (charge[ii] > Convert.ToInt32(textBox_holdScanMinGradient.Text))
                    {
                        double point = (charge[ii] - minValue) / maxValue;
                        if (point > 1) point = 1;
                        Color color = HSL2RGB(0.67 - point * 0.67, 0.7, 0.7);
                        Rectangle rect = new Rectangle(delay * ptWidth, ii, ptWidth, 1);
                        grph.FillRectangle(new SolidBrush(color), rect);
                    }
                }

                progress.Value = i * 100 / DataArrayListCount;
            }

            outputImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            outputImage.Save(imageSavePath, System.Drawing.Imaging.ImageFormat.Png);
            
            progress.Dispose();
            imageProcessing.Dispose();
        }
        
        private void checkBox_showScatterPlot_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox_showScatterPlot.Checked) LoadHoldScan();
                else
                {
                    chart_holdScan.Series.Remove(chart_holdScan.Series["Scatter plot"]);
                    resetZoom(chart_holdScan);
                }
            }
            catch { }
        }

        private void button_refreshHoldScan_Click(object sender, EventArgs e)
        {
            LoadHoldScan();
        }

        private void textBox_holdScanMaxGradient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                try
                {
                    double minValue = Convert.ToDouble(textBox_holdScanMinGradient.Text);
                    double maxValue = Convert.ToDouble(textBox_holdScanMaxGradient.Text) - minValue;
                    for (int i = 0; i < chart_holdScan.Series["Scatter plot"].Points.Count; i++)
                    {
                        double point = (Convert.ToDouble(chart_holdScan.Series["Scatter plot"].Points[i].ToolTip) - minValue) / maxValue;
                        if (point > 1) point = 1;
                        chart_holdScan.Series["Scatter plot"].Points[i].Color = HSL2RGB(0.67 - point * 0.67, 0.7, 0.7);
                    }
                }
                catch { }
            }                
        }
        
        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static Color HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;

            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            Color rgb = Color.FromArgb(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));

            return rgb;
        }
        
    }
}
