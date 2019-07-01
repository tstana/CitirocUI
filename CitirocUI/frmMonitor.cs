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

        private UInt32 TelemetryTimestamp
        {
            set
            {
                textBox_timestamp.Invoke( new EventHandler(
                    delegate
                    {
                        textBox_timestamp.Text = value.ToString();
                    }
                    ));
            }
        }

        private UInt32 HitCountMPPC1
        {
            set
            {
                textBox_hitCountMPPC1.Invoke(new EventHandler(
                    delegate
                    {
                        textBox_hitCountMPPC1.Text = value.ToString();
                    }
                    ));
            }
        }

        private UInt32 HitCountMPPC2
        {
            set
            {
                textBox_hitCountMPPC2.Invoke(new EventHandler(
                    delegate
                    {
                        textBox_hitCountMPPC2.Text = value.ToString();
                    }
                    ));
            }
        }

        private UInt32 HitCountMPPC3
        {
            set
            {
                textBox_hitCountMPPC3.Invoke(new EventHandler(
                    delegate
                    {
                        textBox_hitCountMPPC3.Text = value.ToString();
                    }
                    ));
            }
        }

        private UInt32 HitCountOR32
        {
            set
            {
                textBox_hitCountOR32.Invoke(new EventHandler(
                    delegate
                    {
                        textBox_hitCountOR32.Text = value.ToString();
                    }
                    ));
            }
        }

        private UInt32 VoltageFromHVPS
        {
            set
            {
                textBox_voltageFromHVPS.Invoke(new EventHandler(
                    delegate
                    {
                        textBox_voltageFromHVPS.Text = value.ToString();
                    }
                    ));
            }
        }

        private UInt32 CurrentFromHVPS
        {
            set
            {
                textBox_currentFromHVPS.Invoke(new EventHandler(
                    delegate
                    {
                        textBox_currentFromHVPS.Text = value.ToString();
                    }
                    ));
            }
        }

        private UInt32 TempFromHVPS
        {
            set
            {
                textBox_tempFromHVPS.Invoke(new EventHandler(
                    delegate
                    {
                        textBox_tempFromHVPS.Text = value.ToString();
                    }
                    ));
            }
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
            UInt32 timestamp = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(e.DataBytes, 11, 10));
            byte[] ch0_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 23));
            byte[] ch16_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 27));
            byte[] ch31_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 31));
            byte[] ch21_hit_count = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 35));
            byte[] hvps_voltage = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 39));
            byte[] hvps_current = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 43));
            byte[] hvps_temp = BitConverter.GetBytes(BitConverter.ToUInt32(e.DataBytes, 47));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ch0_hit_count);
                Array.Reverse(ch16_hit_count);
                Array.Reverse(ch31_hit_count);
                Array.Reverse(ch21_hit_count);
                Array.Reverse(hvps_temp);
                Array.Reverse(hvps_voltage);
                Array.Reverse(hvps_current);
            }

            TelemetryTimestamp = timestamp;
            HitCountMPPC3 = BitConverter.ToUInt32(ch0_hit_count, 0);
            HitCountMPPC2 = BitConverter.ToUInt32(ch16_hit_count, 0);
            HitCountMPPC1 = BitConverter.ToUInt32(ch31_hit_count, 0);
            HitCountOR32 = BitConverter.ToUInt32( ch21_hit_count, 0);
            VoltageFromHVPS = BitConverter.ToUInt32(hvps_voltage, 0);
            CurrentFromHVPS = BitConverter.ToUInt32(hvps_current, 0);
            TempFromHVPS = BitConverter.ToUInt32(hvps_temp, 0);
        }
        #endregion
    }
}
