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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtxtMonitor
            // 
            this.rtxtMonitor.BackColor = System.Drawing.SystemColors.InfoText;
            this.rtxtMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtMonitor.ForeColor = System.Drawing.SystemColors.Info;
            this.rtxtMonitor.Location = new System.Drawing.Point(0, 0);
            this.rtxtMonitor.Name = "rtxtMonitor";
            this.rtxtMonitor.ReadOnly = true;
            this.rtxtMonitor.Size = new System.Drawing.Size(454, 369);
            this.rtxtMonitor.TabIndex = 0;
            this.rtxtMonitor.Text = "";
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(376, 7);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(75, 23);
            this.button_Clear.TabIndex = 1;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // label_ConnStatus
            // 
            this.label_ConnStatus.Location = new System.Drawing.Point(3, 7);
            this.label_ConnStatus.Name = "label_ConnStatus";
            this.label_ConnStatus.Size = new System.Drawing.Size(360, 23);
            this.label_ConnStatus.TabIndex = 2;
            this.label_ConnStatus.Text = "label_ConnStatus";
            this.label_ConnStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_ConnStatus);
            this.panel1.Controls.Add(this.button_Clear);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 369);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(454, 36);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtxtMonitor);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(454, 369);
            this.panel2.TabIndex = 4;
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 405);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmMonitor";
            this.Text = "frmMonitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMonitor_FormClosed);
            this.Load += new System.EventHandler(this.frmMonitor_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtMonitor;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label_ConnStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}