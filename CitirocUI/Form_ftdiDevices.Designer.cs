namespace CitirocUI
{
    partial class Form_ftdiDevices
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
            this.button_OK = new System.Windows.Forms.Button();
            this.label_titleBar = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.listBox_ftdiDevices = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button_OK
            // 
            this.button_OK.BackColor = System.Drawing.Color.Gainsboro;
            this.button_OK.FlatAppearance.BorderSize = 0;
            this.button_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OK.Location = new System.Drawing.Point(14, 207);
            this.button_OK.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(302, 32);
            this.button_OK.TabIndex = 33;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = false;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // label_titleBar
            // 
            this.label_titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(144)))));
            this.label_titleBar.ForeColor = System.Drawing.Color.White;
            this.label_titleBar.Location = new System.Drawing.Point(0, 0);
            this.label_titleBar.Name = "label_titleBar";
            this.label_titleBar.Size = new System.Drawing.Size(328, 37);
            this.label_titleBar.TabIndex = 37;
            this.label_titleBar.Text = "   FTDI device selection";
            this.label_titleBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_titleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseDown);
            this.label_titleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseMove);
            this.label_titleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseUp);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(1, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(326, 217);
            this.label6.TabIndex = 44;
            // 
            // listBox_ftdiDevices
            // 
            this.listBox_ftdiDevices.FormattingEnabled = true;
            this.listBox_ftdiDevices.ItemHeight = 18;
            this.listBox_ftdiDevices.Location = new System.Drawing.Point(14, 51);
            this.listBox_ftdiDevices.Name = "listBox_ftdiDevices";
            this.listBox_ftdiDevices.Size = new System.Drawing.Size(302, 148);
            this.listBox_ftdiDevices.TabIndex = 45;
            // 
            // Form_ftdiDevices
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(328, 255);
            this.Controls.Add(this.listBox_ftdiDevices);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label_titleBar);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("Bryant Regular Compressed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_ftdiDevices";
            this.Text = "Form_ftdiDevices";
            this.Load += new System.EventHandler(this.Form_ftdiDevices_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Label label_titleBar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBox_ftdiDevices;
    }
}