using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CitirocUI
{
    class DataReadyEventArgs : EventArgs
    {
        #region Members
        private UInt32 _telemetryTimestamp;
        private UInt32 _hitCountMPPC3;
        private UInt32 _hitCountMPPC2;
        private UInt32 _hitCountMPPC1;
        private UInt32 _hitCountOR32;
        private UInt32 _voltageFromHVPS;
        private UInt32 _currentFromHVPS;
        private UInt32 _tempFromHVPS;
        #endregion

        #region Properties
        public UInt32 TelemetryTimestamp;
        public UInt32 HitCountMPPC3;
        public UInt32 HitCountMPPC2;
        public UInt32 HitCountMPPC1;
        public UInt32 HitCountOR32;
        public UInt32 VoltageFromHVPS;
        public UInt32 CurrentFromHVPS;
        public UInt32 TempFromHVPS;
        #endregion

        #region Constructor
        public DataReadyEventArgs()
        {
            _telemetryTimestamp = 0;
            _hitCountMPPC1 = 0;
            _hitCountMPPC2 = 0;
            _hitCountMPPC3 = 0;
            _hitCountOR32 = 0;
            _voltageFromHVPS = 0;
            _currentFromHVPS = 0;
            _tempFromHVPS = 0;
        }
        #endregion
    }

    class ProtoCubesSerial
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

        private byte[] _comBuffer;

        private byte[] _daqDataArray = new byte[25000];
        private byte[] _hkDataArray = new byte[53];

        private bool _retrievingDaqData = false;
        private int _numBytesRetrieved = 0;
        private string _daqDataFileName = "CUBESfile.dat";

         //global variables
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private SerialPort comPort = new SerialPort();
        #endregion

        #region Events
        public event EventHandler DataReadyEvent;

        protected virtual void OnDataReadyEvent(DataReadyEventArgs e)
        {
            EventHandler<DataReadyEventArgs> handler = DataReadyEvent;

            if (handler != null)
            {
                e.HitCountMPPC1 = _hitCountMPPC1;
                e.HitCountMPPC2 = _hitCountMPPC2;
                e.HitCountMPPC3 = _hitCountMPPC3;
            }
        }

        private UInt32 _telemetryTimestamp;
        private UInt32 _hitCountMPPC3;
        private UInt32 _hitCountMPPC2;
        private UInt32 _hitCountMPPC1;
        private UInt32 _hitCountOR32;
        private UInt32 _voltageFromHVPS;
        private UInt32 _currentFromHVPS;
        private UInt32 _tempFromHVPS;
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

        public byte[] ComBuffer
        {
            get { return _comBuffer; }
            set { /* Read-only buffer */ }
        }

        public bool RetrievingDaqData
        {
            get { return _retrievingDaqData; }
            set { _retrievingDaqData = value;  }
        }

        public string DataFileName
        {
            get { return _daqDataFileName; }
            set { _daqDataFileName = value; }
        }

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

            _comBuffer = new byte[comPort.ReadBufferSize];
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

            _comBuffer = new byte[comPort.ReadBufferSize];
            //add event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }
        #endregion

        #region WriteData
        public void WriteData(byte[] msg, int msg_length)
        {
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

        #region OpenPort
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
        #endregion

        public bool ClosePort()
        {
            comPort.Close();
            return true;
        }

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

            if (_retrievingDaqData)
            {
                /*
                    * Store DAQ data to a byte array when it arrives. When the
                    * number of bytes expected as part of one DAQ run has been
                    * received, store it to the file selected by the user in
                    * the UI.
                    */
                try
                {
                    dataBytes.CopyTo(_daqDataArray, _numBytesRetrieved);
                    _numBytesRetrieved += numBytesRead;
                    if (_numBytesRetrieved == _daqDataArray.Length)      // TODO: Replace me with "end-of-DAQ-data" marker...
                    {
                        using (BinaryWriter dataFile = new BinaryWriter(File.Open(_daqDataFileName, FileMode.Create)))
                        {
                            dataFile.Write(_daqDataArray);
                        }
                        _retrievingDaqData = false;
                        _numBytesRetrieved = 0;
                    }
                }
                catch
                {
                    MessageBox.Show("Attempting to write too many bytes to _daqDataArray:\n" +
                        "_numBytesRetrieved = " + _numBytesRetrieved + "\n" +
                        "dataB.Length = " + dataBytes.Length + "\n", "Exception");
                    _retrievingDaqData = false;
                    _numBytesRetrieved = 0;
                }
            }

            else
            {
                try
                {
                    dataBytes.CopyTo(_hkDataArray, _numBytesRetrieved);
                    _numBytesRetrieved += numBytesRead;
                    if (_numBytesRetrieved == _hkDataArray.Length)      // TODO: Replace me with "end-of-DAQ-data" marker...
                    {
                        Form f = Application.OpenForms["frmMonitor"];
                        if (f != null)
                        {
                            frmMonitor fm = (frmMonitor)f;
                            UInt32 timestamp = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(_hkDataArray, 11, 10));

                            byte[] ch0_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 23));
                            byte[] ch16_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 27));
                            byte[] ch31_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 31));
                            byte[] ch21_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 35));
                            byte[] hvps_voltage = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 39));
                            byte[] hvps_current = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 43));
                            byte[] hvps_temp = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 47));
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(ch0_hit_rate);
                                Array.Reverse(ch16_hit_rate);
                                Array.Reverse(ch31_hit_rate);
                                Array.Reverse(ch21_hit_rate);
                                Array.Reverse(hvps_temp);
                                Array.Reverse(hvps_voltage);
                                Array.Reverse(hvps_current);
                            }

                            _telemetryTimestamp = timestamp;
                            _hitCountMPPC3 = BitConverter.ToUInt32(ch0_hit_rate, 0);
                            _hitCountMPPC2 = BitConverter.ToUInt32(ch16_hit_rate, 0);
                            _hitCountMPPC1 = BitConverter.ToUInt32(ch31_hit_rate, 0);
                            _hitCountOR32 = BitConverter.ToUInt32(ch21_hit_rate, 0);
                            _voltageFromHVPS = BitConverter.ToUInt32(hvps_voltage, 0);
                            _currentFromHVPS = BitConverter.ToUInt32(hvps_current, 0);
                            _tempFromHVPS = BitConverter.ToUInt32(hvps_temp, 0);

                            _numBytesRetrieved = 0;
                        }
                    }
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Attempting to write too many bytes to _hkDataArray:\n" +
                        "_numBytesRetrieved = " + _numBytesRetrieved + "\n" +
                        "dataB.Length = " + dataBytes.Length + "\n", "Exception");
                    _numBytesRetrieved = 0;
                }
                catch (Exception excep)
                {
                    MessageBox.Show(excep.Message, "Error");
                    _numBytesRetrieved = 0;
                }
            }
        }
        #endregion
    }
}
