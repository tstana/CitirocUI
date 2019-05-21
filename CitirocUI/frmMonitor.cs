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
            Citiroc.showMonitor = false;
        }

        public void PublishData(byte[] data, bool Tx)
        {
            AppendByteArray(rtxtMonitor, data, Tx);
        }

        private static void AppendText(RichTextBox box, string text, Color color)
        {

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        private static void AppendByteArray(RichTextBox box, byte[] rdata, bool txData)
        {
            if (txData)
            {
                box.SelectionColor = Color.Yellow;
            }
            else {
                box.SelectionColor = Color.LightGreen;
            }
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.AppendText("\n");
            box.AppendText(BitConverter.ToString(rdata).Replace("-", " " /*string.Empty*/));
            box.SelectionColor = box.ForeColor;
            box.ScrollToCaret();
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            rtxtMonitor.Clear();
        }
    }
}
