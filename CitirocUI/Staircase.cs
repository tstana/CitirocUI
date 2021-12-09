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
        private void button_startStaircase_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker_staircase.IsBusy)
                {
                    backgroundWorker_staircase.CancelAsync();
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

                int VthMin = Convert.ToInt32(textBox_minStaircase.Text);
                int VthMax = Convert.ToInt32(textBox_maxStaircase.Text);
                int VthStep = Convert.ToInt32(textBox_stepStaircase.Text);

                string seriesName = "";

                chart_staircase.Series.Clear();
                chart_staircase.Palette = Charting.ChartColorPalette.SeaGreen;
                for (int i = 0; i < NbChannels; i++)
                {
                    seriesName = "Staircase ch" + i.ToString();
                    chart_staircase.Series.Add(seriesName);
                    chart_staircase.Series[seriesName].ChartType = Charting.SeriesChartType.Line;
                    chart_staircase.Series[seriesName].BorderWidth = 2;
                }
                chart_staircase.ChartAreas[0].AxisX.Minimum = VthMin;
                chart_staircase.ChartAreas[0].AxisX.Maximum = VthMax;

                string saveFileName = textBox_staircaseSavePath.Text;
                if (!validPath(saveFileName)) { MessageBox.Show("The save path does not exist."); return; }

                button_startStaircase.Text = "Stop";
                int timeWindow = 1000;
                Int32.TryParse(textBox_staircaseTimeWindow.Text, out timeWindow);

                // parameters to pass to the DoWork event of the background worker.
                object[] parameters = new object[] { VthMin, VthMax, VthStep, saveFileName, timeWindow };
                // runs the event on new background worker thread.
                backgroundWorker_staircase.RunWorkerAsync(parameters);
            }
            catch
            {
                MessageBox.Show("An error occured. Please verify the input parameters are correctly specified.");
                return;
            }
        }

        private void backgroundWorker_staircase_DoWork(object sender, DoWorkEventArgs e)
        {
            // Same algorithm than for the S-curves but reads the subadd 70, 71, 72, 73 
            // instead of 8, 18, 9, 19 to have the total number of triggers.
            object[] parameters = e.Argument as object[];
            int VthMin = (int)parameters[0];
            int VthMax = (int)parameters[1];
            int VthStep = (int)parameters[2];
            string saveFileName = (string)parameters[3];
            int timeWindow = (int)parameters[4];
            int nbPts = (VthMax - VthMin) / VthStep + 1;

            double progress = 1;
            int progressMax = (nbPts + 1) * NbChannels;
            double[,] staircaseDataArray = new double[nbPts, NbChannels + 1];

            CheckBox[] channelSelected = { checkBox_selStaircaseCh0, checkBox_selStaircaseCh1, checkBox_selStaircaseCh2, checkBox_selStaircaseCh3, checkBox_selStaircaseCh4, checkBox_selStaircaseCh5, checkBox_selStaircaseCh6, checkBox_selStaircaseCh7, checkBox_selStaircaseCh8, checkBox_selStaircaseCh9,
                checkBox_selStaircaseCh10, checkBox_selStaircaseCh11, checkBox_selStaircaseCh12, checkBox_selStaircaseCh13, checkBox_selStaircaseCh14, checkBox_selStaircaseCh15, checkBox_selStaircaseCh16, checkBox_selStaircaseCh17, checkBox_selStaircaseCh18, checkBox_selStaircaseCh19,
                checkBox_selStaircaseCh20, checkBox_selStaircaseCh21, checkBox_selStaircaseCh22, checkBox_selStaircaseCh23, checkBox_selStaircaseCh24, checkBox_selStaircaseCh25, checkBox_selStaircaseCh26, checkBox_selStaircaseCh27, checkBox_selStaircaseCh28, checkBox_selStaircaseCh29,
                checkBox_selStaircaseCh30, checkBox_selStaircaseCh31 };
            // parameters to pass to the ProgressChanged event of the background worker.
            uint nbTrig = 0;
            int DacCode = VthMin;
            List<Charting.Series> staircaseSeries = chart_staircase.Series.ToList();
            Charting.Series currentSerie = null;

            object[] progressChangedParameters = new object[] { VthMin, VthMax, 0.0, DacCode, currentSerie };

            backgroundWorker_staircase.ReportProgress((int)progress, progressChangedParameters);

            TextWriter tw = new StreamWriter(saveFileName);

            string strSC = getSC();
            string strTempSC;
            char[] chrTemp1;
            string strTemp2;

            if (checkBox_staircaseTorQ.Checked)
            {
                chrTemp1 = strSC.Substring(0, 1117).ToCharArray();
                strTemp2 = strSC.Substring(1127, 17);
            }
            else
            {
                chrTemp1 = strSC.Substring(0, 1107).ToCharArray();
                strTemp2 = strSC.Substring(1117, 27);
            }

            for (int i = 0; i < nbPts; i++)
            {
                DacCode = VthMin + i * VthStep;
                staircaseDataArray[i, 0] = DacCode;

                // Mask all the channels
                if (checkBox_useMaskScurves.Checked && !checkBox_staircaseTorQ.Checked) for (int j = 0; j < NbChannels; j++) chrTemp1[j + 265] = '0';

                for (int chn = 0; chn < NbChannels; chn++)
                {
                    if (backgroundWorker_staircase.CancellationPending)
                    {
                        e.Cancel = true;
                        tw.Close();
                        return;
                    }

                    if (!channelSelected[chn].Checked) continue;

                    if (checkBox_useMaskStaircase.Checked)
                    {
                        // unmask the measured channel
                        chrTemp1[chn + 265] = '1';
                        // mask the previously measured channel
                        if (chn > 0) chrTemp1[chn + 264] = '0';
                    }

                    // reconstruct the slow control with the unmasked measured channel
                    string strTemp1 = new string(chrTemp1);
                    strTempSC = strTemp1 + IntToBin(DacCode, 10) + strTemp2;
                    sendSC(usbDevId, strTempSC, ProtoCubesSerial.Command.SendCitirocConf);

                    // start data acquisition for channel chn
                    if (checkBox_staircaseTorQ.Checked) Firmware.sendWord(31, IntToBin(chn, 8), usbDevId);
                    else Firmware.sendWord(31, IntToBin(33, 8), usbDevId);
                    // Reset and disable pedestal
                    Firmware.sendWord(6, "00000000", usbDevId);
                    // Stop reseting pedestal
                    Firmware.sendWord(6, "01000000", usbDevId);
                    // Enable pedestal
                    Firmware.sendWord(6, "01100000", usbDevId);
                    // wait enough time for the acquisition to perform
                    Thread.Sleep(timeWindow);
                    // Disable pedestal
                    Firmware.sendWord(6, "01000000", usbDevId);

                    // read the results in the FPGA
                    string tmpFIFO70 = Firmware.readWord(70, usbDevId);
                    string tmpFIFO71 = Firmware.readWord(71, usbDevId);
                    string tmpFIFO72 = Firmware.readWord(72, usbDevId);
                    string tmpFIFO73 = Firmware.readWord(73, usbDevId);

                    nbTrig = Convert.ToUInt32(tmpFIFO73, 2) * 16777216 + Convert.ToUInt32(tmpFIFO72, 2) * 65536 + Convert.ToUInt32(tmpFIFO71, 2) * 256 + Convert.ToUInt32(tmpFIFO70, 2);

                    staircaseDataArray[i, chn + 1] = nbTrig;

                    progress = (((i + 1) * NbChannels + chn) * 100) / progressMax;
                    currentSerie = staircaseSeries[chn];
                    progressChangedParameters[0] = VthMin;
                    progressChangedParameters[1] = VthMax;
                    progressChangedParameters[2] = Convert.ToDouble(nbTrig) / (Convert.ToDouble(timeWindow) / 1000);
                    progressChangedParameters[3] = DacCode;
                    progressChangedParameters[4] = currentSerie;
                    backgroundWorker_staircase.ReportProgress((int)progress, progressChangedParameters);
                }
            }

            // write the measurement results in the output file
            for (int i = 0; i < nbPts; i++)
            {
                string outdata = "";

                for (int j = 0; j < 1 + NbChannels; j++)
                    outdata += staircaseDataArray[i, j] + " ";

                tw.WriteLine(outdata);
            }
            tw.Flush();
            tw.Close();
            
        }

        private void backgroundWorker_staircase_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar_staircase.Value = 0;
            Firmware.sendWord(6, "00000000", usbDevId);
            // send the slow control specified by the user to the ASIC (bits were forced during acquisition)
            sendSC(usbDevId, getSC(), ProtoCubesSerial.Command.SendCitirocConf);
            button_startStaircase.Text = "Start";
        }

        private void backgroundWorker_staircase_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] progressChangedParameters = e.UserState as object[];

            progressBar_staircase.Value = e.ProgressPercentage;

            int VthMin = (int)progressChangedParameters[0];
            int VthMax = (int)progressChangedParameters[1];
            double trigFreq = (double)progressChangedParameters[2];
            int DacCode = (int)progressChangedParameters[3];
            Charting.Series staircaseSerie = (Charting.Series)progressChangedParameters[4];
            
            if (staircaseSerie != null && trigFreq > 0)
            {
                staircaseSerie.Points.AddXY(DacCode, trigFreq);
                staircaseSerie.Points.Last().ToolTip = staircaseSerie.Name.ToString() + "\n" + "DAC code: " + DacCode.ToString() + "\n" + "Trigger freq: " + trigFreq.ToString();
            }
        }
        
        private void button_staircaseSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog DataSDialog = new SaveFileDialog();
            DataSDialog.Title = "Specify Output file";
            DataSDialog.Filter = "All Files(*.*)|*.*";
            DataSDialog.RestoreDirectory = true;
            if (DataSDialog.ShowDialog() == DialogResult.OK) textBox_staircaseSavePath.Text = DataSDialog.FileName;
        }

        private void checkBox_staircaseTorQ_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                ((CheckBox)sender).Text = "Time Staircase";
                checkBox_useMaskStaircase.Enabled = false;
            }
            else
            {
                ((CheckBox)sender).Text = "Charge Staircase";
                checkBox_useMaskStaircase.Enabled = true;
            }
        }

        private void button_channelSelectionStaircase_Click(object sender, EventArgs e)
        {
            if (groupBox_channelSelectionStaircase.Visible)
            {
                for (int i = 0; i < 10; i++)
                {
                    Point locPB = groupBox_channelSelectionStaircase.Location;
                    groupBox_channelSelectionStaircase.Location = new Point(locPB.X, locPB.Y - (int)(44 * scale));
                    wait(20000);
                    this.Update();
                }
                groupBox_channelSelectionStaircase.Visible = false;
                button_staircaseCheckAll.Visible = false;
                button_staircaseUncheckAll.Visible = false;
                button_channelSelectionStaircase.Text = "v   Channel selection   v";
            }
            else
            {
                groupBox_channelSelectionStaircase.Visible = true;
                button_staircaseCheckAll.Visible = true;
                button_staircaseCheckAll.BringToFront();
                button_staircaseUncheckAll.Visible = true;
                button_staircaseUncheckAll.BringToFront();
                button_channelSelectionStaircase.BringToFront();
                for (int i = 0; i < 10; i++)
                {
                    Point locPB = groupBox_channelSelectionStaircase.Location;
                    groupBox_channelSelectionStaircase.Location = new Point(locPB.X, locPB.Y + (int)(44 * scale));
                    wait(20000);
                    this.Update();
                }
                button_channelSelectionStaircase.Text = "^   Channel selection   ^";
            }
        }

        private void button_staircaseCheckAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox cb in groupBox_channelSelectionStaircase.Controls)
                cb.Checked = true;
        }

        private void button_staircaseUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox cb in groupBox_channelSelectionStaircase.Controls)
                cb.Checked = false;
        }

        private void checkBox_staircaseLogScale_CheckedChanged(object sender, EventArgs e)
        {
            if (((checkBox)sender).Checked) chart_staircase.ChartAreas[0].AxisY.IsLogarithmic = true;
            else chart_staircase.ChartAreas[0].AxisY.IsLogarithmic = false;
            chart_staircase.Invalidate();
        }
    }
}
