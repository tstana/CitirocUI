using System;
using System.Drawing;
using System.Windows.Forms;
using FTD2XX_NET;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Linq;

namespace CitirocUI
{
    public partial class Form_ftdiDevices : Form
    {
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        FontFamily ffBryant = FontFamily.GenericSansSerif;
        private PrivateFontCollection pfcBryant = new PrivateFontCollection();

        FTDI.FT_DEVICE ftdiDevice;
        uint ftdiDeviceCount;
        FTDI.FT_STATUS ftStatus;
        FTDI myFtdiDevice;
        FTDI.FT_DEVICE_INFO_NODE[] ftList;

        public Form_ftdiDevices(FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList)
        {
            InitializeComponent();

            ftList = ftdiDeviceList;

            try
            {
                byte[] fontData = Properties.Resources.Bryant_RegularCompressed;
                IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                uint dummy = 0;
                AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.Bryant_RegularCompressed.Length, IntPtr.Zero, ref dummy);
                pfcBryant.AddMemoryFont(fontPtr, Properties.Resources.Bryant_RegularCompressed.Length);
                Marshal.FreeCoTaskMem(fontPtr);

                ffBryant = pfcBryant.Families[0];
            }
            catch { }

            this.listBox_ftdiDevices.KeyUp += new KeyEventHandler(listBox_ftdiDevices_KeyUpHandler);
        }

        List<Control> controlList;
        private IEnumerable<Control> GetControlHierarchy(Control root)
        {
            var queue = new Queue<Control>();
            queue.Enqueue(root);
            do
            {
                var control = queue.Dequeue();
                yield return control;
                foreach (var child in control.Controls.OfType<Control>())
                    queue.Enqueue(child);
            } while (queue.Count > 0);
        }

        private void Form_ftdiDevices_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < ftList.Length; i++) listBox_ftdiDevices.Items.Add(ftList[i].Description);

            listBox_ftdiDevices.SelectedIndex = 0;

            controlList = GetControlHierarchy(this).ToList(); // Get the list of all the controls in the UI

            foreach (var control in controlList) // Set font of all the controls to the Weeroc font.
                control.Font = new Font(ffBryant, control.Font.Size);

            if (label_titleBar.Font.FontFamily.Name != "Bryant Regular Compressed") // Change font to generic if the weeroc font isn't loaded.
            {
                foreach (var control in controlList)
                    control.Font = new Font(FontFamily.GenericSansSerif, control.Font.Size * 0.75F);
            }
        }

        public int ftdiIndex = 0;

        private void button_OK_Click(object sender, EventArgs e)
        {
            ftdiIndex = listBox_ftdiDevices.SelectedIndex;

            ActiveForm.Close();
        }

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private void label_titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void label_titleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void label_titleBar_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void listBox_ftdiDevices_KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) // 13 = Return key
                button_OK_Click(null, null);
        }
    }
}
