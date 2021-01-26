using System;
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
    public partial class CubesMonitor : Form
    {
         private ProtoCubesSerial protoCubes;
        
        public CubesMonitor(ProtoCubesSerial protoCubes)
        {
            InitializeComponent();
            this.protoCubes = protoCubes;
            this.protoCubes.MonitorActive = true;
            this.protoCubes.DisplayWindow = CommMonitorTextBox;
            this.protoCubes.DataReadyEvent += ReqHK_DataReady;
        }

        public int ConnectStatus { get; set; }

        public RichTextBox CommMonitorTextBox
        {
            get { return rtxtMonitor; }
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            textBox_timestamp.Text = "";
            textBox_hitCountMPPC1.Text = "";
            textBox_hitCountMPPC2.Text = "";
            textBox_hitCountMPPC3.Text = "";
            textBox_hitCountOR32.Text = "";
            textBox_voltageFromHVPS.Text = "";
            textBox_currentFromHVPS.Text = "";
            textBox_tempFromHVPS.Text = "";

            rtxtMonitor.Clear();
        }

        private void ReqHK_DataReady(object sender, DataReadyEventArgs e)
        {
            // Quit early if command is not REQ_HK!
            if (e.Command != ProtoCubesSerial.Command.ReqHK)
                return;

            // 1. Handle the simple ones: the hit counts...
            UInt32 timestamp = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(e.DataBytes, 11, 10));

            byte[] ch0_hit_count = new byte[4];
            byte[] ch16_hit_count = new byte[4];
            byte[] ch31_hit_count = new byte[4];
            byte[] or32_hit_count = new byte[4];
            Array.Copy(e.DataBytes, 23, ch0_hit_count, 0, 4);
            Array.Copy(e.DataBytes, 27, ch16_hit_count, 0, 4);
            Array.Copy(e.DataBytes, 31, ch31_hit_count, 0, 4);
            Array.Copy(e.DataBytes, 35, or32_hit_count, 0, 4);

            byte[] reset_count = new byte[4];
            Array.Copy(e.DataBytes, 51, reset_count, 0, 4);

            byte[] hvps_cmds_sent = new byte[2];
            Array.Copy(e.DataBytes, 55, hvps_cmds_sent, 0, 2);
            byte[] hvps_cmds_acked = new byte[2];
            Array.Copy(e.DataBytes, 57, hvps_cmds_acked, 0, 2);
            byte[] hvps_cmds_rej = new byte[2];
            Array.Copy(e.DataBytes, 59, hvps_cmds_rej, 0, 2);

            // Reverse arrays before conversion if on a little-endian machine
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ch0_hit_count);
                Array.Reverse(ch16_hit_count);
                Array.Reverse(ch31_hit_count);
                Array.Reverse(or32_hit_count);
                Array.Reverse(reset_count);
                Array.Reverse(hvps_cmds_sent);
                Array.Reverse(hvps_cmds_acked);
                Array.Reverse(hvps_cmds_rej);
            }

            UInt32 hitCountMPPC3 = BitConverter.ToUInt32(ch0_hit_count, 0);
            UInt32 hitCountMPPC2 = BitConverter.ToUInt32(ch16_hit_count, 0);
            UInt32 hitCountMPPC1 = BitConverter.ToUInt32(ch31_hit_count, 0);
            UInt32 hitCountOR32 = BitConverter.ToUInt32(or32_hit_count, 0);

            UInt32 resetCount = BitConverter.ToUInt32(reset_count, 0);

            UInt16 hvpsCmdsSent = BitConverter.ToUInt16(hvps_cmds_sent, 0);
            UInt16 hvpsCmdsAcked = BitConverter.ToUInt16(hvps_cmds_acked, 0);
            UInt16 hvpsCmdsRej = BitConverter.ToUInt16(hvps_cmds_rej, 0);

            // 2. Now for the HVPS stuff... It is presented as ASCII characters
            // by the HVPS, placed at particular offsets in the HK data stream.
            // These characters need to be converted to a string, which then needs
            // to be converted to a double representation before the conversion
            // formula in the datasheet can be applied.
            string s;

            byte[] hvps_voltage = new byte[4];
            Array.Copy(e.DataBytes, 39, hvps_voltage, 0, 4);
            s = System.Text.Encoding.ASCII.GetString(hvps_voltage);
            if (s == "\0\0\0\0")
                s = "0000";
            double voltageFromHVPS = Convert.ToDouble(Convert.ToUInt16(s, 16));
            voltageFromHVPS *= 1.812 * Math.Pow(10, -3);

            byte[] hvps_current = new byte[4];
            Array.Copy(e.DataBytes, 43, hvps_current, 0, 4);
            s = System.Text.Encoding.ASCII.GetString(hvps_current);
            if (s == "\0\0\0\0")
                s = "0000";
            double currentFromHVPS = Convert.ToDouble(Convert.ToUInt16(s, 16));
            currentFromHVPS *= 5.194 * Math.Pow(10, -3);

            byte[] hvps_temp = new byte[4];
            Array.Copy(e.DataBytes, 47, hvps_temp, 0, 4);
            s = System.Text.Encoding.ASCII.GetString(hvps_temp);
            if (s == "\0\0\0\0")
                s = "0000";
            double tempFromHVPS = Convert.ToDouble(Convert.ToUInt16(s, 16));
            tempFromHVPS = (tempFromHVPS * 1.907 * Math.Pow(10, -5) - 1.035) /
                           (-5.5 * Math.Pow(10, -3));

            // 3. Apply the values into the text boxes; use the Control.Invoke()
            //    method, to make sure the writing is done inside the original
            //    UI thread
            textBox_timestamp.Invoke(new EventHandler(
                delegate
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime time = epoch.AddSeconds(Convert.ToDouble(timestamp));
                    textBox_timestamp.Text = time.ToString("hh:mm:ss tt") + " on " +
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
            textBox_voltageFromHVPS.Invoke(new EventHandler(
                delegate
                {
                    textBox_voltageFromHVPS.Text = voltageFromHVPS.ToString("N3");
                }
            ));
            textBox_currentFromHVPS.Invoke(new EventHandler(
                delegate
                {
                    textBox_currentFromHVPS.Text = currentFromHVPS.ToString("N3");
                }
            ));
            textBox_tempFromHVPS.Invoke(new EventHandler(
                delegate
                {
                    textBox_tempFromHVPS.Text = tempFromHVPS.ToString("N3");
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

        private void numUpDown_dt2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label_hitCountMPPC1_Click(object sender, EventArgs e)
        {

        }

        private void textBox_hitCountMPPC2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label_Voltage_Click(object sender, EventArgs e)
        {

        }
    }
}
