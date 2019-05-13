using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Charting = System.Windows.Forms.DataVisualization.Charting;

namespace CitirocUI
{
    public partial class Citiroc : Form
    {
        int[] positionScurves = new int[NbChannels];

        private void button_Scurves_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_Scurves.IsBusy) { backgroundWorker_Scurves.CancelAsync(); return; }

            try
            {
                if (Firmware.readWord(100, usbDevId) == "00000000")
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

                button_Scurves.Text = "Stop S-curves";

                // Initialize VthMin, VthMax and VthStep
                bool tryparse;
                int VthStep;
                tryparse = int.TryParse(textBox_stepScurves.Text, out VthStep);
                if (!tryparse) VthStep = 0;
                if (VthStep == 0) { VthStep = 1; textBox_stepScurves.Text = "1"; }
                int VthMin;
                tryparse = int.TryParse(textBox_minCodeScurves.Text, out VthMin);
                if (!tryparse) VthMin = 0;
                if (VthMin <= 0) { VthMin = 0; textBox_minCodeScurves.Text = "0"; }
                else if (VthMin > 1023) { VthMin = 1023; textBox_minCodeScurves.Text = "1023"; }
                int VthMax;
                tryparse = int.TryParse(textBox_maxCodeScurves.Text, out VthMax);
                if (!tryparse) VthMax = 1023;
                if (VthMax < VthStep + VthMin) { VthMax = VthStep + VthMin; textBox_maxCodeScurves.Text = (VthStep + VthMin).ToString(); }
                if (VthMax >= 1023)
                {
                    VthMax = 1023; textBox_maxCodeScurves.Text = "1023";
                    if (VthMin > VthMax - VthStep) { VthMin = VthMax - VthStep; textBox_minCodeScurves.Text = (VthMax - VthStep).ToString(); }
                }

                if (VthMax - VthMin < VthStep)
                {
                    MessageBox.Show("The step size must be inferior to the difference between min and max code.");
                    return;
                }
                
                int sleepTicks = 200000; // Sleep ticks depend on the clock used to acquire S-curves
                switch (comboBox_sCurvesClock.SelectedIndex)
                {
                    case 0:
                        Firmware.sendWord(7, "00000000", usbDevId); // set Scurve clock to 1 kHz
                        sleepTicks = 2000000;
                        break;
                    case 1:
                        Firmware.sendWord(7, "00001000", usbDevId); // set Scurve clock to 10 kHz
                        sleepTicks = 200000;
                        break;
                    case 2:
                        Firmware.sendWord(7, "00010000", usbDevId); // set Scurve clock to 50 kHz
                        sleepTicks = 40000;
                        break;
                    case 3:
                        Firmware.sendWord(7, "00011000", usbDevId); // set Scurve clock to 100 kHz
                        sleepTicks = 20000;
                        break;
                }
                
                // Initialize chart
                string seriesName = "";
                chart_Scurves.Series.Clear();
                chart_Scurves.Palette = Charting.ChartColorPalette.SeaGreen;
                for (int i = 0; i < NbChannels; i++)
                {
                    seriesName = "Scurve ch" + i.ToString();
                    chart_Scurves.Series.Add(seriesName);
                    chart_Scurves.Series[seriesName].ChartType = Charting.SeriesChartType.Line;
                    chart_Scurves.Series[seriesName].BorderWidth = 2;
                }

                Charting.Axis ax = chart_Scurves.ChartAreas[0].AxisX;
                Charting.Axis ay = chart_Scurves.ChartAreas[0].AxisY;

                chart_Scurves.ResetAutoValues();

                ax.Minimum = VthMin;
                ax.Maximum = VthMax;
                ax.MajorTickMark.LineColor = Color.White;
                ax.LineColor = Color.White;
                ay.Minimum = -5;
                ay.Maximum = 105;
                ay.Interval = 50;
                ay.LabelStyle.IntervalOffset = 5;
                ay.MajorGrid.IntervalOffset = 5;
                ay.MajorTickMark.IntervalOffset = 5;

                resetZoom(chart_Scurves);

                chart_Scurves.Enabled = false;

                // parameters to pass to the DoWork event of the background worker.
                object[] parameters = new object[] { VthMin, VthMax, VthStep, sleepTicks };
                // runs the event on new background worker thread.
                backgroundWorker_Scurves.RunWorkerAsync(parameters);
            }
            catch
            {
                MessageBox.Show("An error occured. Please verify the input parameters are correctly specified.");
                return;
            }
        }

        private void button_sCurveSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.Title = "Specify Output file";
            SaveDialog.Filter = "All Files(*.*)|*.*";
            SaveDialog.RestoreDirectory = true;
            if (SaveDialog.ShowDialog() == DialogResult.OK) textBox_sCurveSavePath.Text = SaveDialog.FileName;
        }

        private void backgroundWorker_Scurves_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get parameters passed as arguments
            object[] parameters = e.Argument as object[];
            int VthMin = (int)parameters[0];
            int VthMax = (int)parameters[1];
            int VthStep = (int)parameters[2];
            int nbPts = (VthMax - VthMin) / Math.Abs(VthStep) + 1;
            int sleepTicks = (int)parameters[3];

            // Initialize progress bar
            double progress = 1;
            // Create two dimensional double array to store S-curves data
            double[,] ScurvesDataArray = new double[nbPts, NbChannels + 1];
            for (int i = 0; i < nbPts; i++)
                for (int j = 0; j < NbChannels + 1; j++)
                    ScurvesDataArray[i, j] = double.NaN;

            CheckBox[] channelSelected = { checkBox_selChn0, checkBox_selChn1, checkBox_selChn2, checkBox_selChn3, checkBox_selChn4, checkBox_selChn5, checkBox_selChn6, checkBox_selChn7, checkBox_selChn8, checkBox_selChn9,
                checkBox_selChn10, checkBox_selChn11, checkBox_selChn12, checkBox_selChn13, checkBox_selChn14, checkBox_selChn15, checkBox_selChn16, checkBox_selChn17, checkBox_selChn18, checkBox_selChn19,
                checkBox_selChn20, checkBox_selChn21, checkBox_selChn22, checkBox_selChn23, checkBox_selChn24, checkBox_selChn25, checkBox_selChn26, checkBox_selChn27, checkBox_selChn28, checkBox_selChn29,
                checkBox_selChn30, checkBox_selChn31 };

            // parameters to pass to the ProgressChanged event of the background worker.
            double trigEff = 100;
            int DacCode = VthMin;
            List<Charting.Series> scurvesSeries = chart_Scurves.Series.ToList();
            Charting.Series currentSerie = null;

            object[] progressChangedParameters = new object[] { ScurvesDataArray };

            backgroundWorker_Scurves.ReportProgress((int)progress, progressChangedParameters);

            // Get the name and path of the output file
            string sFileName = textBox_sCurveSavePath.Text;
            if (!validPath(sFileName)) { MessageBox.Show("The save path does not exist."); return; }
            TextWriter tw = new StreamWriter(sFileName);

            int[] ScurveData = new int[NbChannels];
            int[] pulseData = new int[NbChannels];

            string strSC = getSC();
            string strTempSC;
            char[] chrTemp1;
            string strTemp2;

            if (checkBox_ScurvesTorQ.Checked)
            {
                chrTemp1 = strSC.Substring(0, 1117).ToCharArray();
                strTemp2 = strSC.Substring(1127, 17);
            }
            else
            {
                chrTemp1 = strSC.Substring(0, 1107).ToCharArray();
                strTemp2 = strSC.Substring(1117, 27);
            }

            if (checkBox_ScurvesChoice.Checked)
            {
                #region on signal
                int progressMax = (NbChannels + 1) * nbPts;

                for (int chn = 0; chn < NbChannels; chn++)
                {
                    if (!channelSelected[chn].Checked) continue;
                    
                    // mask all the channels
                    if (checkBox_useMaskScurves.Checked && !checkBox_ScurvesTorQ.Checked) for (int j = 0; j < NbChannels; j++) chrTemp1[j + 265] = '0';

                    if (checkBox_useMaskScurves.Checked && !checkBox_ScurvesTorQ.Checked)
                    {
                        // unmask the measured channel
                        chrTemp1[chn + 265] = '1';
                        // mask the previously measured channel
                        if (chn > 0) chrTemp1[chn + 264] = '0';
                        // Set the Ctest injection on the measured channel
                        if (checkBox_fshOnLg.Checked)
                        {
                            chrTemp1[chn * 15 + 632] = '1';
                            if (chn > 0) chrTemp1[(chn - 1) * 15 + 632] = '0';
                        }
                        else
                        {
                            chrTemp1[chn * 15 + 631] = '1';
                            if (chn > 0) chrTemp1[(chn - 1) * 15 + 631] = '0';
                        }
                    }

                    string strTemp1 = new string(chrTemp1);
                    for (int i = 0; i < nbPts; i++)
                    {
                        if (VthStep > 0) DacCode = VthMin + i * VthStep;
                        else DacCode = VthMax + i * VthStep;
                        ScurvesDataArray[i, 0] = DacCode;

                        // reconstruct the slow control with the unmasked measured channel
                        strTempSC = strTemp1 + IntToBin(DacCode, 10) + strTemp2;
                        sendSC(usbDevId, strTempSC);

                        // start data acquisition for channel chn
                        if (checkBox_ScurvesTorQ.Checked) Firmware.sendWord(31, IntToBin(chn, 8), usbDevId);
                        else Firmware.sendWord(31, IntToBin(34, 8), usbDevId);

                        pulseData[chn] = 0;
                        int tries = 0;
                        while (pulseData[chn] != 200 || chn == 0 && i == 0 && ScurveData[0] <= 50 && tries < 10) // redo the measurement until it's correct.
                        {
                            if (backgroundWorker_Scurves.CancellationPending)
                            {
                                e.Cancel = true;
                                tw.Close();
                                return;
                            }
                            else if (chn == 0 && i == 0)
                            {
                                sendSC(usbDevId, strTempSC);
                                if (checkBox_ScurvesTorQ.Checked) Firmware.sendWord(31, IntToBin(chn, 8), usbDevId);
                                else Firmware.sendWord(31, IntToBin(33, 8), usbDevId);
                                tries++;
                            }

                            // Reset and disable S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
                            // Stop reseting S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "10" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
                            // Enable S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "11" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
                            // wait enough time for the acquisition to perform
                            wait(sleepTicks);
                            // Disable S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "10" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);

                            // read data in the FPGA
                            byte[] tmpFIFO8 = Firmware.readWord(8, NbChannels, usbDevId);
                            byte[] tmpFIFO18 = Firmware.readWord(18, NbChannels, usbDevId);
                            byte[] tmpFIFO9 = Firmware.readWord(9, NbChannels, usbDevId);
                            byte[] tmpFIFO19 = Firmware.readWord(19, NbChannels, usbDevId);
                            // pulseData is the measured number of acquisition windows
                            pulseData[chn] = Convert.ToInt32(tmpFIFO18[chn]) * 256 + Convert.ToInt32(tmpFIFO8[chn]);
                            // ScurveData is the number of trigger measured at DAC code i
                            ScurveData[chn] = Convert.ToInt32(tmpFIFO19[chn]) * 256 + Convert.ToInt32(tmpFIFO9[chn]);
                        }

                        // Store trigger efficiency of current channel in ScurvesDataArray
                        trigEff = ScurveData[chn] * 100.0 / pulseData[chn];
                        ScurvesDataArray[i, chn + 1] = trigEff;

                        // Get the position of the 50 % trigger efficiency without fit. Precision depends on the step size.
                        if (trigEff < 50 && positionScurves[chn] == 0 && DacCode != VthMin) positionScurves[chn] = DacCode;

                        progress = ((chn + 1) * nbPts + i) * 100 / progressMax;
                        currentSerie = scurvesSeries[chn];
                        progressChangedParameters[0] = VthMin;
                        progressChangedParameters[1] = VthMax;
                        progressChangedParameters[2] = trigEff;
                        progressChangedParameters[3] = DacCode;
                        progressChangedParameters[4] = currentSerie;
                        backgroundWorker_Scurves.ReportProgress((int)progress, progressChangedParameters);
                    }
                }
                #endregion
            }
            else
            {
                #region on pedestal

                int progressMax = (nbPts + 1) * NbChannels;

                for (int i = 0; i < nbPts; i++)
                {
                    if (VthStep > 0) DacCode = VthMin + i * VthStep;
                    else DacCode = VthMax + i * VthStep;
                    ScurvesDataArray[i, 0] = DacCode;

                    // mask all the channels
                    if (checkBox_useMaskScurves.Checked && !checkBox_ScurvesTorQ.Checked) for (int j = 0; j < NbChannels; j++) chrTemp1[j + 265] = '0';

                    for (int chn = 0; chn < NbChannels; chn++)
                    {
                        if (!channelSelected[chn].Checked) continue;

                        if (checkBox_useMaskScurves.Checked && !checkBox_ScurvesTorQ.Checked)
                        {
                            // unmask the measured channel
                            chrTemp1[chn + 265] = '1';
                            // mask the previously measured channel
                            if (chn > 0) chrTemp1[chn + 264] = '0';
                        }

                        string strTemp1 = new string(chrTemp1);

                        // reconstruct the slow control with the unmasked measured channel
                        strTempSC = strTemp1 + IntToBin(DacCode, 10) + strTemp2;
                        sendSC(usbDevId, strTempSC);

                        // start data acquisition for channel chn
                        if (checkBox_ScurvesTorQ.Checked) Firmware.sendWord(31, IntToBin(chn, 8), usbDevId);
                        else Firmware.sendWord(31, IntToBin(34, 8), usbDevId);

                        pulseData[chn] = 0;
                        int tries = 0;
                        while (pulseData[chn] != 200 || chn == 0 && i == 0 && ScurveData[0] <= 50 && tries < 10) // redo the measurement until it's correct.
                        {
                            if (backgroundWorker_Scurves.CancellationPending)
                            {
                                e.Cancel = true;
                                tw.Close();
                                return;
                            }
                            else if (chn == 0 && i == 0)
                            {
                                sendSC(usbDevId, strTempSC);
                                if (checkBox_ScurvesTorQ.Checked) Firmware.sendWord(31, IntToBin(chn, 8), usbDevId);
                                else Firmware.sendWord(31, IntToBin(34, 8), usbDevId);
                                tries++;
                            }

                            // Reset and disable S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
                            // Stop reseting S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "10" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
                            // Enable S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "11" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
                            // wait enough time for the acquisition to perform
                            wait(sleepTicks);
                            // Disable S-curves
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "10" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);

                            // read data in the FPGA
                            byte[] tmpFIFO8 = Firmware.readWord(8, NbChannels, usbDevId);
                            byte[] tmpFIFO18 = Firmware.readWord(18, NbChannels, usbDevId);
                            byte[] tmpFIFO9 = Firmware.readWord(9, NbChannels, usbDevId);
                            byte[] tmpFIFO19 = Firmware.readWord(19, NbChannels, usbDevId);
                            // pulseData is the measured number of acquisition windows
                            pulseData[chn] = Convert.ToInt32(tmpFIFO18[chn]) * 256 + Convert.ToInt32(tmpFIFO8[chn]);
                            // ScurveData is the number of trigger measured at DAC code i
                            ScurveData[chn] = Convert.ToInt32(tmpFIFO19[chn]) * 256 + Convert.ToInt32(tmpFIFO9[chn]);
                        }

                        // Store trigger efficiency of current channel in ScurvesDataArray
                        trigEff = ScurveData[chn] * 100.0 / pulseData[chn];
                        ScurvesDataArray[i, chn + 1] = trigEff;
                    }
                    
                    progress = ((i + 1) * NbChannels) * 100 / progressMax;
                    progressChangedParameters[0] = ScurvesDataArray;
                    backgroundWorker_Scurves.ReportProgress((int)progress, progressChangedParameters);
                }
                #endregion
            }

            // Write the measurement results in the output file
            for (int i = 0; i < nbPts; i++)
            {
                string outdata = "";

                for (int j = 0; j < 1 + NbChannels; j++)
                    outdata += ScurvesDataArray[i, j] + " ";

                tw.WriteLine(outdata);
            }
            tw.Flush();
            tw.Close();
        }

        private void backgroundWorker_Scurves_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar_Scurves.Value = 0;
            button_Scurves.Text = "Start S-curves";
            button_autocalibration.Text = "Auto-calibration";

            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);
            Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);
            Firmware.sendWord(2, "000001" + ((checkBox_ADC1.Checked == true) ? "1" : "0") + ((checkBox_ADC2.Checked == true) ? "1" : "0"), usbDevId);
            Firmware.sendWord(3, ((checkBox_rstbPS.Checked == true) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked == true) ? "1" : "0") + ((checkBox_selHold.Checked == true) ? "1" : "0") + ((checkBox_selTrigToHold.Checked == true) ? "1" : "0") + ((checkBox_triggerTorQ.Checked == true) ? "1" : "0") + ((checkBox_pwrOn.Checked == true) ? "1" : "0"), usbDevId);
            Firmware.sendWord(5, "0" + ((checkBox_selPSGlobalTrigger.Checked == true) ? "1" : "0") + ((checkBox_selPSMode.Checked == true) ? "1" : "0") + "000" + ((checkBox_PSGlobalTrigger.Checked == true) ? "1" : "0") + ((checkBox_PSMode.Checked == true) ? "1" : "0"), usbDevId);
            
            // Send the slow control specified by the user to the ASIC (bits were forced during acquisition)
            sendSC(usbDevId, getSC());

            resetZoom(chart_Scurves);
        }

        private void backgroundWorker_Scurves_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] progressChangedParameters = e.UserState as object[];

            // Update progress bar
            progressBar_Scurves.Value = e.ProgressPercentage;
            
            chart_Scurves.Enabled = true;

            string seriesName = "";
            chart_Scurves.Series.Clear();
            for (int i = 0; i < NbChannels; i++)
            {
                seriesName = "Scurve ch" + i.ToString();
                chart_Scurves.Series.Add(seriesName);
                chart_Scurves.Series[seriesName].ChartType = Charting.SeriesChartType.Line;
                chart_Scurves.Series[seriesName].BorderWidth = 2;
            }

            // Get parameters passed as arguments
            double[,] ScurvesDataArray = (double[,])progressChangedParameters[0];

            List<Charting.Series> scurvesSeries = chart_Scurves.Series.ToList();
            
            // Plot S-curves data
            for (int pts = 0; pts < ScurvesDataArray.GetLength(0); pts++)
            {
                for (int chn = 0; chn < NbChannels; chn++)
                {
                    double DacCode = ScurvesDataArray[pts, 0];
                    double trigEff = ScurvesDataArray[pts, 1 + chn];
                    if (trigEff == double.NaN) continue;
                    scurvesSeries[chn].Points.AddXY(DacCode, trigEff);
                    scurvesSeries[chn].Points.Last().ToolTip = scurvesSeries[chn].Name.ToString() + "\n" + "DAC code: " + DacCode.ToString() + "\n" + "trig. eff.: " + trigEff.ToString();
                }
            }
            
            resetZoom(chart_Scurves);

            chart_Scurves.Enabled = false;
        }

        private void textBox_minCodeScurves_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox_minCodeScurves.Text) > Convert.ToInt32(textBox_maxCodeScurves.Text))
                    textBox_minCodeScurves.Text = textBox_maxCodeScurves.Text;
            }
            catch { textBox_minCodeScurves.Text = "0"; return; }
        }

        private void textBox_maxCodeScurves_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox_maxCodeScurves.Text) < Convert.ToInt32(textBox_minCodeScurves.Text))
                    textBox_maxCodeScurves.Text = textBox_minCodeScurves.Text;
                if (Convert.ToInt32(textBox_maxCodeScurves.Text) > 1023)
                    textBox_maxCodeScurves.Text = "1023";
            }
            catch { textBox_maxCodeScurves.Text = "1023"; return; }
        }

        #region channel selection
        private void checkBox_ScurvesTorQ_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                ((CheckBox)sender).Text = "Time S-curves";
                checkBox_useMaskScurves.Enabled = false;
            }
            else
            {
                ((CheckBox)sender).Text = "Charge S-curves";
                checkBox_useMaskScurves.Enabled = true;
            }
        }

        private void checkBox_ScurvesChoice_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                ((CheckBox)sender).Text = "On signal";
                button_autocalibration.Enabled = false;
            }
            else
            {
                ((CheckBox)sender).Text = "On pedestal";
                button_autocalibration.Enabled = true;
            }
        }

        private void button_channelSelectionScurves_Click(object sender, EventArgs e)
        {
            if (groupBox_channelSelectionScurves.Visible)
            {
                for (int i = 0; i < 10; i++)
                {
                    Point locPB = groupBox_channelSelectionScurves.Location;
                    groupBox_channelSelectionScurves.Location = new Point(locPB.X, locPB.Y - (int)(44 * scale));
                    wait(20000);
                    this.Update();
                }
                groupBox_channelSelectionScurves.Visible = false;
                button_ScurvesCheckAll.Visible = false;
                button_ScurvesUncheckAll.Visible = false;
                button_channelSelectionScurves.Text = "v   Channel selection   v";
            }
            else
            {
                groupBox_channelSelectionScurves.Visible = true;
                button_ScurvesCheckAll.Visible = true;
                button_ScurvesCheckAll.BringToFront();
                button_ScurvesUncheckAll.Visible = true;
                button_ScurvesUncheckAll.BringToFront();
                button_channelSelectionScurves.BringToFront();
                for (int i = 0; i < 10; i++)
                {
                    Point locPB = groupBox_channelSelectionScurves.Location;
                    groupBox_channelSelectionScurves.Location = new Point(locPB.X, locPB.Y + (int)(44 * scale));
                    wait(20000);
                    this.Update();
                }
                button_channelSelectionScurves.Text = "^   Channel selection   ^";
            }
        }
        private void button_ScurvesCheckAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox cb in groupBox_channelSelectionScurves.Controls)
                cb.Checked = true;
        }
        private void button_ScurvesUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox cb in groupBox_channelSelectionScurves.Controls)
                cb.Checked = false;
        }
        #endregion
    }
}
