using System;
using System.Drawing;
using System.Windows.Forms;

namespace CitirocUI
{
    public partial class frmMonitor : Form
    {

        #region "Constructor and Load"
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
        #endregion

        #region Properties
        public string ConnStatusLabel
        {
            set { label_ConnStatus.Text = value; }
        }

        public UInt32 hitCountMPPC1
        {
            set { textBox_hitCountMPPC1.Text = value.ToString(); }
        }

        public UInt32 hitCountMPPC2
        {
            set { textBox_hitCountMPPC2.Text = value.ToString(); }
        }

        public UInt32 hitCountMPPC3
        {
            set { textBox_hitCountMPPC3.Text = value.ToString(); }
        }

        public UInt32 hitCountOR32
        {
            set { textBox_hitCountOR32.Text = value.ToString(); }
        }

        public UInt32 voltageFromHVPS
        {
            set { textBox_voltageFromHVPS.Text = value.ToString(); }
        }

        public UInt32 currentFromHVPS
        {
            set { textBox_currentFromHVPS.Text = value.ToString(); }
        }

        public UInt32 tempFromHVPS
        {
            set { textBox_tempFromHVPS.Text = value.ToString(); }
        }
        #endregion

        #region "Methods"
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
        #endregion
    }
}
