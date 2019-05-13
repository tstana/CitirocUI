namespace CitirocUI
{
    partial class Form_chartParameters
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
            this.button_close = new System.Windows.Forms.Button();
            this.label_titleBar = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox_Xaxis = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_yAxisMin = new CitirocUI.doubleTextBox();
            this.textBox_yAxisMax = new CitirocUI.doubleTextBox();
            this.textBox_yAxisInterval = new CitirocUI.doubleTextBox();
            this.textBox_xAxisMin = new CitirocUI.doubleTextBox();
            this.textBox_xAxisMax = new CitirocUI.doubleTextBox();
            this.textBox_xAxisInterval = new CitirocUI.doubleTextBox();
            this.groupBox_Xaxis.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_close
            // 
            this.button_close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(144)))));
            this.button_close.FlatAppearance.BorderSize = 0;
            this.button_close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.IndianRed;
            this.button_close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            this.button_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_close.Font = new System.Drawing.Font("Bryant Regular Compressed", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_close.ForeColor = System.Drawing.Color.White;
            this.button_close.Location = new System.Drawing.Point(375, 0);
            this.button_close.Margin = new System.Windows.Forms.Padding(4, 8, 4, 8);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(30, 30);
            this.button_close.TabIndex = 31;
            this.button_close.Text = "X";
            this.button_close.UseVisualStyleBackColor = false;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // label_titleBar
            // 
            this.label_titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(144)))));
            this.label_titleBar.ForeColor = System.Drawing.Color.White;
            this.label_titleBar.Location = new System.Drawing.Point(0, 0);
            this.label_titleBar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_titleBar.Name = "label_titleBar";
            this.label_titleBar.Size = new System.Drawing.Size(405, 30);
            this.label_titleBar.TabIndex = 32;
            this.label_titleBar.Text = "   Axes setup";
            this.label_titleBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_titleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseDown);
            this.label_titleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseMove);
            this.label_titleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseUp);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(1, 30);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(403, 182);
            this.label6.TabIndex = 33;
            // 
            // groupBox_Xaxis
            // 
            this.groupBox_Xaxis.BackColor = System.Drawing.Color.White;
            this.groupBox_Xaxis.Controls.Add(this.label3);
            this.groupBox_Xaxis.Controls.Add(this.label2);
            this.groupBox_Xaxis.Controls.Add(this.label1);
            this.groupBox_Xaxis.Controls.Add(this.textBox_xAxisMin);
            this.groupBox_Xaxis.Controls.Add(this.textBox_xAxisMax);
            this.groupBox_Xaxis.Controls.Add(this.textBox_xAxisInterval);
            this.groupBox_Xaxis.Location = new System.Drawing.Point(12, 33);
            this.groupBox_Xaxis.Name = "groupBox_Xaxis";
            this.groupBox_Xaxis.Size = new System.Drawing.Size(187, 134);
            this.groupBox_Xaxis.TabIndex = 40;
            this.groupBox_Xaxis.TabStop = false;
            this.groupBox_Xaxis.Text = "X axis";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(6, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 22);
            this.label3.TabIndex = 43;
            this.label3.Text = "Interval";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(28, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 22);
            this.label2.TabIndex = 42;
            this.label2.Text = "Max";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(30, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 22);
            this.label1.TabIndex = 41;
            this.label1.Text = "Min";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox_yAxisMin);
            this.groupBox1.Controls.Add(this.textBox_yAxisMax);
            this.groupBox1.Controls.Add(this.textBox_yAxisInterval);
            this.groupBox1.Location = new System.Drawing.Point(205, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 134);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Y axis";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(6, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 22);
            this.label4.TabIndex = 43;
            this.label4.Text = "Interval";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(28, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 22);
            this.label5.TabIndex = 42;
            this.label5.Text = "Max";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(30, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 22);
            this.label7.TabIndex = 41;
            this.label7.Text = "Min";
            // 
            // button_OK
            // 
            this.button_OK.BackColor = System.Drawing.Color.Gainsboro;
            this.button_OK.FlatAppearance.BorderSize = 0;
            this.button_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OK.Location = new System.Drawing.Point(313, 173);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(80, 28);
            this.button_OK.TabIndex = 42;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = false;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(12, 176);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(275, 22);
            this.label8.TabIndex = 43;
            this.label8.Text = "Interval = 0 -> Automatic interval values";
            // 
            // textBox_yAxisMin
            // 
            this.textBox_yAxisMin.enableNegative = false;
            this.textBox_yAxisMin.Location = new System.Drawing.Point(71, 26);
            this.textBox_yAxisMin.Name = "textBox_yAxisMin";
            this.textBox_yAxisMin.Size = new System.Drawing.Size(100, 29);
            this.textBox_yAxisMin.TabIndex = 34;
            // 
            // textBox_yAxisMax
            // 
            this.textBox_yAxisMax.enableNegative = false;
            this.textBox_yAxisMax.Location = new System.Drawing.Point(71, 61);
            this.textBox_yAxisMax.Name = "textBox_yAxisMax";
            this.textBox_yAxisMax.Size = new System.Drawing.Size(100, 29);
            this.textBox_yAxisMax.TabIndex = 35;
            // 
            // textBox_yAxisInterval
            // 
            this.textBox_yAxisInterval.enableNegative = false;
            this.textBox_yAxisInterval.Location = new System.Drawing.Point(71, 96);
            this.textBox_yAxisInterval.Name = "textBox_yAxisInterval";
            this.textBox_yAxisInterval.Size = new System.Drawing.Size(100, 29);
            this.textBox_yAxisInterval.TabIndex = 36;
            // 
            // textBox_xAxisMin
            // 
            this.textBox_xAxisMin.enableNegative = true;
            this.textBox_xAxisMin.Location = new System.Drawing.Point(71, 26);
            this.textBox_xAxisMin.Name = "textBox_xAxisMin";
            this.textBox_xAxisMin.Size = new System.Drawing.Size(100, 29);
            this.textBox_xAxisMin.TabIndex = 34;
            // 
            // textBox_xAxisMax
            // 
            this.textBox_xAxisMax.enableNegative = true;
            this.textBox_xAxisMax.Location = new System.Drawing.Point(71, 61);
            this.textBox_xAxisMax.Name = "textBox_xAxisMax";
            this.textBox_xAxisMax.Size = new System.Drawing.Size(100, 29);
            this.textBox_xAxisMax.TabIndex = 35;
            // 
            // textBox_xAxisInterval
            // 
            this.textBox_xAxisInterval.enableNegative = false;
            this.textBox_xAxisInterval.Location = new System.Drawing.Point(71, 96);
            this.textBox_xAxisInterval.Name = "textBox_xAxisInterval";
            this.textBox_xAxisInterval.Size = new System.Drawing.Size(100, 29);
            this.textBox_xAxisInterval.TabIndex = 36;
            // 
            // Form_chartParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(405, 213);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox_Xaxis);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.label_titleBar);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("Bryant Regular Compressed", 14F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form_chartParameters";
            this.Text = "Form_chartParameters";
            this.groupBox_Xaxis.ResumeLayout(false);
            this.groupBox_Xaxis.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_close;
        private System.Windows.Forms.Label label_titleBar;
        private System.Windows.Forms.Label label6;
        private doubleTextBox textBox_xAxisMin;
        private doubleTextBox textBox_xAxisMax;
        private doubleTextBox textBox_xAxisInterval;
        private System.Windows.Forms.GroupBox groupBox_Xaxis;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private doubleTextBox textBox_yAxisMin;
        private doubleTextBox textBox_yAxisMax;
        private doubleTextBox textBox_yAxisInterval;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Label label8;
    }
}