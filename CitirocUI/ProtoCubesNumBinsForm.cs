using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CitirocUI
{
    public partial class ProtoCubesNumBinsForm : Form
    {
        private const int NumBinsMin = 7;       // 2048 >> NumBinsMin
        private readonly string[] comboVar = { "log scale", "log"};

        public int[] IndexArray = {0,0,0,0,0,0};

        private bool update = false;

        public ProtoCubesNumBinsForm()
        {
            InitializeComponent();
        }

        private void ProtoCubesNumBinsForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < NumBinsMin; i++)
            {
                int numBins = 2048 >> i;
                comboBox_Ch0_hgNumBinsFixed.Items.Add(numBins);
                comboBox_Ch0_lgNumBinsFixed.Items.Add(numBins);
                comboBox_Ch16_hgNumBinsFixed.Items.Add(numBins);
                comboBox_Ch16_lgNumBinsFixed.Items.Add(numBins);
                comboBox_Ch31_hgNumBinsFixed.Items.Add(numBins);
                comboBox_Ch31_lgNumBinsFixed.Items.Add(numBins);
            }

            for (int i = 0; i < comboVar.Length; i++)
            {
                comboBox_Ch0_hgNumBinsVar.Items.Add(comboVar[i]);
                comboBox_Ch0_lgNumBinsVar.Items.Add(comboVar[i]);
                comboBox_Ch16_hgNumBinsVar.Items.Add(comboVar[i]);
                comboBox_Ch16_lgNumBinsVar.Items.Add(comboVar[i]);
                comboBox_Ch31_hgNumBinsVar.Items.Add(comboVar[i]);
                comboBox_Ch31_lgNumBinsVar.Items.Add(comboVar[i]);
            }

            comboBox_Ch0_hgNumBinsFixed.SelectedIndex = 0;
            comboBox_Ch0_lgNumBinsFixed.SelectedIndex = 0;
            comboBox_Ch16_hgNumBinsFixed.SelectedIndex = 0;
            comboBox_Ch16_lgNumBinsFixed.SelectedIndex = 0;
            comboBox_Ch31_hgNumBinsFixed.SelectedIndex = 0;
            comboBox_Ch31_lgNumBinsFixed.SelectedIndex = 0;
            comboBox_Ch0_hgNumBinsVar.SelectedIndex = 0;
            comboBox_Ch0_lgNumBinsVar.SelectedIndex = 0;
            comboBox_Ch16_hgNumBinsVar.SelectedIndex = 0;
            comboBox_Ch16_lgNumBinsVar.SelectedIndex = 0;
            comboBox_Ch31_hgNumBinsVar.SelectedIndex = 0;
            comboBox_Ch31_lgNumBinsVar.SelectedIndex = 0;

            checkBox_Ch0_lgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch16_hgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch16_lgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch31_hgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch31_lgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);

            UpdateControlValues();
        }

        private void UpdateComboBoxes()
        {
            comboBox_Ch0_hgNumBinsFixed.Enabled = !checkBox_Ch0_hgVarBin.Checked;
            comboBox_Ch0_hgNumBinsVar.Enabled =  checkBox_Ch0_hgVarBin.Checked;
            comboBox_Ch0_lgNumBinsFixed.Enabled = !checkBox_Ch0_lgVarBin.Checked;
            comboBox_Ch0_lgNumBinsVar.Enabled =  checkBox_Ch0_lgVarBin.Checked;

            comboBox_Ch16_hgNumBinsFixed.Enabled = !checkBox_Ch16_hgVarBin.Checked;
            comboBox_Ch16_hgNumBinsVar.Enabled = checkBox_Ch16_hgVarBin.Checked;
            comboBox_Ch16_lgNumBinsFixed.Enabled = !checkBox_Ch16_lgVarBin.Checked;
            comboBox_Ch16_lgNumBinsVar.Enabled = checkBox_Ch16_lgVarBin.Checked;

            comboBox_Ch31_hgNumBinsFixed.Enabled = !checkBox_Ch31_hgVarBin.Checked;
            comboBox_Ch31_hgNumBinsVar.Enabled = checkBox_Ch31_hgVarBin.Checked;
            comboBox_Ch31_lgNumBinsFixed.Enabled = !checkBox_Ch31_lgVarBin.Checked;
            comboBox_Ch31_lgNumBinsVar.Enabled = checkBox_Ch31_lgVarBin.Checked;
        }

         private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxes();
        }

        const string CfgHeader = "BinNumConfigData";
        private void button_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                //InitialDirectory = @"D:\",
                Title = "Browse Configuration Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "conf",
                Filter = "config files (*.conf)|*.conf",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //textBox1.Text = openFileDialog1.FileName;
                using (StreamReader file = new StreamReader(openFileDialog1.FileName))
                {
                    string ln;

                    if ((ln = file.ReadLine()) != null)
                    {
                        if(ln.Contains(CfgHeader))
                        {
                            ln = file.ReadLine();
                            int[] nums = Array.ConvertAll(ln.Split(','), int.Parse);

                            if(nums.Length != 6)
                            {
                                // ERROR
                                return;
                            }

                            IndexArray = nums;
                            UpdateControlValues();
                        }
                    }
                }
            }
              
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            string strSaveFileName = "";
            SaveFileDialog ScSaveDialog = new SaveFileDialog();
            ScSaveDialog.Title = "Specify Output file";
            //ScSaveDialog.Filter = "All Files(*.*)|*.*";
            ScSaveDialog.DefaultExt = "conf";
            ScSaveDialog.Filter = "config files (*.conf)|*.conf";
            ScSaveDialog.RestoreDirectory = true;

            if (ScSaveDialog.ShowDialog() == DialogResult.OK)
                strSaveFileName = ScSaveDialog.FileName;
            else return;
            
            TextWriter tw = new StreamWriter(strSaveFileName);
            tw.WriteLine(CfgHeader);
            tw.WriteLine(string.Join(",", IndexArray));
            tw.Close();
        }

        private void button_Apply_Click(object sender, EventArgs e)
        {
            
            
            this.Close();
        }

        private void comboBox_Ch0_hgNumBinsFixed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (update)
            {
                int idx = comboBox_Ch0_hgNumBinsFixed.SelectedIndex;
                IndexArray[0] = idx;
            }
        }

        private void comboBox_Ch0_hgNumBinsVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[0] = comboBox_Ch0_hgNumBinsVar.SelectedIndex + 11;
        }

        private void comboBox_Ch0_lgNumBinsFixed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[1] = comboBox_Ch0_lgNumBinsFixed.SelectedIndex;
        }

        private void comboBox_Ch0_lgNumBinsVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[1] = comboBox_Ch0_lgNumBinsVar.SelectedIndex + 11;
        }

        private void comboBox_Ch16_hgNumBinsFixed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[2] = comboBox_Ch16_hgNumBinsFixed.SelectedIndex;
        }

        private void comboBox_Ch16_hgNumBinsVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[2] = comboBox_Ch16_hgNumBinsVar.SelectedIndex + 11;
        }

        private void comboBox_Ch16_lgNumBinsFixed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[3] = comboBox_Ch16_lgNumBinsFixed.SelectedIndex;
        }

        private void comboBox_Ch16_lgNumBinsVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[3] = comboBox_Ch16_lgNumBinsVar.SelectedIndex + 11;
        }

        private void comboBox_Ch31_hgNumBinsFixed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[4] = comboBox_Ch31_hgNumBinsFixed.SelectedIndex;
        }

        private void comboBox_Ch31_hgNumBinsVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[4] = comboBox_Ch31_hgNumBinsVar.SelectedIndex + 11;
        }

        private void comboBox_Ch31_lgNumBinsFixed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[5] = comboBox_Ch31_lgNumBinsFixed.SelectedIndex;
        }

        private void comboBox_Ch31_lgNumBinsVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(update)
                IndexArray[5] = comboBox_Ch31_lgNumBinsVar.SelectedIndex + 11;
        }

        private void UpdateControlValues()
        {
            //// load indexes, order:
            update = false;

            bool mod = true;
            if (IndexArray[0] <= 7)
            {
                comboBox_Ch0_hgNumBinsFixed.SelectedIndex = IndexArray[0];
                comboBox_Ch0_hgNumBinsVar.SelectedIndex = 0;
            }
            else if (IndexArray[0] >= 11)
            {
                comboBox_Ch0_hgNumBinsFixed.SelectedIndex = 0;
                comboBox_Ch0_hgNumBinsVar.SelectedIndex = IndexArray[0] - 11;
                mod = false;
            }
            comboBox_Ch0_hgNumBinsFixed.Enabled = mod;
            comboBox_Ch0_hgNumBinsVar.Enabled = !mod;
            checkBox_Ch0_hgVarBin.Checked = !mod;

            if (IndexArray[1] <= 7)
            {
                comboBox_Ch0_lgNumBinsFixed.SelectedIndex = IndexArray[1];
                comboBox_Ch0_lgNumBinsVar.SelectedIndex = 0;
                mod = true;
            }
            else if (IndexArray[1] >= 11)
            {
                comboBox_Ch0_lgNumBinsFixed.SelectedIndex = 0;
                comboBox_Ch0_lgNumBinsVar.SelectedIndex = IndexArray[1] - 11;
                mod = false;
            }
            comboBox_Ch0_lgNumBinsFixed.Enabled = mod;
            comboBox_Ch0_lgNumBinsVar.Enabled = !mod;
            checkBox_Ch0_lgVarBin.Checked = !mod;

            if (IndexArray[2] <= 7)
            {
                comboBox_Ch16_hgNumBinsFixed.SelectedIndex = IndexArray[2];
                comboBox_Ch16_hgNumBinsVar.SelectedIndex = 0;
                mod = true;
            }
            else if (IndexArray[2] >= 11)
            {
                comboBox_Ch16_hgNumBinsFixed.SelectedIndex = 0;
                comboBox_Ch16_hgNumBinsVar.SelectedIndex = IndexArray[2] - 11;
                mod = false;
            }
            comboBox_Ch16_hgNumBinsFixed.Enabled = mod;
            comboBox_Ch16_hgNumBinsVar.Enabled = !mod;
            checkBox_Ch16_hgVarBin.Checked = !mod;

            if (IndexArray[3] <= 7)
            {
                comboBox_Ch16_lgNumBinsFixed.SelectedIndex = IndexArray[3];
                comboBox_Ch16_lgNumBinsVar.SelectedIndex = 0;
                mod = true;
            }
            else if (IndexArray[3] >= 11)
            {
                comboBox_Ch16_lgNumBinsFixed.SelectedIndex = 0;
                comboBox_Ch16_lgNumBinsVar.SelectedIndex = IndexArray[3] - 11;
                mod = false;
            }
            comboBox_Ch16_lgNumBinsFixed.Enabled = mod;
            comboBox_Ch16_lgNumBinsVar.Enabled = !mod;
            checkBox_Ch16_lgVarBin.Checked = !mod;

            if (IndexArray[4] <= 7)
            {
                comboBox_Ch31_hgNumBinsFixed.SelectedIndex = IndexArray[4];
                comboBox_Ch31_hgNumBinsVar.SelectedIndex = 0;
                mod = true;
            }
            else if (IndexArray[4] >= 11)
            {
                comboBox_Ch31_hgNumBinsFixed.SelectedIndex = 0;
                comboBox_Ch31_hgNumBinsVar.SelectedIndex = IndexArray[4] - 11;
                mod = false;
            }
            comboBox_Ch31_hgNumBinsFixed.Enabled = mod;
            comboBox_Ch31_hgNumBinsVar.Enabled = !mod;
            checkBox_Ch31_hgVarBin.Checked = !mod;

            if (IndexArray[5] <= 7)
            {
                comboBox_Ch31_lgNumBinsFixed.SelectedIndex = IndexArray[5];
                comboBox_Ch31_lgNumBinsVar.SelectedIndex = 0;
                mod = true;
            }
            else if (IndexArray[5] >= 11)
            {
                comboBox_Ch31_lgNumBinsFixed.SelectedIndex = 0;
                comboBox_Ch31_lgNumBinsVar.SelectedIndex = IndexArray[5] - 11;
                mod = false;
            }
            comboBox_Ch31_lgNumBinsFixed.Enabled = mod;
            comboBox_Ch31_lgNumBinsVar.Enabled = !mod;
            checkBox_Ch31_lgVarBin.Checked = !mod;

            update = true;
        }
    }
}
