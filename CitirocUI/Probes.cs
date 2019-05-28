﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace CitirocUI
{
    public partial class Citiroc : Form
    {
        private void button_sendProbes_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (comboBox_SelectConnection.SelectedIndex == 0)
            //if (mySerialPort.IsOpen == false)
            {
                // Test if software can read firmware version. If not, the board is not connected.
                if (Firmware.readWord(100, usbDevId) != "00000000")
                {
                    result = sendProbes(usbDevId);
                    if (result) button_sendProbes.BackColor = WeerocGreen;
                    else button_sendProbes.BackColor = Color.LightCoral;
                    button_sendProbes.ForeColor = Color.White;
                }
                else
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
                }
            }
            else if (comboBox_SelectConnection.SelectedIndex == 1)
            {
                result = sendProbes(usbDevId);
                if (result)
                    button_sendProbes.BackColor = WeerocGreen;
                else
                    button_sendProbes.BackColor = Color.LightCoral;
                button_sendProbes.ForeColor = Color.White;
            }
                       
          //  return;
        }

        private bool sendProbes(int usbDevId)
        {
            bool result = false;
            char[] tmpProbeStream = new char[256];

            for (int i = 0; i < 256; i++) tmpProbeStream[i] = '0';

            tmpProbeStream[(int)numericUpDown_probeAChannel.Value] = (radioButton_probeFsh.Checked == true) ? '1' : '0';
            tmpProbeStream[(int)numericUpDown_probeAChannel.Value + 32] = (radioButton_probeSshLG.Checked == true) ? '1' : '0';
            tmpProbeStream[(int)numericUpDown_probeDChannel.Value + 64] = (radioButton_probePDLG.Checked == true) ? '1' : '0';
            tmpProbeStream[(int)numericUpDown_probeAChannel.Value + 96] = (radioButton_probeSshHG.Checked == true) ? '1' : '0';
            tmpProbeStream[(int)numericUpDown_probeDChannel.Value + 128] = (radioButton_probePDHG.Checked == true) ? '1' : '0';
            tmpProbeStream[(int)numericUpDown_probeAChannel.Value * 2 + 160] = (radioButton_probePaHG.Checked == true) ? '1' : '0';
            tmpProbeStream[(int)numericUpDown_probeAChannel.Value * 2 + 160 + 1] = (radioButton_probePaLG.Checked == true) ? '1' : '0';

            string probeStream = new string(tmpProbeStream);

            int intLenProbeStream = probeStream.Length;

            if (comboBox_SelectConnection.SelectedIndex == 1)
            {
                byte[] bytProbe = new byte[1 + intLenProbeStream / 8];

                bytProbe[0] = Convert.ToByte('P');
                for (int i = 0; i < (intLenProbeStream / 8); i++)
                {
                    string strProbeCmdTmp = probeStream.Substring(i * 8, 8);
                    strProbeCmdTmp = strRev(strProbeCmdTmp);
                    UInt32 intCmdTmp = Convert.ToUInt32(strProbeCmdTmp, 2);
                    bytProbe[i + 1] = Convert.ToByte(intCmdTmp);
                }

                try
                {
                    //mySerialPort.Write(bytProbe, 0, 1 + intLenProbeStream / 8);
                    if (showMonitor)
                    {
                        SendDataToMonitorEvent(bytProbe, true);
                    }
                    result = true;
                }
                catch (IOException)
                {
                    return false;
                }
            }
            else if (comboBox_SelectConnection.SelectedIndex == 0)
            {
                byte[] bytProbe = new byte[intLenProbeStream / 8];

                probeStream = strRev(probeStream);

                bytProbe[0] = Convert.ToByte('P');
                for (int i = 0; i < (intLenProbeStream / 8); i++)
                {
                    string strProbeCmdTmp = probeStream.Substring(i * 8, 8);
                    strProbeCmdTmp = strRev(strProbeCmdTmp);
                    UInt32 intCmdTmp = Convert.ToUInt32(strProbeCmdTmp, 2);
                    bytProbe[i] = Convert.ToByte(intCmdTmp);
                }

                // Select probes parameters to FPGA
                Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);
                // Send probes parameters to FPGA
                int intLenBytProbes = bytProbe.Length;
                Firmware.sendWord(10, bytProbe, intLenBytProbes, usbDevId);

                // Start shift parameters to ASIC
                Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "10", usbDevId);
                // Stop shift parameters to ASIC
                Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

                // probes test checksum test query
                Firmware.sendWord(0, "10" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);

                // Load probes
                Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "01", usbDevId);
                Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

                // Send probes parameters to FPGA
                Firmware.sendWord(10, bytProbe, intLenBytProbes, usbDevId);

                // Start shift parameters to ASIC
                Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "10", usbDevId);
                // Stop shift parameters to ASIC
                Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

                // Probes Correlation Test Result
                if (Firmware.readWord(4, usbDevId) == "00000000") result = true;

                // Reset probes test checksum test query
                Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);

                return true;
            }

            return result;
        }

        private void button_sendReadRegister_Click(object sender, EventArgs e)
        {
            bool result = false;

            // Test if software can read firmware version. If not, the board is not connected.
            if (Firmware.readWord(100, usbDevId) != "00000000")
            {
                result = sendReadRegister(usbDevId);
                if (result) button_sendReadRegister.BackColor = WeerocGreen;
                else button_sendReadRegister.BackColor = Color.LightCoral;
                button_sendReadRegister.ForeColor = Color.White;
            }
            else
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
            }
        }

        private bool sendReadRegister(int usbDevId)
        {
            bool result = false;
            char[] tmpRrStream = new char[40];
            
            for (int i = 0; i < 40; i++) tmpRrStream[i] = '0';
            
            tmpRrStream[(int)numericUpDown_rr.Value] = (checkBox_rr.Checked == true) ? '1' : '0';
            
            string rrStream = new string(tmpRrStream);
            
            byte[] bytRr = new byte[5];
            
            rrStream = strRev(rrStream);
            
            for (int i = 0; i < 5; i++)
            {
                string strRrCmdTmp = rrStream.Substring(i * 8, 8);
                strRrCmdTmp = strRev(strRrCmdTmp);
                UInt32 intCmdTmp = Convert.ToUInt32(strRrCmdTmp, 2);
                bytRr[i] = Convert.ToByte(intCmdTmp);
            }

            // Select probes parameters to FPGA
            Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);
            // Send probes parameters to FPGA
            Firmware.sendWord(12, bytRr, 5, usbDevId);

            // Start shift parameters to ASIC
            Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "10", usbDevId);
            // Stop shift parameters to ASIC
            Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

            // probes test checksum test query
            Firmware.sendWord(0, "10" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);

            // Load probes
            Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "01", usbDevId);
            Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

            // Send probes parameters to FPGA
            Firmware.sendWord(12, bytRr, 5, usbDevId);

            // Start shift parameters to ASIC
            Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "10", usbDevId);
            // Stop shift parameters to ASIC
            Firmware.sendWord(1, "110" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

            // Probes Correlation Test Result
            if (Firmware.readWord(4, usbDevId) == "00000000") result = true;

            // Reset probes test checksum test query
            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);

            return result;
        }
    }
}
