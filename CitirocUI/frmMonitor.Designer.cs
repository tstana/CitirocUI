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
            this.rtxtMonitor = new System.Windows.Forms.RichTextBox();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label_ConnStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.panelHKControl = new System.Windows.Forms.Panel();
            this.panel_HKParameters = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label_Current = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label_Voltage = new System.Windows.Forms.Label();
            this.textBox_Time = new System.Windows.Forms.TextBox();
            this.label_Time = new System.Windows.Forms.Label();
            this.label_HKParameters = new System.Windows.Forms.Label();
            this.textBox_Ch21Counts = new System.Windows.Forms.TextBox();
            this.label_Ch21Counts = new System.Windows.Forms.Label();
            this.textBox_Ch31Counts = new System.Windows.Forms.TextBox();
            this.label_Ch31Counts = new System.Windows.Forms.Label();
            this.textBox_Ch16Counts = new System.Windows.Forms.TextBox();
            this.label_Ch16Counts = new System.Windows.Forms.Label();
            this.textBox_Ch0Counts = new System.Windows.Forms.TextBox();
            this.label_Ch0Counts = new System.Windows.Forms.Label();
            this.textBox_Temperature = new System.Windows.Forms.TextBox();
            this.label_Temperature = new System.Windows.Forms.Label();
            this.label_HKFrequency = new System.Windows.Forms.Label();
            this.textBox_HKFrequency = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panelHKControl.SuspendLayout();
            this.panel_HKParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtxtMonitor
            // 
            this.rtxtMonitor.BackColor = System.Drawing.SystemColors.InfoText;
            this.rtxtMonitor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rtxtMonitor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtMonitor.ForeColor = System.Drawing.SystemColors.Info;
            this.rtxtMonitor.Location = new System.Drawing.Point(0, 221);
            this.rtxtMonitor.Name = "rtxtMonitor";
            this.rtxtMonitor.ReadOnly = true;
            this.rtxtMonitor.Size = new System.Drawing.Size(454, 148);
            this.rtxtMonitor.TabIndex = 0;
            this.rtxtMonitor.Text = "";
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
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.label_ConnStatus);
            this.panel1.Controls.Add(this.buttonHelp);
            this.panel1.Controls.Add(this.button_Clear);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 369);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(454, 36);
            this.panel1.TabIndex = 3;
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
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // panelHKControl
            // 
            this.panelHKControl.Controls.Add(this.panel_HKParameters);
            this.panelHKControl.Controls.Add(this.rtxtMonitor);
            this.panelHKControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHKControl.Location = new System.Drawing.Point(0, 0);
            this.panelHKControl.Name = "panelHKControl";
            this.panelHKControl.Size = new System.Drawing.Size(454, 369);
            this.panelHKControl.TabIndex = 4;
            // 
            // panel_HKParameters
            // 
            this.panel_HKParameters.Controls.Add(this.button1);
            this.panel_HKParameters.Controls.Add(this.textBox2);
            this.panel_HKParameters.Controls.Add(this.label_Current);
            this.panel_HKParameters.Controls.Add(this.textBox1);
            this.panel_HKParameters.Controls.Add(this.label_Voltage);
            this.panel_HKParameters.Controls.Add(this.textBox_Time);
            this.panel_HKParameters.Controls.Add(this.label_Time);
            this.panel_HKParameters.Controls.Add(this.label_HKParameters);
            this.panel_HKParameters.Controls.Add(this.textBox_Ch21Counts);
            this.panel_HKParameters.Controls.Add(this.label_Ch21Counts);
            this.panel_HKParameters.Controls.Add(this.textBox_Ch31Counts);
            this.panel_HKParameters.Controls.Add(this.label_Ch31Counts);
            this.panel_HKParameters.Controls.Add(this.textBox_Ch16Counts);
            this.panel_HKParameters.Controls.Add(this.label_Ch16Counts);
            this.panel_HKParameters.Controls.Add(this.textBox_Ch0Counts);
            this.panel_HKParameters.Controls.Add(this.label_Ch0Counts);
            this.panel_HKParameters.Controls.Add(this.textBox_Temperature);
            this.panel_HKParameters.Controls.Add(this.label_Temperature);
            this.panel_HKParameters.Controls.Add(this.label_HKFrequency);
            this.panel_HKParameters.Controls.Add(this.textBox_HKFrequency);
            this.panel_HKParameters.Location = new System.Drawing.Point(6, 3);
            this.panel_HKParameters.Name = "panel_HKParameters";
            this.panel_HKParameters.Size = new System.Drawing.Size(445, 200);
            this.panel_HKParameters.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(328, 99);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(83, 20);
            this.textBox2.TabIndex = 18;
            // 
            // label_Current
            // 
            this.label_Current.AutoSize = true;
            this.label_Current.Location = new System.Drawing.Point(251, 99);
            this.label_Current.Name = "label_Current";
            this.label_Current.Size = new System.Drawing.Size(41, 13);
            this.label_Current.TabIndex = 17;
            this.label_Current.Text = "Current";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(90, 99);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(83, 20);
            this.textBox1.TabIndex = 16;
            // 
            // label_Voltage
            // 
            this.label_Voltage.AutoSize = true;
            this.label_Voltage.Location = new System.Drawing.Point(36, 99);
            this.label_Voltage.Name = "label_Voltage";
            this.label_Voltage.Size = new System.Drawing.Size(43, 13);
            this.label_Voltage.TabIndex = 15;
            this.label_Voltage.Text = "Voltage";
            // 
            // textBox_Time
            // 
            this.textBox_Time.Location = new System.Drawing.Point(204, 29);
            this.textBox_Time.Name = "textBox_Time";
            this.textBox_Time.ReadOnly = true;
            this.textBox_Time.Size = new System.Drawing.Size(83, 20);
            this.textBox_Time.TabIndex = 14;
            // 
            // label_Time
            // 
            this.label_Time.AutoSize = true;
            this.label_Time.Location = new System.Drawing.Point(148, 29);
            this.label_Time.Name = "label_Time";
            this.label_Time.Size = new System.Drawing.Size(30, 13);
            this.label_Time.TabIndex = 13;
            this.label_Time.Text = "Time";
            // 
            // label_HKParameters
            // 
            this.label_HKParameters.AutoSize = true;
            this.label_HKParameters.Location = new System.Drawing.Point(11, 6);
            this.label_HKParameters.Name = "label_HKParameters";
            this.label_HKParameters.Size = new System.Drawing.Size(99, 13);
            this.label_HKParameters.TabIndex = 12;
            this.label_HKParameters.Text = "HK PARAMETERS";
            // 
            // textBox_Ch21Counts
            // 
            this.textBox_Ch21Counts.Location = new System.Drawing.Point(328, 174);
            this.textBox_Ch21Counts.Name = "textBox_Ch21Counts";
            this.textBox_Ch21Counts.ReadOnly = true;
            this.textBox_Ch21Counts.Size = new System.Drawing.Size(83, 20);
            this.textBox_Ch21Counts.TabIndex = 11;
            // 
            // label_Ch21Counts
            // 
            this.label_Ch21Counts.AutoSize = true;
            this.label_Ch21Counts.Location = new System.Drawing.Point(248, 174);
            this.label_Ch21Counts.Name = "label_Ch21Counts";
            this.label_Ch21Counts.Size = new System.Drawing.Size(68, 13);
            this.label_Ch21Counts.TabIndex = 10;
            this.label_Ch21Counts.Text = "Ch21 Counts";
            // 
            // textBox_Ch31Counts
            // 
            this.textBox_Ch31Counts.Location = new System.Drawing.Point(90, 167);
            this.textBox_Ch31Counts.Name = "textBox_Ch31Counts";
            this.textBox_Ch31Counts.ReadOnly = true;
            this.textBox_Ch31Counts.Size = new System.Drawing.Size(83, 20);
            this.textBox_Ch31Counts.TabIndex = 9;
            // 
            // label_Ch31Counts
            // 
            this.label_Ch31Counts.AutoSize = true;
            this.label_Ch31Counts.Location = new System.Drawing.Point(10, 170);
            this.label_Ch31Counts.Name = "label_Ch31Counts";
            this.label_Ch31Counts.Size = new System.Drawing.Size(71, 13);
            this.label_Ch31Counts.TabIndex = 8;
            this.label_Ch31Counts.Text = "Ch 31 Counts";
            // 
            // textBox_Ch16Counts
            // 
            this.textBox_Ch16Counts.Location = new System.Drawing.Point(328, 136);
            this.textBox_Ch16Counts.Name = "textBox_Ch16Counts";
            this.textBox_Ch16Counts.ReadOnly = true;
            this.textBox_Ch16Counts.Size = new System.Drawing.Size(83, 20);
            this.textBox_Ch16Counts.TabIndex = 7;
            // 
            // label_Ch16Counts
            // 
            this.label_Ch16Counts.AutoSize = true;
            this.label_Ch16Counts.Location = new System.Drawing.Point(248, 139);
            this.label_Ch16Counts.Name = "label_Ch16Counts";
            this.label_Ch16Counts.Size = new System.Drawing.Size(71, 13);
            this.label_Ch16Counts.TabIndex = 6;
            this.label_Ch16Counts.Text = "Ch 16 Counts";
            // 
            // textBox_Ch0Counts
            // 
            this.textBox_Ch0Counts.Location = new System.Drawing.Point(90, 132);
            this.textBox_Ch0Counts.Name = "textBox_Ch0Counts";
            this.textBox_Ch0Counts.ReadOnly = true;
            this.textBox_Ch0Counts.Size = new System.Drawing.Size(83, 20);
            this.textBox_Ch0Counts.TabIndex = 5;
            // 
            // label_Ch0Counts
            // 
            this.label_Ch0Counts.AutoSize = true;
            this.label_Ch0Counts.Location = new System.Drawing.Point(11, 136);
            this.label_Ch0Counts.Name = "label_Ch0Counts";
            this.label_Ch0Counts.Size = new System.Drawing.Size(65, 13);
            this.label_Ch0Counts.TabIndex = 4;
            this.label_Ch0Counts.Text = "Ch 0 Counts";
            // 
            // textBox_Temperature
            // 
            this.textBox_Temperature.Location = new System.Drawing.Point(328, 63);
            this.textBox_Temperature.Name = "textBox_Temperature";
            this.textBox_Temperature.ReadOnly = true;
            this.textBox_Temperature.Size = new System.Drawing.Size(83, 20);
            this.textBox_Temperature.TabIndex = 3;
            // 
            // label_Temperature
            // 
            this.label_Temperature.AutoSize = true;
            this.label_Temperature.Location = new System.Drawing.Point(235, 63);
            this.label_Temperature.Name = "label_Temperature";
            this.label_Temperature.Size = new System.Drawing.Size(67, 13);
            this.label_Temperature.TabIndex = 2;
            this.label_Temperature.Text = "Temperature";
            // 
            // label_HKFrequency
            // 
            this.label_HKFrequency.AutoSize = true;
            this.label_HKFrequency.Location = new System.Drawing.Point(6, 63);
            this.label_HKFrequency.Name = "label_HKFrequency";
            this.label_HKFrequency.Size = new System.Drawing.Size(75, 13);
            this.label_HKFrequency.TabIndex = 1;
            this.label_HKFrequency.Text = "HK Frequency";
            // 
            // textBox_HKFrequency
            // 
            this.textBox_HKFrequency.Location = new System.Drawing.Point(90, 63);
            this.textBox_HKFrequency.Name = "textBox_HKFrequency";
            this.textBox_HKFrequency.ReadOnly = true;
            this.textBox_HKFrequency.Size = new System.Drawing.Size(83, 20);
            this.textBox_HKFrequency.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(281, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 19;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 405);
            this.Controls.Add(this.panelHKControl);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmMonitor";
            this.Text = "frmMonitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMonitor_FormClosed);
            this.Load += new System.EventHandler(this.frmMonitor_Load);
            this.panel1.ResumeLayout(false);
            this.panelHKControl.ResumeLayout(false);
            this.panel_HKParameters.ResumeLayout(false);
            this.panel_HKParameters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label_ConnStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelHKControl;
        private System.Windows.Forms.Button buttonHelp;
        public System.Windows.Forms.RichTextBox rtxtMonitor;
        private System.Windows.Forms.Panel panel_HKParameters;
        private System.Windows.Forms.TextBox textBox_HKFrequency;
        private System.Windows.Forms.TextBox textBox_Temperature;
        private System.Windows.Forms.Label label_Temperature;
        private System.Windows.Forms.Label label_HKFrequency;
        private System.Windows.Forms.Label label_Ch0Counts;
        private System.Windows.Forms.Label label_Ch16Counts;
        private System.Windows.Forms.TextBox textBox_Ch0Counts;
        private System.Windows.Forms.TextBox textBox_Ch16Counts;
        private System.Windows.Forms.TextBox textBox_Ch21Counts;
        private System.Windows.Forms.Label label_Ch21Counts;
        private System.Windows.Forms.TextBox textBox_Ch31Counts;
        private System.Windows.Forms.Label label_Ch31Counts;
        private System.Windows.Forms.Label label_HKParameters;
        private System.Windows.Forms.TextBox textBox_Time;
        private System.Windows.Forms.Label label_Time;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label_Current;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label_Voltage;
        private System.Windows.Forms.Button button1;
    }
}