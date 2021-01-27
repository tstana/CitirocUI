namespace CitirocUI
{
    partial class ProtoCubesMonitor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button_readHK = new System.Windows.Forms.Button();
            this.textBox_ResetCount = new System.Windows.Forms.TextBox();
            this.textBox_timestamp = new System.Windows.Forms.TextBox();
            this.label_ResetCount = new System.Windows.Forms.Label();
            this.label_timestamp = new System.Windows.Forms.Label();
            this.groupBox_HitRates = new System.Windows.Forms.GroupBox();
            this.label_hitCountMPPC1 = new System.Windows.Forms.Label();
            this.textBox_hitCountOR32 = new System.Windows.Forms.TextBox();
            this.textBox_hitCountMPPC2 = new System.Windows.Forms.TextBox();
            this.label_hitCountOR32 = new System.Windows.Forms.Label();
            this.textBox_hitCountMPPC1 = new System.Windows.Forms.TextBox();
            this.textBox_hitCountMPPC3 = new System.Windows.Forms.TextBox();
            this.label_hitCountMPPC2 = new System.Windows.Forms.Label();
            this.label_hitCountMPPC3 = new System.Windows.Forms.Label();
            this.groupBox_HvpsTelemetry = new System.Windows.Forms.GroupBox();
            this.label_Voltage = new System.Windows.Forms.Label();
            this.label142 = new System.Windows.Forms.Label();
            this.label_Temperature = new System.Windows.Forms.Label();
            this.textBox_hvpsCmdsSent = new System.Windows.Forms.TextBox();
            this.textBox_hvpsCmdsAcked = new System.Windows.Forms.TextBox();
            this.textBox_hvpsCmdsRej = new System.Windows.Forms.TextBox();
            this.textBox_tempFromHVPS = new System.Windows.Forms.TextBox();
            this.textBox_voltageFromHVPS = new System.Windows.Forms.TextBox();
            this.label_Current = new System.Windows.Forms.Label();
            this.textBox_currentFromHVPS = new System.Windows.Forms.TextBox();
            this.groupBox_HV = new System.Windows.Forms.GroupBox();
            this.groupBox_hvPersistent = new System.Windows.Forms.GroupBox();
            this.numUpDown_dt2 = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_dt1 = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_refVolt = new System.Windows.Forms.NumericUpDown();
            this.label_dt2 = new System.Windows.Forms.Label();
            this.label_refVolt = new System.Windows.Forms.Label();
            this.label_dt1 = new System.Windows.Forms.Label();
            this.numUpDown_dtp2 = new System.Windows.Forms.NumericUpDown();
            this.numUpDown_dtp1 = new System.Windows.Forms.NumericUpDown();
            this.label_dtp2 = new System.Windows.Forms.Label();
            this.label_dtp1 = new System.Windows.Forms.Label();
            this.button_hvSendPersistent = new System.Windows.Forms.Button();
            this.numUpDown_refTemp = new System.Windows.Forms.NumericUpDown();
            this.label_refTemp = new System.Windows.Forms.Label();
            this.checkBox_hvReset = new System.Windows.Forms.CheckBox();
            this.groupBox_hvTemporary = new System.Windows.Forms.GroupBox();
            this.button_hvSendTemp = new System.Windows.Forms.Button();
            this.numUpDown_HV = new System.Windows.Forms.NumericUpDown();
            this.label140 = new System.Windows.Forms.Label();
            this.checkBox_HVON = new System.Windows.Forms.CheckBox();
            this.rtxtMonitor = new System.Windows.Forms.RichTextBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.tmrButtonColor = new System.Windows.Forms.Timer(this.components);
            this.groupBoxMonitor = new System.Windows.Forms.GroupBox();
            this.label_ConnStatus = new System.Windows.Forms.Label();
            this.groupBox_HitRates.SuspendLayout();
            this.groupBox_HvpsTelemetry.SuspendLayout();
            this.groupBox_HV.SuspendLayout();
            this.groupBox_hvPersistent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dt2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dt1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_refVolt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dtp2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dtp1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_refTemp)).BeginInit();
            this.groupBox_hvTemporary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_HV)).BeginInit();
            this.groupBoxMonitor.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_readHK
            // 
            this.button_readHK.BackColor = System.Drawing.SystemColors.Control;
            this.button_readHK.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_readHK.Location = new System.Drawing.Point(7, 26);
            this.button_readHK.Margin = new System.Windows.Forms.Padding(4);
            this.button_readHK.Name = "button_readHK";
            this.button_readHK.Size = new System.Drawing.Size(520, 42);
            this.button_readHK.TabIndex = 2;
            this.button_readHK.Text = "&Read HK";
            this.button_readHK.UseVisualStyleBackColor = false;
            this.button_readHK.Click += new System.EventHandler(this.button_readHK_Click);
            // 
            // textBox_ResetCount
            // 
            this.textBox_ResetCount.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ResetCount.Location = new System.Drawing.Point(299, 113);
            this.textBox_ResetCount.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_ResetCount.Name = "textBox_ResetCount";
            this.textBox_ResetCount.ReadOnly = true;
            this.textBox_ResetCount.Size = new System.Drawing.Size(221, 26);
            this.textBox_ResetCount.TabIndex = 5;
            this.textBox_ResetCount.TabStop = false;
            this.textBox_ResetCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_timestamp
            // 
            this.textBox_timestamp.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_timestamp.Location = new System.Drawing.Point(299, 82);
            this.textBox_timestamp.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_timestamp.Name = "textBox_timestamp";
            this.textBox_timestamp.ReadOnly = true;
            this.textBox_timestamp.Size = new System.Drawing.Size(221, 26);
            this.textBox_timestamp.TabIndex = 6;
            this.textBox_timestamp.TabStop = false;
            this.textBox_timestamp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label_ResetCount
            // 
            this.label_ResetCount.AutoSize = true;
            this.label_ResetCount.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ResetCount.Location = new System.Drawing.Point(24, 116);
            this.label_ResetCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_ResetCount.Name = "label_ResetCount";
            this.label_ResetCount.Size = new System.Drawing.Size(91, 18);
            this.label_ResetCount.TabIndex = 3;
            this.label_ResetCount.Text = "Reset count:";
            // 
            // label_timestamp
            // 
            this.label_timestamp.AutoSize = true;
            this.label_timestamp.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_timestamp.Location = new System.Drawing.Point(22, 85);
            this.label_timestamp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_timestamp.Name = "label_timestamp";
            this.label_timestamp.Size = new System.Drawing.Size(276, 18);
            this.label_timestamp.TabIndex = 4;
            this.label_timestamp.Text = "UTC timestamp of last telemetry packet:";
            // 
            // groupBox_HitRates
            // 
            this.groupBox_HitRates.Controls.Add(this.label_hitCountMPPC1);
            this.groupBox_HitRates.Controls.Add(this.textBox_hitCountOR32);
            this.groupBox_HitRates.Controls.Add(this.textBox_hitCountMPPC2);
            this.groupBox_HitRates.Controls.Add(this.label_hitCountOR32);
            this.groupBox_HitRates.Controls.Add(this.textBox_hitCountMPPC1);
            this.groupBox_HitRates.Controls.Add(this.textBox_hitCountMPPC3);
            this.groupBox_HitRates.Controls.Add(this.label_hitCountMPPC2);
            this.groupBox_HitRates.Controls.Add(this.label_hitCountMPPC3);
            this.groupBox_HitRates.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox_HitRates.Location = new System.Drawing.Point(7, 169);
            this.groupBox_HitRates.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox_HitRates.Name = "groupBox_HitRates";
            this.groupBox_HitRates.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox_HitRates.Size = new System.Drawing.Size(231, 157);
            this.groupBox_HitRates.TabIndex = 42;
            this.groupBox_HitRates.TabStop = false;
            this.groupBox_HitRates.Text = "Channel Hit Rates";
            // 
            // label_hitCountMPPC1
            // 
            this.label_hitCountMPPC1.AutoSize = true;
            this.label_hitCountMPPC1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hitCountMPPC1.Location = new System.Drawing.Point(17, 31);
            this.label_hitCountMPPC1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_hitCountMPPC1.Name = "label_hitCountMPPC1";
            this.label_hitCountMPPC1.Size = new System.Drawing.Size(64, 18);
            this.label_hitCountMPPC1.TabIndex = 20;
            this.label_hitCountMPPC1.Text = "MPPC1:";
            // 
            // textBox_hitCountOR32
            // 
            this.textBox_hitCountOR32.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_hitCountOR32.Location = new System.Drawing.Point(107, 124);
            this.textBox_hitCountOR32.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_hitCountOR32.Name = "textBox_hitCountOR32";
            this.textBox_hitCountOR32.ReadOnly = true;
            this.textBox_hitCountOR32.Size = new System.Drawing.Size(109, 24);
            this.textBox_hitCountOR32.TabIndex = 11;
            this.textBox_hitCountOR32.TabStop = false;
            // 
            // textBox_hitCountMPPC2
            // 
            this.textBox_hitCountMPPC2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_hitCountMPPC2.Location = new System.Drawing.Point(107, 60);
            this.textBox_hitCountMPPC2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_hitCountMPPC2.Name = "textBox_hitCountMPPC2";
            this.textBox_hitCountMPPC2.ReadOnly = true;
            this.textBox_hitCountMPPC2.Size = new System.Drawing.Size(109, 24);
            this.textBox_hitCountMPPC2.TabIndex = 23;
            this.textBox_hitCountMPPC2.TabStop = false;
            // 
            // label_hitCountOR32
            // 
            this.label_hitCountOR32.AutoSize = true;
            this.label_hitCountOR32.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hitCountOR32.Location = new System.Drawing.Point(17, 127);
            this.label_hitCountOR32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_hitCountOR32.Name = "label_hitCountOR32";
            this.label_hitCountOR32.Size = new System.Drawing.Size(80, 18);
            this.label_hitCountOR32.TabIndex = 10;
            this.label_hitCountOR32.Text = "All (OR32):";
            // 
            // textBox_hitCountMPPC1
            // 
            this.textBox_hitCountMPPC1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_hitCountMPPC1.Location = new System.Drawing.Point(107, 28);
            this.textBox_hitCountMPPC1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_hitCountMPPC1.Name = "textBox_hitCountMPPC1";
            this.textBox_hitCountMPPC1.ReadOnly = true;
            this.textBox_hitCountMPPC1.Size = new System.Drawing.Size(109, 24);
            this.textBox_hitCountMPPC1.TabIndex = 21;
            this.textBox_hitCountMPPC1.TabStop = false;
            // 
            // textBox_hitCountMPPC3
            // 
            this.textBox_hitCountMPPC3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_hitCountMPPC3.Location = new System.Drawing.Point(107, 92);
            this.textBox_hitCountMPPC3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_hitCountMPPC3.Name = "textBox_hitCountMPPC3";
            this.textBox_hitCountMPPC3.ReadOnly = true;
            this.textBox_hitCountMPPC3.Size = new System.Drawing.Size(109, 24);
            this.textBox_hitCountMPPC3.TabIndex = 7;
            this.textBox_hitCountMPPC3.TabStop = false;
            // 
            // label_hitCountMPPC2
            // 
            this.label_hitCountMPPC2.AutoSize = true;
            this.label_hitCountMPPC2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hitCountMPPC2.Location = new System.Drawing.Point(17, 63);
            this.label_hitCountMPPC2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_hitCountMPPC2.Name = "label_hitCountMPPC2";
            this.label_hitCountMPPC2.Size = new System.Drawing.Size(64, 18);
            this.label_hitCountMPPC2.TabIndex = 22;
            this.label_hitCountMPPC2.Text = "MPPC2:";
            // 
            // label_hitCountMPPC3
            // 
            this.label_hitCountMPPC3.AutoSize = true;
            this.label_hitCountMPPC3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hitCountMPPC3.Location = new System.Drawing.Point(17, 95);
            this.label_hitCountMPPC3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_hitCountMPPC3.Name = "label_hitCountMPPC3";
            this.label_hitCountMPPC3.Size = new System.Drawing.Size(64, 18);
            this.label_hitCountMPPC3.TabIndex = 6;
            this.label_hitCountMPPC3.Text = "MPPC3:";
            // 
            // groupBox_HvpsTelemetry
            // 
            this.groupBox_HvpsTelemetry.Controls.Add(this.label_Voltage);
            this.groupBox_HvpsTelemetry.Controls.Add(this.label142);
            this.groupBox_HvpsTelemetry.Controls.Add(this.label_Temperature);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_hvpsCmdsSent);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_hvpsCmdsAcked);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_hvpsCmdsRej);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_tempFromHVPS);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_voltageFromHVPS);
            this.groupBox_HvpsTelemetry.Controls.Add(this.label_Current);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_currentFromHVPS);
            this.groupBox_HvpsTelemetry.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox_HvpsTelemetry.Location = new System.Drawing.Point(246, 169);
            this.groupBox_HvpsTelemetry.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox_HvpsTelemetry.Name = "groupBox_HvpsTelemetry";
            this.groupBox_HvpsTelemetry.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox_HvpsTelemetry.Size = new System.Drawing.Size(274, 157);
            this.groupBox_HvpsTelemetry.TabIndex = 41;
            this.groupBox_HvpsTelemetry.TabStop = false;
            this.groupBox_HvpsTelemetry.Text = "HVPS Telemetry";
            // 
            // label_Voltage
            // 
            this.label_Voltage.AutoSize = true;
            this.label_Voltage.Location = new System.Drawing.Point(8, 31);
            this.label_Voltage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Voltage.Name = "label_Voltage";
            this.label_Voltage.Size = new System.Drawing.Size(87, 18);
            this.label_Voltage.TabIndex = 15;
            this.label_Voltage.Text = "Voltage (V):";
            // 
            // label142
            // 
            this.label142.AutoSize = true;
            this.label142.Location = new System.Drawing.Point(8, 127);
            this.label142.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label142.Name = "label142";
            this.label142.Size = new System.Drawing.Size(50, 18);
            this.label142.TabIndex = 2;
            this.label142.Text = "Cmds.";
            // 
            // label_Temperature
            // 
            this.label_Temperature.AutoSize = true;
            this.label_Temperature.Location = new System.Drawing.Point(8, 95);
            this.label_Temperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Temperature.Name = "label_Temperature";
            this.label_Temperature.Size = new System.Drawing.Size(132, 18);
            this.label_Temperature.TabIndex = 2;
            this.label_Temperature.Text = "Temperature (°C):";
            // 
            // textBox_hvpsCmdsSent
            // 
            this.textBox_hvpsCmdsSent.Location = new System.Drawing.Point(62, 124);
            this.textBox_hvpsCmdsSent.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_hvpsCmdsSent.Name = "textBox_hvpsCmdsSent";
            this.textBox_hvpsCmdsSent.ReadOnly = true;
            this.textBox_hvpsCmdsSent.Size = new System.Drawing.Size(56, 26);
            this.textBox_hvpsCmdsSent.TabIndex = 3;
            this.textBox_hvpsCmdsSent.TabStop = false;
            this.textBox_hvpsCmdsSent.Text = "Sent";
            // 
            // textBox_hvpsCmdsAcked
            // 
            this.textBox_hvpsCmdsAcked.Location = new System.Drawing.Point(133, 124);
            this.textBox_hvpsCmdsAcked.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_hvpsCmdsAcked.Name = "textBox_hvpsCmdsAcked";
            this.textBox_hvpsCmdsAcked.ReadOnly = true;
            this.textBox_hvpsCmdsAcked.Size = new System.Drawing.Size(63, 26);
            this.textBox_hvpsCmdsAcked.TabIndex = 3;
            this.textBox_hvpsCmdsAcked.TabStop = false;
            this.textBox_hvpsCmdsAcked.Text = "Acked";
            // 
            // textBox_hvpsCmdsRej
            // 
            this.textBox_hvpsCmdsRej.Location = new System.Drawing.Point(204, 124);
            this.textBox_hvpsCmdsRej.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_hvpsCmdsRej.Name = "textBox_hvpsCmdsRej";
            this.textBox_hvpsCmdsRej.ReadOnly = true;
            this.textBox_hvpsCmdsRej.Size = new System.Drawing.Size(63, 26);
            this.textBox_hvpsCmdsRej.TabIndex = 3;
            this.textBox_hvpsCmdsRej.TabStop = false;
            this.textBox_hvpsCmdsRej.Text = "Rej";
            // 
            // textBox_tempFromHVPS
            // 
            this.textBox_tempFromHVPS.Location = new System.Drawing.Point(158, 92);
            this.textBox_tempFromHVPS.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_tempFromHVPS.Name = "textBox_tempFromHVPS";
            this.textBox_tempFromHVPS.ReadOnly = true;
            this.textBox_tempFromHVPS.Size = new System.Drawing.Size(109, 26);
            this.textBox_tempFromHVPS.TabIndex = 3;
            this.textBox_tempFromHVPS.TabStop = false;
            // 
            // textBox_voltageFromHVPS
            // 
            this.textBox_voltageFromHVPS.Location = new System.Drawing.Point(158, 28);
            this.textBox_voltageFromHVPS.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_voltageFromHVPS.Name = "textBox_voltageFromHVPS";
            this.textBox_voltageFromHVPS.ReadOnly = true;
            this.textBox_voltageFromHVPS.Size = new System.Drawing.Size(109, 26);
            this.textBox_voltageFromHVPS.TabIndex = 16;
            this.textBox_voltageFromHVPS.TabStop = false;
            // 
            // label_Current
            // 
            this.label_Current.AutoSize = true;
            this.label_Current.Location = new System.Drawing.Point(8, 63);
            this.label_Current.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Current.Name = "label_Current";
            this.label_Current.Size = new System.Drawing.Size(100, 18);
            this.label_Current.TabIndex = 17;
            this.label_Current.Text = "Current (mA):";
            // 
            // textBox_currentFromHVPS
            // 
            this.textBox_currentFromHVPS.Location = new System.Drawing.Point(158, 60);
            this.textBox_currentFromHVPS.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_currentFromHVPS.Name = "textBox_currentFromHVPS";
            this.textBox_currentFromHVPS.ReadOnly = true;
            this.textBox_currentFromHVPS.Size = new System.Drawing.Size(109, 26);
            this.textBox_currentFromHVPS.TabIndex = 18;
            this.textBox_currentFromHVPS.TabStop = false;
            // 
            // groupBox_HV
            // 
            this.groupBox_HV.Controls.Add(this.groupBox_hvPersistent);
            this.groupBox_HV.Controls.Add(this.groupBox_hvTemporary);
            this.groupBox_HV.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox_HV.Location = new System.Drawing.Point(7, 334);
            this.groupBox_HV.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox_HV.Name = "groupBox_HV";
            this.groupBox_HV.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox_HV.Size = new System.Drawing.Size(520, 257);
            this.groupBox_HV.TabIndex = 43;
            this.groupBox_HV.TabStop = false;
            this.groupBox_HV.Text = "HV settings";
            // 
            // groupBox_hvPersistent
            // 
            this.groupBox_hvPersistent.Controls.Add(this.numUpDown_dt2);
            this.groupBox_hvPersistent.Controls.Add(this.numUpDown_dt1);
            this.groupBox_hvPersistent.Controls.Add(this.numUpDown_refVolt);
            this.groupBox_hvPersistent.Controls.Add(this.label_dt2);
            this.groupBox_hvPersistent.Controls.Add(this.label_refVolt);
            this.groupBox_hvPersistent.Controls.Add(this.label_dt1);
            this.groupBox_hvPersistent.Controls.Add(this.numUpDown_dtp2);
            this.groupBox_hvPersistent.Controls.Add(this.numUpDown_dtp1);
            this.groupBox_hvPersistent.Controls.Add(this.label_dtp2);
            this.groupBox_hvPersistent.Controls.Add(this.label_dtp1);
            this.groupBox_hvPersistent.Controls.Add(this.button_hvSendPersistent);
            this.groupBox_hvPersistent.Controls.Add(this.numUpDown_refTemp);
            this.groupBox_hvPersistent.Controls.Add(this.label_refTemp);
            this.groupBox_hvPersistent.Controls.Add(this.checkBox_hvReset);
            this.groupBox_hvPersistent.Location = new System.Drawing.Point(8, 98);
            this.groupBox_hvPersistent.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox_hvPersistent.Name = "groupBox_hvPersistent";
            this.groupBox_hvPersistent.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox_hvPersistent.Size = new System.Drawing.Size(505, 149);
            this.groupBox_hvPersistent.TabIndex = 1;
            this.groupBox_hvPersistent.TabStop = false;
            this.groupBox_hvPersistent.Text = "Persistent (set temperature correction factor)";
            // 
            // numUpDown_dt2
            // 
            this.numUpDown_dt2.DecimalPlaces = 2;
            this.numUpDown_dt2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numUpDown_dt2.Location = new System.Drawing.Point(291, 106);
            this.numUpDown_dt2.Margin = new System.Windows.Forms.Padding(4);
            this.numUpDown_dt2.Maximum = new decimal(new int[] {
            3400,
            0,
            0,
            0});
            this.numUpDown_dt2.Name = "numUpDown_dt2";
            this.numUpDown_dt2.Size = new System.Drawing.Size(60, 26);
            this.numUpDown_dt2.TabIndex = 6;
            // 
            // numUpDown_dt1
            // 
            this.numUpDown_dt1.DecimalPlaces = 2;
            this.numUpDown_dt1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numUpDown_dt1.Location = new System.Drawing.Point(291, 64);
            this.numUpDown_dt1.Margin = new System.Windows.Forms.Padding(4);
            this.numUpDown_dt1.Maximum = new decimal(new int[] {
            3400,
            0,
            0,
            0});
            this.numUpDown_dt1.Name = "numUpDown_dt1";
            this.numUpDown_dt1.Size = new System.Drawing.Size(60, 26);
            this.numUpDown_dt1.TabIndex = 4;
            // 
            // numUpDown_refVolt
            // 
            this.numUpDown_refVolt.DecimalPlaces = 3;
            this.numUpDown_refVolt.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numUpDown_refVolt.Location = new System.Drawing.Point(424, 27);
            this.numUpDown_refVolt.Margin = new System.Windows.Forms.Padding(4);
            this.numUpDown_refVolt.Maximum = new decimal(new int[] {
            55,
            0,
            0,
            0});
            this.numUpDown_refVolt.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.numUpDown_refVolt.Name = "numUpDown_refVolt";
            this.numUpDown_refVolt.Size = new System.Drawing.Size(70, 26);
            this.numUpDown_refVolt.TabIndex = 2;
            this.numUpDown_refVolt.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // label_dt2
            // 
            this.label_dt2.AutoSize = true;
            this.label_dt2.Location = new System.Drawing.Point(195, 108);
            this.label_dt2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_dt2.Name = "label_dt2";
            this.label_dt2.Size = new System.Drawing.Size(95, 18);
            this.label_dt2.TabIndex = 12;
            this.label_dt2.Text = "dT2 (mV/°C)";
            // 
            // label_refVolt
            // 
            this.label_refVolt.AutoSize = true;
            this.label_refVolt.Location = new System.Drawing.Point(313, 30);
            this.label_refVolt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_refVolt.Name = "label_refVolt";
            this.label_refVolt.Size = new System.Drawing.Size(105, 18);
            this.label_refVolt.TabIndex = 0;
            this.label_refVolt.Text = "Ref. Volt. (V) :";
            // 
            // label_dt1
            // 
            this.label_dt1.AutoSize = true;
            this.label_dt1.Location = new System.Drawing.Point(195, 67);
            this.label_dt1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_dt1.Name = "label_dt1";
            this.label_dt1.Size = new System.Drawing.Size(95, 18);
            this.label_dt1.TabIndex = 13;
            this.label_dt1.Text = "dT1 (mV/°C)";
            // 
            // numUpDown_dtp2
            // 
            this.numUpDown_dtp2.DecimalPlaces = 2;
            this.numUpDown_dtp2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numUpDown_dtp2.Location = new System.Drawing.Point(126, 106);
            this.numUpDown_dtp2.Margin = new System.Windows.Forms.Padding(4);
            this.numUpDown_dtp2.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            this.numUpDown_dtp2.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            -2147418112});
            this.numUpDown_dtp2.Name = "numUpDown_dtp2";
            this.numUpDown_dtp2.Size = new System.Drawing.Size(60, 26);
            this.numUpDown_dtp2.TabIndex = 5;
            // 
            // numUpDown_dtp1
            // 
            this.numUpDown_dtp1.DecimalPlaces = 2;
            this.numUpDown_dtp1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numUpDown_dtp1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numUpDown_dtp1.Location = new System.Drawing.Point(126, 64);
            this.numUpDown_dtp1.Margin = new System.Windows.Forms.Padding(4);
            this.numUpDown_dtp1.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            65536});
            this.numUpDown_dtp1.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            -2147418112});
            this.numUpDown_dtp1.Name = "numUpDown_dtp1";
            this.numUpDown_dtp1.Size = new System.Drawing.Size(60, 25);
            this.numUpDown_dtp1.TabIndex = 3;
            // 
            // label_dtp2
            // 
            this.label_dtp2.AutoSize = true;
            this.label_dtp2.Location = new System.Drawing.Point(7, 108);
            this.label_dtp2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_dtp2.Name = "label_dtp2";
            this.label_dtp2.Size = new System.Drawing.Size(117, 18);
            this.label_dtp2.TabIndex = 2;
            this.label_dtp2.Text = "dT\'2 (mV/°C^2)";
            // 
            // label_dtp1
            // 
            this.label_dtp1.AutoSize = true;
            this.label_dtp1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_dtp1.Location = new System.Drawing.Point(7, 67);
            this.label_dtp1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_dtp1.Name = "label_dtp1";
            this.label_dtp1.Size = new System.Drawing.Size(117, 18);
            this.label_dtp1.TabIndex = 0;
            this.label_dtp1.Text = "dT\'1 (mV/°C^2)";
            // 
            // button_hvSendPersistent
            // 
            this.button_hvSendPersistent.BackColor = System.Drawing.SystemColors.Control;
            this.button_hvSendPersistent.Location = new System.Drawing.Point(389, 77);
            this.button_hvSendPersistent.Margin = new System.Windows.Forms.Padding(4);
            this.button_hvSendPersistent.Name = "button_hvSendPersistent";
            this.button_hvSendPersistent.Size = new System.Drawing.Size(105, 40);
            this.button_hvSendPersistent.TabIndex = 7;
            this.button_hvSendPersistent.Text = "Send HV";
            this.button_hvSendPersistent.UseVisualStyleBackColor = false;
            this.button_hvSendPersistent.Click += new System.EventHandler(this.button_hvSendPersistent_Click);
            // 
            // numUpDown_refTemp
            // 
            this.numUpDown_refTemp.DecimalPlaces = 3;
            this.numUpDown_refTemp.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numUpDown_refTemp.Location = new System.Drawing.Point(229, 27);
            this.numUpDown_refTemp.Margin = new System.Windows.Forms.Padding(4);
            this.numUpDown_refTemp.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numUpDown_refTemp.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.numUpDown_refTemp.Name = "numUpDown_refTemp";
            this.numUpDown_refTemp.Size = new System.Drawing.Size(70, 26);
            this.numUpDown_refTemp.TabIndex = 1;
            this.numUpDown_refTemp.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label_refTemp
            // 
            this.label_refTemp.AutoSize = true;
            this.label_refTemp.Location = new System.Drawing.Point(94, 29);
            this.label_refTemp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_refTemp.Name = "label_refTemp";
            this.label_refTemp.Size = new System.Drawing.Size(127, 18);
            this.label_refTemp.TabIndex = 0;
            this.label_refTemp.Text = "Ref. Temp. (°C) :";
            // 
            // checkBox_hvReset
            // 
            this.checkBox_hvReset.AutoSize = true;
            this.checkBox_hvReset.Location = new System.Drawing.Point(10, 29);
            this.checkBox_hvReset.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox_hvReset.Name = "checkBox_hvReset";
            this.checkBox_hvReset.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox_hvReset.Size = new System.Drawing.Size(64, 22);
            this.checkBox_hvReset.TabIndex = 0;
            this.checkBox_hvReset.Text = "Reset";
            this.checkBox_hvReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox_hvReset.UseVisualStyleBackColor = true;
            // 
            // groupBox_hvTemporary
            // 
            this.groupBox_hvTemporary.Controls.Add(this.button_hvSendTemp);
            this.groupBox_hvTemporary.Controls.Add(this.numUpDown_HV);
            this.groupBox_hvTemporary.Controls.Add(this.label140);
            this.groupBox_hvTemporary.Controls.Add(this.checkBox_HVON);
            this.groupBox_hvTemporary.Location = new System.Drawing.Point(8, 24);
            this.groupBox_hvTemporary.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox_hvTemporary.Name = "groupBox_hvTemporary";
            this.groupBox_hvTemporary.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox_hvTemporary.Size = new System.Drawing.Size(505, 67);
            this.groupBox_hvTemporary.TabIndex = 0;
            this.groupBox_hvTemporary.TabStop = false;
            this.groupBox_hvTemporary.Text = "Temporary";
            // 
            // button_hvSendTemp
            // 
            this.button_hvSendTemp.BackColor = System.Drawing.SystemColors.Control;
            this.button_hvSendTemp.Location = new System.Drawing.Point(389, 18);
            this.button_hvSendTemp.Margin = new System.Windows.Forms.Padding(4);
            this.button_hvSendTemp.Name = "button_hvSendTemp";
            this.button_hvSendTemp.Size = new System.Drawing.Size(105, 40);
            this.button_hvSendTemp.TabIndex = 4;
            this.button_hvSendTemp.Text = "Send HV";
            this.button_hvSendTemp.UseVisualStyleBackColor = false;
            this.button_hvSendTemp.Click += new System.EventHandler(this.button_hvSendTemp_Click);
            // 
            // numUpDown_HV
            // 
            this.numUpDown_HV.DecimalPlaces = 3;
            this.numUpDown_HV.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numUpDown_HV.Location = new System.Drawing.Point(229, 27);
            this.numUpDown_HV.Margin = new System.Windows.Forms.Padding(4);
            this.numUpDown_HV.Maximum = new decimal(new int[] {
            55,
            0,
            0,
            0});
            this.numUpDown_HV.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.numUpDown_HV.Name = "numUpDown_HV";
            this.numUpDown_HV.Size = new System.Drawing.Size(70, 26);
            this.numUpDown_HV.TabIndex = 3;
            this.numUpDown_HV.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // label140
            // 
            this.label140.AutoSize = true;
            this.label140.Location = new System.Drawing.Point(129, 29);
            this.label140.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label140.Name = "label140";
            this.label140.Size = new System.Drawing.Size(92, 18);
            this.label140.TabIndex = 1;
            this.label140.Text = "Voltage (V) :";
            // 
            // checkBox_HVON
            // 
            this.checkBox_HVON.AutoSize = true;
            this.checkBox_HVON.Checked = true;
            this.checkBox_HVON.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_HVON.Location = new System.Drawing.Point(10, 28);
            this.checkBox_HVON.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox_HVON.Name = "checkBox_HVON";
            this.checkBox_HVON.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox_HVON.Size = new System.Drawing.Size(72, 22);
            this.checkBox_HVON.TabIndex = 2;
            this.checkBox_HVON.Text = "HV ON";
            this.checkBox_HVON.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox_HVON.UseVisualStyleBackColor = true;
            // 
            // rtxtMonitor
            // 
            this.rtxtMonitor.BackColor = System.Drawing.SystemColors.InfoText;
            this.rtxtMonitor.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtMonitor.ForeColor = System.Drawing.SystemColors.Info;
            this.rtxtMonitor.Location = new System.Drawing.Point(556, 13);
            this.rtxtMonitor.Margin = new System.Windows.Forms.Padding(4);
            this.rtxtMonitor.Name = "rtxtMonitor";
            this.rtxtMonitor.ReadOnly = true;
            this.rtxtMonitor.Size = new System.Drawing.Size(271, 637);
            this.rtxtMonitor.TabIndex = 44;
            this.rtxtMonitor.TabStop = false;
            this.rtxtMonitor.Text = "";
            // 
            // buttonHelp
            // 
            this.buttonHelp.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHelp.Location = new System.Drawing.Point(356, 602);
            this.buttonHelp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(24, 31);
            this.buttonHelp.TabIndex = 46;
            this.buttonHelp.Text = "?";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Clear.Location = new System.Drawing.Point(404, 598);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(4);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(105, 38);
            this.button_Clear.TabIndex = 45;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // tmrButtonColor
            // 
            this.tmrButtonColor.Interval = 500;
            this.tmrButtonColor.Tick += new System.EventHandler(this.tmrButtonColor_Tick);
            // 
            // groupBoxMonitor
            // 
            this.groupBoxMonitor.Controls.Add(this.label_ConnStatus);
            this.groupBoxMonitor.Controls.Add(this.textBox_ResetCount);
            this.groupBoxMonitor.Controls.Add(this.buttonHelp);
            this.groupBoxMonitor.Controls.Add(this.button_readHK);
            this.groupBoxMonitor.Controls.Add(this.button_Clear);
            this.groupBoxMonitor.Controls.Add(this.label_timestamp);
            this.groupBoxMonitor.Controls.Add(this.label_ResetCount);
            this.groupBoxMonitor.Controls.Add(this.groupBox_HV);
            this.groupBoxMonitor.Controls.Add(this.textBox_timestamp);
            this.groupBoxMonitor.Controls.Add(this.groupBox_HvpsTelemetry);
            this.groupBoxMonitor.Controls.Add(this.groupBox_HitRates);
            this.groupBoxMonitor.Location = new System.Drawing.Point(12, 4);
            this.groupBoxMonitor.Name = "groupBoxMonitor";
            this.groupBoxMonitor.Size = new System.Drawing.Size(537, 646);
            this.groupBoxMonitor.TabIndex = 47;
            this.groupBoxMonitor.TabStop = false;
            // 
            // label_ConnStatus
            // 
            this.label_ConnStatus.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ConnStatus.Location = new System.Drawing.Point(24, 608);
            this.label_ConnStatus.Name = "label_ConnStatus";
            this.label_ConnStatus.Size = new System.Drawing.Size(306, 18);
            this.label_ConnStatus.TabIndex = 47;
            this.label_ConnStatus.Text = "label_ConnStatus";
            // 
            // ProtoCubesMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 653);
            this.Controls.Add(this.groupBoxMonitor);
            this.Controls.Add(this.rtxtMonitor);
            this.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "ProtoCubesMonitor";
            this.Text = "CUBES Monitor Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CubesMonitor_FormClosing);
            this.groupBox_HitRates.ResumeLayout(false);
            this.groupBox_HitRates.PerformLayout();
            this.groupBox_HvpsTelemetry.ResumeLayout(false);
            this.groupBox_HvpsTelemetry.PerformLayout();
            this.groupBox_HV.ResumeLayout(false);
            this.groupBox_hvPersistent.ResumeLayout(false);
            this.groupBox_hvPersistent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dt2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dt1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_refVolt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dtp2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_dtp1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_refTemp)).EndInit();
            this.groupBox_hvTemporary.ResumeLayout(false);
            this.groupBox_hvTemporary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown_HV)).EndInit();
            this.groupBoxMonitor.ResumeLayout(false);
            this.groupBoxMonitor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_readHK;
        private System.Windows.Forms.TextBox textBox_ResetCount;
        private System.Windows.Forms.TextBox textBox_timestamp;
        private System.Windows.Forms.Label label_ResetCount;
        private System.Windows.Forms.Label label_timestamp;
        private System.Windows.Forms.GroupBox groupBox_HitRates;
        private System.Windows.Forms.Label label_hitCountMPPC1;
        private System.Windows.Forms.TextBox textBox_hitCountOR32;
        private System.Windows.Forms.TextBox textBox_hitCountMPPC2;
        private System.Windows.Forms.Label label_hitCountOR32;
        private System.Windows.Forms.TextBox textBox_hitCountMPPC1;
        private System.Windows.Forms.TextBox textBox_hitCountMPPC3;
        private System.Windows.Forms.Label label_hitCountMPPC2;
        private System.Windows.Forms.Label label_hitCountMPPC3;
        private System.Windows.Forms.GroupBox groupBox_HvpsTelemetry;
        private System.Windows.Forms.Label label_Voltage;
        private System.Windows.Forms.Label label142;
        private System.Windows.Forms.Label label_Temperature;
        private System.Windows.Forms.TextBox textBox_hvpsCmdsSent;
        private System.Windows.Forms.TextBox textBox_hvpsCmdsAcked;
        private System.Windows.Forms.TextBox textBox_hvpsCmdsRej;
        private System.Windows.Forms.TextBox textBox_tempFromHVPS;
        private System.Windows.Forms.TextBox textBox_voltageFromHVPS;
        private System.Windows.Forms.Label label_Current;
        private System.Windows.Forms.TextBox textBox_currentFromHVPS;
        private System.Windows.Forms.GroupBox groupBox_HV;
        private System.Windows.Forms.GroupBox groupBox_hvPersistent;
        private System.Windows.Forms.NumericUpDown numUpDown_dt2;
        private System.Windows.Forms.NumericUpDown numUpDown_dt1;
        private System.Windows.Forms.NumericUpDown numUpDown_refVolt;
        private System.Windows.Forms.Label label_dt2;
        private System.Windows.Forms.Label label_refVolt;
        private System.Windows.Forms.Label label_dt1;
        private System.Windows.Forms.NumericUpDown numUpDown_dtp2;
        private System.Windows.Forms.NumericUpDown numUpDown_dtp1;
        private System.Windows.Forms.Label label_dtp2;
        private System.Windows.Forms.Label label_dtp1;
        private System.Windows.Forms.Button button_hvSendPersistent;
        private System.Windows.Forms.NumericUpDown numUpDown_refTemp;
        private System.Windows.Forms.Label label_refTemp;
        private System.Windows.Forms.CheckBox checkBox_hvReset;
        private System.Windows.Forms.GroupBox groupBox_hvTemporary;
        private System.Windows.Forms.Button button_hvSendTemp;
        private System.Windows.Forms.NumericUpDown numUpDown_HV;
        private System.Windows.Forms.Label label140;
        private System.Windows.Forms.CheckBox checkBox_HVON;
        public System.Windows.Forms.RichTextBox rtxtMonitor;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Timer tmrButtonColor;
        private System.Windows.Forms.GroupBox groupBoxMonitor;
        private System.Windows.Forms.Label label_ConnStatus;
    }
}