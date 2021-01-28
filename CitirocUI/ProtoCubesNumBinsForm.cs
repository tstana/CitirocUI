using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        
        public int[] indexArray = new int[6];


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

            checkBox_Ch0_lgVarBin.CheckedChanged +=new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch16_hgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch16_lgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch31_hgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
            checkBox_Ch31_lgVarBin.CheckedChanged += new EventHandler(checkBox_CheckedChanged);

            UpdateComboBoxes();
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
    }
}
