using System;
using System.Drawing;
using System.Windows.Forms;

namespace CitirocUI
{
    public partial class frmMonitor : Form
    {
        public static int x;
        public static int vert;


        public frmMonitor()
        {
            InitializeComponent();
        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            this.Left = x;
            this.Height = vert;
        }

        private void frmMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Citiroc.showMonitor = false;
        }

        public void PublishData(byte[] data, bool Tx)
        {
            Color actColor = Color.Green;
            if (Tx)
            {
                actColor = Color.Yellow;
            }

            AppendByteArray(rtxtMonitor, data, actColor);
        }

        private static void AppendText(RichTextBox box, string text, Color color)
        {

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        private static void AppendByteArray(RichTextBox box, byte[] rdata, Color color)
        {

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.AppendText("\n");
            box.SelectionColor = color;
            box.AppendText(BitConverter.ToString(rdata).Replace("-", " " /*string.Empty*/));
            box.SelectionColor = box.ForeColor;
            box.ScrollToCaret();
        }
    }
}
