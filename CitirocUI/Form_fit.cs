using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Text;

namespace CitirocUI
{
    public partial class Form_fit : Form
    {
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        FontFamily ffBryant = FontFamily.GenericSansSerif;
        private PrivateFontCollection pfcBryant = new PrivateFontCollection();

        double[] Xdata;
        double[] Ydata;
        public Form_fit(DataSet dataset, System.Windows.Forms.DataVisualization.Charting.Chart chart, int fitMin, int fitMax)
        {
            InitializeComponent();

            Xdata = new double[dataset.Tables[0].Rows.Count];
            Ydata = new double[dataset.Tables[0].Rows.Count];

            for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
            {
                Xdata[i] = Convert.ToDouble(dataset.Tables[0].Rows[i].ItemArray[0].ToString());
                Ydata[i] = Convert.ToDouble(dataset.Tables[0].Rows[i].ItemArray[1].ToString());
            }

            textBox_fitMin.Text = fitMin.ToString();
            textBox_fitMax.Text = fitMax.ToString();
            textBox_sigmaMax.Text = (fitMax - fitMin).ToString();
            textBox_sigmaGuess.Text = ((fitMax - fitMin) / 2).ToString();

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

        private void Form_fit_Load(object sender, EventArgs e)
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

        private void btn_close_Click(object sender, EventArgs e)
        {
            fitResult[3] = 0;
            ActiveForm.Close();
        }

        public double[] fitResult = new double[4];
        private void button_fit_Click(object sender, EventArgs e)
        {
            double sigmaGuess;
            double sigmaMin;
            double sigmaMax;
            double fitMin;
            double fitMax;
            double.TryParse(textBox_sigmaGuess.Text, out sigmaGuess);
            double.TryParse(textBox_sigmaMin.Text, out sigmaMin);
            double.TryParse(textBox_sigmaMax.Text, out sigmaMax);
            double.TryParse(textBox_fitMin.Text, out fitMin);
            double.TryParse(textBox_fitMax.Text, out fitMax);

            double[] p = { sigmaGuess, (fitMax + fitMin) / 2, Ydata.Sum() }; // Initial conditions
            int[] sigmaLimited = { 1, 1 };
            double[] sigmaLimits = { sigmaMin, sigmaMax };
            int[] meanLimited = { 1, 1 };
            double[] meanLimits = { fitMin, fitMax };
            int[] amplitudeLimited = { 1, 1 };
            double[] amplitudeLimits = { 1, Ydata.Sum() * 100 };

            for (int i = 0; i < Xdata.Length; i++)
                if (Xdata[i] < fitMin || Xdata[i] > fitMax) Ydata[i] = 0;

            mp_par[] pars = new mp_par[3] { new mp_par() { limited = sigmaLimited, limits = sigmaLimits }, new mp_par() { limited = meanLimited, limits = meanLimits }, new mp_par() { limited = amplitudeLimited, limits = amplitudeLimits } }; // Parameter constraints
            int status;

            mp_result result = new mp_result(3);

            CustomUserVariable v = new CustomUserVariable() { X = Xdata, Y = Ydata };

            // Call fitting function
            status = MPFit.Solve(fitFunction, Xdata.Length, 3, p, pars, null, v, ref result);

            fitResult[0] = p[0];
            fitResult[1] = p[1];
            fitResult[2] = p[2];
            fitResult[3] = 1;

            ActiveForm.Close();
        }

        private int fitFunction(double[] p, double[] dy, IList<double>[] dvec, object vars)
        {
            double[] x, y;

            CustomUserVariable v = (CustomUserVariable)vars;

            x = v.X;
            y = v.Y;

            for (int i = 0; i < dy.Length; i++)
            {
                dy[i] = y[i] - (p[2] / (p[0] * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x[i] - p[1]) * (x[i] - p[1]) / (2 * p[0] * p[0])));
            }

            return 0;
        }

        public double[] GetMyResult(double[] result)
        {
            return result;
        }

    }
}
