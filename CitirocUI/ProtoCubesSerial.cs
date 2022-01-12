using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CitirocUI
{
    public class DataReadyEventArgs : EventArgs
    {
        #region Constructor
        public DataReadyEventArgs(ProtoCubesSerial.Command cmd, byte[] dataBytes)
        {
            _cmd = cmd;
            _dataBytes = dataBytes;
        }
        #endregion

        #region Members
        ProtoCubesSerial.Command _cmd;
        byte[] _dataBytes;
        #endregion

        #region Properties
        public ProtoCubesSerial.Command Command
        {
            // TODO: This property is not currently used -- implement its usage!
            get { return _cmd; }
            set { _cmd = value; }
        }
        public byte[] DataBytes
        {
            get { return _dataBytes; }
            set { _dataBytes = value; }
        }
        #endregion
    }

    public class ProtoCubesSerial
    {
        #region Enums
        /// <summary>
        /// enumeration to hold our transmission types
        /// </summary>
        public enum TransmissionType { Text, Hex }

        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };

        /// <summary>
        /// Enum to hold Proto-CUBES commands
        /// </summary>
        public enum Command
        {
            None                = '-',

            SendCitirocConf     = 'C',
            SendProbeConf       = 'P',
            SendHVPSConf        = 'H',
            SendHVPSTmpVolt     = 'V',
            SendDAQConf         = 'D',
            SendReadReg         = 'R',
            SendGatewareConf    = 'G',
            SendCalibPulseConf  = 'A',

            SendTime            = 'Z',

            DAQStart            = 'S',
            DAQStop             = 'T',
            DelFiles            = 'Q',

            ReqHK               = 'h',
            ReqStatus           = 's',
            ReqPayload          = 'p',
            ReqBoardID          = 'i'
        }
        #endregion

        #region Variables
        //property variables
        private int _baudRate = 0;
        private Parity _parity = 0;
        private StopBits _stopBits = 0;
        private int _dataBits = 8;
        private string _portName = string.Empty;
        private Handshake _handshake = 0;
        private int _writetimeout = 0;
        private int _readtimeout = 0;
        private bool _rtsenable = false;
        private string _info;
        private TransmissionType _transType;
        private RichTextBox _displayWindow;

        private bool _monvisible = false;

        private byte[] commandReplyBuffer = new byte[25000];
        private int commandReplyDataLen = 0;

        private int commandReplyBytesRead = 0;

         //global variables
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private SerialPort comPort = new SerialPort();

        private Command lastSentCommand = Command.None;
        #endregion

        #region Events
        public event EventHandler<DataReadyEventArgs> DataReadyEvent;
        #endregion

        #region Properties
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public StopBits StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public int DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        public Handshake Handshake
        {
            get { return _handshake; }
            set { _handshake = value; }
        }

        public bool RtsEnable
        {
            get { return _rtsenable; }
            set { _rtsenable = value; }
        }

        public int ReadTimeout
        {
            get { return _readtimeout; }
            set { _readtimeout = value; }
        }
        public int WriteTimeout
        {
            get { return _writetimeout; }
            set { _writetimeout = value; }
        }

        public string info
        {
            get { return _info; }
            set { _info = value; }
        }

        public bool MonitorActive
        {
            get { return _monvisible; }
            set { _monvisible = value; }
        }

        public int[] NumBins { get; set; } = new int[6];

        /// <summary>
        /// property to hold our TransmissionType
        /// of our manager class
        /// </summary>
        public TransmissionType CurrentTransmissionType
        {
            get { return _transType; }
            set { _transType = value; }
        }

        /// <summary>
        /// property to hold our display window
        /// value
        /// </summary>
        public RichTextBox DisplayWindow
        {
            get { return _displayWindow; }
            set { _displayWindow = value; }
        }

        public Command LastSentCommand
        {
            get { return lastSentCommand; }
            set { lastSentCommand = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor to set the properties of our Manager Class
        /// </summary>
        /// <param name="baud">Desired BaudRate</param>
        /// <param name="par">Desired Parity</param>
        /// <param name="sBits">Desired StopBits</param>
        /// <param name="dBits">Desired DataBits</param>
        /// <param name="name">Desired PortName</param>
        public ProtoCubesSerial(int baud, Parity par, StopBits sBits, int dBits, string name, RichTextBox rtb)
        {
            _baudRate = baud;
            _parity = par;
            _stopBits = sBits;
            _dataBits = dBits;
            _portName = name;
            _displayWindow = rtb;

            //now add an event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        /// <summary>
        /// Comstructor to set the properties of our
        /// serial port communicator to nothing
        /// </summary>
        public ProtoCubesSerial()
        {
            _baudRate = 115200;
            _parity = Parity.None;
            _stopBits = StopBits.One;
            _dataBits = 8;
            _portName = "COM1";
            _displayWindow = null;
            _monvisible = false;

            //add event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }
        #endregion

        #region Send Data
        public void SendData(byte[] msg, int msg_length)
        {
            if (comPort.IsOpen == false)
                return;

            try
            {
                comPort.Write(msg, 0, msg_length);
                
                if(_monvisible)
                    DisplayBData(msg);
            }
            catch (FormatException ex)
            {
                //display error message
                if(_monvisible)
                    DisplayData(System.Text.Encoding.UTF8.GetBytes(ex.ToString()));
            }
            finally
            {
             //   _displayWindow.SelectAll();
            }
        }

        public void SendCommand(ProtoCubesSerial.Command cmd, byte[] cmdParam)
        {
            byte[] cmdBytes = new byte[1];

            /* Check that we're getting the right number of bytes as param. */
            switch (cmd) {
                case Command.None:
                    throw new ArgumentException("Cannot send Command.None!");

                case Command.DAQStart:
                case Command.DAQStop:
                case Command.ReqBoardID:
                case Command.ReqHK:
                case Command.ReqStatus:
                case Command.ReqPayload:
                case Command.DelFiles:
                    if (cmdParam != null)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' does not accept any arguments.");
                    }
                    break;

                case Command.SendCitirocConf:
                    if (cmdParam.Length != 143)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 143 bytes as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[144];
                    break;

                case Command.SendProbeConf:
                    if (cmdParam.Length != 32)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 32 bytes as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[33];
                    break;

                case Command.SendDAQConf:
                    if (cmdParam.Length != 7)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 7 bytes as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[8];
                    break;

                case Command.SendReadReg:
                case Command.SendGatewareConf:
                    if (cmdParam.Length != 1)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 1 byte as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[2];
                    break;

                case Command.SendCalibPulseConf:
                    if (cmdParam.Length != 4)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 4 bytes as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[5];
                    break;

                case Command.SendTime:
                    if (cmdParam.Length != 4)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 4 bytes as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[5];
                    break;

                case Command.SendHVPSTmpVolt:
                    if (cmdParam.Length != 3)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 2 bytes as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[4];
                    break;

                case Command.SendHVPSConf:
                    if (cmdParam.Length != 13)
                    {
                        throw new ArgumentException("Command '" +
                            (char)cmd + "' takes in 13 bytes as parameter; " +
                            "the parameter array consists of " +
                            cmdParam.Length.ToString() + " bytes instead!");
                    }
                    cmdBytes = new byte[14];
                    break;

                default:
                    throw new ArgumentException("Unknown command '" +
                        (char)cmd + "' received.");
            }

            lastSentCommand = cmd;

            /*
             * Expected number of bytes:
             *      ID:
             *          Unix time: 0123456789\r\n (23 bytes)
             *          Data (30 bytes)
             *          \r\n (2 bytes)
             *          ---
             *          Total: 55
             *      Status:
             *          Unix time: 0123456789\r\n (23 bytes)
             *          Data (1 byte)
             *          \r\n (2 bytes)
             *          ---
             *          Total: 26
             *      HK:
             *          Unix time: 0123456789\r\n (23 bytes)
             *          Data (46 bytes)
             *          \r\n (2 bytes)
             *          ---
             *          Total: 71
             *      DAQ:
             *          Unix time: 0123456789\r\n (23 bytes)
             *          Histo header (256 bytes)
             *          Bins data (variable)
             *          \r\n
             *          ---
             *          Total: calculated below
             */
            switch (lastSentCommand)
            {
                case Command.ReqBoardID:
                    commandReplyDataLen = 55;
                    break;
                case Command.ReqHK:
                    commandReplyDataLen = 71;
                    break;
                case Command.ReqStatus:
                    commandReplyDataLen = 26;
                    break;
                case Command.ReqPayload:
                    commandReplyDataLen = 23 + 256; // Unix time + histo. header
                    for (int i = 0; i < 6; i++)
                        commandReplyDataLen += 2 * NumBins[i]; // 2 bytes / bin
                    commandReplyDataLen += 2; // "\r\n"
                    break;
                default:
                    commandReplyDataLen = 0;
                    break;
            }

            /* Prep the array, send it and set the currently executing cmd. */
            cmdBytes[0] = Convert.ToByte(cmd);
            if (cmdParam != null)
                Array.Copy(cmdParam, 0, cmdBytes, 1, cmdParam.Length);

            SendData(cmdBytes, cmdBytes.Length);
        }
        #endregion

        #region DisplayData
        [STAThread]
        private void DisplayData(byte[] msg)
        {
            _displayWindow.Invoke(new EventHandler(delegate
            {
                _displayWindow.SelectionStart = _displayWindow.TextLength;
                _displayWindow.SelectionLength = 0;
                _displayWindow.AppendText("\n");
                _displayWindow.SelectionColor = Color.LightGreen;
                _displayWindow.AppendText(System.Text.Encoding.UTF8.GetString(msg, 0, msg.Length));
                _displayWindow.SelectionColor = _displayWindow.ForeColor;
                _displayWindow.ScrollToCaret();
            }));
        }

        [STAThread]
        private void DisplayBData(byte[] msg)
        {
            _displayWindow.Invoke(new EventHandler(delegate
            {
                //_displayWindow.SelectedText = string.Empty;
                //_displayWindow.SelectionFont = new Font(_displayWindow.SelectionFont, FontStyle.Bold);
                _displayWindow.SelectionStart = _displayWindow.TextLength;
                _displayWindow.SelectionLength = 0;
                _displayWindow.AppendText("\n");
                _displayWindow.SelectionColor = Color.Yellow;
                _displayWindow.AppendText(BitConverter.ToString(msg).Replace("-", " " /*string.Empty*/));
                _displayWindow.SelectionColor = _displayWindow.ForeColor;
                _displayWindow.ScrollToCaret();
            }));
        }

         #endregion

        #region Open and Close Port
        public bool OpenPort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (comPort.IsOpen == true) comPort.Close();

                //set the properties of our SerialPort Object
                comPort.BaudRate = _baudRate;    //BaudRate
                comPort.DataBits = _dataBits;    //DataBits
                comPort.StopBits = _stopBits;    //StopBits
                comPort.Parity =  _parity;    //Parity
                comPort.PortName = _portName;   //PortName

                comPort.RtsEnable = _rtsenable;
                comPort.WriteTimeout = _writetimeout;
                comPort.ReadTimeout = _readtimeout;
                //now open the port
                comPort.Open();

                _info = "Connected / " + _portName + " / " + _baudRate.ToString();
                return true;
            }
            catch (Exception ex)
            {
                //_info = "Error opening serial port!" + ex.Message;
                MessageBox.Show("Error: " + ex.Message, "Error opening serial port!", MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
        }

        public bool ClosePort()
        {
            comPort.Close();
            return true;
        }
        #endregion

        #region SetParityValues
        public void SetParityValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

        #region SetStopBitValues
        public void SetStopBitValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

        #region SetPortNameValues
        public void SetPortNameValues(object obj)
        {

            foreach (string str in SerialPort.GetPortNames())
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int numBytesRead = comPort.BytesToRead;
            byte[] dataBytes = new byte[numBytesRead];
            comPort.Read(dataBytes, 0, numBytesRead);
            
            if(_monvisible)
                DisplayData(dataBytes);

            /*
             * Buffer received data as it arrives until the expected number of
             * bytes have been received
             */
            try
            {
                dataBytes.CopyTo(commandReplyBuffer, commandReplyBytesRead);
                commandReplyBytesRead += numBytesRead;
                if (commandReplyBytesRead >= commandReplyDataLen)
                {
                    DataReadyEvent(this,
                        new DataReadyEventArgs(lastSentCommand,
                            commandReplyBuffer));
                    commandReplyBytesRead = 0;
                }
            }
            catch (ArgumentException)
            {
                string s = "Attempting to write too many bytes to " +
                    "command reply buffer:" +
                    Environment.NewLine +
                    "commandReplyBytesRead = " + commandReplyBytesRead +
                    Environment.NewLine +
                    "dataB.Length = " + dataBytes.Length +
                    Environment.NewLine +
                    Environment.NewLine;
                MessageBox.Show(s, "Exception", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                commandReplyBytesRead = 0;
            }
            catch (Exception excep)
            {
                string s = "Exception in comPort_DataReceived():" +
                    Environment.NewLine +
                    excep.Message +
                    Environment.NewLine +
                    Environment.NewLine;
                MessageBox.Show(s, "Exception", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                commandReplyBytesRead = 0;
            }

        }
        #endregion
    }
}
