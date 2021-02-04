using System;
using System.Drawing;
using System.Windows.Forms;
using FTD2XX_NET;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;

namespace CitirocUI
{
    public partial class Citiroc : Form
    {

        #region FTDI Public def.
        FTDI.FT_DEVICE ftdiDevice;
        uint ftdiDeviceCount;
        FTDI.FT_STATUS ftStatus;
        FTDI myFtdiDevice;
        FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList;
        FTDI.FT_DEVICE_INFO_NODE[] testBoardFtdiDevice = new FTDI.FT_DEVICE_INFO_NODE[1];

        public Int32 usbDevId;
        #endregion

        #region EPCS Operation Code
        private const byte AS_NOP = 0x00;
        private const byte AS_WRITE_ENABLE = 0x06;
        private const byte AS_WRITE_DISABLE = 0x04;
        private const byte AS_READ_STATUS = 0x05;
        private const byte AS_WRITE_STATUS = 0x01;
        private const byte AS_READ_BYTES = 0x03;
        private const byte AS_FAST_READ_BYTES = 0x0B;
        private const byte AS_PAGE_PROGRAM = 0x02;
        private const byte AS_ERASE_SECTOR = 0xD8;
        private const byte AS_ERASE_BULK = 0xC7;
        private const byte AS_READ_SILICON_ID = 0xAB;
        private const byte AS_CHECK_SILICON_ID = 0x9F;
        #endregion

        #region Pin Definition
        private const byte CONF_DONE = 0x80;
        private const byte ASDI = 0x40;
        private const byte DATAOUT = 0x10;
        private const byte NCE = 0x08;
        private const byte NCS = 0x04;
        private const byte NCONFIG = 0x02;
        private const byte DCLK = 0x01;
        private const byte CUR_DATA = 0x00;
        private const byte DEF_VALUE = 0x0E;
        #endregion

        // Member to store currently selected connection mode; needed since
        // the connection mode is accessed from a separate thread, which can't
        // gain access to the SelectedIndex property of the combo-box (due to
        // WinForms threading issues...)
        //
        // = 0 for USB and 1 for Serial;
        private int selectedConnectionMode;

        int connectStatus = -1; // -1 = not connected, 0 = board connected but most likely powering issue, 1 = board connected
        unsafe private void roundButton_connect_Click(object sender, EventArgs e)
        {
            if (comboBox_SelectConnection.SelectedIndex == 0)
            {
                label_plug.Visible = false;

                // If usb already opened, close it.
                if (usbDevId > 0)
                {
                    USB.CloseUsbDevice(usbDevId);
                    usbDevId = 0;
                }

                label_boardStatus.Text = "Board status\n" + "Not connected";

                // if connectStatus was "connected", make it disconnected and return
                if (connectStatus == 1)
                {
                    roundButton_connect.BackColor = Color.Gainsboro;
                    roundButton_connect.ForeColor = Color.Black;
                    roundButton_connectSmall.BackColor = Color.Gainsboro;
                    roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff.png");
                    connectStatus = -1;
                    label_boardStatus.Text = "Board status\n" + "No board connected";
                    button_loadFw.Visible = false;
                    progressBar_loadFw.Visible = false;
                    return;
                }

                // check for usb devices
                ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);

                if (ftdiDeviceCount > 0)
                {
                    ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
                    ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

                    testBoardFtdiDevice[0] = ftdiDeviceList[0];
                    int index = 0;
                    if (ftdiDeviceList.Length > 2)
                    {
                        using (Form_ftdiDevices frm = new Form_ftdiDevices(ftdiDeviceList))
                        {
                            frm.ShowDialog();

                            index = frm.ftdiIndex;
                        }
                    }

                    testBoardFtdiDevice[0] = ftdiDeviceList[index];

                    // Force connection to the A port of the USB. The B port is for firmware loading.
                    string serialNumber = testBoardFtdiDevice[0].SerialNumber;
                    serialNumber = serialNumber.Remove(serialNumber.Length - 1) + "A";
                    string description = testBoardFtdiDevice[0].Description;
                    description = description.Remove(description.Length - 1) + "A";
                    string boardFirmware = "";

                    int numUsbDev = USB.USB_GetNumberOfDevs();
                    // Open the usb device
                    usbDevId = USB.OpenUsbDevice(serialNumber);

                    bool retVerbose = false;
                    bool retUsbOpen = USB.USB_Init(usbDevId, ref retVerbose);
                    bool retSetLT = USB.USB_SetLatencyTimer(usbDevId, 2);

                    // Read Latency Time from FPGA
                    byte[] templtime = new byte[1];
                    unsafe
                    {
                        fixed (byte* array = templtime)
                        {
                            bool ret_value = USB.USB_GetLatencyTimer(usbDevId, array);
                        }
                    }
                    string strLatency = templtime[0].ToString();

                    bool retSetBufSize = USB.USB_SetXferSize(usbDevId, 8192, 32768);
                    bool retSetTimeOuts = USB.USB_SetTimeouts(usbDevId, 20, 20);

                    if (retUsbOpen && retSetLT && retSetBufSize && retSetTimeOuts)
                    {
                        byte[] tempRx = new byte[1];
                        string rdSubAdd100 = "00000000";
                        int testCon = 0;
                        // Try to connect to the device up to 10 times
                        while (rdSubAdd100 == "00000000" && testCon < 10)
                        {
                            testCon++;
                            // Sub address 100 contain the firware version. If rdSubAdd100 == 0 the board failed to connect
                            rdSubAdd100 = Firmware.readWord(100, usbDevId);
                            // Wait 20 ms
                            Thread.Sleep(20);
                        }

                        // If rdSubAdd100 == 0, the board failed to connect 10 times in a row
                        if (rdSubAdd100 == "00000000")
                        {
                            MessageBox.Show("Looks like there is an issue with the connection. Please verify the board is well plugged and powered and click again.");
                            connectStatus = 0;
                            roundButton_connect.BackColor = Color.IndianRed;
                            roundButton_connect.ForeColor = Color.White;
                            roundButton_connectSmall.BackColor = Color.IndianRed;
                            roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff2.png");
                            label_boardStatus.Text = "Board status\n" + "Connection error";
                            return;
                        }
                        else
                        {
                            label_help.Text = "The Citiroc testboard is connected. Click again if you wish to disconnect.";
                            connectStatus = 1;
                            roundButton_connect.BackColor = WeerocGreen;
                            roundButton_connect.ForeColor = Color.White;
                            roundButton_connectSmall.BackColor = WeerocGreen;
                            roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff2.png");

                            // Initialize firmware static registers (contains firmware options)
                            Firmware.sendWord(63, "00110100", usbDevId); // Temperature sensor config
                            Firmware.sendWord(62, "00000011", usbDevId); // Send temp sensor config
                            Firmware.sendWord(62, "00000010", usbDevId);
                            tempTimer = new System.Threading.Timer(tempCallback, null, 1000, Timeout.Infinite);

                            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);
                            Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);
                            Firmware.sendWord(2, "000001" + ((checkBox_ADC1.Checked == true) ? "1" : "0") + ((checkBox_ADC2.Checked == true) ? "1" : "0"), usbDevId);
                            Firmware.sendWord(3, ((checkBox_rstbPS.Checked == true) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked == true) ? "1" : "0") + ((checkBox_selHold.Checked == true) ? "1" : "0") + ((checkBox_selTrigToHold.Checked == true) ? "1" : "0") + ((checkBox_triggerTorQ.Checked == true) ? "1" : "0") + ((checkBox_pwrOn.Checked == true) ? "1" : "0"), usbDevId);
                            Firmware.sendWord(5, "0" + ((checkBox_selPSGlobalTrigger.Checked == true) ? "1" : "0") + ((checkBox_selPSMode.Checked == true) ? "1" : "0") + "000" + ((checkBox_PSGlobalTrigger.Checked == true) ? "1" : "0") + ((checkBox_PSMode.Checked == true) ? "1" : "0"), usbDevId);
                            Firmware.sendWord(30, IntToBin(60, 8), usbDevId);
                            readAllFPGAWords();
                        }

                        /* KTH firmware starts at v.250 */
                        bool addKth = false;
                        int fwVers = Convert.ToInt32(rdSubAdd100, 2);
                        if (fwVers >= 250)
                            addKth = true;
                        boardFirmware = (fwVers.ToString());
                        label_boardStatus.Text = "Board status\n" + "Serial number: " + serialNumber + "\n" + "Description: " + description + "\n" + "Firmware version: v" + boardFirmware;
                        if (addKth)
                            label_boardStatus.Text += " (KTH)";

                        // Test if the firmware version correspond to the software version
                        if (boardFirmware.ToString() == System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString())
                        {
                            button_loadFw.Visible = false;
                            progressBar_loadFw.Visible = false;
                        }
                        else
                        {
                            label_boardStatus.Text += "\n\n" + "This software has been designed to work with the firmware version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString() + ". You can load it to the testboard with the button below.";
                            button_loadFw.Visible = true;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No USB Devices are connected.");
                    connectStatus = -1;
                    roundButton_connect.BackColor = Color.IndianRed;
                    roundButton_connect.ForeColor = Color.White;
                    roundButton_connectSmall.BackColor = Color.IndianRed;
                    roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff2.png");
                    label_boardStatus.Text = "Board status\n" + "Not connected";
                }
            }

            else if (comboBox_SelectConnection.SelectedIndex == 1)
            {
                /* Disconnect and exit if we are already connected... */
                if (connectStatus == 1)
                {
                    protoCubes.ClosePort();

                    roundButton_connect.BackColor = Color.Gainsboro;
                    roundButton_connect.ForeColor = Color.Black;
                    roundButton_connectSmall.BackColor = Color.Gainsboro;
                    roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff.png");
                    connectStatus = -1;
                    label_boardStatus.Text = "Board status\n" + "No board connected";
                    if (protoCubesMonitorForm != null)
                        protoCubesMonitorForm.ConnectStatus = connectStatus;
                    return;
                }

                /* Otherwise, connect to selected port, creating it on first run of the code */
                try
                {
                    if (protoCubes == null)
                        protoCubes = new ProtoCubesSerial();

                    protoCubes.PortName = comboBox_COMPortList.SelectedItem.ToString();
                    protoCubes.BaudRate = Convert.ToInt32(comboBox_Baudrate.SelectedItem.ToString());

                    protoCubes.Parity = Parity.None;
                    protoCubes.StopBits = StopBits.One;
                    protoCubes.DataBits = 8;
                    protoCubes.Handshake = Handshake.None;
                    protoCubes.RtsEnable = false;

                    //// Set the read/write timeouts
                    protoCubes.ReadTimeout = 500;
                    protoCubes.WriteTimeout = 500;

                    // Finally! Open serial port!
                    if (!protoCubes.OpenPort())
                    {
                        throw new Exception();
                    }

                    // Update connection indicators
                    connectStatus = 1;
                    roundButton_connect.BackColor = WeerocGreen;
                    roundButton_connect.ForeColor = Color.White;
                    roundButton_connectSmall.BackColor = WeerocGreen;
                    roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff2.png");
                    label_help.Text = comboBox_SelectConnection.Text + " is connected. Click again if you wish to disconnect.";

                    // Send time on connection (Proto-CUBES)
                    SendTimeToProtoCUBES();

                    // Request Board ID on connect
                    try
                    {
                        protoCubes.SendCommand(ProtoCubesSerial.Command.ReqBoardID, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to send request board ID command " +
                            "to Proto-CUBES!"
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Error message:"
                            + Environment.NewLine
                            + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        throw new Exception();
                    }
                }
                catch
                {
                    connectStatus = -1;
                    roundButton_connect.BackColor = Color.IndianRed;
                    roundButton_connect.ForeColor = Color.White;
                    roundButton_connectSmall.BackColor = Color.IndianRed;
                    roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff2.png");
                    label_boardStatus.Text = "Board status\n" + "Not connected";
                }

                if (protoCubesMonitorForm != null)
                    protoCubesMonitorForm.ConnectStatus = connectStatus;
            }
            else
            {
                MessageBox.Show("Please select an instrument to connect to via the drop-down.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                connectStatus = -1;
                roundButton_connect.BackColor = Color.IndianRed;
                roundButton_connect.ForeColor = Color.White;
                roundButton_connectSmall.BackColor = Color.IndianRed;
                roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff2.png");
                label_boardStatus.Text = "Board status\n" + "Not connected";
            }
        }

        private void roundButton_connect_MouseEnter(object sender, EventArgs e)
        {
            if (connectStatus == -1)
                label_help.Text = "Click on this button to connect the selected board to the computer.";
            else if (connectStatus == 0)
                label_help.Text = "Looks like there is an issue with the connection. Please verify the board is properly plugged in and powered and click again.";
            else if (connectStatus == 1)
                label_help.Text = "Connected successfully! Click again if you wish to disconnect.";
        }

        private void SendTimeToProtoCUBES()
        {
            TimeSpan timeSinceEpoch = DateTime.UtcNow -
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            UInt32 unixTime = Convert.ToUInt32(timeSinceEpoch.TotalSeconds);

            byte[] time = new byte[4];
            time[0] = (byte)((unixTime >> 24) & 0xff);
            time[1] = (byte)((unixTime >> 16) & 0xff);
            time[2] = (byte)((unixTime >> 8) & 0xff);
            time[3] = (byte)((unixTime) & 0xff);

            try
            {
                protoCubes.SendCommand(ProtoCubesSerial.Command.SendTime, time);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send time " +
                    "to Proto-CUBES!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw new Exception();
            }
        }

        private void button_loadFw_Click(object sender, EventArgs e)
        {
            // Connect to the selected device
            string serialNumber = testBoardFtdiDevice[0].SerialNumber.ToString();
            serialNumber = serialNumber.Remove(serialNumber.Length - 1) + "B"; // Ensure to connect to the B port instead of the A.

            ftStatus = myFtdiDevice.OpenBySerialNumber(serialNumber);

            // Start updating progress bar
            progressBar_loadFw.Visible = true;
            progressBar_loadFw.Value = 5;

            // Program Epcs device
            Thread thdEpcsProg = new Thread(new ThreadStart(programEpcsDev));
            thdEpcsProg.IsBackground = true;
            thdEpcsProg.SetApartmentState(ApartmentState.STA);
            thdEpcsProg.Start();
        }

        private void programEpcsDev()
        {
            // Store the embedded rpd file in a byte array
            Stream rpdStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("CitirocUI.Resources.citiroc_v250.rpd");
            byte[] rpdArray = new byte[rpdStream.Length];
            rpdStream.Read(rpdArray, 0, (int)rpdStream.Length);
            rpdStream.Close();

            long rpdFileSize = 0;
            rpdFileSize = rpdArray.Length;

            // Calculate EPCS page numbers
            int rpdPage = 0;
            int rpdBalByte = 0;
            int rpdBytePerPage = 256;
            int intRpdPointer = 0;

            for (int i = 0; i < rpdFileSize; i++) if (rpdArray[i] != 0xFF) intRpdPointer = i;

            rpdPage = intRpdPointer / 256;
            rpdBalByte = intRpdPointer % 256;
            if (rpdBalByte != 0) rpdPage += 3;

            var sw = Stopwatch.StartNew();

            // Erase the current firmware in EPCS device
            int erEpStatus = Firmware.eraseEpcsDev(myFtdiDevice);
            if (erEpStatus == -1)
            {
                MessageBox.Show("An error occured during firmware erasing.");
                return;
            }

            // Set FTDI to Bit-bang mode
            ftStatus = myFtdiDevice.SetLatency(2);
            ftStatus = myFtdiDevice.SetBitMode(0x4f, 0x1); //0x4f -- 01001111
            ftStatus = myFtdiDevice.SetBaudRate(153600);

            // Reset PINs
            uint numByteWr = 0;
            byte[] byteWr = new byte[1];
            byteWr[0] = DEF_VALUE;
            ftStatus = myFtdiDevice.Write(byteWr, 1, ref numByteWr);

            // Set NCS to 0
            string strFtWr = Firmware.strFtDataSetBit(NCS, 0);

            // Send Write enable
            strFtWr += Firmware.strFtDataMsb(AS_WRITE_ENABLE);

            // Set NCS to 1
            strFtWr += Firmware.strFtDataSetBit(NCS, 1);

            uint numFtWr = 0;
            ftStatus = myFtdiDevice.Write(strFtWr, strFtWr.Length, ref numFtWr);

            int intRpdWtPt = 0;
            for (int i = 0; i < rpdPage; i++)
            {
                // Set NCS to 0
                strFtWr = Firmware.strFtDataSetBit(NCS, 0);

                // Send Write enable
                strFtWr += Firmware.strFtDataMsb(AS_WRITE_ENABLE);

                // Set NCS to 1
                strFtWr += Firmware.strFtDataSetBit(NCS, 1);

                // Set NCS to 0
                strFtWr += Firmware.strFtDataSetBit(NCS, 0);

                // Send Write byte
                strFtWr += Firmware.strFtDataMsb(AS_PAGE_PROGRAM);

                // Send Epcs Address
                byte epcsAddress1 = (byte)(((i * 256) & 0xff0000) >> 16);
                byte epcsAddress2 = (byte)(((i * 256) & 0x00ff00) >> 8);
                byte epcsAddress3 = (byte)(((i * 256) & 0x0000ff));
                strFtWr += Firmware.strFtDataEpcs(epcsAddress1) + Firmware.strFtDataEpcs(epcsAddress2) + Firmware.strFtDataEpcs(epcsAddress3);
                numFtWr = 0;
                ftStatus = myFtdiDevice.Write(strFtWr, strFtWr.Length, ref numFtWr);

                strFtWr = "";

                for (int j = 0; j < rpdBytePerPage; j++)
                {
                    byte bytePageWr = rpdArray[intRpdWtPt];
                    strFtWr += Firmware.strFtDataLsb(bytePageWr);
                    intRpdWtPt++;
                }

                // Set NCS to 1
                strFtWr += Firmware.strFtDataSetBit(NCS, 1);
                numFtWr = 0;
                ftStatus = myFtdiDevice.Write(strFtWr, strFtWr.Length, ref numFtWr);

                // Progress Bar update
                UpdatingProBar(i * 95 / rpdPage + 5, progressBar_loadFw);

                // Wait 20 ms
                Thread.Sleep(20);
            }

            //Set NCE to 0
            ftStatus = myFtdiDevice.Write(Firmware.strFtDataSetBit(NCE, 0), 1, ref numFtWr);

            // Set NCONFIG to 1
            ftStatus = myFtdiDevice.Write(Firmware.strFtDataSetBit(NCONFIG, 1), 1, ref numFtWr);

            // Stop timer
            sw.Stop();

            double tsConsumeTime = sw.Elapsed.TotalMilliseconds;
            string strConsumeTime = Math.Round(tsConsumeTime / 1000, 1).ToString();

            UpdatingProBar(0, progressBar_loadFw);

            // Close
            myFtdiDevice.Close();
            if (usbDevId > 0)
            {
                USB.CloseUsbDevice(usbDevId);
                usbDevId = 0;
            }

            roundButton_connect.BackColor = Color.Gainsboro;
            roundButton_connect.ForeColor = Color.Black;
            roundButton_connectSmall.BackColor = Color.Gainsboro;
            roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff.png");

            MessageBox.Show("Firmware successfully loaded in " + strConsumeTime + " seconds. Please unplug your testboard and plug it again to have it working properly.");

            SetVisible(false, button_loadFw);
            SetVisible(false, progressBar_loadFw);
            UpdatingLabel("Board status\n" + "Not connected", label_boardStatus);
            SetVisible(true, label_plug);

            connectStatus = -1;
        }

        private void comboBox_SelectConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedConnectionMode = comboBox_SelectConnection.SelectedIndex;

            if (comboBox_SelectConnection.SelectedIndex == 0)
            {
                groupBox_SerialPortSettings.Visible = false;

                switchBox_acquisitionMode.Visible = true;
                label_numData.Text = "Number of acquisitions:";
                textBox_numData.Text = "100";
                switchBox_acquisitionMode_CheckedChanged(switchBox_acquisitionMode, EventArgs.Empty);

                // Adjust other controls
                label_DataFile.Visible = false;
                button_SelectNumBinsCubes.Visible = false;

                textBox_HgCutHigh.Text = "4095";
                textBox_LgCutHigh.Text = "4095";

                label115.Enabled = true;
                textBox_delay.Enabled = true;
                label112.Enabled = true;
            }
            else if (comboBox_SelectConnection.SelectedIndex == 1)
            {
                // Find COM ports and add them to the combo box, selecting the
                // last COM port in list by default.
                groupBox_SerialPortSettings.Visible = true;
                AddCurrentComPortsToComboBox();
                comboBox_Baudrate.SelectedIndex = comboBox_Baudrate.Items.Count - 1;

                // Use the number of acquisitions box as the individual DAQ timer
                // setting for Proto-CUBES...
                switchBox_acquisitionMode.Visible = false;
                label_numData.Enabled = true;
                label_numData.Text = "Individual DAQ time (s):";
                textBox_numData.Enabled = true;
                textBox_numData.Text = "60";

                AdjustAcquisitionTime();

                // Adjust other controls
                label_DataFile.Visible = true;
                button_SelectNumBinsCubes.Visible = true;

                textBox_HgCutHigh.Text = "2048"; // TODO: Change - this is temporary!
                textBox_LgCutHigh.Text = "2048"; // TODO: Change - this is temporary!

                label115.Enabled = false;
                textBox_delay.Enabled = false;
                label112.Enabled = false;
            }

            /// Select the appropriate tab page in the tab control under "Firmware", enable its
            /// controls and disable the controls of the other.
            Control.ControlCollection controls;
            tabControl_firmware.SelectedIndex = (comboBox_SelectConnection.SelectedIndex + 1) % 2;
            controls = tabControl_firmware.SelectedTab.Controls;
            foreach (Control control in controls)
            {
                control.Enabled = false;
            }

            tabControl_firmware.SelectedIndex = comboBox_SelectConnection.SelectedIndex;
            controls = tabControl_firmware.SelectedTab.Controls;
            foreach (Control control in controls)
            {
                control.Enabled = true;
            }
        }

        private void comboBox_COMPortList_OnClick(object sender, EventArgs e)
        {
            comboBox_COMPortList.Items.Clear();
            AddCurrentComPortsToComboBox();
        }

        private void AddCurrentComPortsToComboBox()
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
                comboBox_COMPortList.Items.Add(port);
            if (comboBox_COMPortList.Items.Count > 0)
            {
                comboBox_COMPortList.SelectedIndex = 0;
            }
        }

        private void btn_OpenProtoCubesMonitor_Click(object sender, EventArgs e)
        {
            if (protoCubesMonitorForm == null)
            {
                protoCubesMonitorForm = new ProtoCubesMonitor(protoCubes);
                protoCubesMonitorForm.ConnectStatus = connectStatus;
                protoCubesMonitorForm.FormClosed += delegate
                {
                    protoCubesMonitorForm = null;
                };
                protoCubesMonitorForm.Show();
            }
            else
            {
                protoCubesMonitorForm.WindowState = FormWindowState.Normal;
                protoCubesMonitorForm.Focus();
            }
        }

        private void ReqBoardID_DataReady(object sender, DataReadyEventArgs e)
        {
            if (e.Command != ProtoCubesSerial.Command.ReqBoardID)
                return;

            string s = "Board status\n" +
                "Proto-CUBES is connected.\n\n" +
                "Firmware and gateware info:\n" +
                System.Text.Encoding.ASCII.GetString(e.DataBytes, 0, 55);
            // TODO: Update the "55" above to the number sent as part of
            //       REQ_BOARD_ID reply...

            UpdatingLabel(s, label_boardStatus);
        }
    }
}
