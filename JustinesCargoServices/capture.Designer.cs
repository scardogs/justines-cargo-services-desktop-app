namespace FinalGUI
{
    partial class capture
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Prompt = new System.Windows.Forms.TextBox();
            this.StatusText = new System.Windows.Forms.TextBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.fname = new System.Windows.Forms.TextBox();
            this.start_scan = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lname = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.empID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnTimeIn = new System.Windows.Forms.Button();
            this.fImage = new System.Windows.Forms.PictureBox();
            this.btnTimeOut = new System.Windows.Forms.Button();
            this.btn2ndFinger = new System.Windows.Forms.Button();
            this.btn3rdFinger = new System.Windows.Forms.Button();
            this.lb2ndfinger = new System.Windows.Forms.Label();
            this.lb3rdFinger = new System.Windows.Forms.Label();
            this.lbFirstFinger = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fImage)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, -2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(744, 42);
            this.panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(28)))), ((int)(((byte)(50)))));
            this.btnClose.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClose.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnClose.FillColor = System.Drawing.Color.Maroon;
            this.btnClose.Font = new System.Drawing.Font("Arial Black", 13.77391F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(703, 3);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(41, 39);
            this.btnClose.TabIndex = 151;
            this.btnClose.Text = "X";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(269, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "BIOMETRICS";
            // 
            // Prompt
            // 
            this.Prompt.Enabled = false;
            this.Prompt.Location = new System.Drawing.Point(332, 92);
            this.Prompt.Name = "Prompt";
            this.Prompt.Size = new System.Drawing.Size(388, 20);
            this.Prompt.TabIndex = 2;
            // 
            // StatusText
            // 
            this.StatusText.Enabled = false;
            this.StatusText.Location = new System.Drawing.Point(332, 132);
            this.StatusText.Multiline = true;
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(388, 38);
            this.StatusText.TabIndex = 3;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.ForeColor = System.Drawing.Color.IndianRed;
            this.StatusLabel.Location = new System.Drawing.Point(19, 426);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(53, 13);
            this.StatusLabel.TabIndex = 4;
            this.StatusLabel.Text = "STATUS:";
            // 
            // fname
            // 
            this.fname.Enabled = false;
            this.fname.Location = new System.Drawing.Point(332, 241);
            this.fname.Name = "fname";
            this.fname.Size = new System.Drawing.Size(280, 20);
            this.fname.TabIndex = 5;
            this.fname.TextChanged += new System.EventHandler(this.fname_TextChanged);
            // 
            // start_scan
            // 
            this.start_scan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.start_scan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            this.start_scan.Font = new System.Drawing.Font("Microsoft JhengHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_scan.ForeColor = System.Drawing.Color.White;
            this.start_scan.Location = new System.Drawing.Point(613, 386);
            this.start_scan.Name = "start_scan";
            this.start_scan.Size = new System.Drawing.Size(107, 40);
            this.start_scan.TabIndex = 6;
            this.start_scan.Text = "Start Scan";
            this.start_scan.UseVisualStyleBackColor = false;
            this.start_scan.Click += new System.EventHandler(this.start_scan_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(332, 225);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "First Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(332, 264);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Last Name:";
            // 
            // lname
            // 
            this.lname.Enabled = false;
            this.lname.Location = new System.Drawing.Point(332, 280);
            this.lname.Name = "lname";
            this.lname.Size = new System.Drawing.Size(280, 20);
            this.lname.TabIndex = 9;
            this.lname.TextChanged += new System.EventHandler(this.lname_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(333, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Prompt:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(333, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Status:";
            // 
            // empID
            // 
            this.empID.Enabled = false;
            this.empID.Location = new System.Drawing.Point(332, 202);
            this.empID.Name = "empID";
            this.empID.Size = new System.Drawing.Size(280, 20);
            this.empID.TabIndex = 12;
            this.empID.TextChanged += new System.EventHandler(this.empID_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(329, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Employee ID";
            // 
            // btnTimeIn
            // 
            this.btnTimeIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTimeIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            this.btnTimeIn.ForeColor = System.Drawing.Color.White;
            this.btnTimeIn.Location = new System.Drawing.Point(269, 386);
            this.btnTimeIn.Name = "btnTimeIn";
            this.btnTimeIn.Size = new System.Drawing.Size(107, 40);
            this.btnTimeIn.TabIndex = 14;
            this.btnTimeIn.Text = "Time In";
            this.btnTimeIn.UseVisualStyleBackColor = false;
            this.btnTimeIn.Click += new System.EventHandler(this.btnTimeIn_Click);
            // 
            // fImage
            // 
            this.fImage.Location = new System.Drawing.Point(22, 82);
            this.fImage.Name = "fImage";
            this.fImage.Size = new System.Drawing.Size(304, 241);
            this.fImage.TabIndex = 1;
            this.fImage.TabStop = false;
            // 
            // btnTimeOut
            // 
            this.btnTimeOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTimeOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            this.btnTimeOut.ForeColor = System.Drawing.Color.White;
            this.btnTimeOut.Location = new System.Drawing.Point(382, 386);
            this.btnTimeOut.Name = "btnTimeOut";
            this.btnTimeOut.Size = new System.Drawing.Size(107, 40);
            this.btnTimeOut.TabIndex = 15;
            this.btnTimeOut.Text = "Time Out";
            this.btnTimeOut.UseVisualStyleBackColor = false;
            this.btnTimeOut.Click += new System.EventHandler(this.btnTimeOut_Click);
            // 
            // btn2ndFinger
            // 
            this.btn2ndFinger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn2ndFinger.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            this.btn2ndFinger.ForeColor = System.Drawing.Color.White;
            this.btn2ndFinger.Location = new System.Drawing.Point(466, 46);
            this.btn2ndFinger.Name = "btn2ndFinger";
            this.btn2ndFinger.Size = new System.Drawing.Size(107, 40);
            this.btn2ndFinger.TabIndex = 16;
            this.btn2ndFinger.Text = "2nd Finger Print ";
            this.btn2ndFinger.UseVisualStyleBackColor = false;
            this.btn2ndFinger.Click += new System.EventHandler(this.btn2ndFinger_Click);
            // 
            // btn3rdFinger
            // 
            this.btn3rdFinger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn3rdFinger.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            this.btn3rdFinger.ForeColor = System.Drawing.Color.White;
            this.btn3rdFinger.Location = new System.Drawing.Point(588, 46);
            this.btn3rdFinger.Name = "btn3rdFinger";
            this.btn3rdFinger.Size = new System.Drawing.Size(107, 40);
            this.btn3rdFinger.TabIndex = 17;
            this.btn3rdFinger.Text = "3rd Finger Print";
            this.btn3rdFinger.UseVisualStyleBackColor = false;
            this.btn3rdFinger.Click += new System.EventHandler(this.button2_Click);
            // 
            // lb2ndfinger
            // 
            this.lb2ndfinger.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lb2ndfinger.AutoSize = true;
            this.lb2ndfinger.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb2ndfinger.ForeColor = System.Drawing.Color.Black;
            this.lb2ndfinger.Location = new System.Drawing.Point(88, 41);
            this.lb2ndfinger.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lb2ndfinger.Name = "lb2ndfinger";
            this.lb2ndfinger.Size = new System.Drawing.Size(173, 38);
            this.lb2ndfinger.TabIndex = 153;
            this.lb2ndfinger.Text = "2nd Finger";
            // 
            // lb3rdFinger
            // 
            this.lb3rdFinger.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lb3rdFinger.AutoSize = true;
            this.lb3rdFinger.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb3rdFinger.ForeColor = System.Drawing.Color.Black;
            this.lb3rdFinger.Location = new System.Drawing.Point(88, 41);
            this.lb3rdFinger.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lb3rdFinger.Name = "lb3rdFinger";
            this.lb3rdFinger.Size = new System.Drawing.Size(168, 38);
            this.lb3rdFinger.TabIndex = 154;
            this.lb3rdFinger.Text = "3rd Finger";
            // 
            // lbFirstFinger
            // 
            this.lbFirstFinger.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lbFirstFinger.AutoSize = true;
            this.lbFirstFinger.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFirstFinger.ForeColor = System.Drawing.Color.Black;
            this.lbFirstFinger.Location = new System.Drawing.Point(90, 41);
            this.lbFirstFinger.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbFirstFinger.Name = "lbFirstFinger";
            this.lbFirstFinger.Size = new System.Drawing.Size(166, 38);
            this.lbFirstFinger.TabIndex = 155;
            this.lbFirstFinger.Text = "1st Finger";
            // 
            // capture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 448);
            this.Controls.Add(this.lbFirstFinger);
            this.Controls.Add(this.lb3rdFinger);
            this.Controls.Add(this.lb2ndfinger);
            this.Controls.Add(this.btn3rdFinger);
            this.Controls.Add(this.btn2ndFinger);
            this.Controls.Add(this.btnTimeOut);
            this.Controls.Add(this.btnTimeIn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.empID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lname);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.start_scan);
            this.Controls.Add(this.fname);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.StatusText);
            this.Controls.Add(this.Prompt);
            this.Controls.Add(this.fImage);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "capture";
            this.Text = "dashboardAdmin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.testingg_FormClosing);
            this.Load += new System.EventHandler(this.capture_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox fImage;
        private System.Windows.Forms.TextBox Prompt;
        private System.Windows.Forms.TextBox StatusText;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.TextBox fname;
        private System.Windows.Forms.Button start_scan;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox lname;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox empID;
        private System.Windows.Forms.Label label6;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private System.Windows.Forms.Button btnTimeIn;
        private System.Windows.Forms.Button btnTimeOut;
        private System.Windows.Forms.Button btn2ndFinger;
        private System.Windows.Forms.Button btn3rdFinger;
        private System.Windows.Forms.Label lb2ndfinger;
        private System.Windows.Forms.Label lb3rdFinger;
        private System.Windows.Forms.Label lbFirstFinger;
    }
}