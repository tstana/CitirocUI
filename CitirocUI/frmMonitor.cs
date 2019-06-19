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

        public void textBox_time(string a)
        {
            textBox_Time.Text = a;
        }

        public void textBox_temp (string a)
        {
            textBox_Temperature.Text = a;
        }

        public void textBox_ch0(string a)
        {
            textBox_Ch0Counts.Text = a;
        }

        public void textBox_ch16(string a)
        {
            textBox_Ch16Counts.Text = a;
        }

        public void textBox_ch31(string a)
        {
            textBox_Ch31Counts.Text = a;
        }

        public void textBox_ch21(string a)
        {
            textBox_Ch21Counts.Text = a;
        }

        public void textBox_current(string a)
        {
            textBox2.Text = a;
        }

        public void textBox_voltage(string a)
        {
            textBox1.Text = a;
        }

    }
}
