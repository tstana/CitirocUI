namespace CitirocUI
{
    partial class Form_fit
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
            this.button_fit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_close = new System.Windows.Forms.Button();
            this.label_titleBar = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_sigmaMax = new CitirocUI.intTextBox();
            this.textBox_sigmaMin = new CitirocUI.intTextBox();
            this.textBox_sigmaGuess = new CitirocUI.intTextBox();
            this.textBox_fitMax = new CitirocUI.intTextBox();
            this.textBox_fitMin = new CitirocUI.intTextBox();
            this.SuspendLayout();
            // 
            // button_fit
            // 
            this.button_fit.BackColor = System.Drawing.Color.Gainsboro;
            this.button_fit.FlatAppearance.BorderSize = 0;
            this.button_fit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_fit.Location = new System.Drawing.Point(14, 188);
            this.button_fit.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.button_fit.Name = "button_fit";
            this.button_fit.Size = new System.Drawing.Size(299, 50);
            this.button_fit.TabIndex = 2;
            this.button_fit.Text = "FIT !";
            this.button_fit.UseVisualStyleBackColor = false;
            this.button_fit.Click += new System.EventHandler(this.button_fit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(11, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 22);
            this.label1.TabIndex = 3;
            this.label1.Text = "Fit from x =";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(184, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 22);
            this.label2.TabIndex = 4;
            this.label2.Text = "to x =";
            // 
            // btn_close
            // 
            this.btn_close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(144)))));
            this.btn_close.FlatAppearance.BorderSize = 0;
            this.btn_close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.IndianRed;
            this.btn_close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            this.btn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_close.ForeColor = System.Drawing.Color.White;
            this.btn_close.Location = new System.Drawing.Point(293, 0);
            this.btn_close.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(34, 37);
            this.btn_close.TabIndex = 22;
            this.btn_close.Text = "X";
            this.btn_close.UseVisualStyleBackColor = false;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // label_titleBar
            // 
            this.label_titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(144)))));
            this.label_titleBar.ForeColor = System.Drawing.Color.White;
            this.label_titleBar.Location = new System.Drawing.Point(0, 0);
            this.label_titleBar.Name = "label_titleBar";
            this.label_titleBar.Size = new System.Drawing.Size(327, 37);
            this.label_titleBar.TabIndex = 23;
            this.label_titleBar.Text = "   Gaussian fit setup";
            this.label_titleBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_titleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseDown);
            this.label_titleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseMove);
            this.label_titleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_titleBar_MouseUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(11, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 22);
            this.label3.TabIndex = 24;
            this.label3.Text = "Sigma guess";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(11, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 22);
            this.label4.TabIndex = 26;
            this.label4.Text = "Sigma limits";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(201, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 22);
            this.label5.TabIndex = 29;
            this.label5.Text = "to";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(1, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(325, 216);
            this.label6.TabIndex = 30;
            // 
            // textBox_sigmaMax
            // 
            this.textBox_sigmaMax.enableNegative = false;
            this.textBox_sigmaMax.Location = new System.Drawing.Point(234, 139);
            this.textBox_sigmaMax.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textBox_sigmaMax.Name = "textBox_sigmaMax";
            this.textBox_sigmaMax.Size = new System.Drawing.Size(78, 29);
            this.textBox_sigmaMax.TabIndex = 28;
            this.textBox_sigmaMax.Text = "10";
            // 
            // textBox_sigmaMin
            // 
            this.textBox_sigmaMin.enableNegative = false;
            this.textBox_sigmaMin.Location = new System.Drawing.Point(110, 139);
            this.textBox_sigmaMin.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textBox_sigmaMin.Name = "textBox_sigmaMin";
            this.textBox_sigmaMin.Size = new System.Drawing.Size(78, 29);
            this.textBox_sigmaMin.TabIndex = 27;
            this.textBox_sigmaMin.Text = "0";
            // 
            // textBox_sigmaGuess
            // 
            this.textBox_sigmaGuess.enableNegative = false;
            this.textBox_sigmaGuess.Location = new System.Drawing.Point(110, 98);
            this.textBox_sigmaGuess.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textBox_sigmaGuess.Name = "textBox_sigmaGuess";
            this.textBox_sigmaGuess.Size = new System.Drawing.Size(78, 29);
            this.textBox_sigmaGuess.TabIndex = 25;
            this.textBox_sigmaGuess.Text = "1";
            // 
            // textBox_fitMax
            // 
            this.textBox_fitMax.enableNegative = false;
            this.textBox_fitMax.Location = new System.Drawing.Point(234, 56);
            this.textBox_fitMax.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textBox_fitMax.Name = "textBox_fitMax";
            this.textBox_fitMax.Size = new System.Drawing.Size(78, 29);
            this.textBox_fitMax.TabIndex = 1;
            this.textBox_fitMax.Text = "1024";
            // 
            // textBox_fitMin
            // 
            this.textBox_fitMin.enableNegative = false;
            this.textBox_fitMin.Location = new System.Drawing.Point(98, 56);
            this.textBox_fitMin.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textBox_fitMin.Name = "textBox_fitMin";
            this.textBox_fitMin.Size = new System.Drawing.Size(78, 29);
            this.textBox_fitMin.TabIndex = 0;
            this.textBox_fitMin.Text = "0";
            // 
            // Form_fit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(327, 254);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_sigmaMax);
            this.Controls.Add(this.textBox_sigmaMin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_sigmaGuess);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_fit);
            this.Controls.Add(this.textBox_fitMax);
            this.Controls.Add(this.textBox_fitMin);
            this.Controls.Add(this.label_titleBar);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "Form_fit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form_fit";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Load += new System.EventHandler(this.Form_fit_Load);

        }

        #endregion

        private intTextBox textBox_fitMin;
        private intTextBox textBox_fitMax;
        private System.Windows.Forms.Button button_fit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Label label_titleBar;
        private System.Windows.Forms.Label label3;
        private intTextBox textBox_sigmaGuess;
        private System.Windows.Forms.Label label4;
        private intTextBox textBox_sigmaMin;
        private intTextBox textBox_sigmaMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}