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
        public ProtoCubesNumBinsForm()
        {
            InitializeComponent();
        }

        private void ProtoCubesNumBinsForm_Load(object sender, EventArgs e)
        {
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

        private void checkBox_Ch0_hgVarBin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxes();
        }

        private void checkBox_Ch0_lgVarBin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxes();
        }

        private void checkBox_Ch16_hgVarBin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxes();
        }

        private void checkBox_Ch16_lgVarBin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxes();
        }

        private void checkBox_Ch31_hgVarBin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxes();
        }

        private void checkBox_Ch31_lgVarBin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxes();
        }
    }
}
