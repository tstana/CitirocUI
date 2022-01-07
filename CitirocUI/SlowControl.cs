using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace CitirocUI
{
    public partial class Citiroc : Form
    {
        #region slow control parameters declaration
        int[] sc_calibDacT = new int[NbChannels];
        int[] sc_calibDacQ = new int[NbChannels];
        int sc_enDiscri;
        int sc_ppDiscri;
        int sc_latchDiscri;
        int sc_enDiscriT;
        int sc_ppDiscriT;
        int sc_enCalibDacQ;
        int sc_ppCalibDacQ;
        int sc_enCalibDacT;
        int sc_ppCalibDacT;
        int[] sc_mask = new int[NbChannels];
        int sc_ppThHg;
        int sc_enThHg;
        int sc_ppThLg;
        int sc_enThLg;
        int sc_biasSca;
        int sc_ppPdetHg;
        int sc_enPdetHg;
        int sc_ppPdetLg;
        int sc_enPdetLg;
        int sc_scaOrPdHg;
        int sc_scaOrPdLg;
        int sc_bypassPd;
        int sc_selTrigExtPd;
        int sc_ppFshBuffer;
        int sc_enFsh;
        int sc_ppFsh;
        int sc_ppSshLg;
        int sc_enSshLg;
        int sc_shapingTimeLg;
        int sc_ppSshHg;
        int sc_enSshHg;
        int sc_shapingTimeHg;
        int sc_paLgBias;
        int sc_ppPaLg;
        int sc_enPaLg;
        int sc_ppPaHg;
        int sc_enPaHg;
        int sc_fshOnLg;
        int sc_enInputDac;
        int sc_dacRef;
        int[] sc_inputDac = new int[NbChannels];
        int[] sc_cmdInputDac = new int[NbChannels];
        int[] sc_paHgGain = new int[NbChannels];
        int[] sc_paLgGain = new int[NbChannels];
        int[] sc_CtestHg = new int[NbChannels];
        int[] sc_CtestLg = new int[NbChannels];
        int[] sc_enPa = new int[NbChannels];
        int sc_ppTemp;
        int sc_enTemp;
        int sc_ppBg;
        int sc_enBg;
        int sc_enThresholdDac1;
        int sc_ppThresholdDac1;
        int sc_enThresholdDac2;
        int sc_ppThresholdDac2;
        int sc_threshold1;
        int sc_threshold2;
        int sc_enHgOtaQ;
        int sc_ppHgOtaQ;
        int sc_enLgOtaQ;
        int sc_ppLgOtaQ;
        int sc_enProbeOtaQ;
        int sc_ppProbeOtaQ;
        int sc_testBitOtaQ;
        int sc_enValEvtReceiver;
        int sc_ppValEvtReceiver;
        int sc_enRazChnReceiver;
        int sc_ppRazChnReceiver;
        int sc_enDigitalMuxOutput;
        int sc_enOr32;
        int sc_enNor32Oc;
        int sc_triggerPolarity;
        int sc_enNor32TOc;
        int sc_enTriggersOutput;

        private static string strDefSC = "1110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111011101110111111111111111111111111111111111111111111111011111100111111101111001111011100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001100000001000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000000100000000000111111110100101100010010110011111111111111011";
        string strSC = strDefSC;
        #endregion

        #region drag n drop slow control
        void slowControl_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the Dataformat of the data can be accepted
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // Okay
            else
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
        }

        void slowControl_DragDrop(object sender, DragEventArgs e)
        {
            // Extract the data from the DataObject-Container into a string list
            string[] File = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ArrayList dataArrayList;
            dataArrayList = ReadFileLine(File[0]);
            FileInfo fi = new FileInfo(File[0]);

            if (dataArrayList[0].ToString().Length == strDefSC.Length)
            {
                try
                {
                    setSC(dataArrayList[0].ToString());
                    DialogResult res = MessageBox.Show(fi.Name + " has been loaded as slow control configuration. Do you wish to send the slow control parameters to the Citiroc ASIC ?", "", MessageBoxButtons.YesNo);

                    if (res == DialogResult.Yes)
                    {
                        //radioButton_slowControl.PerformClick();
                        //button_sendSC.PerformClick();
                        button_sendSC_Click(null, null);
                    }
                    else
                    {
                        button_sendSC.BackColor = Color.Gainsboro;
                        button_sendSC.ForeColor = Color.Black;
                    }
                }
                catch
                {
                    MessageBox.Show("Wrong file format.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Wrong file format.");
                return;
            }
        }
        #endregion

        #region slow control management

        public string getSC() // ASIC specific
        {
            strSC = "";

            for (int i = 0; i < NbChannels; i++)
                strSC += strRev(IntToBin(sc_calibDacT[i], 4));

            for (int i = 0; i < NbChannels; i++)
                strSC += strRev(IntToBin(sc_calibDacQ[i], 4));

            strSC += sc_enDiscri.ToString()
            + sc_ppDiscri.ToString()
            + sc_latchDiscri.ToString()
            + sc_enDiscriT.ToString()
            + sc_ppDiscriT.ToString()
            + sc_enCalibDacQ.ToString()
            + sc_ppCalibDacQ.ToString()
            + sc_enCalibDacT.ToString()
            + sc_ppCalibDacT.ToString();

            for (int i = 0; i < NbChannels; i++)
                strSC += sc_mask[i].ToString();

            strSC += sc_ppThHg.ToString()
            + sc_enThHg.ToString()
            + sc_ppThLg.ToString()
            + sc_enThLg.ToString()
            + sc_biasSca.ToString()
            + sc_ppPdetHg.ToString()
            + sc_enPdetHg.ToString()
            + sc_ppPdetLg.ToString()
            + sc_enPdetLg.ToString()
            + sc_scaOrPdHg.ToString()
            + sc_scaOrPdLg.ToString()
            + sc_bypassPd.ToString()
            + sc_selTrigExtPd.ToString()
            + sc_ppFshBuffer.ToString()
            + sc_enFsh.ToString()
            + sc_ppFsh.ToString()
            + sc_ppSshLg.ToString()
            + sc_enSshLg.ToString()
            + strRev(IntToBin(sc_shapingTimeLg, 3))
            + sc_ppSshHg.ToString()
            + sc_enSshHg.ToString()
            + strRev(IntToBin(sc_shapingTimeHg, 3))
            + sc_paLgBias.ToString()
            + sc_ppPaHg.ToString()
            + sc_enPaHg.ToString()
            + sc_ppPaLg.ToString()
            + sc_enPaLg.ToString()
            + sc_fshOnLg.ToString()
            + sc_enInputDac.ToString()
            + sc_dacRef.ToString();

            for (int i = 0; i < NbChannels; i++)
                strSC += IntToBin(sc_inputDac[i], 8) + sc_cmdInputDac[i].ToString();

            for (int i = 0; i < NbChannels; i++)
                strSC += IntToBin(sc_paHgGain[i], 6) + IntToBin(sc_paLgGain[i], 6) + sc_CtestHg[i].ToString() + sc_CtestLg[i].ToString() + sc_enPa[i].ToString();

            strSC += sc_ppTemp.ToString()
            + sc_enTemp.ToString()
            + sc_ppBg.ToString()
            + sc_enBg.ToString()
            + sc_enThresholdDac1.ToString()
            + sc_ppThresholdDac1.ToString()
            + sc_enThresholdDac2.ToString()
            + sc_ppThresholdDac2.ToString()
            + IntToBin(sc_threshold1, 10)
            + IntToBin(sc_threshold2, 10)
            + sc_enHgOtaQ.ToString()
            + sc_ppHgOtaQ.ToString()
            + sc_enLgOtaQ.ToString()
            + sc_ppLgOtaQ.ToString()
            + sc_enProbeOtaQ.ToString()
            + sc_ppProbeOtaQ.ToString()
            + sc_testBitOtaQ.ToString()
            + sc_enValEvtReceiver.ToString()
            + sc_ppValEvtReceiver.ToString()
            + sc_enRazChnReceiver.ToString()
            + sc_ppRazChnReceiver.ToString()
            + sc_enDigitalMuxOutput.ToString()
            + sc_enOr32.ToString()
            + sc_enNor32Oc.ToString()
            + sc_triggerPolarity.ToString()
            + sc_enNor32TOc.ToString()
            + sc_enTriggersOutput.ToString();

            return strSC;
        }

        public void setSC(string strSC) // ASIC specific
        {
            TextBox[] textBox_calibDac = {textBox_calibDac0, textBox_calibDac1, textBox_calibDac2, textBox_calibDac3, textBox_calibDac4, textBox_calibDac5, textBox_calibDac6, textBox_calibDac7, textBox_calibDac8, textBox_calibDac9,
                         textBox_calibDac10, textBox_calibDac11, textBox_calibDac12, textBox_calibDac13, textBox_calibDac14, textBox_calibDac15, textBox_calibDac16, textBox_calibDac17, textBox_calibDac18, textBox_calibDac19,
                         textBox_calibDac20, textBox_calibDac21, textBox_calibDac22, textBox_calibDac23, textBox_calibDac24, textBox_calibDac25, textBox_calibDac26, textBox_calibDac27, textBox_calibDac28, textBox_calibDac29,
                         textBox_calibDac30, textBox_calibDac31};

            TextBox[] textBox_calibDacT = {textBox_calibDacT0, textBox_calibDacT1, textBox_calibDacT2, textBox_calibDacT3, textBox_calibDacT4, textBox_calibDacT5, textBox_calibDacT6, textBox_calibDacT7, textBox_calibDacT8, textBox_calibDacT9,
                         textBox_calibDacT10, textBox_calibDacT11, textBox_calibDacT12, textBox_calibDacT13, textBox_calibDacT14, textBox_calibDacT15, textBox_calibDacT16, textBox_calibDacT17, textBox_calibDacT18, textBox_calibDacT19,
                         textBox_calibDacT20, textBox_calibDacT21, textBox_calibDacT22, textBox_calibDacT23, textBox_calibDacT24, textBox_calibDacT25, textBox_calibDacT26, textBox_calibDacT27, textBox_calibDacT28, textBox_calibDacT29,
                         textBox_calibDacT30, textBox_calibDacT31};

            CheckBox[] checkBox_mask = {checkBox_mask0, checkBox_mask1, checkBox_mask2, checkBox_mask3, checkBox_mask4, checkBox_mask5, checkBox_mask6, checkBox_mask7, checkBox_mask8, checkBox_mask9,
                         checkBox_mask10, checkBox_mask11, checkBox_mask12, checkBox_mask13, checkBox_mask14, checkBox_mask15, checkBox_mask16, checkBox_mask17, checkBox_mask18, checkBox_mask19,
                         checkBox_mask20, checkBox_mask21, checkBox_mask22, checkBox_mask23, checkBox_mask24, checkBox_mask25, checkBox_mask26, checkBox_mask27, checkBox_mask28, checkBox_mask29,
                         checkBox_mask30, checkBox_mask31};

            TextBox[] textBox_inputDac = {textBox_inputDac0, textBox_inputDac1, textBox_inputDac2, textBox_inputDac3, textBox_inputDac4, textBox_inputDac5, textBox_inputDac6, textBox_inputDac7, textBox_inputDac8, textBox_inputDac9,
                         textBox_inputDac10, textBox_inputDac11, textBox_inputDac12, textBox_inputDac13, textBox_inputDac14, textBox_inputDac15, textBox_inputDac16, textBox_inputDac17, textBox_inputDac18, textBox_inputDac19,
                         textBox_inputDac20, textBox_inputDac21, textBox_inputDac22, textBox_inputDac23, textBox_inputDac24, textBox_inputDac25, textBox_inputDac26, textBox_inputDac27, textBox_inputDac28, textBox_inputDac29,
                         textBox_inputDac30, textBox_inputDac31};

            CheckBox[] checkBox_cmdInputDac = {checkBox_cmdInputDac0, checkBox_cmdInputDac1, checkBox_cmdInputDac2, checkBox_cmdInputDac3, checkBox_cmdInputDac4, checkBox_cmdInputDac5, checkBox_cmdInputDac6, checkBox_cmdInputDac7, checkBox_cmdInputDac8, checkBox_cmdInputDac9,
                         checkBox_cmdInputDac10, checkBox_cmdInputDac11, checkBox_cmdInputDac12, checkBox_cmdInputDac13, checkBox_cmdInputDac14, checkBox_cmdInputDac15, checkBox_cmdInputDac16, checkBox_cmdInputDac17, checkBox_cmdInputDac18, checkBox_cmdInputDac19,
                         checkBox_cmdInputDac20, checkBox_cmdInputDac21, checkBox_cmdInputDac22, checkBox_cmdInputDac23, checkBox_cmdInputDac24, checkBox_cmdInputDac25, checkBox_cmdInputDac26, checkBox_cmdInputDac27, checkBox_cmdInputDac28, checkBox_cmdInputDac29,
                         checkBox_cmdInputDac30, checkBox_cmdInputDac31};

            TextBox[] textBox_paHgGain = {textBox_paHgGain0, textBox_paHgGain1, textBox_paHgGain2, textBox_paHgGain3, textBox_paHgGain4, textBox_paHgGain5, textBox_paHgGain6, textBox_paHgGain7, textBox_paHgGain8, textBox_paHgGain9,
                         textBox_paHgGain10, textBox_paHgGain11, textBox_paHgGain12, textBox_paHgGain13, textBox_paHgGain14, textBox_paHgGain15, textBox_paHgGain16, textBox_paHgGain17, textBox_paHgGain18, textBox_paHgGain19,
                         textBox_paHgGain20, textBox_paHgGain21, textBox_paHgGain22, textBox_paHgGain23, textBox_paHgGain24, textBox_paHgGain25, textBox_paHgGain26, textBox_paHgGain27, textBox_paHgGain28, textBox_paHgGain29,
                         textBox_paHgGain30, textBox_paHgGain31};

            TextBox[] textBox_paLgGain = {textBox_paLgGain0, textBox_paLgGain1, textBox_paLgGain2, textBox_paLgGain3, textBox_paLgGain4, textBox_paLgGain5, textBox_paLgGain6, textBox_paLgGain7, textBox_paLgGain8, textBox_paLgGain9,
                         textBox_paLgGain10, textBox_paLgGain11, textBox_paLgGain12, textBox_paLgGain13, textBox_paLgGain14, textBox_paLgGain15, textBox_paLgGain16, textBox_paLgGain17, textBox_paLgGain18, textBox_paLgGain19,
                         textBox_paLgGain20, textBox_paLgGain21, textBox_paLgGain22, textBox_paLgGain23, textBox_paLgGain24, textBox_paLgGain25, textBox_paLgGain26, textBox_paLgGain27, textBox_paLgGain28, textBox_paLgGain29,
                         textBox_paLgGain30, textBox_paLgGain31};

            CheckBox[] checkBox_CtestHg = {checkBox_CtestHg0, checkBox_CtestHg1, checkBox_CtestHg2, checkBox_CtestHg3, checkBox_CtestHg4, checkBox_CtestHg5, checkBox_CtestHg6, checkBox_CtestHg7, checkBox_CtestHg8, checkBox_CtestHg9,
                         checkBox_CtestHg10, checkBox_CtestHg11, checkBox_CtestHg12, checkBox_CtestHg13, checkBox_CtestHg14, checkBox_CtestHg15, checkBox_CtestHg16, checkBox_CtestHg17, checkBox_CtestHg18, checkBox_CtestHg19,
                         checkBox_CtestHg20, checkBox_CtestHg21, checkBox_CtestHg22, checkBox_CtestHg23, checkBox_CtestHg24, checkBox_CtestHg25, checkBox_CtestHg26, checkBox_CtestHg27, checkBox_CtestHg28, checkBox_CtestHg29,
                         checkBox_CtestHg30, checkBox_CtestHg31};

            CheckBox[] checkBox_CtestLg = {checkBox_CtestLg0, checkBox_CtestLg1, checkBox_CtestLg2, checkBox_CtestLg3, checkBox_CtestLg4, checkBox_CtestLg5, checkBox_CtestLg6, checkBox_CtestLg7, checkBox_CtestLg8, checkBox_CtestLg9,
                         checkBox_CtestLg10, checkBox_CtestLg11, checkBox_CtestLg12, checkBox_CtestLg13, checkBox_CtestLg14, checkBox_CtestLg15, checkBox_CtestLg16, checkBox_CtestLg17, checkBox_CtestLg18, checkBox_CtestLg19,
                         checkBox_CtestLg20, checkBox_CtestLg21, checkBox_CtestLg22, checkBox_CtestLg23, checkBox_CtestLg24, checkBox_CtestLg25, checkBox_CtestLg26, checkBox_CtestLg27, checkBox_CtestLg28, checkBox_CtestLg29,
                         checkBox_CtestLg30, checkBox_CtestLg31};

            CheckBox[] checkBox_enPa = {checkBox_enPa0, checkBox_enPa1, checkBox_enPa2, checkBox_enPa3, checkBox_enPa4, checkBox_enPa5, checkBox_enPa6, checkBox_enPa7, checkBox_enPa8, checkBox_enPa9,
                         checkBox_enPa10, checkBox_enPa11, checkBox_enPa12, checkBox_enPa13, checkBox_enPa14, checkBox_enPa15, checkBox_enPa16, checkBox_enPa17, checkBox_enPa18, checkBox_enPa19,
                         checkBox_enPa20, checkBox_enPa21, checkBox_enPa22, checkBox_enPa23, checkBox_enPa24, checkBox_enPa25, checkBox_enPa26, checkBox_enPa27, checkBox_enPa28, checkBox_enPa29,
                         checkBox_enPa30, checkBox_enPa31};

            for (int chn = 0; chn < NbChannels; chn++)
            {
                sc_calibDacT[chn] = Convert.ToInt32(strRev(strSC.Substring(chn * 4, 4)), 2);
                textBox_calibDacT[chn].Text = sc_calibDacT[chn].ToString();
            }

            for (int chn = 0; chn < NbChannels; chn++)
            {
                sc_calibDacQ[chn] = Convert.ToInt32(strRev(strSC.Substring(128 + chn * 4, 4)), 2);
                textBox_calibDac[chn].Text = sc_calibDacQ[chn].ToString();
            }

            sc_enDiscri = Convert.ToInt32(strSC.Substring(256, 1));
            checkBox_enDiscri.Checked = Convert.ToBoolean(sc_enDiscri);

            sc_ppDiscri = Convert.ToInt32(strSC.Substring(257, 1));
            checkBox_ppDiscri.Checked = Convert.ToBoolean(sc_ppDiscri);

            sc_latchDiscri = Convert.ToInt32(strSC.Substring(258, 1));
            checkBox_latchDiscri.Checked = Convert.ToBoolean(sc_latchDiscri);

            sc_enDiscriT = Convert.ToInt32(strSC.Substring(259, 1));
            checkBox_enDiscriT.Checked = Convert.ToBoolean(sc_enDiscriT);

            sc_ppDiscriT = Convert.ToInt32(strSC.Substring(260, 1));
            checkBox_ppDiscriT.Checked = Convert.ToBoolean(sc_ppDiscriT);

            sc_enCalibDacQ = Convert.ToInt32(strSC.Substring(261, 1));
            checkBox_enCalibDacQ.Checked = Convert.ToBoolean(sc_enCalibDacQ);

            sc_ppCalibDacQ = Convert.ToInt32(strSC.Substring(262, 1));
            checkBox_ppCalibDacQ.Checked = Convert.ToBoolean(sc_ppCalibDacQ);

            sc_enCalibDacT = Convert.ToInt32(strSC.Substring(263, 1));
            checkBox_enCalibDacT.Checked = Convert.ToBoolean(sc_enCalibDacT);

            sc_ppCalibDacT = Convert.ToInt32(strSC.Substring(264, 1));
            checkBox_ppCalibDacT.Checked = Convert.ToBoolean(sc_ppCalibDacT);

            for (int chn = 0; chn < NbChannels; chn++)
            {
                sc_mask[chn] = Convert.ToInt32(strSC.Substring(265 + chn, 1));
                checkBox_mask[chn].Checked = Convert.ToBoolean(sc_mask[chn]);
            }

            sc_ppThHg = Convert.ToInt32(strSC.Substring(297, 1));
            checkBox_ppThHg.Checked = Convert.ToBoolean(sc_ppThHg);

            sc_enThHg = Convert.ToInt32(strSC.Substring(298, 1));
            checkBox_enThHg.Checked = Convert.ToBoolean(sc_enThHg);

            sc_ppThLg = Convert.ToInt32(strSC.Substring(299, 1));
            checkBox_ppThLg.Checked = Convert.ToBoolean(sc_ppThLg);

            sc_enThLg = Convert.ToInt32(strSC.Substring(300, 1));
            checkBox_enThLg.Checked = Convert.ToBoolean(sc_enThLg);

            sc_biasSca = Convert.ToInt32(strSC.Substring(301, 1));
            checkBox_biasSca.Checked = Convert.ToBoolean(sc_biasSca);

            sc_ppPdetHg = Convert.ToInt32(strSC.Substring(302, 1));
            checkBox_ppPdetHg.Checked = Convert.ToBoolean(sc_ppPdetHg);

            sc_enPdetHg = Convert.ToInt32(strSC.Substring(303, 1));
            checkBox_enPdetHg.Checked = Convert.ToBoolean(sc_enPdetHg);

            sc_ppPdetLg = Convert.ToInt32(strSC.Substring(304, 1));
            checkBox_ppPdetLg.Checked = Convert.ToBoolean(sc_ppPdetLg);

            sc_enPdetLg = Convert.ToInt32(strSC.Substring(305, 1));
            checkBox_enPdetLg.Checked = Convert.ToBoolean(sc_enPdetLg);

            sc_scaOrPdHg = Convert.ToInt32(strSC.Substring(306, 1));
            checkBox_scaOrPdHg.Checked = Convert.ToBoolean(sc_scaOrPdHg);

            sc_scaOrPdLg = Convert.ToInt32(strSC.Substring(307, 1));
            checkBox_scaOrPdLg.Checked = Convert.ToBoolean(sc_scaOrPdLg);

            sc_bypassPd = Convert.ToInt32(strSC.Substring(308, 1));
            checkBox_bypassPd.Checked = Convert.ToBoolean(sc_bypassPd);

            sc_selTrigExtPd = Convert.ToInt32(strSC.Substring(309, 1));
            checkBox_selTrigExtPd.Checked = Convert.ToBoolean(sc_selTrigExtPd);

            sc_ppFshBuffer = Convert.ToInt32(strSC.Substring(310, 1));
            checkBox_ppFshBuffer.Checked = Convert.ToBoolean(sc_ppFshBuffer);

            sc_enFsh = Convert.ToInt32(strSC.Substring(311, 1));
            checkBox_enFsh.Checked = Convert.ToBoolean(sc_enFsh);

            sc_ppFsh = Convert.ToInt32(strSC.Substring(312, 1));
            checkBox_ppFsh.Checked = Convert.ToBoolean(sc_ppFsh);

            sc_ppSshLg = Convert.ToInt32(strSC.Substring(313, 1));
            checkBox_ppSshLg.Checked = Convert.ToBoolean(sc_ppSshLg);

            sc_enSshLg = Convert.ToInt32(strSC.Substring(314, 1));
            checkBox_enSshLg.Checked = Convert.ToBoolean(sc_enSshLg);

            sc_shapingTimeLg = Convert.ToInt32(strRev(strSC.Substring(315, 3)), 2);
            textBox_shapingTimeLg.Text = sc_shapingTimeLg.ToString();
            label_tauLg.Text = "τ = " + (87.5 - 12.5 * sc_shapingTimeLg) + " ns";

            sc_ppSshHg = Convert.ToInt32(strSC.Substring(318, 1));
            checkBox_ppSshHg.Checked = Convert.ToBoolean(sc_ppSshHg);

            sc_enSshHg = Convert.ToInt32(strSC.Substring(319, 1));
            checkBox_enSshHg.Checked = Convert.ToBoolean(sc_enSshHg);

            sc_shapingTimeHg = Convert.ToInt32(strRev(strSC.Substring(320, 3)), 2);
            textBox_shapingTimeHg.Text = sc_shapingTimeHg.ToString();
            label_tauHg.Text = "τ = " + (87.5 - 12.5 * sc_shapingTimeHg) + " ns";

            sc_paLgBias = Convert.ToInt32(strSC.Substring(323, 1));
            checkBox_paLgBias.Checked = Convert.ToBoolean(sc_paLgBias);

            sc_ppPaHg = Convert.ToInt32(strSC.Substring(324, 1));
            checkBox_ppPaHg.Checked = Convert.ToBoolean(sc_ppPaHg);

            sc_enPaHg = Convert.ToInt32(strSC.Substring(325, 1));
            checkBox_enPaHg.Checked = Convert.ToBoolean(sc_enPaHg);

            sc_ppPaLg = Convert.ToInt32(strSC.Substring(326, 1));
            checkBox_ppPaLg.Checked = Convert.ToBoolean(sc_ppPaLg);

            sc_enPaLg = Convert.ToInt32(strSC.Substring(327, 1));
            checkBox_enPaLg.Checked = Convert.ToBoolean(sc_enPaLg);

            sc_fshOnLg = Convert.ToInt32(strSC.Substring(328, 1));
            checkBox_fshOnLg.Checked = Convert.ToBoolean(sc_fshOnLg);

            sc_enInputDac = Convert.ToInt32(strSC.Substring(329, 1));
            checkBox_enInputDac.Checked = Convert.ToBoolean(sc_enInputDac);

            sc_dacRef = Convert.ToInt32(strSC.Substring(330, 1));
            checkBox_dacRef.Checked = Convert.ToBoolean(sc_dacRef);

            for (int chn = 0; chn < NbChannels; chn++)
            {
                sc_inputDac[chn] = Convert.ToInt32(strSC.Substring(331 + chn * 9, 8), 2);
                textBox_inputDac[chn].Text = sc_inputDac[chn].ToString();

                sc_cmdInputDac[chn] = Convert.ToInt32(strSC.Substring(339 + chn * 9, 1));
                checkBox_cmdInputDac[chn].Checked = Convert.ToBoolean(sc_cmdInputDac[chn]);
            }

            for (int chn = 0; chn < NbChannels; chn++)
            {
                sc_paHgGain[chn] = Convert.ToInt32(strSC.Substring(619 + chn * 15, 6), 2);
                textBox_paHgGain[chn].Text = sc_paHgGain[chn].ToString();

                sc_paLgGain[chn] = Convert.ToInt32(strSC.Substring(625 + chn * 15, 6), 2);
                textBox_paLgGain[chn].Text = sc_paLgGain[chn].ToString();

                sc_CtestHg[chn] = Convert.ToInt32(strSC.Substring(631 + chn * 15, 1));
                checkBox_CtestHg[chn].Checked = Convert.ToBoolean(sc_CtestHg[chn]);

                sc_CtestLg[chn] = Convert.ToInt32(strSC.Substring(632 + chn * 15, 1));
                checkBox_CtestLg[chn].Checked = Convert.ToBoolean(sc_CtestLg[chn]);

                sc_enPa[chn] = Convert.ToInt32(strSC.Substring(633 + chn * 15, 1));
                checkBox_enPa[chn].Checked = Convert.ToBoolean(sc_enPa[chn]);
            }

            sc_ppTemp = Convert.ToInt32(strSC.Substring(1099, 1));
            checkBox_ppTemp.Checked = Convert.ToBoolean(sc_ppTemp);

            sc_enTemp = Convert.ToInt32(strSC.Substring(1100, 1));
            checkBox_enTemp.Checked = Convert.ToBoolean(sc_enTemp);

            sc_ppBg = Convert.ToInt32(strSC.Substring(1101, 1));
            checkBox_ppBg.Checked = Convert.ToBoolean(sc_ppBg);

            sc_enBg = Convert.ToInt32(strSC.Substring(1102, 1));
            checkBox_enBg.Checked = Convert.ToBoolean(sc_enBg);

            sc_enThresholdDac1 = Convert.ToInt32(strSC.Substring(1103, 1));
            checkBox_enThresholdDac1.Checked = Convert.ToBoolean(sc_enThresholdDac1);

            sc_ppThresholdDac1 = Convert.ToInt32(strSC.Substring(1104, 1));
            checkBox_ppThresholdDac1.Checked = Convert.ToBoolean(sc_ppThresholdDac1);

            sc_enThresholdDac2 = Convert.ToInt32(strSC.Substring(1105, 1));
            checkBox_enThresholdDac2.Checked = Convert.ToBoolean(sc_enThresholdDac2);

            sc_ppThresholdDac2 = Convert.ToInt32(strSC.Substring(1106, 1));
            checkBox_ppThresholdDac2.Checked = Convert.ToBoolean(sc_ppThresholdDac2);

            sc_threshold1 = Convert.ToInt32(strSC.Substring(1107, 10), 2);
            textBox_threshold1.Text = sc_threshold1.ToString();

            sc_threshold2 = Convert.ToInt32(strSC.Substring(1117, 10), 2);
            textBox_threshold2.Text = sc_threshold2.ToString();

            sc_enHgOtaQ = Convert.ToInt32(strSC.Substring(1127, 1));
            checkBox_enHgOtaQ.Checked = Convert.ToBoolean(sc_enHgOtaQ);

            sc_ppHgOtaQ = Convert.ToInt32(strSC.Substring(1128, 1));
            checkBox_ppHgOtaQ.Checked = Convert.ToBoolean(sc_ppHgOtaQ);

            sc_enLgOtaQ = Convert.ToInt32(strSC.Substring(1129, 1));
            checkBox_enLgOtaQ.Checked = Convert.ToBoolean(sc_enLgOtaQ);

            sc_ppLgOtaQ = Convert.ToInt32(strSC.Substring(1130, 1));
            checkBox_ppLgOtaQ.Checked = Convert.ToBoolean(sc_ppLgOtaQ);

            sc_enProbeOtaQ = Convert.ToInt32(strSC.Substring(1131, 1));
            checkBox_enProbeOtaQ.Checked = Convert.ToBoolean(sc_enProbeOtaQ);

            sc_ppProbeOtaQ = Convert.ToInt32(strSC.Substring(1132, 1));
            checkBox_ppProbeOtaQ.Checked = Convert.ToBoolean(sc_ppProbeOtaQ);

            sc_testBitOtaQ = Convert.ToInt32(strSC.Substring(1133, 1));
            checkBox_testBitOtaQ.Checked = Convert.ToBoolean(sc_testBitOtaQ);

            sc_enValEvtReceiver = Convert.ToInt32(strSC.Substring(1134, 1));
            checkBox_enValEvtReceiver.Checked = Convert.ToBoolean(sc_enValEvtReceiver);

            sc_ppValEvtReceiver = Convert.ToInt32(strSC.Substring(1135, 1));
            checkBox_ppValEvtReceiver.Checked = Convert.ToBoolean(sc_ppValEvtReceiver);

            sc_enRazChnReceiver = Convert.ToInt32(strSC.Substring(1136, 1));
            checkBox_enRazChnReceiver.Checked = Convert.ToBoolean(sc_enRazChnReceiver);

            sc_ppRazChnReceiver = Convert.ToInt32(strSC.Substring(1137, 1));
            checkBox_ppRazChnReceiver.Checked = Convert.ToBoolean(sc_ppRazChnReceiver);

            sc_enDigitalMuxOutput = Convert.ToInt32(strSC.Substring(1138, 1));
            checkBox_enDigitalMuxOutput.Checked = Convert.ToBoolean(sc_enDigitalMuxOutput);

            sc_enOr32 = Convert.ToInt32(strSC.Substring(1139, 1));
            checkBox_enOr32.Checked = Convert.ToBoolean(sc_enOr32);

            sc_enNor32Oc = Convert.ToInt32(strSC.Substring(1140, 1));
            checkBox_enNor32Oc.Checked = Convert.ToBoolean(sc_enNor32Oc);

            sc_triggerPolarity = Convert.ToInt32(strSC.Substring(1141, 1));
            checkBox_triggerPolarity.Checked = Convert.ToBoolean(sc_triggerPolarity);

            sc_enNor32TOc = Convert.ToInt32(strSC.Substring(1142, 1));
            checkBox_enNor32TOc.Checked = Convert.ToBoolean(sc_enNor32TOc);

            sc_enTriggersOutput = Convert.ToInt32(strSC.Substring(1143, 1));
            checkBox_enTriggersOutput.Checked = Convert.ToBoolean(sc_enTriggersOutput);
        }

        private void button_sendSC_Click(object sender, EventArgs e)
        {
            bool result = false;
            string strSC = getSC();

            if (comboBox_SelectConnection.SelectedIndex == 0)
            {
                // Test if software can read firmware version. If not, the board is not connected.
                if (Firmware.readWord(100, usbDevId) != "00000000")
                {
                    result = sendSC(usbDevId, strSC);
                    if (result) button_sendSC.BackColor = WeerocGreen;
                    else button_sendSC.BackColor = Color.LightCoral;
                    button_sendSC.ForeColor = Color.White;
                }
                else
                {
                    roundButton_connect.BackColor = Color.Gainsboro;
                    roundButton_connect.ForeColor = Color.Black;
                    roundButton_connectSmall.BackColor = Color.Gainsboro;
                    roundButton_connectSmall.BackgroundImage = new Bitmap(typeof(Citiroc), "Resources.onoff.png");
                    connectStatus = -1;
                    label_boardStatus.Text = "Board status\n" + "No board connected";
                    button_loadFw.Visible = false;
                    progressBar_loadFw.Visible = false;
                    MessageBox.Show("No USB Devices found.", "Warning", MessageBoxButtons.OKCancel);
                }
            }
            else if (comboBox_SelectConnection.SelectedIndex == 1)
            {
                string configNum = "";
                if ((checkBox_sendToNVM.Checked == true) & (connectStatus != 1))
                {
                    button_sendSC.BackColor = Color.LightCoral;
                    tmrButtonColor.Enabled = true;
                    label_help.Text = "Please configure your connection.";
                    return;
                }
                
                if (checkBox_sendToNVM.Checked == true)
                {
                    if (InputForm.InputBox("Select Config Number", "Config Number:", ref configNum) == DialogResult.OK)
                    {
                        try
                        {
                            if (Convert.ToInt32(configNum, 10) > 255)
                            {
                                MessageBox.Show("Config number too large! Please choose a number below 255"
                                + Environment.NewLine,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            }
                            else
                            {
                                configNum = Convert.ToString(Convert.ToInt32(configNum, 10), 2);
                                configNum = configNum.PadLeft(8, '0');
                                strSC += strRev(configNum);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Input of incorrect type!"
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Error message:"
                            + Environment.NewLine
                            + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    label_help.Text = "Please configure your connection.";
                }
                
                result = sendSC(usbDevId, strSC);
                if (result)
                    button_sendSC.BackColor = WeerocGreen;
                else
                {
                    button_sendSC.BackColor = Color.LightCoral;
                    label_help.Text = "Please configure your connection.";
                }
                    
                button_sendSC.ForeColor = Color.White;
            }
            else
            {
                label_help.Text= "Please configure your connection.";
                button_sendSC.BackColor = Color.LightCoral;
                button_sendSC.ForeColor = Color.White;
            }

            tmrButtonColor.Enabled = true;
        }

        private void button_selectSC_Click(object sender, EventArgs e)
        {
            //Initialize string for input of config number selection
            string configNum = "";

            if (connectStatus != 1)
            {
                    button_sendSC.BackColor = Color.LightCoral;
                    tmrButtonColor.Enabled = true;
                    label_help.Text = "Please configure your connection.";
                    return;
            }

            if (InputForm.InputBox("Select Config Number on NVM", "Config Number:", ref configNum) == DialogResult.OK)
            {
                try
                {
                    if (Convert.ToInt32(configNum, 10) > 255)
                    {
                        MessageBox.Show("Config number too large! Please choose a number below 255"
                        + Environment.NewLine,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        byte[] citiConf = new byte[1];
                        configNum = Convert.ToString(Convert.ToInt32(configNum, 10), 2);
                        configNum = configNum.PadLeft(8, '0');
                        uint intCmdTmp = Convert.ToUInt32(configNum, 2);
                        citiConf[0] = Convert.ToByte(intCmdTmp);
                        protoCubes.SendCommand(ProtoCubesSerial.Command.SelectNVMCitirocConf, citiConf);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Input of incorrect type!"
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Error message:"
                    + Environment.NewLine
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
        }

        private bool sendSC(int usbDevId, string strSC)
        {
            if (connectStatus != 1)
                return false;

            // Initialize result as false
            bool result = false;

            // Get standard length of slow control bitstream
            int scLength = strDefSC.Length;
            // Get length of current slow control bitstream
            int intLenStrSC = strSC.Length;
            byte[] bytSC = new byte[scLength / 8];

            // If the length of the current bitstream is not OK, return false
            // else store the slow control in byte array
            if (intLenStrSC == scLength)
            {
                for (int i = 0; i < (scLength / 8); i++)
                {
                    string strScCmdTmp = strSC.Substring(i * 8, 8);
                    strScCmdTmp = strRev(strScCmdTmp);
                    uint intCmdTmp = Convert.ToUInt32(strScCmdTmp, 2);
                    bytSC[i] = Convert.ToByte(intCmdTmp);
                }
            }
            else
                return result;

            // Send configuration to Weeroc board
            if (comboBox_SelectConnection.SelectedIndex == 0)
            {
                // Select slow control parameters to FPGA
                Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);
                // Send slow control parameters to FPGA
                int intLenBytSC = bytSC.Length;
                Firmware.sendWord(10, bytSC, intLenBytSC, usbDevId);

                // Start shift parameters to ASIC
                Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "10", usbDevId);
                // Stop shift parameters to ASIC
                Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

                // Slow control test checksum test query
                Firmware.sendWord(0, "10" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);

                // Load slow control
                Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "01", usbDevId);
                Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

                // Send slow control parameters to FPGA
                Firmware.sendWord(10, bytSC, intLenBytSC, usbDevId);

                // Start shift parameters to ASIC
                Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "10", usbDevId);
                // Stop shift parameters to ASIC
                Firmware.sendWord(1, "111" + ((checkBox_rstbPa.Checked == true) ? "1" : "0") + ((checkBox_readOutSpeed.Checked == true) ? "1" : "0") + ((checkBox_OR32polarity.Checked == true) ? "1" : "0") + "00", usbDevId);

                // Slow Control Correlation Test Result
                if (Firmware.readWord(4, usbDevId) == "00000000") result = true;

                // Reset slow control test checksum test query
                Firmware.sendWord(0, "00" + ((checkBox_disReadAdc.Checked == true) ? "1" : "0") + ((checkBox_enSerialLink.Checked == true) ? "1" : "0") + ((checkBox_selRazChn.Checked == true) ? "1" : "0") + ((checkBox_valEvt.Checked == true) ? "1" : "0") + ((checkBox_razChn.Checked == true) ? "1" : "0") + ((checkBox_selValEvt.Checked == true) ? "1" : "0"), usbDevId);

                // Send delay in firmware
                int delay = 130;
                int.TryParse(textBox_delay.Text, out delay);
                if (delay > 255) delay = 255;
                else if (delay < 0) delay = 0;
                Firmware.sendWord(30, IntToBin(delay, 8), usbDevId);
            
                result = true;
            }
            // Proto-CUBES
            else if (comboBox_SelectConnection.SelectedIndex == 1)
            {
                try
                {
                    if (checkBox_sendToNVM.Checked == false)
                    {
                        protoCubes.SendCommand(ProtoCubesSerial.Command.SendCitirocConf, bytSC);
                    }
                    else
                    {
                        protoCubes.SendCommand(ProtoCubesSerial.Command.SendNVMCitirocConf, bytSC);
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send Citiroc configuration " +
                        "to Proto-CUBES!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + "Error message:"
                        + Environment.NewLine
                        + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    result = false;
                }
            }

            return result;
        }

        #region mask
        private void groupBox_mask_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "If checked, mask the corresponding channel trigger. Masked channels do not participate " +
                "neither in the multiplexed trigger hit register nor the OR32/NOR32 trigger output. A trigger on masked " +
                "channels will not initiate data acquisition.";
        }

        private void button_maskAll_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_mask.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = false;
        }

        private void button_unmaskAll_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_mask.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = true;
        }

        private void checkBox_mask_CheckedChanged(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((CheckBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_mask[channel] = SC.Switch((CheckBox)sender, button_sendSC);
        }
        #endregion

        #region Ctest
        private void groupBox_Ctest_MouseEnter(object sender, EventArgs e)
        {
            label_help.Text = "If checked, enable the injection capacitor to the corresponding channels. Injection" +
                " is done through the \"IN_CALIB\" connector. Please refer to the user guide for the Ctest injection schematic.";
        }

        private void button_injectAllLg_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_CtestLg.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = true;
        }

        private void button_injectNoneLg_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_CtestLg.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = false;
        }

        private void checkBox_CtestLg_CheckedChanged(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((CheckBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_CtestLg[channel] = SC.Switch((CheckBox)sender, button_sendSC);
        }

        private void button_injectAllHg_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_CtestHg.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = true;
        }

        private void button_injectNoneHg_Click(object sender, EventArgs e)
        {
            foreach (Control cb in groupBox_CtestHg.Controls) if (cb is CheckBox) ((CheckBox)cb).Checked = false;
        }

        private void checkBox_CtestHg_CheckedChanged(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((CheckBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_CtestHg[channel] = SC.Switch((CheckBox)sender, button_sendSC);
        }
        #endregion

        #region misc
        private void checkBox_latch_CheckedChanged(object sender, EventArgs e)
        {
            sc_latchDiscri = SC.Switch((CheckBox)sender, "Direct trigger", "Latch trigger", button_sendSC);
        }

        private void checkBox_biasSca_CheckedChanged(object sender, EventArgs e)
        {
            sc_biasSca = SC.Switch((CheckBox)sender, "High SCA bias", "Weak SCA bias", button_sendSC);
        }

        private void checkBox_scaOrPdLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_scaOrPdLg = SC.Switch((CheckBox)sender, "Use peak sensing (low gain)", "Use SCA (low gain)", button_sendSC);
        }

        private void checkBox_scaOrPdHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_scaOrPdHg = SC.Switch((CheckBox)sender, "Use peak sensing (high gain)", "Use SCA (high gain)", button_sendSC);
        }

        private void checkBox_selTrigExtPd_CheckedChanged(object sender, EventArgs e)
        {
            sc_selTrigExtPd = SC.Switch((CheckBox)sender, button_sendSC);
        }

        private void checkBox_bypassPd_CheckedChanged(object sender, EventArgs e)
        {
            sc_bypassPd = SC.Switch((CheckBox)sender, button_sendSC);
        }

        private void checkBox_dacRef_CheckedChanged(object sender, EventArgs e)
        {
            sc_dacRef = SC.Switch((CheckBox)sender, "Input DAC reference = 2.5 V", "Input DAC reference = 4.5 V", button_sendSC);
        }

        private void checkBox_triggerPolarity_CheckedChanged(object sender, EventArgs e)
        {
            sc_triggerPolarity = SC.Switch((CheckBox)sender, "Positive trigger", "Negative trigger", button_sendSC);
        }

        private void checkBox_fshOnLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_fshOnLg = SC.Switch((CheckBox)sender, "High gain to fast shaper", "Low gain to fast shaper", button_sendSC);
        }

        private void checkBox_testBitOtaQ_CheckedChanged(object sender, EventArgs e)
        {
            sc_testBitOtaQ = SC.Switch((CheckBox)sender, button_sendSC);
        }

        private void checkBox_paLgBias_CheckedChanged(object sender, EventArgs e)
        {
            sc_paLgBias = SC.Switch((CheckBox)sender, button_sendSC);
        }

        private void textBox_shapingTimeHg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                sc_shapingTimeHg = SC.txtBoxSC((TextBox)sender, 3, button_sendSC);
                label_tauHg.Text = "τ = " + (87.5 - 12.5 * sc_shapingTimeHg) + " ns";
            }
        }

        private void textBox_shapingTimeHg_Leave(object sender, EventArgs e)
        {
            sc_shapingTimeHg = SC.txtBoxSC((TextBox)sender, 3, button_sendSC);
            label_tauHg.Text = "τ = " + (87.5 - 12.5 * sc_shapingTimeHg) + " ns";
        }

        private void textBox_shapingTimeLg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                sc_shapingTimeLg = SC.txtBoxSC((TextBox)sender, 3, button_sendSC);
                label_tauLg.Text = "τ = " + (87.5 - 12.5 * sc_shapingTimeLg) + " ns";
            }
        }

        private void textBox_shapingTimeLg_Leave(object sender, EventArgs e)
        {
            sc_shapingTimeLg = SC.txtBoxSC((TextBox)sender, 3, button_sendSC);
            label_tauLg.Text = "τ = " + (87.5 - 12.5 * sc_shapingTimeLg) + " ns";
        }

        private void textBox_threshold1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13) sc_threshold1 = SC.txtBoxSC((TextBox)sender, 10, button_sendSC);
        }

        private void textBox_threshold1_Leave(object sender, EventArgs e)
        {
            sc_threshold1 = SC.txtBoxSC((TextBox)sender, 10, button_sendSC);
        }

        private void textBox_threshold2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13) sc_threshold2 = SC.txtBoxSC((TextBox)sender, 10, button_sendSC);
        }

        private void textBox_threshold2_Leave(object sender, EventArgs e)
        {
            sc_threshold2 = SC.txtBoxSC((TextBox)sender, 10, button_sendSC);
        }
        #endregion

        #region preamplifier
        private void textBox_paLgGain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            if (e.KeyValue == 13) sc_paLgGain[channel] = SC.txtBoxSC((TextBox)sender, 6, button_sendSC);
        }

        private void textBox_paLgGain_Leave(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_paLgGain[channel] = SC.txtBoxSC((TextBox)sender, 6, button_sendSC);
        }

        private void textBox_paHgGain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            if (e.KeyValue == 13) sc_paHgGain[channel] = SC.txtBoxSC((TextBox)sender, 6, button_sendSC);
        }

        private void textBox_paHgGain_Leave(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_paHgGain[channel] = SC.txtBoxSC((TextBox)sender, 6, button_sendSC);
        }

        private void checkBox_enPa_CheckedChanged(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((CheckBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_enPa[channel] = SC.Switch((CheckBox)sender, button_sendSC);
        }

        private void button_setAllPaHg_Click(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(textBox_setAllPaHg.Text, out value);
            if (value > 63)
            {
                value = 63;
                textBox_setAllPaHg.Text = "63";
            }
            if (value < 0)
            {
                value = 0;
                textBox_setAllPaHg.Text = "0";
            }

            TextBox[] textBox_paHgGain = { textBox_paHgGain0, textBox_paHgGain1, textBox_paHgGain2, textBox_paHgGain3, textBox_paHgGain4,
                textBox_paHgGain5, textBox_paHgGain6, textBox_paHgGain7, textBox_paHgGain8, textBox_paHgGain9,
                textBox_paHgGain10, textBox_paHgGain11, textBox_paHgGain12, textBox_paHgGain13, textBox_paHgGain14,
                textBox_paHgGain15, textBox_paHgGain16, textBox_paHgGain17, textBox_paHgGain18, textBox_paHgGain19,
                textBox_paHgGain20, textBox_paHgGain21, textBox_paHgGain22, textBox_paHgGain23, textBox_paHgGain24,
                textBox_paHgGain25, textBox_paHgGain26, textBox_paHgGain27, textBox_paHgGain28, textBox_paHgGain29,
                textBox_paHgGain30, textBox_paHgGain31 };

            for (int i = 0; i < NbChannels; i++)
            {
                sc_paHgGain[i] = value;
                textBox_paHgGain[i].Text = value.ToString();
            }
        }

        private void button_setAllPaLg_Click(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(textBox_setAllPaLg.Text, out value);
            if (value > 63)
            {
                value = 63;
                textBox_setAllPaLg.Text = "63";
            }
            if (value <= 0)
            {
                value = 0;
                textBox_setAllPaLg.Text = "0";
            }

            TextBox[] textBox_paLgGain = { textBox_paLgGain0, textBox_paLgGain1, textBox_paLgGain2, textBox_paLgGain3, textBox_paLgGain4,
                textBox_paLgGain5, textBox_paLgGain6, textBox_paLgGain7, textBox_paLgGain8, textBox_paLgGain9,
                textBox_paLgGain10, textBox_paLgGain11, textBox_paLgGain12, textBox_paLgGain13, textBox_paLgGain14,
                textBox_paLgGain15, textBox_paLgGain16, textBox_paLgGain17, textBox_paLgGain18, textBox_paLgGain19,
                textBox_paLgGain20, textBox_paLgGain21, textBox_paLgGain22, textBox_paLgGain23, textBox_paLgGain24,
                textBox_paLgGain25, textBox_paLgGain26, textBox_paLgGain27, textBox_paLgGain28, textBox_paLgGain29,
                textBox_paLgGain30, textBox_paLgGain31 };

            for (int i = 0; i < NbChannels; i++)
            {
                sc_paLgGain[i] = value;
                textBox_paLgGain[i].Text = value.ToString();
            }
        }

        private void button_uncheckAllPa_Click(object sender, EventArgs e)
        {
            foreach (Control c in tabPage_preamplifier.Controls) if (c is CheckBox) ((CheckBox)c).Checked = false;
        }

        private void button_checkAllPa_Click(object sender, EventArgs e)
        {
            foreach (Control c in tabPage_preamplifier.Controls) if (c is CheckBox) ((CheckBox)c).Checked = true;
        }
        #endregion

        #region EN PP
        private void checkBox_enTriggersOutput_CheckedChanged(object sender, EventArgs e)
        {
            sc_enTriggersOutput = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppPaHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppPaHg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enPaHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enPaHg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppPaLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppPaLg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enPaLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enPaLg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppSshHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppSshHg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enSshHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enSshHg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppSshLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppSshLg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enSshLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enSshLg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppPdetLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppPdetLg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enPdetLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enPdetLg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppPdetHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppPdetHg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enPdetHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enPdetHg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppThLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppThLg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enThLg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enThLg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppThHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppThHg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enThHg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enThHg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppThresholdDac2_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppThresholdDac2 = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enThresholdDac2_CheckedChanged(object sender, EventArgs e)
        {
            sc_enThresholdDac2 = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppThresholdDac1_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppThresholdDac1 = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enThresholdDac1_CheckedChanged(object sender, EventArgs e)
        {
            sc_enThresholdDac1 = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppTemp_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppTemp = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enTemp_CheckedChanged(object sender, EventArgs e)
        {
            sc_enTemp = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppFsh_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppFsh = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enFsh_CheckedChanged(object sender, EventArgs e)
        {
            sc_enFsh = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppFshBuffer_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppFshBuffer = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppCalibDacT_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppCalibDacT = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enCalibDacT_CheckedChanged(object sender, EventArgs e)
        {
            sc_enCalibDacT = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppCalibDac_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppCalibDacQ = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enCalibDac_CheckedChanged(object sender, EventArgs e)
        {
            sc_enCalibDacQ = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppDiscriT_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppDiscriT = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enDiscriT_CheckedChanged(object sender, EventArgs e)
        {
            sc_enDiscriT = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppDiscri_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppDiscri = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enDiscri_CheckedChanged(object sender, EventArgs e)
        {
            sc_enDiscri = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enInputDac_CheckedChanged(object sender, EventArgs e)
        {
            sc_enInputDac = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enNor32TOc_CheckedChanged(object sender, EventArgs e)
        {
            sc_enNor32TOc = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enNor32Oc_CheckedChanged(object sender, EventArgs e)
        {
            sc_enNor32Oc = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enOr32_CheckedChanged(object sender, EventArgs e)
        {
            sc_enOr32 = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enDigitalMuxOutput_CheckedChanged(object sender, EventArgs e)
        {
            sc_enDigitalMuxOutput = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppRazChnReceiver_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppRazChnReceiver = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enRazChnReceiver_CheckedChanged(object sender, EventArgs e)
        {
            sc_enRazChnReceiver = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppValEvtReceiver_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppValEvtReceiver = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enValEvtReceiver_CheckedChanged(object sender, EventArgs e)
        {
            sc_enValEvtReceiver = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppLgOtaQ_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppLgOtaQ = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enLgOtaQ_CheckedChanged(object sender, EventArgs e)
        {
            sc_enLgOtaQ = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppHgOtaQ_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppHgOtaQ = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enHgOtaQ_CheckedChanged(object sender, EventArgs e)
        {
            sc_enHgOtaQ = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enBg_CheckedChanged(object sender, EventArgs e)
        {
            sc_enBg = SC.EN((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppBg_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppBg = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_ppProbeOtaQ_CheckedChanged(object sender, EventArgs e)
        {
            sc_ppProbeOtaQ = SC.PP((CheckBox)sender, button_sendSC);
        }

        private void checkBox_enProbeOtaQ_CheckedChanged(object sender, EventArgs e)
        {
            sc_enProbeOtaQ = SC.EN((CheckBox)sender, button_sendSC);
        }
        #endregion

        #region calib DAC

        private void textBox_inputDac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            if (e.KeyValue == 13) sc_inputDac[channel] = SC.txtBoxSC((TextBox)sender, 8, button_sendSC);
        }

        private void textBox_inputDac_Leave(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_inputDac[channel] = SC.txtBoxSC((TextBox)sender, 8, button_sendSC);
        }

        private void checkBox_cmdInputDac_CheckedChanged(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((CheckBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_cmdInputDac[channel] = SC.Switch((CheckBox)sender, button_sendSC);
        }

        private void textBox_calibDac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            if (e.KeyValue == 13) sc_calibDacQ[channel] = SC.txtBoxSC((TextBox)sender, 4, button_sendSC);
        }

        private void textBox_calibDac_Leave(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_calibDacQ[channel] = SC.txtBoxSC((TextBox)sender, 4, button_sendSC);
        }

        private void textBox_calibDacT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            if (e.KeyValue == 13) sc_calibDacT[channel] = SC.txtBoxSC((TextBox)sender, 4, button_sendSC);
        }

        private void textBox_calibDacT_Leave(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(((TextBox)sender).Name, @"\d+").Value); // find channel number in sender name
            sc_calibDacT[channel] = SC.txtBoxSC((TextBox)sender, 4, button_sendSC);
        }

        private void button_setAllDac_Click(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(textBox_setAllDac.Text, out value);
            if (value > 15)
            {
                value = 15;
                textBox_setAllDac.Text = "15";
            }
            if (value < 0)
            {
                value = 0;
                textBox_setAllDac.Text = "0";
            }

            TextBox[] textBox_calibDac = { textBox_calibDac0, textBox_calibDac1, textBox_calibDac2, textBox_calibDac3, textBox_calibDac4,
                textBox_calibDac5, textBox_calibDac6, textBox_calibDac7, textBox_calibDac8, textBox_calibDac9,
                textBox_calibDac10, textBox_calibDac11, textBox_calibDac12, textBox_calibDac13, textBox_calibDac14,
                textBox_calibDac15, textBox_calibDac16, textBox_calibDac17, textBox_calibDac18, textBox_calibDac19,
                textBox_calibDac20, textBox_calibDac21, textBox_calibDac22, textBox_calibDac23, textBox_calibDac24,
                textBox_calibDac25, textBox_calibDac26, textBox_calibDac27, textBox_calibDac28, textBox_calibDac29,
                textBox_calibDac30, textBox_calibDac31 };

            for (int i = 0; i < NbChannels; i++)
            {
                sc_calibDacQ[i] = value;
                textBox_calibDac[i].Text = value.ToString();
            }
        }

        private void button_setAllDacT_Click(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(textBox_setAllDacT.Text, out value);
            if (value > 15)
            {
                value = 15;
                textBox_setAllDacT.Text = "15";
            }
            if (value < 0)
            {
                value = 0;
                textBox_setAllDacT.Text = "0";
            }

            TextBox[] textBox_calibDacT = { textBox_calibDacT0, textBox_calibDacT1, textBox_calibDacT2, textBox_calibDacT3, textBox_calibDacT4,
                textBox_calibDacT5, textBox_calibDacT6, textBox_calibDacT7, textBox_calibDacT8, textBox_calibDacT9,
                textBox_calibDacT10, textBox_calibDacT11, textBox_calibDacT12, textBox_calibDacT13, textBox_calibDacT14,
                textBox_calibDacT15, textBox_calibDacT16, textBox_calibDacT17, textBox_calibDacT18, textBox_calibDacT19,
                textBox_calibDacT20, textBox_calibDacT21, textBox_calibDacT22, textBox_calibDacT23, textBox_calibDacT24,
                textBox_calibDacT25, textBox_calibDacT26, textBox_calibDacT27, textBox_calibDacT28, textBox_calibDacT29,
                textBox_calibDacT30, textBox_calibDacT31 };

            for (int i = 0; i < NbChannels; i++)
            {
                sc_calibDacT[i] = value;
                textBox_calibDacT[i].Text = value.ToString();
            }
        }

        private void button_setAllInputDac_Click(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(textBox_setAllInputDac.Text, out value);
            if (value > 255)
            {
                value = 255;
                textBox_setAllInputDac.Text = "255";
            }
            if (value < 0)
            {
                value = 0;
                textBox_setAllInputDac.Text = "0";
            }

            TextBox[] textBox_inputDac = { textBox_inputDac0, textBox_inputDac1, textBox_inputDac2, textBox_inputDac3, textBox_inputDac4,
                textBox_inputDac5, textBox_inputDac6, textBox_inputDac7, textBox_inputDac8, textBox_inputDac9,
                textBox_inputDac10, textBox_inputDac11, textBox_inputDac12, textBox_inputDac13, textBox_inputDac14,
                textBox_inputDac15, textBox_inputDac16, textBox_inputDac17, textBox_inputDac18, textBox_inputDac19,
                textBox_inputDac20, textBox_inputDac21, textBox_inputDac22, textBox_inputDac23, textBox_inputDac24,
                textBox_inputDac25, textBox_inputDac26, textBox_inputDac27, textBox_inputDac28, textBox_inputDac29,
                textBox_inputDac30, textBox_inputDac31 };

            for (int i = 0; i < NbChannels; i++)
            {
                sc_inputDac[i] = value;
                textBox_inputDac[i].Text = value.ToString();
            }
        }

        private void button_checkAllInputDac_Click(object sender, EventArgs e)
        {
            foreach (Control c in groupBox_inputDac.Controls) if (c is CheckBox) ((CheckBox)c).Checked = true;
        }

        private void button_uncheckAllInputDac_Click(object sender, EventArgs e)
        {
            foreach (Control c in groupBox_inputDac.Controls) if (c is CheckBox) ((CheckBox)c).Checked = false;
        }
        #endregion
        
        private void textBox_delay_TextChanged(object sender, EventArgs e)
        {
            if (button_sendSC.BackColor != Color.Gainsboro)
            {
                button_sendSC.BackColor = Color.Gainsboro;
                button_sendSC.ForeColor = Color.Black;
            }
            int delay = 130;
            int.TryParse(textBox_delay.Text, out delay);
            if (delay > 255) delay = 255;
            else if (delay < 0) delay = 0;
            textBox_delay.Text = delay.ToString();
        }

        private void button_saveSC_Click(object sender, EventArgs e)
        {
            string strSaveFileName = "";
            SaveFileDialog ScSaveDialog = new SaveFileDialog();
            ScSaveDialog.Title = "Specify Output file";
            ScSaveDialog.Filter = "All Files(*.*)|*.*";
            ScSaveDialog.RestoreDirectory = true;

            if (ScSaveDialog.ShowDialog() == DialogResult.OK)
                strSaveFileName = ScSaveDialog.FileName;
            else return;
            TextWriter tw = new StreamWriter(strSaveFileName);
            tw.WriteLine(getSC());
            tw.Close();
        }

        private void button_loadSC_Click(object sender, EventArgs e)
        {            
            OpenFileDialog ScLoadDialog = new OpenFileDialog();
            ScLoadDialog.Title = "Specify Data file";
            //DataLDialog.Filter = "TxT files|*.txt";
            ScLoadDialog.RestoreDirectory = true;

            ArrayList DataArrayList;

            if (ScLoadDialog.ShowDialog() == DialogResult.OK)
            {
                if (ScLoadDialog.FileName == null) return;
                else
                {
                    DataArrayList = ReadFileLine(ScLoadDialog.FileName);
                    setSC(DataArrayList[0].ToString());
                    button_sendSC.BackColor = Color.Gainsboro;
                    button_sendSC.ForeColor = Color.Black;
                }
            }
            else return;
        }
        
        #endregion
    }
}
