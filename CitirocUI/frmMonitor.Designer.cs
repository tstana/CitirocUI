namespace CitirocUI
{
    partial class frmMonitor
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
            this.panel_main = new System.Windows.Forms.TableLayoutPanel();
            this.panel_clearSerialMonitor = new System.Windows.Forms.Panel();
            this.label_ConnStatus = new System.Windows.Forms.Label();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.groupBox_HitRates = new System.Windows.Forms.GroupBox();
            this.label_hitCountMPPC1 = new System.Windows.Forms.Label();
            this.textBox_hitCountOR32 = new System.Windows.Forms.TextBox();
            this.textBox_hitCountMPPC2 = new System.Windows.Forms.TextBox();
            this.label_hitCountOR32 = new System.Windows.Forms.Label();
            this.textBox_hitCountMPPC1 = new System.Windows.Forms.TextBox();
            this.textBox_hitCountMPPC3 = new System.Windows.Forms.TextBox();
            this.label_hitCountMPPC2 = new System.Windows.Forms.Label();
            this.label_hitCountMPPC3 = new System.Windows.Forms.Label();
            this.rtxtMonitor = new System.Windows.Forms.RichTextBox();
            this.groupBox_HvpsTelemetry = new System.Windows.Forms.GroupBox();
            this.label_Voltage = new System.Windows.Forms.Label();
            this.label_Temperature = new System.Windows.Forms.Label();
            this.textBox_tempFromHVPS = new System.Windows.Forms.TextBox();
            this.textBox_voltageFromHVPS = new System.Windows.Forms.TextBox();
            this.label_Current = new System.Windows.Forms.Label();
            this.textBox_currentFromHVPS = new System.Windows.Forms.TextBox();
            this.panel_main.SuspendLayout();
            this.panel_clearSerialMonitor.SuspendLayout();
            this.groupBox_HitRates.SuspendLayout();
            this.groupBox_HvpsTelemetry.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_main
            // 
            this.panel_main.ColumnCount = 2;
            this.panel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel_main.Controls.Add(this.groupBox_HitRates, 0, 0);
            this.panel_main.Controls.Add(this.rtxtMonitor, 0, 1);
            this.panel_main.Controls.Add(this.groupBox_HvpsTelemetry, 1, 0);
            this.panel_main.Controls.Add(this.panel_clearSerialMonitor, 0, 2);
            this.panel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_main.Location = new System.Drawing.Point(0, 0);
            this.panel_main.Name = "panel_main";
            this.panel_main.RowCount = 3;
            this.panel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.panel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.panel_main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel_main.Size = new System.Drawing.Size(454, 405);
            this.panel_main.TabIndex = 4;
            // 
            // panel_clearSerialMonitor
            // 
            this.panel_main.SetColumnSpan(this.panel_clearSerialMonitor, 2);
            this.panel_clearSerialMonitor.Controls.Add(this.label_ConnStatus);
            this.panel_clearSerialMonitor.Controls.Add(this.buttonHelp);
            this.panel_clearSerialMonitor.Controls.Add(this.button_Clear);
            this.panel_clearSerialMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_clearSerialMonitor.Location = new System.Drawing.Point(3, 366);
            this.panel_clearSerialMonitor.Name = "panel_clearSerialMonitor";
            this.panel_clearSerialMonitor.Size = new System.Drawing.Size(448, 36);
            this.panel_clearSerialMonitor.TabIndex = 20;
            // 
            // label_ConnStatus
            // 
            this.label_ConnStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label_ConnStatus.Location = new System.Drawing.Point(3, 7);
            this.label_ConnStatus.Name = "label_ConnStatus";
            this.label_ConnStatus.Size = new System.Drawing.Size(325, 23);
            this.label_ConnStatus.TabIndex = 2;
            this.label_ConnStatus.Text = "label_ConnStatus";
            this.label_ConnStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonHelp
            // 
            this.buttonHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.buttonHelp.Location = new System.Drawing.Point(334, 0);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(29, 36);
            this.buttonHelp.TabIndex = 1;
            this.buttonHelp.Text = "?";
            this.buttonHelp.UseVisualStyleBackColor = true;
            // 
            // button_Clear
            // 
            this.button_Clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.button_Clear.Location = new System.Drawing.Point(369, 0);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(85, 36);
            this.button_Clear.TabIndex = 1;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
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
            this.groupBox_HitRates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_HitRates.Location = new System.Drawing.Point(3, 3);
            this.groupBox_HitRates.Name = "groupBox_HitRates";
            this.groupBox_HitRates.Size = new System.Drawing.Size(221, 115);
            this.groupBox_HitRates.TabIndex = 0;
            this.groupBox_HitRates.TabStop = false;
            this.groupBox_HitRates.Text = "Channel Hit Rates";
            // 
            // label_hitCountMPPC1
            // 
            this.label_hitCountMPPC1.AutoSize = true;
            this.label_hitCountMPPC1.Location = new System.Drawing.Point(6, 20);
            this.label_hitCountMPPC1.Name = "label_hitCountMPPC1";
            this.label_hitCountMPPC1.Size = new System.Drawing.Size(46, 13);
            this.label_hitCountMPPC1.TabIndex = 20;
            this.label_hitCountMPPC1.Text = "MPPC1:";
            // 
            // textBox_hitCountOR32
            // 
            this.textBox_hitCountOR32.Location = new System.Drawing.Point(85, 93);
            this.textBox_hitCountOR32.Name = "textBox_hitCountOR32";
            this.textBox_hitCountOR32.ReadOnly = true;
            this.textBox_hitCountOR32.Size = new System.Drawing.Size(83, 20);
            this.textBox_hitCountOR32.TabIndex = 11;
            // 
            // textBox_hitCountMPPC2
            // 
            this.textBox_hitCountMPPC2.Location = new System.Drawing.Point(85, 42);
            this.textBox_hitCountMPPC2.Name = "textBox_hitCountMPPC2";
            this.textBox_hitCountMPPC2.ReadOnly = true;
            this.textBox_hitCountMPPC2.Size = new System.Drawing.Size(83, 20);
            this.textBox_hitCountMPPC2.TabIndex = 23;
            // 
            // label_hitCountOR32
            // 
            this.label_hitCountOR32.AutoSize = true;
            this.label_hitCountOR32.Location = new System.Drawing.Point(6, 96);
            this.label_hitCountOR32.Name = "label_hitCountOR32";
            this.label_hitCountOR32.Size = new System.Drawing.Size(58, 13);
            this.label_hitCountOR32.TabIndex = 10;
            this.label_hitCountOR32.Text = "All (OR32):";
            // 
            // textBox_hitCountMPPC1
            // 
            this.textBox_hitCountMPPC1.Location = new System.Drawing.Point(85, 16);
            this.textBox_hitCountMPPC1.Name = "textBox_hitCountMPPC1";
            this.textBox_hitCountMPPC1.ReadOnly = true;
            this.textBox_hitCountMPPC1.Size = new System.Drawing.Size(83, 20);
            this.textBox_hitCountMPPC1.TabIndex = 21;
            // 
            // textBox_hitCountMPPC3
            // 
            this.textBox_hitCountMPPC3.Location = new System.Drawing.Point(85, 68);
            this.textBox_hitCountMPPC3.Name = "textBox_hitCountMPPC3";
            this.textBox_hitCountMPPC3.ReadOnly = true;
            this.textBox_hitCountMPPC3.Size = new System.Drawing.Size(83, 20);
            this.textBox_hitCountMPPC3.TabIndex = 7;
            // 
            // label_hitCountMPPC2
            // 
            this.label_hitCountMPPC2.AutoSize = true;
            this.label_hitCountMPPC2.Location = new System.Drawing.Point(6, 45);
            this.label_hitCountMPPC2.Name = "label_hitCountMPPC2";
            this.label_hitCountMPPC2.Size = new System.Drawing.Size(46, 13);
            this.label_hitCountMPPC2.TabIndex = 22;
            this.label_hitCountMPPC2.Text = "MPPC2:";
            // 
            // label_hitCountMPPC3
            // 
            this.label_hitCountMPPC3.AutoSize = true;
            this.label_hitCountMPPC3.Location = new System.Drawing.Point(6, 71);
            this.label_hitCountMPPC3.Name = "label_hitCountMPPC3";
            this.label_hitCountMPPC3.Size = new System.Drawing.Size(46, 13);
            this.label_hitCountMPPC3.TabIndex = 6;
            this.label_hitCountMPPC3.Text = "MPPC3:";
            // 
            // rtxtMonitor
            // 
            this.rtxtMonitor.BackColor = System.Drawing.SystemColors.InfoText;
            this.panel_main.SetColumnSpan(this.rtxtMonitor, 2);
            this.rtxtMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtMonitor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtMonitor.ForeColor = System.Drawing.SystemColors.Info;
            this.rtxtMonitor.Location = new System.Drawing.Point(3, 124);
            this.rtxtMonitor.Name = "rtxtMonitor";
            this.rtxtMonitor.ReadOnly = true;
            this.rtxtMonitor.Size = new System.Drawing.Size(448, 236);
            this.rtxtMonitor.TabIndex = 0;
            this.rtxtMonitor.Text = "";
            // 
            // groupBox_HvpsTelemetry
            // 
            this.groupBox_HvpsTelemetry.Controls.Add(this.label_Voltage);
            this.groupBox_HvpsTelemetry.Controls.Add(this.label_Temperature);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_tempFromHVPS);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_voltageFromHVPS);
            this.groupBox_HvpsTelemetry.Controls.Add(this.label_Current);
            this.groupBox_HvpsTelemetry.Controls.Add(this.textBox_currentFromHVPS);
            this.groupBox_HvpsTelemetry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_HvpsTelemetry.Location = new System.Drawing.Point(230, 3);
            this.groupBox_HvpsTelemetry.Name = "groupBox_HvpsTelemetry";
            this.groupBox_HvpsTelemetry.Size = new System.Drawing.Size(221, 115);
            this.groupBox_HvpsTelemetry.TabIndex = 19;
            this.groupBox_HvpsTelemetry.TabStop = false;
            this.groupBox_HvpsTelemetry.Text = "HVPS Telemetry";
            // 
            // label_Voltage
            // 
            this.label_Voltage.AutoSize = true;
            this.label_Voltage.Location = new System.Drawing.Point(6, 19);
            this.label_Voltage.Name = "label_Voltage";
            this.label_Voltage.Size = new System.Drawing.Size(46, 13);
            this.label_Voltage.TabIndex = 15;
            this.label_Voltage.Text = "Voltage:";
            // 
            // label_Temperature
            // 
            this.label_Temperature.AutoSize = true;
            this.label_Temperature.Location = new System.Drawing.Point(6, 71);
            this.label_Temperature.Name = "label_Temperature";
            this.label_Temperature.Size = new System.Drawing.Size(70, 13);
            this.label_Temperature.TabIndex = 2;
            this.label_Temperature.Text = "Temperature:";
            // 
            // textBox_tempFromHVPS
            // 
            this.textBox_tempFromHVPS.Location = new System.Drawing.Point(86, 68);
            this.textBox_tempFromHVPS.Name = "textBox_tempFromHVPS";
            this.textBox_tempFromHVPS.ReadOnly = true;
            this.textBox_tempFromHVPS.Size = new System.Drawing.Size(83, 20);
            this.textBox_tempFromHVPS.TabIndex = 3;
            // 
            // textBox_voltageFromHVPS
            // 
            this.textBox_voltageFromHVPS.Location = new System.Drawing.Point(86, 16);
            this.textBox_voltageFromHVPS.Name = "textBox_voltageFromHVPS";
            this.textBox_voltageFromHVPS.ReadOnly = true;
            this.textBox_voltageFromHVPS.Size = new System.Drawing.Size(83, 20);
            this.textBox_voltageFromHVPS.TabIndex = 16;
            // 
            // label_Current
            // 
            this.label_Current.AutoSize = true;
            this.label_Current.Location = new System.Drawing.Point(6, 45);
            this.label_Current.Name = "label_Current";
            this.label_Current.Size = new System.Drawing.Size(44, 13);
            this.label_Current.TabIndex = 17;
            this.label_Current.Text = "Current:";
            // 
            // textBox_currentFromHVPS
            // 
            this.textBox_currentFromHVPS.Location = new System.Drawing.Point(86, 42);
            this.textBox_currentFromHVPS.Name = "textBox_currentFromHVPS";
            this.textBox_currentFromHVPS.ReadOnly = true;
            this.textBox_currentFromHVPS.Size = new System.Drawing.Size(83, 20);
            this.textBox_currentFromHVPS.TabIndex = 18;
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 405);
            this.Controls.Add(this.panel_main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmMonitor";
            this.Text = "frmMonitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMonitor_FormClosed);
            this.Load += new System.EventHandler(this.frmMonitor_Load);
            this.panel_main.ResumeLayout(false);
            this.panel_clearSerialMonitor.ResumeLayout(false);
            this.groupBox_HitRates.ResumeLayout(false);
            this.groupBox_HitRates.PerformLayout();
            this.groupBox_HvpsTelemetry.ResumeLayout(false);
            this.groupBox_HvpsTelemetry.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel panel_main;
        private System.Windows.Forms.Panel panel_clearSerialMonitor;
        private System.Windows.Forms.Label label_ConnStatus;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.GroupBox groupBox_HitRates;
        private System.Windows.Forms.Label label_hitCountMPPC1;
        private System.Windows.Forms.TextBox textBox_hitCountOR32;
        private System.Windows.Forms.TextBox textBox_hitCountMPPC2;
        private System.Windows.Forms.Label label_hitCountOR32;
        private System.Windows.Forms.TextBox textBox_hitCountMPPC1;
        private System.Windows.Forms.TextBox textBox_hitCountMPPC3;
        private System.Windows.Forms.Label label_hitCountMPPC2;
        private System.Windows.Forms.Label label_hitCountMPPC3;
        public System.Windows.Forms.RichTextBox rtxtMonitor;
        private System.Windows.Forms.GroupBox groupBox_HvpsTelemetry;
        private System.Windows.Forms.Label label_Voltage;
        private System.Windows.Forms.Label label_Temperature;
        private System.Windows.Forms.TextBox textBox_tempFromHVPS;
        private System.Windows.Forms.TextBox textBox_voltageFromHVPS;
        private System.Windows.Forms.Label label_Current;
        private System.Windows.Forms.TextBox textBox_currentFromHVPS;
    }
}