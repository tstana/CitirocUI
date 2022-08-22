﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CitirocUI
{
    public partial class ProtoCubesMonitor : Form
    {
         private ProtoCubesSerial protoCubes;
        
        public ProtoCubesMonitor(ProtoCubesSerial protoCubes)
        {
            InitializeComponent();
            this.protoCubes = protoCubes;
            this.protoCubes.MonitorActive = true;
            this.protoCubes.DisplayWindow = SerialMonitorTextBox;
            this.protoCubes.DataReadyEvent += ReqHK_DataReady;
        }

        private int connectStatus;

        public int ConnectStatus
        {
            get { return connectStatus; }
            set
            {
                connectStatus = value;
                if (connectStatus == 1)
                {
                    label_ConnStatus.Text = "Connected to Proto-CUBES on " +
                        protoCubes.PortName;
                    label_ConnStatus.ForeColor = Color.DarkGreen;
                }
                else if (connectStatus == -1)
                {
                    label_ConnStatus.Text = "Not connected.";
                    label_ConnStatus.ForeColor = Color.IndianRed;
                }
            }
        }

        public RichTextBox SerialMonitorTextBox
        {
            get { return rtxtMonitor; }
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            textBox_arduinoTime.Text = "";
            textBox_hitCountMPPC1.Text = "";
            textBox_hitCountMPPC2.Text = "";
            textBox_hitCountMPPC3.Text = "";
            textBox_hitCountOR32.Text = "";
            textBox_hvpsVolt.Text = "";
            textBox_hvpsCurr.Text = "";
            textBox_hvpsTemp.Text = "";
            textBox_hkadcCitiTemp.Text = "";
            textBox_hkadcCurr.Text = "";
            textBox_hkadcVolt.Text = "";

            rtxtMonitor.Clear();
        }

        private void button_readHK_Click(object sender, EventArgs e)
        {
            try
            {
                if (ConnectStatus == 1)
                {
                    protoCubes.SendCommand(ProtoCubesSerial.Command.ReqHK, null);
                    button_readHK.BackColor = Color.ForestGreen;
                }
                else
                {
                    throw new Exception("Please connect to an instrument " +
                        "using the \"Connect\" tab.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send HK request command " +
                    "to Proto-CUBES!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                button_readHK.BackColor = Color.IndianRed;
            }

            tmrButtonColor.Enabled = true;
        }

        private void ReqHK_DataReady(object sender, DataReadyEventArgs e)
        {
            // Quit early if command is not REQ_HK:
            if (e.Command != ProtoCubesSerial.Command.ReqHK)
                return;


            ///
            /// Step 1: Copy data bytes into local arrays
            ///


            /// Arduino timestamp, 10 chars after the text "Unix time: ", hence
            /// offset should be 11:
            int offset = 11;
            int bytes = 10;
            uint arduinoTime =
                Convert.ToUInt32(Encoding.ASCII.GetString(e.DataBytes, offset, bytes));
            offset += bytes;
            offset += 2; // for the \r\n

            /// CUBES timestamp, reset count and channel hit counters,
            /// 4 bytes each:
            bytes = 4;
            byte[] cubes_time = new byte[bytes];
            Array.Copy(e.DataBytes, offset, cubes_time, 0, bytes);
            offset += bytes;

            byte[] reset_count = new byte[4];
            Array.Copy(e.DataBytes, offset, reset_count, 0, bytes);
            offset += bytes;

            byte[] ch0_hit_count = new byte[4];
            Array.Copy(e.DataBytes, offset, ch0_hit_count, 0, bytes);
            offset += bytes;

            byte[] ch16_hit_count = new byte[4];
            Array.Copy(e.DataBytes, offset, ch16_hit_count, 0, bytes);
            offset += bytes;

            byte[] ch31_hit_count = new byte[4];
            Array.Copy(e.DataBytes, offset, ch31_hit_count, 0, bytes);
            offset += bytes;

            byte[] or32_hit_count = new byte[4];
            Array.Copy(e.DataBytes, offset, or32_hit_count, 0, bytes);
            offset += bytes;

            // HVPS voltage, current, temperature and status:
            bytes = 2;
            byte[] hvps_volt = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_volt, 0, bytes);
            offset += bytes;

            byte[] hvps_curr = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_curr, 0, bytes);
            offset += bytes;

            byte[] hvps_temp = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_temp, 0, bytes);
            offset += bytes;

            byte[] hvps_stat = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_stat, 0, bytes);
            offset += bytes;

            // HVPS number of commands sent, acked and rejected:
            byte[] hvps_cmds_sent = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_cmds_sent, 0, bytes);
            offset += bytes;

            byte[] hvps_cmds_acked = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_cmds_acked, 0, bytes);
            offset += bytes;

            byte[] hvps_cmds_rej = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_cmds_rej, 0, bytes);
            offset += bytes;

            byte[] hvps_last_cmd_err = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hvps_last_cmd_err, 0, bytes);
            offset += bytes;

            // Finally, the on-board ADC readouts:
            byte[] hkadc_batt_volt = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hkadc_batt_volt, 0, bytes);
            offset += bytes;

            byte[] hkadc_batt_curr = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hkadc_batt_curr, 0, bytes);
            offset += bytes;

            byte[] hkadc_citi_temp = new byte[bytes];
            Array.Copy(e.DataBytes, offset, hkadc_citi_temp, 0, bytes);


            ///
            /// Step 2: Reverse arrays before conversion if we are on a
            ///         little-endian machine
            /// 


            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(cubes_time);
                Array.Reverse(reset_count);
                Array.Reverse(ch0_hit_count);
                Array.Reverse(ch16_hit_count);
                Array.Reverse(ch31_hit_count);
                Array.Reverse(or32_hit_count);
                Array.Reverse(hvps_volt);
                Array.Reverse(hvps_curr);
                Array.Reverse(hvps_temp);
                Array.Reverse(hvps_stat);
                Array.Reverse(hvps_cmds_sent);
                Array.Reverse(hvps_cmds_acked);
                Array.Reverse(hvps_cmds_rej);
                Array.Reverse(hvps_last_cmd_err);
                Array.Reverse(hkadc_batt_volt);
                Array.Reverse(hkadc_batt_curr);
                Array.Reverse(hkadc_citi_temp);
            }


            ///
            /// Step 3: Convert byte arrays to usable values
            ///


            // CUBES timestamp and various counters:
            uint cubesTime = BitConverter.ToUInt32(cubes_time, 0);

            uint resetCount = BitConverter.ToUInt32(reset_count, 0);

            uint hitCountMPPC1 = BitConverter.ToUInt32(ch0_hit_count, 0);
            uint hitCountMPPC2 = BitConverter.ToUInt32(ch16_hit_count, 0);
            uint hitCountMPPC3 = BitConverter.ToUInt32(ch31_hit_count, 0);
            uint hitCountOR32 = BitConverter.ToUInt32(or32_hit_count, 0);

            /// Convert 16-bit values from HVPS to doubles using formulas from
            /// the C11204-02 Command Reference Manual:
            double hvpsVolt = Convert.ToDouble(
                BitConverter.ToUInt16(hvps_volt, 0));
            hvpsVolt *= 1.812 * Math.Pow(10, -3);

            double hvpsCurr = Convert.ToDouble(
                BitConverter.ToUInt16(hvps_curr, 0));
            hvpsCurr *= 5.194 * Math.Pow(10, -3);

            double hvpsTemp = Convert.ToDouble(
                BitConverter.ToUInt16(hvps_temp, 0));
            hvpsTemp = (hvpsTemp * 1.907 * Math.Pow(10, -5) - 1.035) /
                           (-5.5 * Math.Pow(10, -3));

            UInt16 hvpsStatus = BitConverter.ToUInt16(hvps_stat, 0);

            // Other HVPS items:
            uint hvpsCmdsSent = BitConverter.ToUInt16(hvps_cmds_sent, 0);
            uint hvpsCmdsAcked = BitConverter.ToUInt16(hvps_cmds_acked, 0);
            uint hvpsCmdsRej = BitConverter.ToUInt16(hvps_cmds_rej, 0);

            // On-board ADC fields:
            double hkadcVolt = Convert.ToDouble(
                BitConverter.ToUInt16(hkadc_batt_volt, 0));
            hkadcVolt = hkadcVolt * 2.0 * 8 / 1000;

            double hkadcCurr = Convert.ToDouble(
                BitConverter.ToUInt16(hkadc_batt_curr, 0));
            hkadcCurr = hkadcCurr * 2.0 / 2500;

            double hkadcCitiTemp = Convert.ToDouble(
                BitConverter.ToUInt16(hkadc_citi_temp, 0));
            hkadcCitiTemp = hkadcCitiTemp * 2.0 / 1000;
            hkadcCitiTemp = (2.7 - hkadcCitiTemp) / 8e-3;


            ///
            /// Step 4: Apply the values into the text boxes; use the
            ///         Control.Invoke() method to make sure the writing to
            ///         text boxes is done inside the original UI thread.


            textBox_arduinoTime.Invoke(new EventHandler(
                delegate
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime time = epoch.AddSeconds(Convert.ToDouble(arduinoTime));
                    textBox_arduinoTime.Text = time.ToString("hh:mm:ss tt") + " on " +
                        time.ToString("yyyy-MM-dd");
                }
            ));

            textBox_cubesTime.Invoke(new EventHandler(
                delegate
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime time = epoch.AddSeconds(Convert.ToDouble(cubesTime));
                    textBox_cubesTime.Text = time.ToString("hh:mm:ss tt") + " on " +
                        time.ToString("yyyy-MM-dd");
                }
            ));

            textBox_hitCountMPPC1.Invoke(new EventHandler(
                delegate
                {
                    textBox_hitCountMPPC1.Text = hitCountMPPC1.ToString();
                }
            ));
            textBox_hitCountMPPC2.Invoke(new EventHandler(
                delegate
                {
                    textBox_hitCountMPPC2.Text = hitCountMPPC2.ToString();
                }
            ));
            textBox_hitCountMPPC3.Invoke(new EventHandler(
                delegate
                {
                    textBox_hitCountMPPC3.Text = hitCountMPPC3.ToString();
                }
            ));
            textBox_hitCountOR32.Invoke(new EventHandler(
                delegate
                {
                    textBox_hitCountOR32.Text = hitCountOR32.ToString();
                }
              ));
            textBox_hvpsVolt.Invoke(new EventHandler(
                delegate
                {
                    textBox_hvpsVolt.Text = hvpsVolt.ToString("N3");
                }
            ));
            textBox_hvpsCurr.Invoke(new EventHandler(
                delegate
                {
                    textBox_hvpsCurr.Text = hvpsCurr.ToString("N3");
                }
            ));
            textBox_hvpsTemp.Invoke(new EventHandler(
                delegate
                {
                    textBox_hvpsTemp.Text = hvpsTemp.ToString("N3");
                }
            ));
            textBox_hvpsStatus.Invoke(new EventHandler(
                delegate
                {
                    textBox_hvpsStatus.Text = "0x" + hvpsStatus.ToString("X");
                }
            ));
            textBox_ResetCount.Invoke(new EventHandler(
                delegate
                {
                    textBox_ResetCount.Text = resetCount.ToString();
                }
            ));
            textBox_hvpsCmdsSent.Invoke(new EventHandler(
                delegate
                {
                    textBox_hvpsCmdsSent.Text = hvpsCmdsSent.ToString();
                }
            ));
            textBox_hvpsCmdsAcked.Invoke(new EventHandler(
                delegate
                {
                    textBox_hvpsCmdsAcked.Text = hvpsCmdsAcked.ToString();
                }
            ));
            textBox_hvpsCmdsRej.Invoke(new EventHandler(
                delegate
                {
                    textBox_hvpsCmdsRej.Text = hvpsCmdsRej.ToString();
                }
            ));
            textBox_hkadcVolt.Invoke(new EventHandler(
                delegate
                {
                    textBox_hkadcVolt.Text = hkadcVolt.ToString("N3");
                }
            ));
            textBox_hkadcCurr.Invoke(new EventHandler(
                delegate
                {
                    textBox_hkadcCurr.Text = hkadcCurr.ToString("N3");
                }
            ));
            textBox_hkadcCitiTemp.Invoke(new EventHandler(
                delegate
                {
                    textBox_hkadcCitiTemp.Text = hkadcCitiTemp.ToString("N3");
                }
            ));
        }

        private void button_hvSendTemp_Click(object sender, EventArgs e)
        {
            // Set conversion factor from the command reference dcoument of C11204-02.
            UInt16 volt = (UInt16)(Convert.ToDouble(numUpDown_HV.Text) / 1.812e-3);
            byte[] voltBytes = BitConverter.GetBytes(volt);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(voltBytes);

            byte[] hvpsConf = new byte[3];
            hvpsConf[0] = checkBox_HVON.Checked ? Convert.ToByte(1) : Convert.ToByte(0);
            hvpsConf[1] = voltBytes[0];
            hvpsConf[2] = voltBytes[1];

            try
            {
                if (ConnectStatus == 1)
                {
                    protoCubes.SendCommand(ProtoCubesSerial.Command.SendHVPSTmpVolt,
                        hvpsConf);
                    button_hvSendTemp.BackColor = Color.ForestGreen;
                }
                else
                {
                    throw new Exception("Please connect to an instrument " +
                        "using the \"Connect\" tab.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send HVPS configuration " +
                    "to Proto-CUBES!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                button_hvSendTemp.BackColor = Color.IndianRed;
            }

            tmrButtonColor.Enabled = true;
        }

        private void button_hvSendPersistent_Click(object sender, EventArgs e)
        {
            byte[] hvpsConf = new byte[13];

            Int16 dtp1 = 0;
            Int16 dtp2 = 0;
            UInt16 dt1 = 0;
            UInt16 dt2 = 0;
            UInt16 v = 0;
            UInt16 t = 0;

            // Send zeros as temp. compensation factors if sending reset...
            if (checkBox_hvReset.Checked == false)
            {
                dtp1 = Convert.ToInt16((double)numUpDown_dtp1.Value / 1.507e-3);
                dtp2 = Convert.ToInt16((double)numUpDown_dtp2.Value / 1.507e-3);
                dt1 = Convert.ToUInt16((double)numUpDown_dt1.Value / 5.225e-2);
                dt2 = Convert.ToUInt16((double)numUpDown_dt2.Value / 5.225e-2);
                v = Convert.ToUInt16((double)numUpDown_refVolt.Value / 1.812e-3);
                t = Convert.ToUInt16((((double)(numUpDown_refTemp.Value)) * (-5.5e-3) + 1.035) / 1.907e-5);
            }

            hvpsConf[0] = (byte)(checkBox_hvReset.Checked ? 0x03 : 0x01);
            hvpsConf[1] = (byte)((dtp1 & 0xff00) >> 8);
            hvpsConf[2] = (byte)((dtp1 & 0x00ff));
            hvpsConf[3] = (byte)((dtp2 & 0xff00) >> 8);
            hvpsConf[4] = (byte)((dtp2 & 0x00ff));
            hvpsConf[5] = (byte)((dt1 & 0xff00) >> 8);
            hvpsConf[6] = (byte)((dt1 & 0x00ff));
            hvpsConf[7] = (byte)((dt2 & 0xff00) >> 8);
            hvpsConf[8] = (byte)((dt2 & 0x00ff));
            hvpsConf[9] = (byte)((v & 0xff00) >> 8);
            hvpsConf[10] = (byte)((v & 0x00ff));
            hvpsConf[11] = (byte)((t & 0xff00) >> 8);
            hvpsConf[12] = (byte)((t & 0x00ff));

            try
            {
                if (ConnectStatus == 1)
                {
                    protoCubes.SendCommand(ProtoCubesSerial.Command.SendHVPSConf,
                        hvpsConf);
                    button_hvSendPersistent.BackColor = Color.ForestGreen;
                }
                else
                {
                    throw new Exception("Please connect to an instrument " +
                        "using the \"Connect\" tab.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send HVPS configuration " +
                    "to Proto-CUBES!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                button_hvSendPersistent.BackColor = Color.IndianRed;
            }

            tmrButtonColor.Enabled = true;
        }

        private void checkBox_hvReset_CheckedChanged(object sender, EventArgs e)
        {
            label_refTemp.Enabled = !(checkBox_hvReset.Checked);
            label_refVolt.Enabled = !(checkBox_hvReset.Checked);
            label_dtp1.Enabled = !(checkBox_hvReset.Checked);
            label_dtp2.Enabled = !(checkBox_hvReset.Checked);
            label_dt1.Enabled = !(checkBox_hvReset.Checked);
            label_dt2.Enabled = !(checkBox_hvReset.Checked);
            numUpDown_refTemp.Enabled = !(checkBox_hvReset.Checked);
            numUpDown_refVolt.Enabled = !(checkBox_hvReset.Checked);
            numUpDown_dtp1.Enabled = !(checkBox_hvReset.Checked);
            numUpDown_dtp2.Enabled = !(checkBox_hvReset.Checked);
            numUpDown_dt1.Enabled = !(checkBox_hvReset.Checked);
            numUpDown_dt2.Enabled = !(checkBox_hvReset.Checked);
        }

        private void tmrButtonColor_Tick(object sender, EventArgs e)
        {
            tmrButtonColor.Enabled = false;

            button_readHK.BackColor = SystemColors.Control;

            button_hvSendTemp.BackColor = SystemColors.Control;
            button_hvSendTemp.ForeColor = SystemColors.ControlText;

            button_hvSendPersistent.BackColor = SystemColors.Control;
            button_hvSendPersistent.ForeColor = SystemColors.ControlText;
        }

        private void CubesMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            protoCubes.DataReadyEvent -= ReqHK_DataReady;
            protoCubes.MonitorActive = false;
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string helpString =
             "TX data is colored yellow\r\n" +
             "RX data is colored green\r\n" +
             "\r\n" +
             "Press \"Clear\" to clear monitor contents.";
            MessageBox.Show(helpString, "Help");
        }
    }
}
