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
    public partial class ConfigIdInputForm : Form
    {
        public static string configNum = "";
        public ConfigIdInputForm()
        {
            InitializeComponent();
        }

        public static DialogResult InputForm(ref string value)
        {
            Form form = new ConfigIdInputForm();
            DialogResult dialogResult = form.ShowDialog();
            value = configNum;
            return dialogResult;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            label.Text = "Enter configuration number";
            try
            {
                if (Convert.ToUInt32(textBox.Text, 10) > 255)
                {
                    label.Text = "Input too large, must be between 0-255";
                    AcceptButton.Enabled = false;
                }
                else
                {
                    configNum = textBox.Text;
                    AcceptButton.Enabled = true;
                }
            }
            catch
            {
                label.Text = "Invalid data type!";
                AcceptButton.Enabled = false;
            }
        }

    }
}
