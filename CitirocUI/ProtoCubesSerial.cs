using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CitirocUI
{
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

        private bool _retrievingDaqData = false;
        private bool _storingDaqData = false;
        private int _numDaqBytesRetrieved = 0;
        private string _daqDataFileName = "CUBESfile.dat";

         //global manager variables
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private SerialPort comPort = new SerialPort();
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
                _info = "Error opening serial port!" + ex.Message;
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
                if (!_storingDaqData)
                {
                    /*
                     * Look for "Unix time" string in received data and
                     * start storing DAQ data when it has been obtained.
                     */
                    Array.Copy(dataBytes, 0, _comBuffer, _numDaqBytesRetrieved, numBytesRead);
                    _numDaqBytesRetrieved += numBytesRead;
                    var tmpStr = System.Text.Encoding.Default.GetString(_comBuffer);

                    if (tmpStr.Contains("Unix time"))
                    {
                        /* Start storing data from index of "Unix time" */
                        int startIndex = tmpStr.IndexOf("Unix time");
                        Array.Copy(_comBuffer, startIndex, _daqDataArray, 0, _numDaqBytesRetrieved - startIndex);
                        Array.Clear(_comBuffer, 0, _comBuffer.Length);
                        _numDaqBytesRetrieved -= startIndex;
                        _storingDaqData = true;
                    }
                }
                else
                {
                    /*
                     * Store DAQ data when it arrives; stop storing when
                     * the number of bytes expected as part of one DAQ
                     * run has been received...
                     */
                    if (_numDaqBytesRetrieved < _daqDataArray.Length)      // TODO: Replace me with "end-of-DAQ-data" marker...
                    {
                        using (BinaryWriter dataFile = new BinaryWriter(File.Open(_daqDataFileName, FileMode.Create)))
                        {
                            dataFile.Write(_daqDataArray);
                        }
                        _retrievingDaqData = false;
                        _storingDaqData = false;
                        _numDaqBytesRetrieved = 0;
                    }
                    /* ... and write data to file */
                    else
                    {
                        try
                        {
                            dataBytes.CopyTo(_daqDataArray, _numDaqBytesRetrieved);
                            _numDaqBytesRetrieved += numBytesRead;
                        }
                        catch
                        {
                            MessageBox.Show("Attempting to write too many bytes to _daqDataArray:\n" +
                                "_numDaqBytesRetrieved = " + _numDaqBytesRetrieved + "\n" +
                                "dataB.Length = " + dataBytes.Length + "\n", "Exception");
                            _retrievingDaqData = false;
                            _storingDaqData = false;
                            _numDaqBytesRetrieved = 0;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
