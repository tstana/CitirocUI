using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;

namespace CitirocUI
{
    public static class SC
    {
        static Color WeerocBlack = Color.FromArgb(255, 30, 30, 28);
        static Color WeerocDarkGreen = Color.FromArgb(255, 0, 83, 52);
        static Color WeerocGreen = Color.FromArgb(255, 29, 121, 104);
        static Color WeerocPaleBlue = Color.FromArgb(255, 0, 121, 144);
        static Color WeerocDarkBlue = Color.FromArgb(255, 34, 32, 70);
        static Color WeerocBlue = Color.FromArgb(255, 2, 65, 103);

        public static int ShapingTime(System.Windows.Forms.TextBox txtBox_shaper, double R, double C, int nbBits)
        {
            double step = R * C;
            return ValuetoCode(txtBox_shaper, step, nbBits, 1e-9, 1, 0);
        }

        public static int DelayTime(System.Windows.Forms.TextBox txtBox_delay, double vth, double cap, double ibslope, int nbBits)
        {
            double step = vth * cap / ibslope;
            return ValuetoCode(txtBox_delay, step, nbBits, 1e-9, 1, 0);
        }

        public static int DAC(System.Windows.Forms.TextBox txtBox_dac, System.Windows.Forms.Label lbl_dac, double DynamicRange, int nbBits, double baseValue)
        {
            int code = 0;
            double tmpCode = 0;
            double value = baseValue;

            try
            {
                tmpCode = double.Parse(txtBox_dac.Text);
                if (tmpCode > Math.Pow(2, nbBits) - 1)
                {
                    tmpCode = Math.Pow(2, nbBits) - 1;
                    txtBox_dac.Text = (Math.Pow(2, nbBits) - 1).ToString();
                }
                else if (tmpCode < 0)
                {
                    tmpCode = 0;
                    txtBox_dac.Text = "0";
                }
                double step = DynamicRange / (Math.Pow(2, nbBits) - 1);
                value = baseValue + tmpCode * step;
                txtBox_dac.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                txtBox_dac.Text = "NaN";
                txtBox_dac.ForeColor = Color.Red; //Change the color to Red when NaN
            }
            code = Convert.ToInt32(tmpCode);
            lbl_dac.Text = "≈ " + (Math.Round(value * 100) / 100) + " V";
            return code;
        }

        public static int PreampGain(System.Windows.Forms.TextBox txtBox_paGain, System.Windows.Forms.Label lbl_paGain, double Cin, double Cfb, int nbBits)
        {
            double value = 0;
            double tmpCode = 0;
            double maxValue = Cin / Cfb;
            int code = 0;

            try //To avoid an error if the value entered is not a number, we use the try + catch(Exception)
            {
                value = double.Parse(txtBox_paGain.Text); //Converting the number in the text box (which is considered as a string) into a double

                tmpCode = Math.Round(maxValue / value);
                if (tmpCode > Math.Pow(2, nbBits) - 1) tmpCode = Math.Pow(2, nbBits) - 1;
                else if (tmpCode < 1) tmpCode = 1;

                value = maxValue / tmpCode;

                if (value < 30) txtBox_paGain.Text = (Math.Round(value * 100) / 100).ToString();
                else txtBox_paGain.Text = Math.Round(value).ToString();
                txtBox_paGain.ForeColor = Color.Black; //Change the color to black when validated
            }
            catch (Exception) //If the value is not a number, then write "NaN" in the text box
            {
                txtBox_paGain.Text = "NaN";
                txtBox_paGain.ForeColor = Color.Red; //Change the color to Red when NaN
            }
            code = Convert.ToInt32(tmpCode);
            lbl_paGain.Text = String.Format("(" + code.ToString() + " x " + (Cfb * 1e15) + " fF)");
            return code;
        }

        public static int ValuetoCode(System.Windows.Forms.TextBox DESIGN_NAME, double step, int nbBits, double unit, double precision, double baseValue)
        /*
        unit = 1e-3 if milli, 1e-6 if micro, 1e-9 if nano etc. precision = 1 for integers, 1e-1 for 1 decimal, 1e-2 for 2 decimals etc. 
        BaseValue is 0.5 for the input DAC for example because it's base value is 500 mV with DAC code 00000000.
        */
        {
            double value = 0;
            double tmpCode = 0;
            int code = 0;

            try //To avoid an error if the value entered is not a number, we use the try + catch(Exception)
            {
                value = double.Parse(DESIGN_NAME.Text) * unit; //Converting the number in the text box (which is considered as a string) into a double.

                tmpCode = Math.Round((value - baseValue) / step);
                if (tmpCode > Math.Pow(2, nbBits) - 1) tmpCode = Math.Pow(2, nbBits) - 1;
                else if (tmpCode < 1) tmpCode = 1;

                value = step * tmpCode + baseValue;
                code = Convert.ToInt32(tmpCode);

                DESIGN_NAME.Text = (Math.Round(value * (1 / unit) * (1 / precision)) * precision).ToString();
            }
            catch //If the value is not a number, then write "0" in the text box
            {
                DESIGN_NAME.Text = "0";
            }
            return code;
        }

        public static int DIS(System.Windows.Forms.CheckBox en_Block, System.Windows.Forms.Button button_sendSC) // !!! 0 is enable !!!
        {
            if (en_Block.Checked == false)
            {
                en_Block.Text = "EN";
                en_Block.ForeColor = Color.White;
                en_Block.BackColor = WeerocGreen;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                    return 0;
            }
            else
            {
                en_Block.Text = "DIS";
                en_Block.ForeColor = Color.White;
                en_Block.FlatAppearance.CheckedBackColor = Color.IndianRed;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 1;
            }
        }

        public static int EN(System.Windows.Forms.CheckBox en_Block, System.Windows.Forms.Button button_sendSC)
        {
            if (en_Block.Checked == true)
            {
                en_Block.Text = "EN";
                en_Block.ForeColor = Color.White;
                en_Block.FlatAppearance.CheckedBackColor = WeerocGreen;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 1;
            }
            else
            {
                en_Block.Text = "DIS";
                en_Block.ForeColor = Color.White;
                en_Block.BackColor = Color.IndianRed;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 0;
            }
        }

        public static int ON(System.Windows.Forms.CheckBox en_Block, System.Windows.Forms.Button button_sendSC)
        {
            if (en_Block.Checked == true)
            {
                en_Block.Text = "ON";
                en_Block.ForeColor = Color.White;
                en_Block.FlatAppearance.CheckedBackColor = WeerocGreen;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 1;
            }
            else
            {
                en_Block.Text = "OFF";
                en_Block.ForeColor = Color.White;
                en_Block.BackColor = Color.IndianRed;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 0;
            }
        }

        public static int OFF(System.Windows.Forms.CheckBox en_Block, System.Windows.Forms.Button button_sendSC)
        {
            if (en_Block.Checked == false)
            {
                en_Block.Text = "ON";
                en_Block.ForeColor = Color.White;
                en_Block.BackColor = WeerocGreen;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 0;
            }
            else
            {
                en_Block.Text = "OFF";
                en_Block.ForeColor = Color.White;
                en_Block.FlatAppearance.CheckedBackColor = Color.IndianRed;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 1;
            }
        }

        public static int Switch(System.Windows.Forms.CheckBox chkBox_switch, string value0, string value1, System.Windows.Forms.Button button_sendSC)
        {
            if (chkBox_switch.Checked == true)
            {
                chkBox_switch.Text = value1;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 1;
            }
            else
            {
                chkBox_switch.Text = value0;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 0;
            }
        }

        public static int Switch(System.Windows.Forms.CheckBox chkBox_switch, System.Windows.Forms.Button button_sendSC)
        {
            if (chkBox_switch.Checked == true)
            {
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 1;
            }
            else
            {
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 0;
            }
        }

        public static int PP(System.Windows.Forms.CheckBox pp_Block, System.Windows.Forms.Button button_sendSC)
        {
            if (pp_Block.Checked == false)
            {
                pp_Block.Text = "PP";
                pp_Block.ForeColor = Color.White;
                pp_Block.BackColor = WeerocPaleBlue;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 0;
            }
            else
            {
                pp_Block.Text = "Ctn";
                pp_Block.ForeColor = Color.White;
                pp_Block.FlatAppearance.CheckedBackColor = Color.Tan;
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }
                return 1;
            }
        }

        public static int txtBoxSC(System.Windows.Forms.TextBox txtBox, int nbBits, System.Windows.Forms.Button button_sendSC)
        {
            try
            {
                int temp_sc = 0;
                temp_sc = Convert.ToInt32(txtBox.Text);
                if (temp_sc > (Math.Pow(2, nbBits) - 1)) temp_sc = Convert.ToInt32(Math.Pow(2, nbBits)) - 1;
                if (temp_sc < 0) temp_sc = 0;

                txtBox.Text = temp_sc.ToString();

                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }

                return temp_sc;
            }
            catch //If the value is not a number, then write "0" in the text box
            {
                txtBox.Text = "0";
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }

                return 0;
            }
        }

        public static int txtBoxSC(System.Windows.Forms.TextBox txtBox, int nbBits, bool activeLow, System.Windows.Forms.Button button_sendSC)
        {
            try
            {
                int temp_sc = 0;
                temp_sc = Convert.ToInt32(txtBox.Text);
                if (temp_sc > (Math.Pow(2, nbBits) - 1)) temp_sc = Convert.ToInt32(Math.Pow(2, nbBits)) - 1;
                if (temp_sc < 0) temp_sc = 0;

                txtBox.Text = temp_sc.ToString();

                if (activeLow) temp_sc = (int)(Math.Pow(2, nbBits) - 1) - temp_sc;

                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }

                return temp_sc;
            }
            catch //If the value is not a number, then write "0" in the text box
            {
                txtBox.Text = "0";
                if (button_sendSC.BackColor != Color.Gainsboro) { button_sendSC.BackColor = Color.Gainsboro; button_sendSC.ForeColor = Color.Black; }

                return 0;
            }
        }
    }
}