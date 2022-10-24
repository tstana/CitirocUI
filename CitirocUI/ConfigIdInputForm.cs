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
        public static uint conf_id_min = 0;
        public static uint conf_id = 0;

        public ConfigIdInputForm()
        {
            InitializeComponent();
        }

        public static DialogResult InputForm(uint min, ref uint val)
        {
            conf_id_min = min;
            Form form = new ConfigIdInputForm();
            DialogResult dialogResult = form.ShowDialog();
            val = conf_id;
            return dialogResult;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                uint userInput = Convert.ToUInt32(textBox.Text, 10);
                if ((userInput < conf_id_min) || (userInput > 254))
                {
                    MessageBox.Show("CONF_ID must be between " +
                        conf_id_min.ToString() + " and 254",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AcceptButton.Enabled = false;
                }
                else
                {
                    conf_id = userInput;
                    AcceptButton.Enabled = true;
                }
            }
            catch
            {
                MessageBox.Show("CONF_ID must be a number between " +
                        conf_id_min.ToString() + " and 254",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AcceptButton.Enabled = false;
            }
        }

    }
}
