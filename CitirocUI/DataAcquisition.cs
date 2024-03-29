﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace CitirocUI
{
    public partial class Citiroc : Form
    {
        string DataLoadFile;
        int loadLargeData;
        int[,] PerChannelChargeHG = new int[NbChannels + 1, 4096];
        int[,] PerChannelChargeLG = new int[NbChannels + 1, 4096];
        int[] Hit = new int[NbChannels + 1];
        UInt32[] HitCK = new UInt32[NbChannels + 1];
        UInt32[] cubesTelemetryArray = new UInt32[16];
        UInt16 daqTimeTotal;
        UInt16 daqTimeActual;
        int nbAcq = 100;
        bool timeAcquisitionMode = true;
        byte arduinoStatus;

        #region Start Acquisition Button
        private void button_startAcquisition_Click(object sender, EventArgs e)
        {
            /* Quit early if not connected to any board */
            if (connectStatus == -1)
            {
                button_startAcquisition.BackColor = Color.IndianRed;
                button_startAcquisition.ForeColor = Color.White;
                tmrButtonColor.Enabled = true;
                MessageBox.Show("Please connect to a board using the " +
                    "\"Connect\" tab",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            /* Color button green otherwise */
            button_startAcquisition.BackColor = WeerocGreen;
            button_startAcquisition.ForeColor = Color.White;
            tmrButtonColor.Enabled = true;

            /* Cancel DAQ if any is running */
            if (backgroundWorker_dataAcquisition.IsBusy) {
                backgroundWorker_dataAcquisition.CancelAsync();
                return;
            }

            /*
             * Check for valid data file path early
             *
             * Use Swedish date and time style, as it is closer to ISO-8601.
             * This style allows for file names that appear chronologically when
             * displayed in a file explorer window, or using 'ls'.
             */
            string date = DateTime.Now.ToString(new System.Globalization.CultureInfo("se-SE"));
            date = date.Replace(' ', '_');
            date = date.Replace(':', '-');
            date = date.Replace('/', '-');
            DataLoadFile = textBox_dataSavePath.Text + "dataCITI_" + date + ".dat";
            try
            {
                if (!validPath(DataLoadFile))
                {
                    throw new ArgumentException();
                }
            }
            catch
            {
                MessageBox.Show("Cannot start data acquisition!\n\n" +
                    "The selected save path is invalid:\n\n" +
                    DataLoadFile,
                    "Error");
                return;
            }

            /* Data file OK, now send bytes on comm. channel to start DAQ */

            // Start by getting the DAQ time
            string[] splitAcqTime = textBox_acquisitionTime.Text.Split(':');

            /* Now actually send the bytes... */

            // USB (Weeroc board)
            if (comboBox_SelectConnection.SelectedIndex == 0)
            {
                Array.Clear(PerChannelChargeHG, 0, PerChannelChargeHG.Length);
                Array.Clear(PerChannelChargeLG, 0, PerChannelChargeLG.Length);
                Array.Clear(Hit, 0, Hit.Length);

                chart_perChannelChargeHG.Series.Clear();
                chart_perChannelChargeLG.Series.Clear();
                chart_perChannelChargeHG.Series.Add("Charge");
                chart_perChannelChargeLG.Series.Add("Charge");

                nbAcq = Convert.ToInt32(textBox_numData.Text);

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

                label_help.Text = "Acquired data will be saved to " + DataLoadFile;
            }

            // serial -- Proto-CUBES board
            else if (comboBox_SelectConnection.SelectedIndex == 1)
            {
                byte individAcqTime = Convert.ToByte(textBox_numData.Text);

                // Proto-CUBES only supports timed acquisition mode:
                if (switchBox_acquisitionMode.Checked == false)
                {
                    MessageBox.Show("Proto-CUBES only (currently) supports time acquisition mode. Please change to this mode via the switch-box.");
                    return;
                }

                // Make sure serial port exists and is open:
                if ((protoCubes == null) || (protoCubes.OpenPort() == false))
                {
                    MessageBox.Show("Please configure and open the serial port connection via the \"Connect\" tab.");
                    return;
                }

                // Just in case we're setting an acquisition time not supported by Proto-CUBES:
                AdjustAcquisitionTime();

                /// Start by synchronizing UTC time between the CitirocUI
                /// and Proto-CUBES, in case a long time has passed since
                /// the last SEND_TIME command was sent to Proto-CUBES.
                /// Also wait some time to ensure all UART commands in this
                /// run are properly received and interpreted. (Without
                /// the delay, it was observed that the DAQ_START cmd.
                /// was not received by the Arduino.)
                SendTimeToProtoCUBES();
                Thread.Sleep(50);

                // Prep and send a SEND_DAQ_CONF command
                byte[] daqConf = new byte[7];

                daqConf[0] = Convert.ToByte(individAcqTime);

                for (int i = 0; i < 6; ++i)
                {
                    daqConf[i+1] = Convert.ToByte(binCfgArray[i]);
                }

                Array.Copy(numBinsArray, protoCubes.NumBins, 6);

                try
                {
                    protoCubes.SendCommand(ProtoCubesSerial.Command.SendDAQConf, daqConf);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send DAQ duration " +
                        "to Proto-CUBES!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + "Error message:"
                        + Environment.NewLine
                        + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                Thread.Sleep(50);

                // Finally, send a DAQ_START command:
                try
                {
                    protoCubes.SendCommand(ProtoCubesSerial.Command.DAQStart, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send DAQ start command " +
                        "to Proto-CUBES!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + "Error message:"
                        + Environment.NewLine
                        + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // No connection
            else {
                MessageBox.Show("No connection mode selected. Please select one via the \"Connect\" tab.");
                return;
            }

            /* Finally, start the DAQ on the UI end... */
            backgroundWorker_dataAcquisition.RunWorkerAsync();

            /* ... and update UI elements to indicate this */
            button_startAcquisition.Text = "Stop Acquisition";

            button_SelectNumBinsCubes.Enabled = false;

            tabControl_dataAcquisition.Enabled = false;
            progressBar_acquisition.Visible = true;

            label_elapsedTimeAcquisition.Enabled = true;
            label_acqTime.Enabled = true;
        }
        #endregion

        #region Background Worker
        private void backgroundWorker_dataAcquisition_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] splitAcqTime = textBox_acquisitionTime.Text.Split(':');
            int acqTimeMillisec = (3600 * Convert.ToInt32(splitAcqTime[0]) +
                                     60 * Convert.ToInt32(splitAcqTime[1]) +
                                          Convert.ToInt32(splitAcqTime[2])) * 1000;

            if (selectedConnectionMode == 0) // USB
            {

                int FIFOAcqLength = 100; // The FIFO in the FPGA can store up to 100 acquisitions per cycle

                TextWriter tw = new StreamWriter(DataLoadFile);

                // write file header
                string header = "";
                for (int i = 0; i < NbChannels; i++) header += "hit" + i.ToString() + " ChargeLG" + i.ToString() + " " + " ChargeHG" + i.ToString() + " ";
                header += "temp";
                tw.WriteLine(header);

                var swDaqRun = Stopwatch.StartNew(); // Start the stopwatch to measure acquisition time
                long actualAcqTime = 0;

                int daqIdleCount = 0;

                int nbCycles = nbAcq / FIFOAcqLength; // Calculate the number of acquisition cycles to do to have "nbAcq" acquisitions.
                if (nbAcq % FIFOAcqLength != 0 || nbCycles == 0) nbCycles++; // add one cycle for the remainder or if nbCycle == 0 (ie nbAcq < 100)

                // If the acquisition is done on a certain amount of time, the loop will be infinite and nbCycles will be 1
                if (timeAcquisitionMode) nbCycles = 1;

                for (int cycle = 0; cycle < nbCycles; cycle++)
                {
                    // Calculate the number of acquisition to do in this cycle, if the acquisition is done on a certain amount of time it is always 100.
                    int nbAcqInCycle = 0;
                    if (nbAcq >= FIFOAcqLength || timeAcquisitionMode) nbAcqInCycle = FIFOAcqLength;
                    else if (nbAcq < FIFOAcqLength) nbAcqInCycle = nbAcq;

                    // SubAdd 45 store the number of acquisitions to save in FIFO before reading it.
                    string strNbAcq = IntToBin(nbAcqInCycle, 8);
                    Firmware.sendWord(45, strNbAcq, usbDevId);

                    Firmware.sendWord(43, "10000001", usbDevId); // Start ADC acquisition cycles (bit 0 signals DAQ is running)

                    string rd4 = "00000000";

                    var swAcqTime = Stopwatch.StartNew(); // Start the stopwatch to measure "real" acquisition time
                                                          // bit 7 of subAdd 4 is 1 when the acquisitions are done
                    while (rd4.Substring(7, 1) == "0" && !backgroundWorker_dataAcquisition.CancellationPending)
                    {
                        if (timeAcquisitionMode && swDaqRun.ElapsedMilliseconds > acqTimeMillisec) // Stop the acquisition when time-out
                        {
                            swAcqTime.Stop(); // Stop the stopwatch to measure "real" acquisition time
                            actualAcqTime += swAcqTime.ElapsedMilliseconds;

                            tw.Flush();
                            tw.Close();

                            swDaqRun.Stop();
                            UpdateDaqTimeLabels(swDaqRun.ElapsedMilliseconds, actualAcqTime);

                            return;
                        }
                        rd4 = Firmware.readWord(4, usbDevId);
                        /* If no events are present in time mode DAQ, update every ~500 ms */
                        ++daqIdleCount;
                        if ((timeAcquisitionMode) && (daqIdleCount == 100))
                        {
                            daqIdleCount = 0;
                            backgroundWorker_dataAcquisition.ReportProgress((int)(swDaqRun.ElapsedMilliseconds * 100 / acqTimeMillisec));
                        }
                        Thread.Sleep(5);
                    }
                    daqIdleCount = 0;
                    swAcqTime.Stop(); // Stop the stopwatch to measure "real" acquisition time
                    actualAcqTime += swAcqTime.ElapsedMilliseconds;
                    if (backgroundWorker_dataAcquisition.CancellationPending)
                    {
                        e.Cancel = true;

                        tw.Flush();
                        tw.Close();

                        swDaqRun.Stop();
                        UpdateDaqTimeLabels(swDaqRun.ElapsedMilliseconds, actualAcqTime);

                        return;
                    }

                    /* Report DAQ progress */
                    if (timeAcquisitionMode)
                    {
                        backgroundWorker_dataAcquisition.ReportProgress((int)(swDaqRun.ElapsedMilliseconds * 100 / acqTimeMillisec));
                    }
                    else
                    {
                        backgroundWorker_dataAcquisition.ReportProgress(cycle * 100 / nbCycles);
                    }

                    /* Skip reading FIFOs if they are empty and go on to next loop iteration (skip remaining part of this loop) */
                    string subAdd22 = Firmware.readWord(22, usbDevId);
                    if (subAdd22 != "00000000") { cycle -= 1; continue; }

                    /* Read FIFOs and update data otherwise */
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
                    int[] dataHG = new int[nbData];
                    int[] dataLG = new int[nbData];
                    int[] hit = new int[nbData];

                    for (int i = 0; i < nbAcqInCycle; i++)
                    {
                        for (int chn = 0; chn < NbChannels + 1; chn++)
                        {
                            bool[] boolArrayDataHG = { bitArrayHG[i * 528 + chn * 16 + 0], bitArrayHG[i * 528 + chn * 16 + 1], bitArrayHG[i * 528 + chn * 16 + 2], bitArrayHG[i * 528 + chn * 16 + 3], bitArrayHG[i * 528 + chn * 16 + 4], bitArrayHG[i * 528 + chn * 16 + 5],
                            bitArrayHG[i * 528 + chn * 16 + 6], bitArrayHG[i * 528 + chn * 16 + 7], bitArrayHG[i * 528 + chn * 16 + 8], bitArrayHG[i * 528 + chn * 16 + 9], bitArrayHG[i * 528 + chn * 16 + 10], bitArrayHG[i * 528 + chn * 16 + 11] };
                            BitArray bitArrayDataHG = new BitArray(boolArrayDataHG);
                            int[] array = new int[1];
                            bitArrayDataHG.CopyTo(array, 0);
                            dataHG[chn] = array[0];

                            PerChannelChargeHG[chn, dataHG[chn]]++;

                            bool[] boolArrayDataLG = { bitArrayLG[i * 528 + chn * 16 + 0], bitArrayLG[i * 528 + chn * 16 + 1], bitArrayLG[i * 528 + chn * 16 + 2], bitArrayLG[i * 528 + chn * 16 + 3], bitArrayLG[i * 528 + chn * 16 + 4], bitArrayLG[i * 528 + chn * 16 + 5],
                            bitArrayLG[i * 528 + chn * 16 + 6], bitArrayLG[i * 528 + chn * 16 + 7], bitArrayLG[i * 528 + chn * 16 + 8], bitArrayLG[i * 528 + chn * 16 + 9], bitArrayLG[i * 528 + chn * 16 + 10], bitArrayLG[i * 528 + chn * 16 + 11] };
                            BitArray bitArrayDataLG = new BitArray(boolArrayDataLG);
                            bitArrayDataLG.CopyTo(array, 0);
                            dataLG[chn] = array[0];

                            PerChannelChargeLG[chn, dataLG[chn]]++;

                            hit[chn] = Convert.ToInt32(bitArrayHG[i * 528 + chn * 16 + 13]);
                            Hit[chn] += hit[chn];
                        }
                        string outdata = "";

                        for (int chn = 0; chn < NbChannels; chn++)
                            outdata += hit[chn] + " " + dataLG[chn] + " " + dataHG[chn] + " ";
                        outdata += (dataHG[NbChannels]).ToString();

                        tw.WriteLine(outdata);
                    }

                    Firmware.sendWord(43, "00000001", usbDevId); // Set word 43 bit 7 to '0' to inform firmware a new ADC cycle can commence

                    if (timeAcquisitionMode) cycle -= 1;
                }

                swDaqRun.Stop();

                /* Close the file */
                tw.Flush();
                tw.Close();

                /* Update elapsed & live time label */
                UpdateDaqTimeLabels(swDaqRun.ElapsedMilliseconds, actualAcqTime);
            }

            else if (selectedConnectionMode == 1)  // Serial
            {
                /// Start two stopwatches to measure acquisition times, one for
                /// total DAQ time and the other for individual DAQs
                /// (Proto-CUBES runs multiple DAQs and sends the file
                /// corresponding to a single DAQ after the run is done).
                var stopwatchTotalDaqRun = Stopwatch.StartNew();
                var stopwatchIndividualDaqRun = Stopwatch.StartNew();
                var individualDaqTimeMillisec = 1000 +
                    (Convert.ToInt32(textBox_numData.Text) * 1000);
                bool timeSent = false;

                // CUBES always works in timed acquisition mode
                timeAcquisitionMode = true;

                while (!backgroundWorker_dataAcquisition.CancellationPending)
                {
                    // Stop the acquisition when on DAQ run "time-out"
                    if (stopwatchTotalDaqRun.ElapsedMilliseconds >= acqTimeMillisec)
                    {
                        stopwatchTotalDaqRun.Stop();
                        stopwatchIndividualDaqRun.Stop();
                        break;
                    }
                    /// Alternatively, if the individual DAQ run time is smaller
                    /// than the total DAQ run time, we need to send multiple
                    /// REQ_PAYLOADs to Proto-CUBES to obtain data.
                    /// 
                    /// On every "DAQ_DUR + 1", stop the stopwatch and send the
                    /// obtain the data when available (using a REQ_STATUS-REQ_PAYLOAD
                    /// command combination). The individual DAQ stopwatch is restarted
                    /// and DAQ restarts while the REQ_PAYLOAD data is still being
                    /// downloaded. This restarting of the individual DAQ stopwatch
                    /// should ensure the CitirocUI and the Arduino DAQ_DUR timers
                    /// stay in sync, on average.
                    else if ((stopwatchIndividualDaqRun.ElapsedMilliseconds) >
                            individualDaqTimeMillisec)
                    {
                        stopwatchIndividualDaqRun.Stop();
                        ReqPayloadProcedure();
                        stopwatchIndividualDaqRun.Restart();
                    }
                    Thread.Sleep(5);

                    /* DAQ progess */
                    backgroundWorker_dataAcquisition.ReportProgress(
                        (int)((stopwatchTotalDaqRun.ElapsedMilliseconds * 100) /
                                acqTimeMillisec));
                }

                return;
            }
        }

        private void backgroundWorker_dataAcquisition_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar_acquisition.Value = e.ProgressPercentage;

            if (selectedConnectionMode == 0)
            {
                if (tabControl_dataAcquisition.SelectedIndex == 0)
                {
                    int chNum = (int)(numericUpDown_loadCh.Value);

                    chart_perChannelChargeHG.Series.Clear();
                    chart_perChannelChargeHG.Series.Add("Charge");
                    chart_perChannelChargeHG.Series[0].Color = WeerocPaleBlue;
                    chart_perChannelChargeHG.Series[0]["PointWidth"] = "1";
                    chart_perChannelChargeHG.ResetAutoValues();

                    chart_perChannelChargeLG.Series.Clear();
                    chart_perChannelChargeLG.Series.Add("Charge");
                    chart_perChannelChargeLG.Series[0].Color = WeerocPaleBlue;
                    chart_perChannelChargeLG.Series[0]["PointWidth"] = "1";
                    chart_perChannelChargeLG.ResetAutoValues();

                    for (int i = 0; i < 4096; i++) if (PerChannelChargeHG[chNum, i] != 0) chart_perChannelChargeHG.Series[0].Points.AddXY(i, PerChannelChargeHG[chNum, i]);
                    for (int i = 0; i < 4096; i++) if (PerChannelChargeLG[chNum, i] != 0) chart_perChannelChargeLG.Series[0].Points.AddXY(i, PerChannelChargeLG[chNum, i]);
                    label_nbHit.Text = "Number of registered hit in channel " + chNum + " = " + Hit[chNum];

                    resetZoom(chart_perChannelChargeHG);
                    resetZoom(chart_perChannelChargeLG);
                }
            }
        }

        private void backgroundWorker_dataAcquisition_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button_startAcquisition.Text = "Start Acquisition";
            tabControl_dataAcquisition.Enabled = true;
            progressBar_acquisition.Value = 0;
            progressBar_acquisition.Visible = false;
            button_SelectNumBinsCubes.Enabled = true;

            if (selectedConnectionMode == 0)
            {
                Firmware.sendWord(43, "00000000", usbDevId);
                refreshDataChart();
            }
            else if (selectedConnectionMode == 1)
            {
                // Send DAQ_STOP command
                try
                {
                    /// Finally, send the DAQ_STOP.
                    protoCubes.SendCommand(ProtoCubesSerial.Command.DAQStop, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send DAQ Stop command " +
                        "to Proto-CUBES!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                // Wait for one second and send REQ_PAYLOAD command
                Stopwatch s = Stopwatch.StartNew();
                while (s.ElapsedMilliseconds < 1000)
                    Thread.Sleep(5);
                label_help.Text = "";
                ReqPayloadProcedure();
            }
        }
        #endregion

        #region Helper Functions
        private void UpdateDaqTimeLabels(long daqRunTime, long actualAcqTime)
        {
            /* Get actual acquistion time -- counted using a 10 MHz clock -- from firmware registers */
            ulong firmwareActualAcqTime = Convert.ToUInt64(Firmware.readWord(114, usbDevId), 2) |
                Convert.ToUInt64(Firmware.readWord(115, usbDevId), 2) << 8 |
                Convert.ToUInt64(Firmware.readWord(116, usbDevId), 2) << 16 |
                Convert.ToUInt64(Firmware.readWord(117, usbDevId), 2) << 24 |
                Convert.ToUInt64(Firmware.readWord(118, usbDevId), 2) << 32 |
                Convert.ToUInt64(Firmware.readWord(119, usbDevId), 2) << 40;

            if (daqRunTime > 1000)
            {
                UpdatingLabel("Elapsed time: " +
                    Math.Round((double)daqRunTime / 1000, 2) + " s",
                    label_elapsedTimeAcquisition);
                UpdatingLabel("Actual acq. time: " + Math.Round((double)actualAcqTime / 1000, 2) + " s" +
                    " (firmware: " + Math.Round((double)firmwareActualAcqTime / 10000000, 2) + " s)",
                    label_acqTime);
            }
            else
            {
                UpdatingLabel("Elapsed time: " +
                    daqRunTime + " ms",
                    label_elapsedTimeAcquisition);
                UpdatingLabel("Actual acq. time: " + actualAcqTime + " ms" +
                    " (firmware: " + Math.Round((double)firmwareActualAcqTime / 10000, 2) + " ms)",
                    label_acqTime);
            }
        }

        private void SendReqStatus()
        {
            UpdatingLabel("Sending REQ_STATUS to Proto-CUBES...", label_help);

            try
            {
                protoCubes.SendCommand(ProtoCubesSerial.Command.ReqStatus, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send request status command " +
                    "to Proto-CUBES!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SendReqPayload()
        {
            UpdatingLabel("Sending REQ_PAYLOAD to Proto-CUBES...", label_help);

            /*
             * Prep the ProtoCubesSerial instance for DAQ data reception
             * and GO!
             */
            try
            {
                protoCubes.SendCommand(ProtoCubesSerial.Command.ReqPayload, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send request payload command " +
                    "to Proto-CUBES!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ReqPayloadProcedure()
        {
            int i = 0;
            do
            {
                /// Try to REQ_STATUS every 500 ms with an 8-try "timeout".
                /// REQ_PAYLOAD and break on new file available (bit 1 in
                /// status = '1').
                SendReqStatus();
                Thread.Sleep(500);
                if ((arduinoStatus & 0x02) != 0) {
                    SendReqPayload();
                    break;
                }
            } while (i++ < 8);
            arduinoStatus = 0;
        }

        private void loadData()
        {
            if (DataLoadFile == null) return;
            FileInfo info = new FileInfo(DataLoadFile);
            if (info.Length > 10e9 || !info.Exists) return;
            else if (info.Length > 100e6 && loadLargeData == 0)
            {
                DialogResult dialogResult = MessageBox.Show("Looks like the data file is pretty large. Displaying these measurements may slow down the UI. Are you sure you want to load this file ?", "Heavy file", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No) { loadLargeData = -1; return; }
                else loadLargeData = 1;
            }
            else if (loadLargeData == -1) return;

            Label dataLoading = new Label();
            ProgressBar progress = new ProgressBar();
            if (info.Length > 10e6)
            {
                tabPage_dataAcquisition.Controls.Add(dataLoading);
                tabPage_dataAcquisition.Controls.Add(progress);
                dataLoading.Text = "Please wait while data are being loaded.";
                dataLoading.TextAlign = ContentAlignment.MiddleCenter;
                dataLoading.Size = new Size((int)(500 * scale), (int)(100 * scale));
                progress.Size = new Size((int)(400 * scale), (int)(20 * scale));
                dataLoading.BorderStyle = BorderStyle.FixedSingle;
                dataLoading.Location = new Point(tabPage_dataAcquisition.Width / 2 - dataLoading.Width / 2, tabPage_dataAcquisition.Height / 2);
                progress.Location = new Point(tabPage_dataAcquisition.Width / 2 - progress.Width / 2, (int)(tabPage_dataAcquisition.Height / 2 * 1.28));
                dataLoading.BringToFront();
                progress.BringToFront();
                Update();
            }

            ArrayList DataArrayList;

            if (DataLoadFile == null) return;
            else DataArrayList = ReadFileLine(DataLoadFile);

            int DataArrayListCount = DataArrayList.Count;

            if (DataArrayListCount <= 1) return;

            string[] DataArray = new string[DataArrayListCount];

            Array.Clear(PerChannelChargeHG, 0, PerChannelChargeHG.Length);
            Array.Clear(PerChannelChargeLG, 0, PerChannelChargeLG.Length);
            Array.Clear(Hit, 0, Hit.Length);

            for (int i = 0; i < DataArrayListCount - 1; i++)
            {
                string[] DataSplit = DataArray[i + 1].Split(' ');
                for (int chn = 0; chn < NbChannels; chn++)
                {
                    PerChannelChargeHG[chn, Convert.ToInt32(DataSplit[chn * 3 + 2])]++;
                    PerChannelChargeLG[chn, Convert.ToInt32(DataSplit[chn * 3 + 1])]++;
                    Hit[chn] += Convert.ToInt32(DataSplit[chn * 3]);
                }
                progress.Value = i * 100 / DataArrayListCount;
            }

            refreshDataChart();

            progress.Dispose();
            dataLoading.Dispose();
        }

        private void refreshDataChart()
        {
            chart_perChannelChargeHG.Legends.Clear();
            chart_perChannelChargeLG.Legends.Clear();

            ArrayList DataArrayList;

            bool tryParse;

            int chNum = 0;
            try { chNum = (int)numericUpDown_loadCh.Value; }
            catch { chNum = 0; }
            if (chNum > 31) { chNum = 31; numericUpDown_loadCh.Text = Convert.ToString(chNum); }
            else if (chNum <= 0) { chNum = 0; numericUpDown_loadCh.Text = Convert.ToString(chNum); }

            int LgCutLow = 0;
            int LgCutHigh = 4095;
            int HgCutLow = 0;
            int HgCutHigh = 4095;

            tryParse = int.TryParse(textBox_LgCutLow.Text, out LgCutLow);
            if (!tryParse) LgCutLow = 0;
            tryParse = int.TryParse(textBox_LgCutHigh.Text, out LgCutHigh);
            if (!tryParse || LgCutHigh > 4095) LgCutHigh = 4095;
            if (LgCutHigh < LgCutLow) LgCutLow = LgCutHigh - 1;
            textBox_LgCutHigh.Text = LgCutHigh.ToString();
            textBox_LgCutLow.Text = LgCutLow.ToString();
            tryParse = int.TryParse(textBox_HgCutLow.Text, out HgCutLow);
            if (!tryParse) HgCutLow = 0;
            tryParse = int.TryParse(textBox_HgCutHigh.Text, out HgCutHigh);
            if (!tryParse || HgCutHigh > 4095) HgCutHigh = 4095;
            if (HgCutHigh < HgCutLow) HgCutLow = HgCutHigh - 1;
            textBox_HgCutHigh.Text = HgCutHigh.ToString();
            textBox_HgCutLow.Text = HgCutLow.ToString();

            #region per channel

            chart_perChannelChargeHG.Series.Clear();
            chart_perChannelChargeHG.ResetAutoValues();
            chart_perChannelChargeHG.ChartAreas[0].AxisX.IsStartedFromZero = false;
            chart_perChannelChargeHG.Series.Add("Charge");
            chart_perChannelChargeHG.ChartAreas[0].AxisX.Title = "High gain charge (ADCu)";
            chart_perChannelChargeHG.ChartAreas[0].AxisY.Title = "Data count";
            chart_perChannelChargeHG.Series[0].Color = WeerocPaleBlue;
            chart_perChannelChargeHG.Series[0].MarkerColor = Color.FromArgb(20, 141, 164);
            chart_perChannelChargeHG.Series[0].BorderColor = WeerocPaleBlue;
            chart_perChannelChargeHG.Series[0].BorderWidth = 0;
            chart_perChannelChargeHG.Series[0]["PointWidth"] = "1";

            for (int i = HgCutLow; i < HgCutHigh + 1; i++)
                if (PerChannelChargeHG[chNum, i] != 0)
                    chart_perChannelChargeHG.Series[0].Points.AddXY(i, PerChannelChargeHG[chNum, i]);

            ulong numTimeTrigs = 0;

            if (selectedConnectionMode == 0)
            {
                numTimeTrigs = (Convert.ToUInt64(Firmware.readWord(120, usbDevId), 2)) |
                                 (Convert.ToUInt64(Firmware.readWord(121, usbDevId), 2) << 8) |
                                 (Convert.ToUInt64(Firmware.readWord(122, usbDevId), 2) << 16) |
                                 (Convert.ToUInt64(Firmware.readWord(123, usbDevId), 2) << 24) |
                                 (Convert.ToUInt64(Firmware.readWord(124, usbDevId), 2) << 32) |
                                 (Convert.ToUInt64(Firmware.readWord(125, usbDevId), 2) << 40) |
                                 (Convert.ToUInt64(Firmware.readWord(126, usbDevId), 2) << 48) |
                                 (Convert.ToUInt64(Firmware.readWord(127, usbDevId), 2) << 56);

                label_nbHit.Text = "Number of registered hit in channel " + chNum + " = " + Hit[chNum] +
                                   " / Total time hits on all channels during actual acq. time = " + numTimeTrigs;
                if (numTimeTrigs == 0)
                    label_nbHit.Text += " (TimeTrig masked?)";
            }
            else
            {
                label_nbHit.Text = "Number of registered hit in channel " + chNum + " = " + HitCK[chNum];
                label_elapsedTimeAcquisition.Text = "Elapsed time: " + ((double)(daqTimeTotal / 256)).ToString("N3") + " s";
                label_acqTime.Text = "Actual acq. time: " + ((double)(daqTimeActual / 256)).ToString("N3") + " s";
            }

            resetZoom(chart_perChannelChargeHG);

            chart_perChannelChargeLG.Series.Clear();
            chart_perChannelChargeLG.ResetAutoValues();
            chart_perChannelChargeLG.ChartAreas[0].AxisX.IsStartedFromZero = false;
            chart_perChannelChargeLG.Series.Add("Charge");
            chart_perChannelChargeLG.ChartAreas[0].AxisX.Title = "Low gain charge (ADCu)";
            chart_perChannelChargeLG.ChartAreas[0].AxisY.Title = "Data count";
            chart_perChannelChargeLG.Series[0].Color = WeerocPaleBlue;
            chart_perChannelChargeLG.Series[0].MarkerColor = Color.FromArgb(20, 141, 164);
            chart_perChannelChargeLG.Series[0].BorderColor = WeerocPaleBlue;
            chart_perChannelChargeLG.Series[0].BorderWidth = 0;
            chart_perChannelChargeLG.Series[0]["PointWidth"] = "1";

            for (int i = LgCutLow; i < LgCutHigh + 1; i++)
                if (PerChannelChargeLG[chNum, i] != 0)
                    chart_perChannelChargeLG.Series[0].Points.AddXY(i, PerChannelChargeLG[chNum, i]);

            resetZoom(chart_perChannelChargeLG);
            #endregion

            #region per acquisition

            if (selectedConnectionMode == 1)
            {
                return;
            }

            if (DataLoadFile == null) return;

            else DataArrayList = ReadFileLine(DataLoadFile);

            int DataArrayListCount = DataArrayList.Count;

            if (DataArrayListCount <= 1) return;

            numericUpDown_acquisitionNumber.Maximum = DataArrayListCount - 1;

            int acqNumber = 1;
            try { acqNumber = (int)numericUpDown_acquisitionNumber.Value; }
            catch { acqNumber = 1; }
            if (acqNumber > DataArrayListCount - 1) { acqNumber = DataArrayListCount - 1; numericUpDown_acquisitionNumber.Text = Convert.ToString(acqNumber); }
            else if (acqNumber < 1) { acqNumber = 1; numericUpDown_acquisitionNumber.Text = Convert.ToString(acqNumber); }

            string[] DataArray = new string[DataArrayListCount];
            DataArrayList.CopyTo(DataArray);

            int[] chargeLG = new int[NbChannels];
            int[] chargeHG = new int[NbChannels];
            int[] hit = new int[NbChannels];

            for (int chn = 0; chn < NbChannels; chn++)
            {
                string[] DataSplit = DataArray[acqNumber].Split(' ');
                chargeLG[chn] = Convert.ToInt32(DataSplit[chn * 3 + 1]);
                chargeHG[chn] = Convert.ToInt32(DataSplit[chn * 3 + 2]);
                hit[chn] = Convert.ToInt32(DataSplit[chn * 3]);
            }

            chart_perAcqChargeHG.Series.Clear();
            chart_perAcqChargeHG.ResetAutoValues();
            chart_perAcqChargeHG.Series.Add("Charge");
            chart_perAcqChargeHG.ChartAreas[0].AxisX.Title = "Channel";
            chart_perAcqChargeHG.ChartAreas[0].AxisY.Title = "High gain charge (ADCu)";
            chart_perAcqChargeHG.Series[0].Color = WeerocPaleBlue;
            chart_perAcqChargeHG.Series[0].MarkerColor = Color.FromArgb(20, 141, 164);
            chart_perAcqChargeHG.Series[0].BorderColor = WeerocPaleBlue;
            chart_perAcqChargeHG.Series[0].BorderWidth = 0;
            chart_perAcqChargeHG.Series[0]["PointWidth"] = "1";

            chart_perAcqChargeLG.Series.Clear();
            chart_perAcqChargeLG.ResetAutoValues();
            chart_perAcqChargeLG.Series.Add("Charge");
            chart_perAcqChargeLG.ChartAreas[0].AxisX.Title = "Channel";
            chart_perAcqChargeLG.ChartAreas[0].AxisY.Title = "Low gain charge (ADCu)";
            chart_perAcqChargeLG.Series[0].Color = WeerocPaleBlue;
            chart_perAcqChargeLG.Series[0].MarkerColor = Color.FromArgb(20, 141, 164);
            chart_perAcqChargeLG.Series[0].BorderColor = WeerocPaleBlue;
            chart_perAcqChargeLG.Series[0].BorderWidth = 0;
            chart_perAcqChargeLG.Series[0]["PointWidth"] = "1";

            chart_perAcqChargeLG.ChartAreas[0].AxisX.Interval = 5;
            chart_perAcqChargeLG.ChartAreas[0].AxisX.IntervalOffset = 1;
            chart_perAcqChargeLG.ChartAreas[0].AxisX.Maximum = 32;
            chart_perAcqChargeLG.ChartAreas[0].AxisX.Minimum = -1;
            chart_perAcqChargeLG.ChartAreas[0].AxisY.LineColor = Color.White;
            chart_perAcqChargeLG.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.White;

            chart_perAcqChargeHG.ChartAreas[0].AxisX.Interval = 5;
            chart_perAcqChargeHG.ChartAreas[0].AxisX.IntervalOffset = 1;
            chart_perAcqChargeHG.ChartAreas[0].AxisX.Maximum = 32;
            chart_perAcqChargeHG.ChartAreas[0].AxisX.Minimum = -1;
            chart_perAcqChargeHG.ChartAreas[0].AxisY.LineColor = Color.White;
            chart_perAcqChargeHG.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.White;

            for (int chn = 0; chn < NbChannels; chn++)
            {
                chart_perAcqChargeHG.Series[0].Points.AddXY(chn, chargeHG[chn]);
                chart_perAcqChargeLG.Series[0].Points.AddXY(chn, chargeLG[chn]);
                if (hit[chn] == 1)
                {
                    chart_perAcqChargeHG.Series[0].Points[chn].Color = Color.IndianRed;
                    chart_perAcqChargeLG.Series[0].Points[chn].Color = Color.IndianRed;
                    chart_perAcqChargeHG.Series[0].Points[chn].MarkerColor = Color.LightCoral;
                    chart_perAcqChargeLG.Series[0].Points[chn].MarkerColor = Color.LightCoral;
                    chart_perAcqChargeHG.Series[0].Points[chn].BorderColor = Color.IndianRed;
                    chart_perAcqChargeLG.Series[0].Points[chn].BorderColor = Color.IndianRed;
                    chart_perAcqChargeHG.Series[0].Points[chn].BorderWidth = 0;
                    chart_perAcqChargeLG.Series[0].Points[chn].BorderWidth = 0;
                }
            }

            #endregion
        }

        private int fitGaussianFunction(double[] p, double[] dy, IList<double>[] dvec, object vars)
        {
            double[] x, y;

            CustomUserVariable v = (CustomUserVariable)vars;

            x = v.X;
            y = v.Y;

            for (int i = 0; i < dy.Length; i++)
            {
                dy[i] = y[i] - (p[2] / (p[0] * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x[i] - p[1]) * (x[i] - p[1]) / (2 * p[0] * p[0])));
            }

            return 0;
        }

        private string UpdateDataArrays(byte[] adata)
        {
            try
            {
                string u_time = System.Text.Encoding.UTF8.GetString(adata, 0, 21);

                if ((adata[21] != 0x0d) || (adata[22] != 0x0a)
                    || (u_time.IndexOf("Unix time:") != 0))
                {
                    return ("Invalid data file format!");
                }

                Array.Clear(PerChannelChargeHG, 0, PerChannelChargeHG.Length);
                Array.Clear(PerChannelChargeLG, 0, PerChannelChargeLG.Length);
                Array.Clear(Hit, 0, Hit.Length);
                Array.Clear(HitCK, 0, HitCK.Length);
                Array.Clear(cubesTelemetryArray, 0, cubesTelemetryArray.Length);

                UInt32 unixTimeArduino = Convert.ToUInt32(u_time.Substring(11));
                cubesTelemetryArray[0] = unixTimeArduino;

                // Header data offset is after "Unix time: <10 chars>"
                int offset = 23;

                // Start by getting the number of bins from the file
                int[] binCfg = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    binCfg[i] = adata[offset + 256 - 6 + i];
                }

                int[] numBins = new int[6];

                NumBinsFromBinCfg(numBins, binCfg);

                // Reverse individual fields to avoid BitConverter endianness issues...
                if (BitConverter.IsLittleEndian)
                {
                    // Start of DAQ HK data
                    Array.Reverse(adata, offset + 2, 4);
                    Array.Reverse(adata, offset + 6, 2);
                    Array.Reverse(adata, offset + 8, 2);
                    Array.Reverse(adata, offset + 10, 2);
                    Array.Reverse(adata, offset + 12, 2);

                    // Hit data         
                    Array.Reverse(adata, offset + 128, 2);
                    Array.Reverse(adata, offset + 130, 2);
                    Array.Reverse(adata, offset + 132, 4);
                    Array.Reverse(adata, offset + 136, 4);
                    Array.Reverse(adata, offset + 140, 4);
                    Array.Reverse(adata, offset + 144, 4);

                    // End of DAQ HK data
                    Array.Reverse(adata, offset + 148, 2);
                    Array.Reverse(adata, offset + 150, 2);
                    Array.Reverse(adata, offset + 152, 2);
                    Array.Reverse(adata, offset + 154, 2);

                    /// Reverse histogram values for all six histo's, starting
                    /// from HISTO_HDR
                    offset += 256;
                    for (int i = 0; i < 6; i++)
                    {
                        for (int j = 0; j < numBins[i]; j++)
                        {
                            Array.Reverse(adata, offset, 2);
                            offset += 2;
                        }
                    }
                }

                // Back to start for data display...
                offset = 23;

                string boardId = System.Text.Encoding.UTF8.GetString(adata, offset, 2);
                UInt32 time_reg = BitConverter.ToUInt32(adata, offset + 2);

                UInt16 temp_citiS = BitConverter.ToUInt16(adata, offset + 6);
                UInt16 temp_hvpsS = BitConverter.ToUInt16(adata, offset + 8);
                UInt16 hvps_voltS = BitConverter.ToUInt16(adata, offset + 10);
                UInt16 hvps_currS = BitConverter.ToUInt16(adata, offset + 12);
                cubesTelemetryArray[1] = time_reg;
                cubesTelemetryArray[2] = temp_citiS;
                cubesTelemetryArray[3] = temp_hvpsS;
                cubesTelemetryArray[4] = hvps_voltS;
                cubesTelemetryArray[5] = hvps_currS;

                daqTimeTotal = BitConverter.ToUInt16(adata, offset + 128);
                daqTimeActual = BitConverter.ToUInt16(adata, offset + 130);
                HitCK[0] = BitConverter.ToUInt32(adata, offset + 132);
                HitCK[16] = BitConverter.ToUInt32(adata, offset + 136);
                HitCK[31] = BitConverter.ToUInt32(adata, offset + 140);
                HitCK[32] = BitConverter.ToUInt32(adata, offset + 144);

                UInt16 temp_citiE = BitConverter.ToUInt16(adata, offset + 148);
                UInt16 temp_hvpsE = BitConverter.ToUInt16(adata, offset + 150);
                UInt16 hvps_voltE = BitConverter.ToUInt16(adata, offset + 152);
                UInt16 hvps_currE = BitConverter.ToUInt16(adata, offset + 154);
                cubesTelemetryArray[6] = temp_citiE;
                cubesTelemetryArray[7] = temp_hvpsE;
                cubesTelemetryArray[8] = hvps_voltE;
                cubesTelemetryArray[9] = hvps_currE;

                // Display histogram data
                offset += 256;

                for (int i = 0; i < 6; i++)
                {
                    if (binCfg[i] < 7)
                    {
                        for (int j = 0; j < numBins[i]; j++)
                        {
                            /// Each bin contains 2 ADC values; binCfg[i] for
                            /// fixed-width binning adds more ADC values within
                            /// each bin.
                            int k = (j * 2) << binCfg[i];

                            // Copy data into proper histogram array
                            FillHistogram(i, k, adata, offset);
                            offset += 2; // two bytes per bin
                        }
                    }
                    else if (binCfg[i] == 11)
                    { //table 1
                        int k = 0;
                        for (int j = 0; j < 1024; j++) {
                            if (j < 513) k += 2; //First boundary 0-512
                            else if (j <= 769) k += 4; // second 513-768
                            else k += 8; // 769 - 1024

                            FillHistogram(i, k, adata, offset);
                            offset += 2; // two bytes per bin
                        }
                    }
                    else if (binCfg[i] == 12)
                    { //table 2
                        int k = 0;
                        for (int j = 0; j < 128; j++)
                        {
                            if (j < 33) k += 2;
                            else if (j < 49) k += 4;
                            else if (j < 65) k += 8;
                            else if (j < 81) k += 16;
                            else if (j < 97) k += 32;
                            else if (j < 113) k += 64;
                            else k += 128;
                            FillHistogram(i, k, adata, offset);
                            offset += 2;
                        }
                    }
                }

                // Reverse fields again for proper writing to file...
                offset = 23;

                if (BitConverter.IsLittleEndian)
                {
                    // start of daq hk data
                    Array.Reverse(adata, offset + 2, 4);
                    Array.Reverse(adata, offset + 6, 2);
                    Array.Reverse(adata, offset + 8, 2);
                    Array.Reverse(adata, offset + 10, 2);
                    Array.Reverse(adata, offset + 12, 2);

                    // hit data         
                    Array.Reverse(adata, offset + 128, 2);
                    Array.Reverse(adata, offset + 130, 2);
                    Array.Reverse(adata, offset + 132, 4);
                    Array.Reverse(adata, offset + 136, 4);
                    Array.Reverse(adata, offset + 140, 4);
                    Array.Reverse(adata, offset + 144, 4);

                    // end of daq hk data
                    Array.Reverse(adata, offset + 148, 2);
                    Array.Reverse(adata, offset + 150, 2);
                    Array.Reverse(adata, offset + 152, 2);
                    Array.Reverse(adata, offset + 154, 2);

                    /// reverse histogram values for all six histo's, starting
                    /// from histo_hdr
                    offset += 256;
                    for (int i = 0; i < 6; i++)
                    {
                        for (int j = 0; j < numBins[i]; j++)
                        {
                            Array.Reverse(adata, offset, 2);
                            offset += 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ("Invalid .dat file format :" + ex.Message);
            }

            return ("");
        }

        private void FillHistogram(int i, int adcValue, byte[] adata, int offset){
            switch (i)
            {
                case 0:
                    PerChannelChargeHG[0, adcValue] = BitConverter.ToUInt16(adata, offset);
                    break;
                case 1:
                    PerChannelChargeLG[0, adcValue] = BitConverter.ToUInt16(adata, offset);
                    break;
                case 2:
                    PerChannelChargeHG[16, adcValue] = BitConverter.ToUInt16(adata, offset);
                    break;
                case 3:
                    PerChannelChargeLG[16, adcValue] = BitConverter.ToUInt16(adata, offset);
                    break;
                case 4:
                    PerChannelChargeHG[31, adcValue] = BitConverter.ToUInt16(adata, offset);
                    break;
                case 5:
                    PerChannelChargeLG[31, adcValue] = BitConverter.ToUInt16(adata, offset);
                    break;
            }
        }

        private void loadCubesData()
        {
            if (DataLoadFile == null) return;

            byte[] data = File.ReadAllBytes(DataLoadFile);

            label_DataFile.Text = "file:" + Path.GetFileName(DataLoadFile);
            label_DataFile.Visible = true;

            string upString = UpdateDataArrays(data);

            refreshDataChart();
        }
        #endregion

        #region Proto-CUBES Data Ready Event Handlers

        private void ReqStatus_DataReady(object sender, DataReadyEventArgs e)
        {
            /* Quit early if not the right command... */
            if (e.Command != ProtoCubesSerial.Command.ReqStatus)
                return;

            /* Set Arduino status variable */
            arduinoStatus = e.DataBytes[23];
        }

        private void ReqPayload_DataReady(object sender, DataReadyEventArgs e)
        {
            if ((e.Command == ProtoCubesSerial.Command.ReqPayload) &&
                (selectedConnectionMode == 1))
            {
                string date = DateTime.UtcNow.ToString(new System.Globalization.CultureInfo("se-SE"));
                date = date.Replace(' ', '_');
                date = date.Replace(':', '-');
                date = date.Replace('/', '-');
                string fileName = textBox_dataSavePath.Text + "dataCITI_" + date + ".dat";
                string hkFileName = textBox_dataSavePath.Text + "_HK.log";

                string update = UpdateDataArrays(e.DataBytes);

                if (update == "")
                {
                    UpdatingLabel("Writing DAQ data to " + fileName, label_help);
                    // display data if tab is active
                    // displayDataFunction(fileName, e.DataBytes);

                    /* Write data file */
                    using (BinaryWriter dataFile = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    {
                        dataFile.Write(e.DataBytes);
                    }

                    /* 
                     * Write header data to HK file if no such file exists
                     * (and will thus be created)
                     */
                    if (!File.Exists(hkFileName))
                    {
                        using (FileStream hkFile = File.Open(hkFileName, FileMode.Append))
                        {
                            string s = "Unix Time (Arduino, DAQ End);" +
                                "Unix Time (Gateware, DAQ Start);" +
                                "Citiroc Temperature (DAQ Start);" +
                                "HVPS Temperature (DAQ Start);" +
                                "HVPS Voltage (DAQ Start);" +
                                "HVPS Current (DAQ Start);" +
                                "Total DAQ Time (1/256 seconds);" +
                                "Actual DAQ Time (1/256 seconds);" +
                                "Trig. Count (Ch. 0);" +
                                "Trig. Count (Ch. 16);" +
                                "Trig. Count (Ch. 31);" +
                                "Trig. Count (OR32);" +
                                "Citiroc Temperature (DAQ End);" +
                                "HVPS Temperature (DAQ End);" +
                                "HVPS Voltage (DAQ End);" +
                                "HVPS Current (DAQ End)" +
                                Environment.NewLine;
                            byte[] hdr = System.Text.Encoding.ASCII.GetBytes(s);
                            hkFile.Write(hdr, 0, hdr.Length);
                        }
                    }

                    using (FileStream hkFile = File.Open(hkFileName, FileMode.Append))
                    {
                        /// TODO: 2.7 V value below may have an offset from
                        ///       ASIC to ASIC... Change to member variable
                        ///       or setting read from CUBES?

                        double citi_temp_f = (double)(cubesTelemetryArray[2] * 2.0 / 1000);
                        double tempCitiS = ((2.7 - (double)citi_temp_f) / 8e-3);
                        citi_temp_f = (double)(cubesTelemetryArray[6] * 2.0 / 1000);
                        double tempCitiE = ((2.7 - (double)citi_temp_f) / 8e-3);
                        double tempHS = ((double)cubesTelemetryArray[3] * 1.907e-5 - 1.035) / (-5.5e-3);
                        double tempHE = ((double)cubesTelemetryArray[7] * 1.907e-5 - 1.035) / (-5.5e-3);
                        double voltHS = (double)cubesTelemetryArray[4] * 1.812e-3;
                        double voltHE = (double)cubesTelemetryArray[8] * 1.812e-3;
                        double currHS = (double)cubesTelemetryArray[5] * 5.194e-3;
                        double currHE = (double)cubesTelemetryArray[9] * 5.194e-3;

                        string tmpStr = cubesTelemetryArray[0].ToString() + ";" +
                            cubesTelemetryArray[1].ToString() + ";" +
                            tempCitiS.ToString("N3") + ";" +
                            tempHS.ToString("N3") + ";" +
                            voltHS.ToString("N3") + ";" +
                            currHS.ToString("N3") + ";" +
                            daqTimeTotal.ToString() + ";" +
                            daqTimeActual.ToString() + ";" +
                            HitCK[0].ToString() + ";" +
                            HitCK[16].ToString() + ";" +
                            HitCK[31].ToString() + ";" +
                            HitCK[32].ToString() + ";" +
                            tempCitiE.ToString("N3") + ";" +
                            tempHE.ToString("N3") + ";" +
                            voltHE.ToString("N3") + ";" +
                            currHE.ToString("N3") +
                            Environment.NewLine;

                        byte[] toSend = System.Text.Encoding.ASCII.GetBytes(tmpStr);

                        hkFile.Write(toSend, 0, toSend.Length);
                    }

                    UpdatingLabel("file: " + Path.GetFileName(fileName), label_DataFile);
                }
                else
                {
                    UpdatingLabel(update, label_help);
                }
            }
        }
        #endregion

        #region UI Event Handlers
        private void label_DataFile_TextChanged(object sender, EventArgs e)
        {
            refreshDataChart();
        }

        private void button_dataSavePath_Click(object sender, EventArgs e)
        {
            String path = textBox_dataSavePath.Text;
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.Description = "Select folder to save to...";
            folderDlg.SelectedPath = path;
            if (folderDlg.ShowDialog() == DialogResult.OK)
            {
                textBox_dataSavePath.Text = folderDlg.SelectedPath + "\\";
            }
        }

        private void numericUpDown_loadData_ValueChanged(object sender, EventArgs e)
        {
            refreshDataChart();
        }

        private void loadData_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13) refreshDataChart();
        }

        private void loadData_Leave(object sender, EventArgs e)
        {
            refreshDataChart();
        }

        private void button_loadData_Click(object sender, EventArgs e)
        {
            loadLargeData = 0;

            // Open dialog box
            OpenFileDialog DataLoadDialog = new OpenFileDialog();
            DataLoadDialog.Title = "Specify data file";

            if (selectedConnectionMode == 1)     // Serial
                DataLoadDialog.Filter = "CUBES files|*.dat";

            DataLoadDialog.RestoreDirectory = true;

            if (DataLoadDialog.ShowDialog() == DialogResult.OK && DataLoadDialog.FileName != null)
            {
                DataLoadFile = DataLoadDialog.FileName;
                FileInfo info = new FileInfo(DataLoadFile);
                if (!info.Exists) { MessageBox.Show("The file does not exist."); return; }
                else if (info.Length > 10e9)
                {
                    MessageBox.Show("The file is too heavy for the soft to handle. Use an external software to process those data.");
                    chart_perChannelChargeHG.Series.Clear();
                    chart_perChannelChargeLG.Series.Clear();
                    return;
                }
                else
                {
                    if (selectedConnectionMode == 1)
                        loadCubesData();
                    else
                        loadData();
                }

            }
            else return;
        }

        private void switchBox_acquisitionMode_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                label_numData.Enabled = false;
                textBox_numData.Enabled = false;
                label_acquisitionTime.Enabled = true;
                textBox_acquisitionTime.Enabled = true;
                timeAcquisitionMode = true;
            }
            else
            {
                label_numData.Enabled = true;
                textBox_numData.Enabled = true;
                label_acquisitionTime.Enabled = false;
                textBox_acquisitionTime.Enabled = false;
                timeAcquisitionMode = false;
            }
        }

        private void textBox_acquisitionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                AdjustAcquisitionTime();
            }
        }

        private void textBox_acquisitionTime_Leave(object sender, EventArgs e)
        {
            AdjustAcquisitionTime();
        }

        private void textBox_numData_Leave(object sender, EventArgs e)
        {
            NumDataCheck();
        }

        private void textBox_numData_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                NumDataCheck();
            }
        }

        int[] binCfgArray = { 0, 0, 0, 0, 0, 0 };
        private void button_SelectNumBinsCubes_Click(object sender, EventArgs e)
        {
            ProtoCubesNumBinsForm frm = new ProtoCubesNumBinsForm();

            // send actual indexes
            Array.Copy(binCfgArray,0,frm.IndexArray,0,6);
            DialogResult result = frm.ShowDialog();

            if (result == DialogResult.OK)
            {
                // retrieve new data
                Array.Copy(frm.IndexArray, binCfgArray, 6);
                NumBinsFromBinCfg(numBinsArray, binCfgArray);
            }
        }

        int[] numBinsArray = { 2048, 2048, 2048, 2048, 2048, 2048 };
        private void NumBinsFromBinCfg(int[] numBins, int[] binCfg)
        {
            for (int i = 0; i < 6; i++)
            {
                /// TODO: Use properties inside ProtoCubesNumBinsForm
                ///       to set the bin_cfg ranges...
                if (binCfg[i] < 7)
                {
                    numBins[i] = 2048 >> binCfg[i];
                }
                else if (binCfg[i] == 11)
                {
                    numBins[i] = 1024;
                }
                else if (binCfg[i] == 12)
                {
                    numBins[i] = 128;
                }
            }
        }
        #endregion

        #region Adjust Acquisition Time
        private void AdjustAcquisitionTime()
        {
            // Split the string in hh:mm:ss and apply two-digit formatting
            string[] splitAcqTime = textBox_acquisitionTime.Text.Split(':');

            for (int i = 0; i < splitAcqTime.Length; ++i)
            { 
                try
                {
                    splitAcqTime[i] = String.Format("{0,2:00}", Convert.ToInt32(splitAcqTime[i]));
                }
                catch
                {
                    splitAcqTime[i] = "00";
                }
            }

            // Make sure the user doesn't set a value higher than 59 in mins or seconds fields
            if (Convert.ToInt32(splitAcqTime[1]) > 59)
                splitAcqTime[1] = "59";

            if (Convert.ToInt32(splitAcqTime[2]) > 59)
                    splitAcqTime[2] = "59";
            
            // Adjust minimum acquisition time according to individual DAQ
            // time selection
            if (selectedConnectionMode == 1)
            {
                int acqTimeSeconds = 3600 * Convert.ToInt32(splitAcqTime[0]) +
                                       60 * Convert.ToInt32(splitAcqTime[1]) +
                                            Convert.ToInt32(splitAcqTime[2]);
                int individualAcqTime = Convert.ToInt32(textBox_numData.Text);
                if (acqTimeSeconds < individualAcqTime)
                {
                    splitAcqTime[2] = String.Format("{0,2:00}", (individualAcqTime % 60));
                    splitAcqTime[1] = String.Format("{0,2:00}", (individualAcqTime / 60));
                    splitAcqTime[0] = "00";
                    label_help.Text = "NOTE: Setting acquisition time to " +
                        "individual DAQ time...";
                }
            }

            // Finally, set the total DAQ time
            textBox_acquisitionTime.Text = splitAcqTime[0] + ":" + splitAcqTime[1] + ":" + splitAcqTime[2];
        }

        private void NumDataCheck()
        {
            if (selectedConnectionMode != 1)
                return;

            // Adjust acquisition times for Proto-CUBES
            int individualAcqTime = Convert.ToInt32(textBox_numData.Text);
            if (individualAcqTime > 255)
            {
                individualAcqTime = 255;

                MessageBox.Show("Setting individual DAQ time to " +
                    individualAcqTime.ToString() + " s (the maximum " +
                    "currently accepted by Proto-CUBES).\n", "Info");
                textBox_numData.Text = individualAcqTime.ToString();
            }
            else if (individualAcqTime < 5)
            {
                individualAcqTime = 5;

                MessageBox.Show("Setting individual DAQ time to " +
                    individualAcqTime.ToString() + " s (the minimum " +
                    "currently accepted by Proto-CUBES).\n", "Info");
                textBox_numData.Text = individualAcqTime.ToString();
            }

            AdjustAcquisitionTime();
        }
        #endregion

        #region Chart Image
        private void DrawChartImage(int[] binCfg)
        {
            Chart tmpChart = new Chart();
            tmpChart.Size = new Size(1800, 1200);

            // save to a bitmap
            Bitmap bmp = new Bitmap(3800, 3800);
            string[] chan_name = { "CH0 High ","CH0 Low ", "CH16 High ", "CH16 Low ", "CH31 High ", "CH31 Low " };
            Point[] P = new Point[]
            {
                new Point { X = 0, Y = 0 },
                new Point { X = 1900, Y = 0 },
                new Point { X = 0, Y = 1300 },
                new Point { X = 1900, Y = 1300 },
                new Point { X = 0, Y = 2600 },
                new Point { X = 1900, Y = 2600 },
            };

            Font chtXFont = new Font("Arial", 42);
            Font chtYFont = new Font("Arial", 32);
            var chartArea = new ChartArea();
            chartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            chartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chartArea.AxisX.LabelStyle.Font = new Font("Consolas", 32);
            chartArea.AxisY.LabelStyle.Font = new Font("Consolas", 32);
            chartArea.AxisX.IsStartedFromZero = false;
            chartArea.AxisX.TitleFont = chtXFont;
            chartArea.AxisY.TitleFont = chtYFont;
            chartArea.AxisY.Title = "Data count";
            tmpChart.BorderlineColor = Color.Red;
            tmpChart.BorderlineWidth = 4;
            tmpChart.ChartAreas.Add(chartArea);

            for (int chart_idx = 0; chart_idx < 6; chart_idx++)
            {
                tmpChart.Series.Clear();
                tmpChart.ResetAutoValues();
                tmpChart.Series.Add("Charge");
                tmpChart.ChartAreas[0].AxisX.Title = chan_name[chart_idx] + "gain charge (ADCu)";
                //tmpChart.Series[0].Color = WeerocPaleBlue;
                //tmpChart.Series[0].BorderColor = Color.Red;
                tmpChart.Series[0].BorderWidth = 1;
                tmpChart.Series[0]["PointWidth"] = "1";

                int value = 0;
                for (int i = 0; i < 2048; i++)
                {
                     switch (chart_idx)
                    {
                        case 0:
                            value = PerChannelChargeHG[0, i];
                            break;
                        case 1:
                            value = PerChannelChargeLG[0, i];
                            break;
                        case 2:
                            value = PerChannelChargeHG[16, i];
                            break;
                        case 3:
                            value = PerChannelChargeLG[16, i];
                            break;
                        case 4:
                            value = PerChannelChargeHG[31, i];
                            break;
                        case 5:
                            value = PerChannelChargeLG[31, i];
                            break;
                    }

                    if (value != 0) tmpChart.Series[0].Points.AddXY(i, value);

                }

                tmpChart.DrawToBitmap(bmp, new Rectangle(P[chart_idx].X, P[chart_idx].Y,1800,1200));
            }
           
            bmp.Save(@"C:\Temp\test.png");
        }
        #endregion
    }
}
