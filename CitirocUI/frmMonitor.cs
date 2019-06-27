using System;
using System.Drawing;
using System.Windows.Forms;

namespace CitirocUI
{
    public partial class frmMonitor : Form
    {
        public string ConnStatusLabel
        {
            set { label_ConnStatus.Text = value; }
        }

        public frmMonitor()
        {
            InitializeComponent();
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            label_ConnStatus.Text = "Not connected.";
        }

        private void frmMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        public void PublishData(byte[] data, bool Tx)
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

        public string textBox_time
        {
            set { textBox_Time.Text = value; }
            get { return textBox_Time.Text; }
        }

        public string textBox_temp
        {
            set { textBox_Temperature.Text = value; }
            get { return textBox_Temperature.Text; }
        }

        public string textBox_ch0
        {
            set { textBox_Ch0Counts.Text = value; }
            get { return textBox_Ch0Counts.Text; }
        }

        public string textBox_ch16
        {
            set { textBox_Ch16Counts.Text = value; }
            get { return textBox_Ch16Counts.Text; }
        }

        public string textBox_ch31
        {
            set { textBox_Ch31Counts.Text = value; }
            get { return textBox_Ch31Counts.Text; }
        }

        public string textBox_ch21
        {
            set { textBox_Ch21Counts.Text = value; }
            get { return textBox_Ch21Counts.Text; }
        }

        public string textBox_current
        {
            set { textBox2.Text = value; }
            get { return textBox2.Text; }
        }

        public string textBox_voltage
        {
            set { textBox1.Text = value; }
            get { return textBox1.Text; }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            byte[] _hkDataArray = new byte[64];
            byte[] time = System.Text.Encoding.ASCII.GetBytes("Unix time: 1560871356\r\n");
            Array.Copy(time, _hkDataArray, time.Length);
                
            byte[] me = {
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x02, 0x05, 0x36, 0x7B,
                0x30, 0x35, 0x36, 0x38,
                0x30, 0x30, 0x35, 0x43,
                0x42, 0x38, 0x36, 0x39
            };

            Array.Copy(me, 0, _hkDataArray, time.Length, me.Length);

            byte[] ch0_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 23));
            byte[] ch16_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 27));
            byte[] ch31_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 31));
            byte[] ch21_hit_rate = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 35));
            byte[] hvps_temp = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 39));
            byte[] hvps_voltage = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 43));
            byte[] hvps_current = BitConverter.GetBytes(BitConverter.ToUInt32(_hkDataArray, 47));
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

            this.textBox_ch0 = Convert.ToString(BitConverter.ToUInt32(ch0_hit_rate, 0));
            this.textBox_ch16 = Convert.ToString(BitConverter.ToUInt32(ch16_hit_rate, 0));
            this.textBox_ch21 = Convert.ToString(BitConverter.ToUInt32(ch31_hit_rate, 0));
            this.textBox_ch31 = Convert.ToString(BitConverter.ToUInt32(ch21_hit_rate, 0));
            this.textBox_temp = Convert.ToString(BitConverter.ToUInt32(hvps_temp, 0));
            this.textBox_voltage = Convert.ToString(BitConverter.ToUInt32(hvps_voltage, 0));
            this.textBox_current = Convert.ToString(BitConverter.ToUInt32(hvps_current, 0));
        }
    }
}
