using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTD2XX_NET;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Diagnostics;
using Chart = System.Windows.Forms.DataVisualization.Charting.Chart;
using Charting = System.Windows.Forms.DataVisualization.Charting;

namespace CitirocUI
{
    public partial class Citiroc : Form
    {
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        FontFamily ffBryant = FontFamily.GenericSansSerif;
        private System.Drawing.Text.PrivateFontCollection pfcBryant = new System.Drawing.Text.PrivateFontCollection();

        ProtoCubesSerial protoCubes;
        ProtoCubesMonitor protoCubesMonitorForm;

        public Citiroc()
        {
            InitializeComponent();

            try
            {
                byte[] fontData = Properties.Resources.Bryant_RegularCompressed;
                IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                uint dummy = 0;
                pfcBryant.AddMemoryFont(fontPtr, Properties.Resources.Bryant_RegularCompressed.Length);
                AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.Bryant_RegularCompressed.Length, IntPtr.Zero, ref dummy);
                Marshal.FreeCoTaskMem(fontPtr);

                ffBryant = pfcBryant.Families[0];
            }
            catch
            {
                MessageBox.Show("An error occured during the loading of the font. A generic font will be used which can impair the optimal display.");
            }
        }

        #region title bar

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
            Application.Exit();
        }

        private void btn_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        double scale = 1;
        private void button_UIScale_Click(object sender, EventArgs e)
        {
            float scaleF = 1;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            int screenWidth = resolution.Width;
            int screenHeight = resolution.Height;

            scaleF = (float)screenWidth / 1280;
            if (scaleF > (float)screenHeight / 720) scaleF = (float)screenHeight / 720;

            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
                SizeF Scale = new SizeF(scaleF, scaleF);
                ActiveForm.Scale(Scale);

                foreach (Control control in controlList)
                {
                    if (control is Chart)
                    {
                        ((Chart)control).ChartAreas[0].AxisX.TitleFont = new Font(((Chart)control).ChartAreas[0].AxisX.TitleFont.FontFamily, ((Chart)control).ChartAreas[0].AxisX.TitleFont.Size * scaleF);
                        ((Chart)control).ChartAreas[0].AxisY.TitleFont = new Font(((Chart)control).ChartAreas[0].AxisY.TitleFont.FontFamily, ((Chart)control).ChartAreas[0].AxisY.TitleFont.Size * scaleF);
                        ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font = new Font(((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font.FontFamily, ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font.Size * scaleF);
                        ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font = new Font(((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font.FontFamily, ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font.Size * scaleF);
                    }
                    else control.Font = new Font(control.Font.FontFamily, control.Font.Size * scaleF);
                }

                tabControl_top.ItemSize = new Size((int)(tabControl_top.ItemSize.Width * scaleF), (int)(tabControl_top.ItemSize.Height * scaleF));

                scale = scaleF;
            }
            else
            {


                WindowState = FormWindowState.Normal;
                SizeF Scale = new SizeF(1.0F / scaleF, 1.0F / scaleF);
                ActiveForm.Scale(Scale);

                foreach (Control control in controlList)
                {
                    if (control is Chart)
                    {
                        ((Chart)control).ChartAreas[0].AxisX.TitleFont = new Font(((Chart)control).ChartAreas[0].AxisX.TitleFont.FontFamily, ((Chart)control).ChartAreas[0].AxisX.TitleFont.Size / scaleF);
                        ((Chart)control).ChartAreas[0].AxisY.TitleFont = new Font(((Chart)control).ChartAreas[0].AxisY.TitleFont.FontFamily, ((Chart)control).ChartAreas[0].AxisY.TitleFont.Size / scaleF);
                        ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font = new Font(((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font.FontFamily, ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font.Size / scaleF);
                        ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font = new Font(((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font.FontFamily, ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font.Size / scaleF);
                    }
                    else control.Font = new Font(control.Font.FontFamily, control.Font.Size / scaleF);
                }

                tabControl_top.ItemSize = new Size((int)(tabControl_top.ItemSize.Width / scaleF), (int)(tabControl_top.ItemSize.Height / scaleF));

                scale = 1;
            }
        }
        #endregion

        #region Weeroc color
        static Color WeerocBlack = Color.FromArgb(255, 30, 30, 28);
        static Color WeerocDarkGreen = Color.FromArgb(255, 0, 83, 52);
        static Color WeerocGreen = Color.FromArgb(255, 29, 121, 104);
        static Color WeerocPaleBlue = Color.FromArgb(255, 0, 121, 144);
        static Color WeerocDarkBlue = Color.FromArgb(255, 34, 32, 70);
        static Color WeerocBlue = Color.FromArgb(255, 2, 65, 103);
        #endregion

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

        static int NbChannels = 32;

        private void Citiroc_Load(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = customCulture;

            tabControl_top.SendToBack();

            tabControl_top.ItemSize = new Size(0, 1);
            tabControl_top.SizeMode = TabSizeMode.Fixed;

            label_titleBar.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision >= 250)
                label_titleBar.Text += " (KTH)";

            // Set FTDI device count to 0
            ftdiDeviceCount = 0;
            // Create new instance of the FTDI device class
            myFtdiDevice = new FTDI();
            // Initalize FTDI status
            ftStatus = FTDI.FT_STATUS.FT_OK;
            // Usb Devices ID
            usbDevId = 0;
            // Set default FTDI chip to FTDI2232H
            ftdiDevice = FTDI.FT_DEVICE.FT_DEVICE_2232H;

            // Initialize the slow control parameters, set and reset all controls at least once in the GUI
            setSC("0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            setSC("1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111");
            setSC(strDefSC);

            var path = Path.Combine(Path.GetTempPath(), @"CitirocUI\");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            textBox_dataSavePath.Text = path;
            textBox_sCurveSavePath.Text = Path.GetTempPath() + "ScurvesCITI";
            textBox_staircaseSavePath.Text = Path.GetTempPath() + "staircaseCITI";
            textBox_holdScanSavePath.Text = Path.GetTempPath() + "holdScanCITI";
            comboBox_sCurvesClock.SelectedIndex = 3;
            comboBox_triggerPreset.SelectedIndex = 0;

            enableZoom(chart_Scurves.ChartAreas[0], true);
            enableZoom(chart_staircase.ChartAreas[0], true);
            enableZoom(chart_perChannelChargeHG.ChartAreas[0], true);
            enableZoom(chart_perAcqChargeHG.ChartAreas[0], true);
            enableZoom(chart_perChannelChargeLG.ChartAreas[0], true);
            enableZoom(chart_perAcqChargeLG.ChartAreas[0], true);
            enableZoom(chart_holdScan.ChartAreas[0], true);

            roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff.png");

            #region Weeroc font
            controlList = GetControlHierarchy(this).ToList(); // Get the list of all the controls in the UI

            // Set font of all the controls to the Weeroc font.
            foreach (var control in controlList)
                if (control is Chart)
                {
                    ((Chart)control).ChartAreas[0].AxisX.TitleFont = new Font(ffBryant, ((Chart)control).ChartAreas[0].AxisX.TitleFont.Size);
                    ((Chart)control).ChartAreas[0].AxisY.TitleFont = new Font(ffBryant, ((Chart)control).ChartAreas[0].AxisY.TitleFont.Size);
                    ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font = new Font(ffBryant, ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font.Size);
                    ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font = new Font(ffBryant, ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font.Size);
                }
                else control.Font = new Font(ffBryant, control.Font.Size);

            if (label_weerocDotCom.Font.FontFamily.Name != "Bryant Regular Compressed") // Change font to generic if the weeroc font isn't loaded.
            {
                foreach (var control in controlList)
                    if (control is Chart)
                    {
                        ((Chart)control).ChartAreas[0].AxisX.TitleFont = new Font(FontFamily.GenericSansSerif, ((Chart)control).ChartAreas[0].AxisX.TitleFont.Size * 0.75F);
                        ((Chart)control).ChartAreas[0].AxisY.TitleFont = new Font(FontFamily.GenericSansSerif, ((Chart)control).ChartAreas[0].AxisY.TitleFont.Size * 0.75F);
                        ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font = new Font(FontFamily.GenericSansSerif, ((Chart)control).ChartAreas[0].AxisX.LabelStyle.Font.Size * 0.75F);
                        ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font = new Font(FontFamily.GenericSansSerif, ((Chart)control).ChartAreas[0].AxisY.LabelStyle.Font.Size * 0.75F);
                    }
                    else
                        control.Font = new Font(FontFamily.GenericSansSerif, control.Font.Size * 0.75F);
            }
            #endregion

            textBox_word0.Font = new Font("Consolas", 11.0F);
            textBox_word1.Font = new Font("Consolas", 11.0F);
            textBox_word2.Font = new Font("Consolas", 11.0F);
            textBox_word3.Font = new Font("Consolas", 11.0F);
            textBox_word5.Font = new Font("Consolas", 11.0F);

            ContextMenu cm1 = new ContextMenu();
            cm1.MenuItems.Add("Reset zoom", new EventHandler(resetZoom_Click));
            //cm1.MenuItems.Add("Axes setup...", new EventHandler(AxesSetup_Click));
            cm1.MenuItems.Add("Save image", new EventHandler(SaveImage_Click));
            cm1.MenuItems.Add("Export as...", new EventHandler(ExportDataSet_Click));

            ContextMenu cm2 = new ContextMenu();
            cm2.MenuItems.Add("Reset zoom", new EventHandler(resetZoom_Click));
            //cm2.MenuItems.Add("Axes setup...", new EventHandler(AxesSetup_Click));
            cm2.MenuItems.Add("Save image", new EventHandler(SaveImage_Click));
            cm2.MenuItems.Add("Export as...", new EventHandler(ExportDataSet_Click));
            if (isInstalled("ROOT")) cm2.MenuItems.Add("Open with ROOT", new EventHandler(openWithRoot_Click));

            ContextMenu cm3 = new ContextMenu();
            cm3.MenuItems.Add("Reset zoom", new EventHandler(resetZoom_Click));
            //cm3.MenuItems.Add("Axes setup...", new EventHandler(AxesSetup_Click));
            cm3.MenuItems.Add("Fit...", new EventHandler(FitDataSet_Click));
            cm3.MenuItems.Add("Save image", new EventHandler(SaveImage_Click));
            cm3.MenuItems.Add("Export as...", new EventHandler(ExportDataSet_Click));
            if (isInstalled("ROOT")) cm3.MenuItems.Add("Open with ROOT", new EventHandler(openWithRoot_Click));

            chart_Scurves.ContextMenu = cm1;
            chart_staircase.ContextMenu = cm1;
            chart_holdScan.ContextMenu = cm1;
            chart_perChannelChargeHG.ContextMenu = cm3;
            chart_perAcqChargeHG.ContextMenu = cm2;
            chart_perChannelChargeLG.ContextMenu = cm3;
            chart_perAcqChargeLG.ContextMenu = cm2;

            groupBox_SerialPortSettings.Visible = false;

            // Adjust enable state of labels in Data Acquisition tab
            if (switchBox_acquisitionMode.Checked == true)
            {
                label_numData.Enabled = false;
                textBox_numData.Enabled = false;
                label_acquisitionTime.Enabled = true;
                textBox_acquisitionTime.Enabled = true;
                timeAcquisitionMode = true;
            }
            else
            {
                label_numData.Enabled = true;
                textBox_numData.Enabled = true;
                label_acquisitionTime.Enabled = false;
                textBox_acquisitionTime.Enabled = false;
                timeAcquisitionMode = false;
            }

            // Create serial comm object and attach event to local function
            protoCubes = new ProtoCubesSerial();
            protoCubes.DataReadyEvent += this.ReqStatus_DataReady;
            protoCubes.DataReadyEvent += this.ReqPayload_DataReady;
            protoCubes.DataReadyEvent += this.ReqBoardID_DataReady;

            // Clear text in some labels
            label_nbHit.Text = "";
            label_DataFile.Text = "";

            // Prep. the user in Proto-CUBES mode
            comboBox_SelectConnection.SelectedIndex = 1;
        }

        #region fancy tabControl

        private void radioButton_Connect_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) tabControl_top.SelectedTab = tabControl_top.TabPages[0];
        }

        private void radioButton_slowControl_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) tabControl_top.SelectedTab = tabControl_top.TabPages[1];
        }

        private void radioButton_probes_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) tabControl_top.SelectedTab = tabControl_top.TabPages[2];
        }

        private void radioButton_calibration_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) tabControl_top.SelectedTab = tabControl_top.TabPages[3];
        }

        private void radioButton_dataAcquisition_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) tabControl_top.SelectedTab = tabControl_top.TabPages[4];
        }

        private void radioButton_firmware_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) tabControl_top.SelectedTab = tabControl_top.TabPages[5];
        }

        #endregion

        #region misc. methods

        public static int GrayToInt(BitArray gray, int length) // Gray code to integer conversion
        {
            // gray conversion
            BitArray binary = new BitArray(length);
            binary[length - 1] = gray[length - 1];
            for (int i = length - 2; i >= 0; i--)
                binary[i] = !gray[i] ^ !binary[i + 1];
            // cast bitarray to int
            int[] result = new int[1];
            binary.CopyTo(result, 0);
            return result[0];
        }

        public static ArrayList ReadFileLine(string FilePathforRead) // Read a file and return the data in an ArrayList
        {
            StreamReader FiletoRead = new StreamReader(FilePathforRead);
            string sLine = "";
            ArrayList LineList = new ArrayList();
            while (sLine != null)
            {
                sLine = FiletoRead.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                    LineList.Add(sLine);
            }
            FiletoRead.Close();
            return LineList;
        }

        public static double[] lookUpRepeatElement(double[] arrayIn, out double[] elementCount)
        {
            Dictionary<double, double> list = new Dictionary<double, double>();
            for (int i = 0; i < arrayIn.Length; i++)
            {
                if (list.ContainsKey(arrayIn[i]))
                    list[arrayIn[i]]++;
                else
                    list.Add(arrayIn[i], 1);
            }
            list = list.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            double[] eleCnt = new double[list.Keys.Count];
            double[] eleNum = new double[list.Values.Count];
            list.Keys.CopyTo(eleNum, 0);
            list.Values.CopyTo(eleCnt, 0);

            elementCount = eleCnt;
            return eleNum;
        }

        private static string IntToBin(int value, int len) // To convert a value from integer to binary representation into a string
        {
            return (len > 1 ? IntToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }

        public static string strRev(string s) // To reverse a string
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        #endregion

        #region firmware options

        private void readAllFPGAWords()
        {
            textBox_word0.Text = Firmware.readWord(0, usbDevId);
            textBox_word1.Text = Firmware.readWord(1, usbDevId);
            textBox_word2.Text = Firmware.readWord(2, usbDevId);
            textBox_word3.Text = Firmware.readWord(3, usbDevId);
            textBox_word5.Text = Firmware.readWord(5, usbDevId);
        }

        private void comboBox_triggerPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_triggerPreset.SelectedIndex)
            {
                case 1:
                    checkBox_triggerTorQ.Checked = true;
                    Thread.Sleep(1);
                    checkBox_selTrigToHold.Checked = false;
                    Thread.Sleep(1);
                    checkBox_iofpgaOr32t.Checked = false;
                    Thread.Sleep(1);
                    checkBox_softwareTrigger.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selPSGlobalTrigger.Checked = false;
                    break;
                case 2:
                    checkBox_triggerTorQ.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selTrigToHold.Checked = true;
                    Thread.Sleep(1);
                    checkBox_iofpgaOr32t.Checked = false;
                    Thread.Sleep(1);
                    checkBox_softwareTrigger.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selPSGlobalTrigger.Checked = false;
                    break;
                case 3:
                    checkBox_triggerTorQ.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selTrigToHold.Checked = false;
                    Thread.Sleep(1);
                    checkBox_iofpgaOr32t.Checked = true;
                    Thread.Sleep(1);
                    checkBox_softwareTrigger.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selPSGlobalTrigger.Checked = true;
                    break;
                case 4:
                    checkBox_triggerTorQ.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selTrigToHold.Checked = false;
                    Thread.Sleep(1);
                    checkBox_iofpgaOr32t.Checked = false;
                    Thread.Sleep(1);
                    checkBox_softwareTrigger.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selPSGlobalTrigger.Checked = true;
                    break;
                case 5:
                    checkBox_triggerTorQ.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selTrigToHold.Checked = false;
                    Thread.Sleep(1);
                    checkBox_iofpgaOr32t.Checked = false;
                    Thread.Sleep(1);
                    checkBox_softwareTrigger.Checked = true;
                    Thread.Sleep(1);
                    checkBox_selPSGlobalTrigger.Checked = true;
                    break;
                default:
                    checkBox_triggerTorQ.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selTrigToHold.Checked = false;
                    Thread.Sleep(1);
                    checkBox_iofpgaOr32t.Checked = false;
                    Thread.Sleep(1);
                    checkBox_softwareTrigger.Checked = false;
                    Thread.Sleep(1);
                    checkBox_selPSGlobalTrigger.Checked = false;
                    break;
            }
        }



        private void checkBox_selValEvt_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) { ((CheckBox)sender).Text = "IO_FPGA5 valid event"; checkBox_valEvt.Visible = false; }
            else { ((CheckBox)sender).Text = "Valid event:"; checkBox_valEvt.Visible = true; }
            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked) ? "1" : "0") + ((checkBox_enSerialLink.Checked) ? "1" : "0") + ((checkBox_selRazChn.Checked) ? "1" : "0") + ((checkBox_valEvt.Checked) ? "1" : "0") + ((checkBox_razChn.Checked) ? "1" : "0") + ((checkBox_selValEvt.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_valEvt_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Valid";
            else ((CheckBox)sender).Text = "Invalid";
            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked) ? "1" : "0") + ((checkBox_enSerialLink.Checked) ? "1" : "0") + ((checkBox_selRazChn.Checked) ? "1" : "0") + ((checkBox_valEvt.Checked) ? "1" : "0") + ((checkBox_razChn.Checked) ? "1" : "0") + ((checkBox_selValEvt.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_selRazChn_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) { ((CheckBox)sender).Text = "IO_FPGA6 reset analog part"; checkBox_razChn.Visible = false; }
            else { ((CheckBox)sender).Text = "Reset analog part:"; checkBox_razChn.Visible = true; }
            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked) ? "1" : "0") + ((checkBox_enSerialLink.Checked) ? "1" : "0") + ((checkBox_selRazChn.Checked) ? "1" : "0") + ((checkBox_valEvt.Checked) ? "1" : "0") + ((checkBox_razChn.Checked) ? "1" : "0") + ((checkBox_selValEvt.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_razChn_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Reset";
            else ((CheckBox)sender).Text = "No reset";
            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked) ? "1" : "0") + ((checkBox_enSerialLink.Checked) ? "1" : "0") + ((checkBox_selRazChn.Checked) ? "1" : "0") + ((checkBox_valEvt.Checked) ? "1" : "0") + ((checkBox_razChn.Checked) ? "1" : "0") + ((checkBox_selValEvt.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_enSerialLink_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Enable serial link";
            else ((CheckBox)sender).Text = "Disable serial link";
            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked) ? "1" : "0") + ((checkBox_enSerialLink.Checked) ? "1" : "0") + ((checkBox_selRazChn.Checked) ? "1" : "0") + ((checkBox_valEvt.Checked) ? "1" : "0") + ((checkBox_razChn.Checked) ? "1" : "0") + ((checkBox_selValEvt.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_disReadAdc_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Read register enabled";
            else ((CheckBox)sender).Text = "Read register on reset";
            Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked) ? "1" : "0") + ((checkBox_enSerialLink.Checked) ? "1" : "0") + ((checkBox_selRazChn.Checked) ? "1" : "0") + ((checkBox_valEvt.Checked) ? "1" : "0") + ((checkBox_razChn.Checked) ? "1" : "0") + ((checkBox_selValEvt.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_readOutSpeed_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Read-out speed = 612 kHz";
            else ((CheckBox)sender).Text = "Read-out speed = 2.5 MHz";
            Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked) ? "1" : "0") + ((checkBox_readOutSpeed.Checked) ? "1" : "0") + ((checkBox_OR32polarity.Checked) ? "1" : "0") + "00", usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_rstbPa_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Enable preamplifiers";
            else ((CheckBox)sender).Text = "Preamplifiers on reset";
            Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked) ? "1" : "0") + ((checkBox_readOutSpeed.Checked) ? "1" : "0") + ((checkBox_OR32polarity.Checked) ? "1" : "0") + "00", usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_OR32polarity_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Output NOR32 (IO_FPGA11)";
            else ((CheckBox)sender).Text = "Output OR32 (IO_FPGA11)";
            Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked) ? "1" : "0") + ((checkBox_readOutSpeed.Checked) ? "1" : "0") + ((checkBox_OR32polarity.Checked) ? "1" : "0") + "00", usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_ADC2_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "ADC2: TEST_ADC input";
            else ((CheckBox)sender).Text = "ADC2: High gain charge";
            Firmware.sendWord(2, "000" + "" + "1" + ((checkBox_ADC1.Checked) ? "1" : "0") + ((checkBox_ADC2.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_ADC1_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "ADC1: ASIC temperature sensor";
            else ((CheckBox)sender).Text = "ADC1: Low gain charge";
            Firmware.sendWord(2, "000" + ((checkBox_softwareTrigger.Checked) ? "1" : "0") + ((checkBox_iofpgaOr32t.Checked) ? "1" : "0") + "1" + ((checkBox_ADC1.Checked) ? "1" : "0") + ((checkBox_ADC2.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_softwareTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                ((CheckBox)sender).Text = "Software trigger";
                if (((CheckBox)sender).Visible) checkBox_PSGlobalTrigger.Visible = true;
                checkBox_iofpgaOr32t.Visible = false;
            }
            else
            {
                ((CheckBox)sender).Text = "Firmware trigger";
                checkBox_PSGlobalTrigger.Visible = false;
                if (((CheckBox)sender).Visible) checkBox_iofpgaOr32t.Visible = true;
            }
            Firmware.sendWord(2, "000" + ((checkBox_softwareTrigger.Checked) ? "1" : "0") + ((checkBox_iofpgaOr32t.Checked) ? "1" : "0") + "1" + ((checkBox_ADC1.Checked) ? "1" : "0") + ((checkBox_ADC2.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_iofpgaOr32t_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "OR32t as external trigger";
            else ((CheckBox)sender).Text = "IO_FPGA6 as external trigger";
            Firmware.sendWord(2, "000" + ((checkBox_softwareTrigger.Checked) ? "1" : "0") + ((checkBox_iofpgaOr32t.Checked) ? "1" : "0") + "1" + ((checkBox_ADC1.Checked) ? "1" : "0") + ((checkBox_ADC2.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_triggerTorQ_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Trigger acquisition on FPGA OR32 (time trigger)";
            else ((CheckBox)sender).Text = "Trigger acquisition on ASIC OR32 (charge trigger)";
            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_pwrOn_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "power ON (no power pulsing)";
            else ((CheckBox)sender).Text = "IO_FPGA4 power on (power pulsing)";
            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_selTrigToHold_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Hold on coincidence (time trigger)";
            else ((CheckBox)sender).Text = "Hold on OR32 (charge or time trigger)";
            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_timeOutHold_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Enable time-out on hold signal";
            else ((CheckBox)sender).Text = "Disable time-out on hold signal";
            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_selHold_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "OR32 delayed as hold";
            else ((CheckBox)sender).Text = "IO_FPGA2 as hold";
            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_rstbPS_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Enable peak sensing";
            else ((CheckBox)sender).Text = "Peak sensing cells on reset";
            Firmware.sendWord(3, ((checkBox_rstbPS.Checked) ? "1" : "0") + "00" + ((checkBox_timeOutHold.Checked) ? "1" : "0") + ((checkBox_selHold.Checked) ? "1" : "0") + ((checkBox_selTrigToHold.Checked) ? "1" : "0") + ((checkBox_triggerTorQ.Checked) ? "1" : "0") + ((checkBox_pwrOn.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_PSGlobalTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "1";
            else ((CheckBox)sender).Text = "0";
            Firmware.sendWord(5, "0" + ((checkBox_selPSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_selPSMode.Checked) ? "1" : "0") + "000" + ((checkBox_PSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_PSMode.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_PSMode_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Track mode";
            else ((CheckBox)sender).Text = "Hold mode";
            Firmware.sendWord(5, "0" + ((checkBox_selPSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_selPSMode.Checked) ? "1" : "0") + "000" + ((checkBox_PSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_PSMode.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_trigPolarity_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) ((CheckBox)sender).Text = "Negative input";
            else ((CheckBox)sender).Text = "Positive input";
            Firmware.sendWord(5, "0" + ((checkBox_selPSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_selPSMode.Checked) ? "1" : "0") + "000" + ((checkBox_PSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_PSMode.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_selPSMode_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) { ((CheckBox)sender).Text = "IO_FPGA6 peak sensing mode"; checkBox_PSMode.Visible = false; }
            else { ((CheckBox)sender).Text = "Peak sensing mode:"; checkBox_PSMode.Visible = true; }
            Firmware.sendWord(5, "0" + ((checkBox_selPSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_selPSMode.Checked) ? "1" : "0") + "000" + ((checkBox_PSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_PSMode.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
        }

        private void checkBox_selPSGlobalTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                ((CheckBox)sender).Text = "External trigger";
                checkBox_softwareTrigger.Visible = true;
                if (!checkBox_softwareTrigger.Checked) checkBox_iofpgaOr32t.Visible = true;
                if (checkBox_softwareTrigger.Checked) checkBox_PSGlobalTrigger.Visible = true;
            }
            else
            {
                ((CheckBox)sender).Text = "Internal trigger";
                checkBox_softwareTrigger.Visible = false;
                checkBox_iofpgaOr32t.Visible = false;
                checkBox_PSGlobalTrigger.Visible = false;
            }
            Firmware.sendWord(5, "0" + ((checkBox_selPSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_selPSMode.Checked) ? "1" : "0") + "000" + ((checkBox_PSGlobalTrigger.Checked) ? "1" : "0") + ((checkBox_PSMode.Checked) ? "1" : "0"), usbDevId);
            readAllFPGAWords();
            Thread.Sleep(1);
            checkBox_timeOutHold.Checked = !checkBox_selPSGlobalTrigger.Checked;
        }

        private void button_sendMaskTime_Click(object sender, EventArgs e)
        {
            string w32 = ((checkBox_timeTriggerMask7.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask6.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask5.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask4.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask3.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask2.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask1.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask0.Checked) ? "1" : "0");
            string w33 = ((checkBox_timeTriggerMask15.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask14.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask13.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask12.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask11.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask10.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask9.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask8.Checked) ? "1" : "0");
            string w34 = ((checkBox_timeTriggerMask23.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask22.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask21.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask20.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask19.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask18.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask17.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask16.Checked) ? "1" : "0");
            string w35 = ((checkBox_timeTriggerMask31.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask30.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask29.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask28.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask27.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask26.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask25.Checked) ? "1" : "0") + ((checkBox_timeTriggerMask24.Checked) ? "1" : "0");

            Firmware.sendWord(32, w32, usbDevId);
            Firmware.sendWord(33, w33, usbDevId);
            Firmware.sendWord(34, w34, usbDevId);
            Firmware.sendWord(35, w35, usbDevId);

            if (Firmware.readWord(32, usbDevId) == w32 && Firmware.readWord(33, usbDevId) == w33 && Firmware.readWord(34, usbDevId) == w34 && Firmware.readWord(35, usbDevId) == w35)
                button_sendMaskTime.BackColor = WeerocGreen;
            else button_sendMaskTime.BackColor = Color.IndianRed;

            button_sendMaskTime.ForeColor = Color.White;
        }

        private void button_unmaskAllTime_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_timeTriggerMask.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = true;
        }

        private void button_maskAllTime_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_timeTriggerMask.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = false;
        }

        private void checkBox_timeTriggerMask_CheckedChanged(object sender, EventArgs e)
        {
            button_sendMaskTime.BackColor = Color.Gainsboro;
            button_sendMaskTime.ForeColor = Color.Black;
        }

        #endregion

        #region chart zoom

        public void enableZoom(Charting.ChartArea chartArea, bool isEnabled)
        {
            chartArea.CursorX.IsUserEnabled = isEnabled;
            chartArea.CursorX.IsUserSelectionEnabled = isEnabled;
            chartArea.CursorX.Interval = 0;
            chartArea.AxisX.ScaleView.Zoomable = isEnabled;
            chartArea.AxisX.ScrollBar.Enabled = false;
            chartArea.AxisX.ScaleView.SmallScrollMinSize = 0;

            chartArea.CursorY.IsUserEnabled = isEnabled;
            chartArea.CursorY.IsUserSelectionEnabled = isEnabled;
            chartArea.CursorY.Interval = 0;
            chartArea.AxisY.ScaleView.Zoomable = isEnabled;
            chartArea.AxisY.ScrollBar.Enabled = false;
            chartArea.AxisY.ScaleView.SmallScrollMinSize = 0;

            if (isEnabled == false)
            {
                chartArea.CursorX.SetCursorPosition(double.NaN);
                chartArea.CursorY.SetCursorPosition(double.NaN);
            }
        }

        private void button_resetZoom_Click(object sender, EventArgs e)
        {
            Control parent = ((Control)sender).Parent;
            Chart chart;
            foreach (Control c in parent.Controls)
            {
                if (c is Chart)
                {
                    chart = (Chart)c;
                    resetZoom(chart);
                }
            }
        }

        private void resetZoom_Click(object sender, EventArgs e)
        {
            Chart chart = (Chart)(((MenuItem)sender).GetContextMenu().SourceControl);
            resetZoom(chart);
        }

        private void resetZoom(Chart chart)
        {
            Charting.Axis ax = chart.ChartAreas[0].AxisX;
            Charting.Axis ay = chart.ChartAreas[0].AxisY;

            ax.ScaleView.ZoomReset(0);
            ay.ScaleView.ZoomReset(0);
            chart.ChartAreas[0].CursorX.SetCursorPosition(double.NaN);
            chart.ChartAreas[0].CursorY.SetCursorPosition(double.NaN);

            ax.Interval = 0;
            ax.LabelStyle.IntervalOffset = 0;
            ax.MajorGrid.IntervalOffset = 0;
            ax.MajorTickMark.IntervalOffset = 0;
            ax.RoundAxisValues();
            ay.Interval = 0;
            ay.LabelStyle.IntervalOffset = 0;
            ay.MajorGrid.IntervalOffset = 0;
            ay.MajorTickMark.IntervalOffset = 0;

            if (chart == chart_Scurves)
            {
                ay.LabelStyle.IntervalOffset += 5;
                ay.MajorGrid.IntervalOffset += 5;
                ay.MajorTickMark.IntervalOffset += 5;
                ay.Interval = 25;
            }

            if (chart == chart_perAcqChargeHG || chart == chart_perAcqChargeLG)
            {
                ax.LabelStyle.IntervalOffset += 1;
                ax.MajorGrid.IntervalOffset += 1;
                ax.MajorTickMark.IntervalOffset += 1;
            }

            if (chart == chart_holdScan && checkBox_showScatterPlot.Checked)
                chart_holdScan.Series["Scatter plot"].MarkerSize = granularity * (int)Math.Ceiling((double)chart_holdScan.Height / (chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMaximum - chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMinimum));

            ax.ScaleView.Zoom(ax.Minimum, ax.Maximum);
            ay.ScaleView.Zoom(ay.Minimum, ay.Maximum);
        }

        double mouseXPosition = double.NaN;
        double mouseYPosition = double.NaN;
        double rangeX = double.NaN;
        double rangeY = double.NaN;
        bool dragMoveChart = false;
        private void chart_MouseDown(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;

            chart.ChartAreas[0].InnerPlotPosition.Auto = false;

            if (e.Button == MouseButtons.Middle)
            {
                Charting.Axis ax = chart.ChartAreas[0].AxisX;
                Charting.Axis ay = chart.ChartAreas[0].AxisY;

                dragMoveChart = true;
                mouseXPosition = ax.PixelPositionToValue(e.Location.X);
                mouseYPosition = ay.PixelPositionToValue(e.Location.Y);

                rangeX = ax.ScaleView.ViewMaximum - ax.ScaleView.ViewMinimum;
                rangeY = ay.ScaleView.ViewMaximum - ay.ScaleView.ViewMinimum;
            }
        }

        private void chart_MouseMove(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;

            if (dragMoveChart)
            {
                Charting.Axis ax = chart.ChartAreas[0].AxisX;
                Charting.Axis ay = chart.ChartAreas[0].AxisY;

                double xv = 0, yv = 0;
                try { xv = ax.PixelPositionToValue(e.Location.X); }
                catch { return; }
                try { yv = ay.PixelPositionToValue(e.Location.Y); }
                catch { return; }

                double scrollX = mouseXPosition - xv;
                double scrollY = mouseYPosition - yv;

                double posXStart = ax.ScaleView.ViewMinimum + scrollX;
                double posXFinish = posXStart + rangeX;
                double posYStart = ay.ScaleView.ViewMinimum + scrollY;
                double posYFinish = posYStart + rangeY;

                if (posXStart < ax.Minimum)
                {
                    posXFinish = ax.Minimum + rangeX;
                    posXStart = ax.Minimum;
                }
                if (posXFinish > ax.Maximum)
                {
                    posXStart = ax.Maximum - rangeX;
                    posXFinish = ax.Maximum;
                }
                if (posYStart < ay.Minimum)
                {
                    posYFinish = ay.Minimum + rangeY;
                    posYStart = ay.Minimum;
                }
                if (posYFinish > ay.Maximum)
                {
                    posYStart = ay.Maximum - rangeY;
                    posYFinish = ay.Maximum;
                }

                ax.ScaleView.Zoom(posXStart, posXFinish);
                ay.ScaleView.Zoom(posYStart, posYFinish);

                ax.RoundAxisValues();

                double Xoffset = ax.Minimum - ax.ScaleView.Position + 1;

                ax.LabelStyle.IntervalOffset = Xoffset;
                ax.MajorGrid.IntervalOffset = Xoffset;
                ax.MajorTickMark.IntervalOffset = Xoffset;

                double Yoffset = ay.Minimum - ay.ScaleView.Position;

                ay.LabelStyle.IntervalOffset = Yoffset;
                ay.MajorGrid.IntervalOffset = Yoffset;
                ay.MajorTickMark.IntervalOffset = Yoffset;

                if (chart == chart_Scurves)
                {
                    ay.LabelStyle.IntervalOffset += 5;
                    ay.MajorGrid.IntervalOffset += 5;
                    ay.MajorTickMark.IntervalOffset += 5;
                }
            }
            else if (chart == chart_Scurves || chart == chart_staircase)
            {
                // Call HitTest
                Charting.HitTestResult result = chart.HitTest(e.X, e.Y);

                // Reset Data Point Attributes
                foreach (Charting.Series series in chart.Series)
                {
                    series.BorderWidth = 2;
                    foreach (Charting.DataPoint point in series.Points)
                    {
                        point.MarkerStyle = Charting.MarkerStyle.None;
                    }
                }

                // If the mouse is over a data point
                if (result.ChartElementType == Charting.ChartElementType.DataPoint)
                {
                    // Find selected data point
                    Charting.DataPoint point = result.Series.Points[result.PointIndex];
                    if (chart == chart_Scurves) label_help.Text = result.Series.Name + "\n" + "DAC code: " + point.XValue + "\n" + "Trigger efficiency: " + point.YValues[0] + "%";
                    else label_help.Text = result.Series.Name + "\n" + "DAC code: " + point.XValue + "\n" + "Trigger frequency: " + point.YValues[0] + " triggers/second";
                    // Change the appearance of the data point
                    point.MarkerStyle = Charting.MarkerStyle.Circle;
                    point.MarkerSize = 6;
                    result.Series.BorderWidth = 4;
                }
                else
                {
                    // Set default cursor
                    this.Cursor = Cursors.Default;
                    label_help.Text = "";
                }
            }
            else if (chart == chart_perChannelChargeHG || chart == chart_perChannelChargeLG || chart == chart_perAcqChargeHG || chart == chart_perAcqChargeLG)
            {
                if (chart.Series.Count == 0) return;

                // Call HitTest
                Charting.HitTestResult result = chart.HitTest(e.X, e.Y);

                // Reset Data Point Attributes
                foreach (Charting.DataPoint point in chart.Series[0].Points)
                    point.Color = point.BorderColor;

                // If the mouse is over a data point
                if (result.ChartElementType == Charting.ChartElementType.DataPoint)
                {
                    if (result.Series != chart.Series[0]) return;
                    // Find selected data point
                    Charting.DataPoint point = result.Series.Points[result.PointIndex];
                    // Change the appearance of the data point
                    point.Color = point.MarkerColor;
                    label_help.Text = chart.ChartAreas[0].AxisX.Title + ": " + point.XValue + "\n" + chart.ChartAreas[0].AxisY.Title + ": " + point.YValues[0];
                }
                else
                {
                    // Set default cursor
                    this.Cursor = Cursors.Default;
                    label_help.Text = "";
                }
            }
        }

        private void chart_MouseWheel(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;

            Charting.Axis ax = chart.ChartAreas[0].AxisX;
            Charting.Axis ay = chart.ChartAreas[0].AxisY;

            double xMin = ax.ScaleView.ViewMinimum;
            double xMax = ax.ScaleView.ViewMaximum;
            double yMin = ay.ScaleView.ViewMinimum;
            double yMax = ay.ScaleView.ViewMaximum;
            rangeX = xMax - xMin;
            rangeY = yMax - yMin;
            double posXStart;
            double posXFinish;
            double posYStart;
            double posYFinish;

            mouseXPosition = ax.PixelPositionToValue(e.Location.X);
            mouseYPosition = ay.PixelPositionToValue(e.Location.Y);

            if (e.Delta > 0)
            {
                posXStart = mouseXPosition - rangeX / 2.5;
                posXFinish = mouseXPosition + rangeX / 2.5;
                posYStart = mouseYPosition - rangeY / 2.5;
                posYFinish = mouseYPosition + rangeY / 2.5;
            }
            else
            {
                posXStart = xMin - rangeX / 5;
                posXFinish = xMax + rangeX / 5;
                posYStart = yMin - rangeY / 5;
                posYFinish = yMax + rangeY / 5;
            }

            if (posXStart < ax.Minimum)
            {
                posXFinish = ax.Minimum + (posXFinish - posXStart);
                posXStart = ax.Minimum;
            }
            if (posXFinish > ax.Maximum)
            {
                posXStart = ax.Maximum - (posXFinish - posXStart);
                posXFinish = ax.Maximum;
            }
            if (posYStart < ay.Minimum)
            {
                posYFinish = ay.Minimum + (posYFinish - posYStart);
                posYStart = ay.Minimum;
            }
            if (posYFinish > ay.Maximum)
            {
                posYStart = ay.Maximum - (posYFinish - posYStart);
                posYFinish = ay.Maximum;
            }

            ax.ScaleView.Zoom(posXStart, posXFinish);
            ay.ScaleView.Zoom(posYStart, posYFinish);

            ax.Interval = 0;
            ay.Interval = 0;
            ax.RoundAxisValues();

            double Xoffset = ax.Minimum - ax.ScaleView.Position + 1;

            ax.LabelStyle.IntervalOffset = Xoffset;
            ax.MajorGrid.IntervalOffset = Xoffset;
            ax.MajorTickMark.IntervalOffset = Xoffset;

            double Yoffset = ay.Minimum - ay.ScaleView.Position;

            ay.LabelStyle.IntervalOffset = Yoffset;
            ay.MajorGrid.IntervalOffset = Yoffset;
            ay.MajorTickMark.IntervalOffset = Yoffset;

            if (chart == chart_Scurves)
            {
                ay.LabelStyle.IntervalOffset += 5;
                ay.MajorGrid.IntervalOffset += 5;
                ay.MajorTickMark.IntervalOffset += 5;
            }
            else if (chart == chart_perAcqChargeHG || chart == chart_perAcqChargeLG)
            {
                ax.LabelStyle.IntervalOffset += 1;
                ax.MajorGrid.IntervalOffset += 1;
                ax.MajorTickMark.IntervalOffset += 1;
            }

            if (chart == chart_holdScan && checkBox_showScatterPlot.Checked)
                chart_holdScan.Series["Scatter plot"].MarkerSize = granularity * (int)Math.Ceiling((double)chart_holdScan.Height / (chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMaximum - chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMinimum));
        }

        private void chart_MouseUp(object sender, MouseEventArgs e)
        {
            Chart chart = (Chart)sender;

            chart.ChartAreas[0].InnerPlotPosition.Auto = true;

            dragMoveChart = false;

            Charting.Axis ax = chart.ChartAreas[0].AxisX;
            Charting.Axis ay = chart.ChartAreas[0].AxisY;

            ax.Interval = 0;
            ay.Interval = 0;
            ax.RoundAxisValues();

            double Xoffset = ax.Minimum - ax.ScaleView.Position + 1;

            if (chart == chart_perAcqChargeHG || chart == chart_perAcqChargeLG) Xoffset += 1;

            ax.LabelStyle.IntervalOffset = Xoffset;
            ax.MajorGrid.IntervalOffset = Xoffset;
            ax.MajorTickMark.IntervalOffset = Xoffset;

            double Yoffset = ay.Minimum - ay.ScaleView.Position;

            if (chart == chart_Scurves) Yoffset += 5;

            ay.LabelStyle.IntervalOffset = Yoffset;
            ay.MajorGrid.IntervalOffset = Yoffset;
            ay.MajorTickMark.IntervalOffset = Yoffset;

            if (chart == chart_holdScan && checkBox_showScatterPlot.Checked && chart_holdScan.Series.Count > 0)
                chart_holdScan.Series["Scatter plot"].MarkerSize = granularity * (int)Math.Ceiling((double)chart_holdScan.Height / (chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMaximum - chart_holdScan.ChartAreas[0].AxisY.ScaleView.ViewMinimum));
        }

        #endregion

        #region delegate
        public delegate void TextBoxDelegate(string message, TextBox txtBox);
        private void UpdatingTextBox(string msg, TextBox TB)
        {
            if (TB.InvokeRequired) TB.BeginInvoke(new TextBoxDelegate(UpdatingTextBox), new object[] { msg, TB });
            else TB.Text = msg;
        }

        public delegate void LabelDelegate(string message, Label label);
        private void UpdatingLabel(string msg, Label lbl)
        {
            if (lbl.InvokeRequired) lbl.BeginInvoke(new LabelDelegate(UpdatingLabel), new object[] { msg, lbl });
            else lbl.Text = msg;
        }

        delegate void SetEnableCB(Boolean enable, Control control);
        private void SetEnable(Boolean enable, Control ctrl)
        {
            if (ctrl.InvokeRequired)
            {
                SetEnableCB d = new SetEnableCB(SetEnable);
                this.BeginInvoke(d, new object[] { enable, ctrl });
            }
            else
            {
                ctrl.Enabled = enable;
            }
        }

        delegate void SetChkStateCB(Boolean _checkState, CheckBox chk);
        private void SetChkState(Boolean _checkState, CheckBox chk)
        {
            if (chk.InvokeRequired)
            {
                SetChkStateCB d = new SetChkStateCB(SetChkState);
                this.BeginInvoke(d, new object[] { _checkState, chk });
            }
            else
            {
                chk.Checked = _checkState;
            }
        }

        private delegate void UpdatingProgressBar(int i, ProgressBar proBar);
        private void UpdatingProBar(int value, ProgressBar proBar)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdatingProgressBar(UpdatingProBar), value, proBar);
            }
            else
            {
                proBar.Value = value;
            }
        }

        private delegate void SetVisibleCB(bool visible, Control control);
        private void SetVisible(bool visible, Control ctrl)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetVisibleCB(SetVisible), visible, ctrl);
            }
            else
            {
                ctrl.Visible = visible;
            }
        }
        #endregion

        #region embeded help

        #region tabControl

        private void radioButton_Connect_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Use this panel to connect your testboard via USB.";
        }

        private void radioButton_slowControl_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Use this panel to adjust the behavior of the Citiroc ASIC via the slow control shift register.";
        }

        private void radioButton_probes_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Use this panel to select the ASIC parts you need to probe.";
        }

        private void radioButton_calibration_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Use this panel to calibrate the ASIC.";
        }

        private void radioButton_dataAcquisition_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Use this panel to acquire high gain and low gain charge measurement data from the ASIC";
        }

        private void radioButton_firmware_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Use this panel to modify the behavior of the firmware. This is for advanced users, use at your own risks !";
        }

        #endregion

        #region slow control

        private void tabPage_mainSettings_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Main settings of the ASIC. Contains the threshold values for charge and time, shaping behavior, trigger masking and" +
                " various behavior options. Ctest are injection capacitors embeded in the ASIC for test purpose. Plug a pulse generator on the" +
                "\"IN_CALIB\" LEMO connector on the testboard and choose the channel to inject by checking the correponding checkbox.";
        }

        private void tabPage_EN_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "On this tab you can choose wether the ASIC blocks are in continuous (Ctn) or power pulsing (PP) power " +
                "supply management. You can also enable (EN) or disable (DIS) certain parts.";
        }

        private void tabPage_preamplifier_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "On this page you can adjust the preamplifiers gains or disable them by individual channels. Refer to the datasheet " +
                "to find the corresponding voltage gains.";
        }

        private void tabPage_calibDac_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Calibration on DAC for charge and time threshold aren't mandatory to perform good measurements since DC dispersion on the" +
                " 32 fast shaper outputs is very low. The input DAC calibration is used to adjust individual high voltage biasing of SiPM in a SiPM array" +
                " so every SiPM has the same gain. Input DAC can be disabled per individual channel thanks to the checkboxes.";
        }

        private void button_saveSC_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Save the current slow control configuration into a file for later use.";
        }

        private void button_loadSC_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Load a slow control configuration from a file. Alternatively you can drag and drop the file.";
        }

        private void button_sendSC_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Load the current slow control configuration into the Citiroc1A ASIC.";
        }

        #endregion

        private void tabPage_probes_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "- The digital probe is outputed on the \"IO_FPGA8\" LEMO connector.\n" +
                "- The analog probe is outputed on the \"OUT_PROBE\" LEMO connector.\n" +
                "- When the read register is enabled, the low gain and high gain SCA or peak detector " +
                "output of the corresponding channel can be measured on the \"OUT_HG\" and \"OUT_LG\" connectors.";
        }

        #region Calibration

        #region Scurves

        private void textBox_stepScurves_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "A positive value will scan the Scurves from min code to max code. You can also set a negative value to Scan from max to min.";
        }

        private void comboBox_sCurveClock_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "The faster the clock, the faster the measurement. Changing the clock value will slightly shift the S-curves position for pedestal measurements.";
        }

        private void channelSelectionScurves_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Select the Scurves you wish to plot by checking the corresponding channel.";
        }

        private void button_Scurves_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Start plotting the S-curves.";
        }

        private void checkBox_useMaskScurves_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Unmask only the measured channel to reduce crosstalk during the measurement. Only available on the charge trigger.";
        }

        private void checkBox_ScurvesChoice_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Choose to plot the S-curves on the pedestal or on an input signal. When plotting on an input signal, plug a step generator " +
                "in the \"IN_CALIB\" LEMO connector. The software will loop through the Ctest to plot the S-curves.";
        }

        #endregion

        #region hold scan

        private void button_holdScan_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "Scan the shaper output thanks to the hold signal by sweeping through the delay value. " +
                "Use this to find the optimal delay for charge measurement. " +
                "When using the peak detector the delay value can be anywhere after the shaper peak.";
        }

        #endregion

        #region staircase

        #endregion

        #endregion

        private void object_MouseLeave(object sender, EventArgs e)
        {
            // Check if really leaving the panel
            if (((Control)sender).GetChildAtPoint(((Control)sender).PointToClient(Cursor.Position)) == null)
            {
                label_help.Text = null;
            }
        }

        #endregion

        private void wait(int nbTicks100ns)
        {
            // 1 tick = 100 ns
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < nbTicks100ns) { }
        }

        private bool validPath(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            if (fileName == null) return false;
            else if (!fi.Directory.Exists) return false;
            else return true;
        }

        System.Threading.Timer tempTimer;

        private void tempCallback(object state)
        {
            int index = 0;
            Invoke(new Action(() => index = tabControl_top.SelectedIndex));

            if (backgroundWorker_dataAcquisition.IsBusy ||
                backgroundWorker_Scurves.IsBusy ||
                backgroundWorker_staircase.IsBusy ||
                backgroundWorker_holdScan.IsBusy ||
                index == 5) { SetEnable(false, label_tempOnBoard); tempTimer.Change(1000, Timeout.Infinite); return; }

            SetEnable(true, label_tempOnBoard);
            double temp = (Convert.ToInt32(Firmware.readWord(64, usbDevId), 2) * 256 + Convert.ToInt32(Firmware.readWord(65, usbDevId), 2)) / 16;
            if (temp >= 2048) temp -= 1024;
            temp *= 0.0625;
            UpdatingLabel(Math.Round(temp, 1) + " °C", label_tempOnBoard);

            tempTimer.Change(1000, Timeout.Infinite);
        }

        private void SaveImage_Click(object sender, EventArgs e)
        {
            Chart chart = (Chart)(((MenuItem)sender).GetContextMenu().SourceControl);

            string strSaveImageName = "";

            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Title = "Save file";
            saveImageDialog.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff|Wmf Image (.wmf)|*.wmf";
            saveImageDialog.FilterIndex = 4;
            saveImageDialog.RestoreDirectory = true;

            if (saveImageDialog.ShowDialog() == DialogResult.OK) strSaveImageName = saveImageDialog.FileName;
            else return;

            if (strSaveImageName == null) return;

            switch (saveImageDialog.FilterIndex)
            {
                case 1:
                    chart.SaveImage(strSaveImageName, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case 2:
                    chart.SaveImage(strSaveImageName, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case 3:
                    chart.SaveImage(strSaveImageName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case 4:
                    chart.SaveImage(strSaveImageName, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case 5:
                    chart.SaveImage(strSaveImageName, System.Drawing.Imaging.ImageFormat.Tiff);
                    break;
                case 6:
                    chart.SaveImage(strSaveImageName, System.Drawing.Imaging.ImageFormat.Wmf);
                    break;
            }
        }

        private void ExportDataSet_Click(object sender, EventArgs e)
        {
            Chart chart = (Chart)(((MenuItem)sender).GetContextMenu().SourceControl);

            string strSaveName = "";
            int exportType;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save file";
            saveDialog.Filter = "csv (.csv)|*.csv|Excel (.xls)|*.xls|txt (.txt)|*.txt|XML (.xml)|*.xml";
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                strSaveName = saveDialog.FileName;
                exportType = saveDialog.FilterIndex;
            }
            else return;

            // Export series values into a DataSet object
            DataSet dataset = chart.DataManipulator.ExportSeriesValues();

            if (exportType == 1 || exportType == 3)
            {
                string separator;
                if (exportType == 1) separator = ","; else separator = " ";

                TextWriter tw = new StreamWriter(strSaveName);
                if (chart == chart_Scurves || chart == chart_staircase)
                {
                    string header = "DACcode ";
                    for (int chn = 0; chn < NbChannels; chn++)
                    {
                        header += "ch" + chn.ToString() + " ";
                    }
                    tw.WriteLine(header);

                    for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                    {
                        string outdata = "";
                        outdata += dataset.Tables[0].Rows[i].ItemArray[0].ToString() + separator;
                        foreach (DataTable table in dataset.Tables)
                            outdata += table.Rows[i].ItemArray[1].ToString() + separator;
                        tw.WriteLine(outdata);
                    }
                }
                else
                {
                    for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                    {
                        string outdata = "";
                        outdata += dataset.Tables[0].Rows[i].ItemArray[0].ToString() + separator + dataset.Tables[0].Rows[i].ItemArray[1].ToString();
                        tw.WriteLine(outdata);
                    }
                }
                tw.Flush();
                tw.Close();
            }
            else if (exportType == 2)
            {
                FileStream stream = new FileStream(strSaveName, FileMode.OpenOrCreate);
                ExcelWriter writer = new ExcelWriter(stream);
                writer.BeginWrite();

                if (chart == chart_Scurves || chart == chart_staircase)
                {
                    writer.WriteCell(0, 0, "DAC code");

                    for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                        writer.WriteCell(i + 1, 0, Convert.ToDouble(dataset.Tables[0].Rows[i].ItemArray[0].ToString()));

                    int shift = 0;
                    foreach (DataTable table in dataset.Tables)
                    {
                        writer.WriteCell(0, 1 + shift, table.TableName);
                        for (int i = 0; i < table.Rows.Count; i++)
                            writer.WriteCell(i + 1, 1 + shift, Convert.ToDouble(table.Rows[i].ItemArray[1].ToString()));
                        shift++;
                    }
                }
                else
                {
                    int shift = 0;
                    foreach (DataTable table in dataset.Tables)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                            writer.WriteCell(0, i + shift, table.Columns[i].ColumnName);

                        for (int j = 0; j < table.Rows.Count; j++)
                            for (int k = 0; k < table.Columns.Count; k++)
                                writer.WriteCell(j + 1, k + shift, Convert.ToDouble(table.Rows[j].ItemArray[k].ToString()));
                        shift += 2;
                    }
                }
                writer.EndWrite();
                stream.Close();
            }
            else if (exportType == 4) dataset.WriteXml(strSaveName);
        }

        private bool isInstalled(string appName)
        {
            try
            {
                string displayName;
                Microsoft.Win32.RegistryKey key;

                // search in: CurrentUser
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (String keyName in key.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(appName))
                    {
                        return true;
                    }
                }

                // search in: LocalMachine_32
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (String keyName in key.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(appName))
                    {
                        return true;
                    }
                }

                // search in: LocalMachine_64
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (String keyName in key.GetSubKeyNames())
                {
                    Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(appName))
                    {
                        return true;
                    }
                }

                // NOT FOUND
                return false;
            }
            catch { return false; }
        }

        private void openWithRoot_Click(object sender, EventArgs e)
        {
            Chart chart = (Chart)(((MenuItem)sender).GetContextMenu().SourceControl);

            // Export series values into a DataSet object
            DataSet dataset = chart.DataManipulator.ExportSeriesValues();

            string tempFile = Path.GetTempPath() + "tmpExportToRootRawFilePT2A";

            TextWriter tw = new StreamWriter(tempFile);

            for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
            {
                string outdata = "";
                outdata += dataset.Tables[0].Rows[i].ItemArray[0].ToString() + " " + dataset.Tables[0].Rows[i].ItemArray[1].ToString();
                tw.WriteLine(outdata);
            }
            tw.Flush();
            tw.Close();

            string dir = AppDomain.CurrentDomain.BaseDirectory + "Resources/";

            tempFile = tempFile.Replace('\\', '/');
            dir = dir.Replace('\\', '/');

            string commandPrompt = "/C root \"" + dir + "openInRoot.c\"(\\\"\"" + tempFile + "\"\\\")";

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            startInfo.Arguments = commandPrompt;
            process.StartInfo = startInfo;
            process.Start();
        }

        private void FitDataSet_Click(object sender, EventArgs e)
        {
            Chart chart = (Chart)(((MenuItem)sender).GetContextMenu().SourceControl);

            // Export series values into a DataSet object
            DataSet dataset = chart.DataManipulator.ExportSeriesValues();

            int fitMin = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
            int fitMax = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

            using (Form_fit frm = new Form_fit(dataset, chart, fitMin, fitMax))
            {
                frm.ShowDialog();

                double[] result = frm.fitResult;

                if (result[3] == 0) return;

                double sigma = result[0];
                double position = result[1];
                double amplitude = result[2];

                try { chart.Series.RemoveAt(1); } catch { }
                chart.Series.Add("Fit");
                chart.Series["Fit"].ChartType = Charting.SeriesChartType.Line;
                chart.Series["Fit"].Color = Color.Black;
                chart.ChartAreas[0].RecalculateAxesScale();

                for (double i = chart.ChartAreas[0].AxisX.Minimum + 1; i < chart.ChartAreas[0].AxisX.Maximum; i++)
                    chart.Series["Fit"].Points.AddXY(i, amplitude / (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(i - position) * (i - position) / (2 * sigma * sigma)));

                chart.ChartAreas[0].AxisX.Title = "Charge (ADCu)\nSigma = " + Math.Round(sigma, 3) + ", Position = " + Math.Round(position, 3) + ", Amplitude = " + Math.Round(amplitude / (sigma * Math.Sqrt(2 * Math.PI)), 3);
            }
        }

        private void AxesSetup_Click(object sender, EventArgs e)
        {
            Chart chart = (Chart)(((MenuItem)sender).GetContextMenu().SourceControl);

            // Export series values into a DataSet object
            DataSet dataset = chart.DataManipulator.ExportSeriesValues();

            using (Form_chartParameters frm = new Form_chartParameters(chart))
            {
                frm.ShowDialog();

                double[] results = frm.results;

                if (results[6] == 0) return;

                double xAxisMin = results[0];
                double xAxisMax = results[1];
                double xAxisInterval = results[2];
                double yAxisMin = results[3];
                double yAxisMax = results[4];
                double yAxisInterval = results[5];

                Charting.Axis ax = chart.ChartAreas[0].AxisX;
                Charting.Axis ay = chart.ChartAreas[0].AxisY;

                ax.ScaleView.Zoom(xAxisMin, xAxisMax);
                ay.ScaleView.Zoom(yAxisMin, yAxisMax);

                ax.Interval = xAxisInterval;
                ay.Interval = yAxisInterval;

                ax.RoundAxisValues();

                double Xoffset = ax.Minimum - ax.ScaleView.Position;

                ax.LabelStyle.IntervalOffset = Xoffset;
                ax.MajorGrid.IntervalOffset = Xoffset;
                ax.MajorTickMark.IntervalOffset = Xoffset;

                double Yoffset = ay.Minimum - ay.ScaleView.Position;

                ay.LabelStyle.IntervalOffset = Yoffset;
                ay.MajorGrid.IntervalOffset = Yoffset;
                ay.MajorTickMark.IntervalOffset = Yoffset;
            }
        }

        private void numericUpDown_loadCh_ValueChanged(object sender, EventArgs e)
        {
            if (selectedConnectionMode == 1)
            {
                NumericUpDown num = (NumericUpDown)sender;
                if ((num.Text == "31") && (num.Value <= 30))
                {
                    num.Value = 16;
                }
                else if (num.Text == "16")
                {
                    if (num.Value <= 16)
                        num.Value = 0;
                    else
                        num.Value = 31;
                }
                else if ((num.Text == "0") && (num.Value > 0))
                {
                    num.Value = 16;
                }

            }
            refreshDataChart();
        }

        private void numericUpDown_acquisitionNumber_ValueChanged(object sender, EventArgs e)
        {
            refreshDataChart();
        }

        private void checkBox_holdScanChoice_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_holdScanChoice.Checked) checkBox_holdScanChoice.Text = "On low gain";
            else checkBox_holdScanChoice.Text = "On high gain";
        }

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(
            [In()] System.IntPtr hdc, int x, int y, int cx, int cy,
            [In()] System.IntPtr hdcSrc, int x1, int y1, uint rop);

        private const int SRC_COPY = 0xCC0020;
        private void button_screenshot_Click(object sender, EventArgs e)
        {
            string strSaveImageName = "";
            label_titleBar.Focus();

            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Title = "Save file";
            saveImageDialog.Filter = "Png Image (.png)|*.png";
            saveImageDialog.FilterIndex = 4;
            saveImageDialog.RestoreDirectory = true;

            if (saveImageDialog.ShowDialog() == DialogResult.OK) strSaveImageName = saveImageDialog.FileName;
            else return;

            if (strSaveImageName == null) return;

            Form frm = label_titleBar.FindForm();

            Thread.Sleep(100);

            using (Bitmap bitmap = new Bitmap(frm.Width, frm.Height))
            {
                using (Graphics gb = Graphics.FromImage(bitmap))
                using (Graphics gc = Graphics.FromHwnd(ActiveForm.Handle))
                {

                    IntPtr hdcDest = IntPtr.Zero;
                    IntPtr hdcSrc = IntPtr.Zero;

                    try
                    {
                        hdcDest = gb.GetHdc();
                        hdcSrc = gc.GetHdc();

                        BitBlt(hdcDest, 0, 0, bitmap.Width, bitmap.Height, hdcSrc, 0, 0, SRC_COPY);
                    }
                    finally
                    {
                        if (hdcDest != IntPtr.Zero) gb.ReleaseHdc(hdcDest);
                        if (hdcSrc != IntPtr.Zero) gc.ReleaseHdc(hdcSrc);
                    }
                }

                bitmap.Save(strSaveImageName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        void button_SendResets_Click(object sender, EventArgs e)
        {
            byte[] gwConf = new byte[1];
            gwConf[0] = (byte)(
                ((checkBox_RstResetCounters.Checked ? 1 : 0)    << 0) |
                ((checkBox_RstHCR.Checked ? 1 : 0)              << 1) |
                ((checkBox_RstHisto.Checked ? 1 : 0)            << 2) |
                ((checkBox_RstPSC.Checked ? 1 : 0)              << 3) |
                ((checkBox_RstSR.Checked ? 1 : 0)               << 4) |
                ((checkBox_RstPA.Checked ? 1 : 0)               << 5) |
                ((checkBox_RstASICTrigs.Checked ? 1 : 0)        << 6) |
                ((checkBox_RstReadReg.Checked ? 1 : 0)          << 7));

            try
            {
                if (connectStatus == 1)
                {
                    protoCubes.SendCommand(ProtoCubesSerial.Command.SendGatewareConf,
                        gwConf);
                    button_SendResets.BackColor = WeerocGreen;
                }
                else
                {
                    throw new Exception("Please connect to an instrument " +
                        "using the \"Connect\" tab.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send gateware configuration " +
                    "to Proto-CUBES!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                button_SendResets.BackColor = Color.IndianRed;
            }
            tmrButtonColor.Enabled = true;
        }


        private void button_ClearArduinoSD_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Please confirm that you want to delete all SD " +
                "card files (and this button was not cliked in error).",
                "Confirm SD card delete",
                MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes) {
                try
                {
                    if (connectStatus == 1)
                    {
                        protoCubes.SendCommand(ProtoCubesSerial.Command.DelFiles, null);
                        button_ClearArduinoSD.BackColor = WeerocGreen;
                    }
                    else
                    {
                        throw new Exception("Please connect to an instrument " +
                            "using the \"Connect\" tab.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send \"Delete SD Card Files\" command " +
                        "to Proto-CUBES!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + "Error message:"
                        + Environment.NewLine
                        + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    button_ClearArduinoSD.BackColor = Color.IndianRed;
                }
                tmrButtonColor.Enabled = true;
            }
        }

        private void tmrButtonColor_Tick(object sender, EventArgs e)
        {
            tmrButtonColor.Enabled = false;

            button_sendSC.BackColor = Color.Gainsboro;
            button_sendSC.ForeColor = SystemColors.ControlText;

            button_sendProbes.BackColor = Color.Gainsboro;
            button_sendProbes.ForeColor = SystemColors.ControlText;

            button_SendResets.BackColor = SystemColors.Control;
            button_SendResets.ForeColor = SystemColors.ControlText;

            button_startAcquisition.BackColor = Color.Gainsboro;
            button_startAcquisition.ForeColor = SystemColors.ControlText;

            button_ClearArduinoSD.BackColor = Color.Gainsboro;
            button_ClearArduinoSD.ForeColor = SystemColors.ControlText;
        }

        private void label_help_TextChanged(object sender, EventArgs e)
        {
            /// Use the writing to label_help in Proto-CUBES mode to refresh the
            /// data chart. This is needed to avoid threading issues.
            if ((selectedConnectionMode == 1) &&
                    label_help.Text.Contains("Writing DAQ data to"))
            {
                refreshDataChart();
            }
        }

        private void Citiroc_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Quit early if not Proto-CUBES or we're not connected...
            if (selectedConnectionMode != 1 ||
                connectStatus != 1)
            {
                return;
            }

            /// Send DAQ stop to Proto - CUBES on form close, just to make
            /// sure experiment is not left in a running DAQ state...
            try
            {
                protoCubes.SendCommand(ProtoCubesSerial.Command.DAQStop, null);
                protoCubes.ClosePort();
            }
            catch { /* Blindly close... */}
        }
    }
}
