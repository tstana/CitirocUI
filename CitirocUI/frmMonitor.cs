using System;
using System.Drawing;
using System.Windows.Forms;

namespace CitirocUI
{
    public partial class frmMonitor : Form
    {

        #region Constructor and Load
        public frmMonitor(ProtoCubesSerial c)
        {
            commChannel = c;
            commChannel.DataReadyEvent += commChannel_DataReady;
            InitializeComponent();
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            label_ConnStatus.Text = "Not connected.";
        }

        private void frmMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            commChannel.DataReadyEvent -= commChannel_DataReady;
        }
        #endregion

        #region Members
        ProtoCubesSerial commChannel;
        #endregion

        #region Properties
        public string ConnStatusLabel
        {
            set { label_ConnStatus.Text = value; }
        }
        #endregion

        #region Methods
        private void PublishData(byte[] data, bool Tx)
        {
            AppendByteArray(rtxtMonitor, data, Tx);
        }

        public void SetPosition(Point position, int f_height)
        {
            this.Height = f_height;
            this.Top = position.Y;
            this.Left = position.X;

            this.Refresh();
        }

        private static void AppendByteArray(RichTextBox box, byte[] rdata, bool txData)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.AppendText("\n");

            if (txData)
            {
                box.SelectionColor = Color.Yellow;
                box.AppendText(BitConverter.ToString(rdata).Replace("-", " " /*string.Empty*/));
            }
            else {
                box.SelectionColor = Color.LightGreen;
                box.AppendText(System.Text.Encoding.UTF8.GetString(rdata, 0, rdata.Length));
            }
            
            box.SelectionColor = box.ForeColor;
            box.ScrollToCaret();
        }
        #endregion

        #region Events
        private void button_Clear_Click(object sender, EventArgs e)
        {
            rtxtMonitor.Clear();
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string helpString =
                "TX data is colored yellow\r\n" +
                "RX data is colored green\r\n" +
                "\r\n" +
                "Press \"Clear\" to clear the window";
            MessageBox.Show(helpString, "Help");
        }

        private void commChannel_DataReady(object sender, DataReadyEventArgs e)
        {
            // 1. Handle the simple ones: the hit counts...
            UInt32 timestamp = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(e.DataBytes, 11, 10));
            byte[] ch0_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 23));
            byte[] ch16_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 27));
            byte[] ch31_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 31));
            byte[] ch21_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 35));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ch0_hit_count);
                Array.Reverse(ch16_hit_count);
                Array.Reverse(ch31_hit_count);
                Array.Reverse(ch21_hit_count);
            }
            UInt32 hitCountMPPC3 = BitConverter.ToUInt32(ch0_hit_count, 0);
            UInt32 hitCountMPPC2 = BitConverter.ToUInt32(ch16_hit_count, 0);
            UInt32 hitCountMPPC1 = BitConverter.ToUInt32(ch31_hit_count, 0);
            UInt32 hitCountOR32 = BitConverter.ToUInt32(ch21_hit_count, 0);

            // 2. Now for the HVPS stuff... It is presented as ASCII characters
            // by the HVPS, placed at particular offsets in the HK data stream.
            // These characters need to be converted to a string, which then needs
            // to be converted to a double representation before the conversion
            // formula in the datasheet can be applied.
            string s;

            byte[] hvps_voltage = new byte[4];
            Array.Copy(e.DataBytes, 39, hvps_voltage, 0, 4);
            s = System.Text.Encoding.ASCII.GetString(hvps_voltage);
            double voltageFromHVPS = Convert.ToDouble(Convert.ToUInt16(s, 16));
            voltageFromHVPS *= 1.812 * Math.Pow(10, -3);

            byte[] hvps_current = new byte[4];
            Array.Copy(e.DataBytes, 43, hvps_current, 0, 4);
            s = System.Text.Encoding.ASCII.GetString(hvps_current);
            double currentFromHVPS = Convert.ToDouble(Convert.ToUInt16(s, 16));
            currentFromHVPS *= 5.194 * Math.Pow(10, -3);

            byte[] hvps_temp = new byte[4];
            Array.Copy(e.DataBytes, 47, hvps_temp, 0, 4);
            s = System.Text.Encoding.ASCII.GetString(hvps_temp);
            double tempFromHVPS = Convert.ToDouble(Convert.ToUInt16(s, 16));
            tempFromHVPS = (tempFromHVPS * 1.907 * Math.Pow(10, -5) - 1.035) /
                           (-5.5 * Math.Pow(10, -3));

            // 3. Apply the values into the text boxes; use the Control.Invoke()
            //    method, to make sure the writing is done inside the original
            //    UI thread
            textBox_timestamp.Invoke(new EventHandler(
                delegate
                {
                    textBox_timestamp.Text = timestamp.ToString();
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
        }
        #endregion
    }
}
