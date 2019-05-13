using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Linq;

namespace CitirocUI
{
    public partial class Form_chartParameters : Form
    {
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        FontFamily ffBryant = FontFamily.GenericSansSerif;
        private PrivateFontCollection pfcBryant = new PrivateFontCollection();

        public Form_chartParameters(System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            InitializeComponent();

            textBox_xAxisMin.Text = chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum.ToString();
            textBox_xAxisMax.Text = chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum.ToString();
            textBox_xAxisInterval.Text = chart.ChartAreas[0].AxisX.Interval.ToString();
            textBox_yAxisMin.Text = chart.ChartAreas[0].AxisY.ScaleView.ViewMinimum.ToString();
            textBox_yAxisMax.Text = chart.ChartAreas[0].AxisY.ScaleView.ViewMaximum.ToString();
            textBox_yAxisInterval.Text = chart.ChartAreas[0].AxisY.Interval.ToString();

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

        private void Form_chartParameters_Load(object sender, EventArgs e)
        {
            controlList = GetControlHierarchy(this).ToList(); // Get the list of all the controls in the UI

            foreach (var control in controlList) // Set font of all the controls to the Weeroc font.
                control.Font = new Font(ffBryant, control.Font.Size);

            if (label_titleBar.Font.FontFamily.Name != "Bryant Regular Compressed") // Change font to generic if the weeroc font isn't loaded.
            {
                foreach (var control in controlList)
                    control.Font = new Font(FontFamily.GenericSansSerif, control.Font.Size * 0.75F);
            }
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

        private void button_close_Click(object sender, EventArgs e)
        {
            results[6] = 0;
            ActiveForm.Close();
        }

        public double[] results = new double[7];
        private void button_OK_Click(object sender, EventArgs e)
        {
            results[0] = Convert.ToDouble(textBox_xAxisMin.Text);
            results[1] = Convert.ToDouble(textBox_xAxisMax.Text);
            results[2] = Convert.ToDouble(textBox_xAxisInterval.Text);
            results[3] = Convert.ToDouble(textBox_yAxisMin.Text);
            results[4] = Convert.ToDouble(textBox_yAxisMax.Text);
            results[5] = Convert.ToDouble(textBox_yAxisInterval.Text);
            results[6] = 1;

            ActiveForm.Close();
        }

    }
}
