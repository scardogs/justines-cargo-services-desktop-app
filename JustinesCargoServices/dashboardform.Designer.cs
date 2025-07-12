namespace JustinesCargoServices
{
    partial class dashboardform
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlLeave = new Guna.UI2.WinForms.Guna2Panel();
            this.dataempLeave = new Guna.UI2.WinForms.Guna2DataGridView();
            this.SERIES = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EMPID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FULLNAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.POSITION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FROM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlFuel = new Guna.UI2.WinForms.Guna2Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlRenewals = new Guna.UI2.WinForms.Guna2Panel();
            this.turnRedLTO = new Guna.UI2.WinForms.Guna2Button();
            this.turnRedLTFRB = new Guna.UI2.WinForms.Guna2Button();
            this.turnRedINSURANCE = new Guna.UI2.WinForms.Guna2Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.renewreport = new Guna.UI2.WinForms.Guna2DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlSales = new Guna.UI2.WinForms.Guna2Panel();
            this.chartYearlyAmount = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnMonthly = new System.Windows.Forms.Button();
            this.btnYearly = new System.Windows.Forms.Button();
            this.EditBulkInfo = new System.Windows.Forms.Label();
            this.PLATE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RENEW = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DATE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asOfDateFuel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtRemainingFuel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel5 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel1.SuspendLayout();
            this.pnlLeave.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataempLeave)).BeginInit();
            this.pnlFuel.SuspendLayout();
            this.pnlRenewals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.renewreport)).BeginInit();
            this.pnlSales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartYearlyAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.Controls.Add(this.pnlLeave);
            this.guna2Panel1.Controls.Add(this.pnlFuel);
            this.guna2Panel1.Controls.Add(this.pnlRenewals);
            this.guna2Panel1.Controls.Add(this.pnlSales);
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Margin = new System.Windows.Forms.Padding(2);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(1292, 852);
            this.guna2Panel1.TabIndex = 0;
            // 
            // pnlLeave
            // 
            this.pnlLeave.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlLeave.Controls.Add(this.dataempLeave);
            this.pnlLeave.Controls.Add(this.label3);
            this.pnlLeave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(248)))), ((int)(((byte)(239)))));
            this.pnlLeave.Location = new System.Drawing.Point(20, 418);
            this.pnlLeave.Margin = new System.Windows.Forms.Padding(2);
            this.pnlLeave.Name = "pnlLeave";
            this.pnlLeave.Size = new System.Drawing.Size(668, 301);
            this.pnlLeave.TabIndex = 4;
            // 
            // dataempLeave
            // 
            this.dataempLeave.AllowUserToAddRows = false;
            this.dataempLeave.AllowUserToDeleteRows = false;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.White;
            this.dataempLeave.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle15;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Arial Narrow", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataempLeave.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle16;
            this.dataempLeave.ColumnHeadersHeight = 45;
            this.dataempLeave.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataempLeave.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SERIES,
            this.EMPID,
            this.FULLNAME,
            this.POSITION,
            this.LT,
            this.FROM,
            this.TO});
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataempLeave.DefaultCellStyle = dataGridViewCellStyle17;
            this.dataempLeave.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dataempLeave.Location = new System.Drawing.Point(16, 53);
            this.dataempLeave.Margin = new System.Windows.Forms.Padding(2);
            this.dataempLeave.Name = "dataempLeave";
            this.dataempLeave.ReadOnly = true;
            this.dataempLeave.RowHeadersVisible = false;
            this.dataempLeave.RowHeadersWidth = 51;
            this.dataempLeave.RowTemplate.Height = 24;
            this.dataempLeave.Size = new System.Drawing.Size(638, 236);
            this.dataempLeave.TabIndex = 86;
            this.dataempLeave.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dataempLeave.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dataempLeave.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dataempLeave.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dataempLeave.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dataempLeave.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dataempLeave.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dataempLeave.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dataempLeave.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataempLeave.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataempLeave.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dataempLeave.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataempLeave.ThemeStyle.HeaderStyle.Height = 45;
            this.dataempLeave.ThemeStyle.ReadOnly = true;
            this.dataempLeave.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dataempLeave.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataempLeave.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataempLeave.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dataempLeave.ThemeStyle.RowsStyle.Height = 24;
            this.dataempLeave.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dataempLeave.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // SERIES
            // 
            this.SERIES.HeaderText = "SERIES NUMBER";
            this.SERIES.MinimumWidth = 6;
            this.SERIES.Name = "SERIES";
            this.SERIES.ReadOnly = true;
            // 
            // EMPID
            // 
            this.EMPID.HeaderText = "EMPLOYEE ID";
            this.EMPID.MinimumWidth = 6;
            this.EMPID.Name = "EMPID";
            this.EMPID.ReadOnly = true;
            // 
            // FULLNAME
            // 
            this.FULLNAME.HeaderText = "NAME";
            this.FULLNAME.MinimumWidth = 6;
            this.FULLNAME.Name = "FULLNAME";
            this.FULLNAME.ReadOnly = true;
            // 
            // POSITION
            // 
            this.POSITION.HeaderText = "POSITION";
            this.POSITION.MinimumWidth = 6;
            this.POSITION.Name = "POSITION";
            this.POSITION.ReadOnly = true;
            // 
            // LT
            // 
            this.LT.HeaderText = "LEAVE TYPE";
            this.LT.MinimumWidth = 6;
            this.LT.Name = "LT";
            this.LT.ReadOnly = true;
            // 
            // FROM
            // 
            this.FROM.HeaderText = "FROM";
            this.FROM.MinimumWidth = 6;
            this.FROM.Name = "FROM";
            this.FROM.ReadOnly = true;
            // 
            // TO
            // 
            this.TO.HeaderText = "TO";
            this.TO.MinimumWidth = 6;
            this.TO.Name = "TO";
            this.TO.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(237, 40);
            this.label3.TabIndex = 87;
            this.label3.Text = "Leave Request";
            // 
            // pnlFuel
            // 
            this.pnlFuel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlFuel.Controls.Add(this.asOfDateFuel);
            this.pnlFuel.Controls.Add(this.txtRemainingFuel);
            this.pnlFuel.Controls.Add(this.guna2HtmlLabel5);
            this.pnlFuel.Controls.Add(this.label2);
            this.pnlFuel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(248)))), ((int)(((byte)(239)))));
            this.pnlFuel.Location = new System.Drawing.Point(701, 418);
            this.pnlFuel.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFuel.Name = "pnlFuel";
            this.pnlFuel.Size = new System.Drawing.Size(277, 204);
            this.pnlFuel.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 40);
            this.label2.TabIndex = 86;
            this.label2.Text = "Fuel";
            // 
            // pnlRenewals
            // 
            this.pnlRenewals.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlRenewals.Controls.Add(this.turnRedLTO);
            this.pnlRenewals.Controls.Add(this.turnRedLTFRB);
            this.pnlRenewals.Controls.Add(this.turnRedINSURANCE);
            this.pnlRenewals.Controls.Add(this.label8);
            this.pnlRenewals.Controls.Add(this.label7);
            this.pnlRenewals.Controls.Add(this.label6);
            this.pnlRenewals.Controls.Add(this.renewreport);
            this.pnlRenewals.Controls.Add(this.label1);
            this.pnlRenewals.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(248)))), ((int)(((byte)(239)))));
            this.pnlRenewals.Location = new System.Drawing.Point(701, 21);
            this.pnlRenewals.Margin = new System.Windows.Forms.Padding(2);
            this.pnlRenewals.Name = "pnlRenewals";
            this.pnlRenewals.Size = new System.Drawing.Size(480, 382);
            this.pnlRenewals.TabIndex = 2;
            // 
            // turnRedLTO
            // 
            this.turnRedLTO.BackColor = System.Drawing.Color.Silver;
            this.turnRedLTO.BorderRadius = 2;
            this.turnRedLTO.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.turnRedLTO.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.turnRedLTO.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.turnRedLTO.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.turnRedLTO.FillColor = System.Drawing.Color.Silver;
            this.turnRedLTO.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.turnRedLTO.ForeColor = System.Drawing.Color.White;
            this.turnRedLTO.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.turnRedLTO.ImageSize = new System.Drawing.Size(10, 10);
            this.turnRedLTO.Location = new System.Drawing.Point(206, 32);
            this.turnRedLTO.Margin = new System.Windows.Forms.Padding(2);
            this.turnRedLTO.Name = "turnRedLTO";
            this.turnRedLTO.Size = new System.Drawing.Size(15, 16);
            this.turnRedLTO.TabIndex = 91;
            // 
            // turnRedLTFRB
            // 
            this.turnRedLTFRB.BackColor = System.Drawing.Color.Silver;
            this.turnRedLTFRB.BorderRadius = 2;
            this.turnRedLTFRB.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.turnRedLTFRB.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.turnRedLTFRB.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.turnRedLTFRB.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.turnRedLTFRB.FillColor = System.Drawing.Color.Silver;
            this.turnRedLTFRB.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.turnRedLTFRB.ForeColor = System.Drawing.Color.White;
            this.turnRedLTFRB.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.turnRedLTFRB.ImageSize = new System.Drawing.Size(10, 10);
            this.turnRedLTFRB.Location = new System.Drawing.Point(272, 32);
            this.turnRedLTFRB.Margin = new System.Windows.Forms.Padding(2);
            this.turnRedLTFRB.Name = "turnRedLTFRB";
            this.turnRedLTFRB.Size = new System.Drawing.Size(15, 16);
            this.turnRedLTFRB.TabIndex = 90;
            // 
            // turnRedINSURANCE
            // 
            this.turnRedINSURANCE.BackColor = System.Drawing.Color.Silver;
            this.turnRedINSURANCE.BorderRadius = 2;
            this.turnRedINSURANCE.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.turnRedINSURANCE.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.turnRedINSURANCE.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.turnRedINSURANCE.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.turnRedINSURANCE.FillColor = System.Drawing.Color.Silver;
            this.turnRedINSURANCE.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.turnRedINSURANCE.ForeColor = System.Drawing.Color.White;
            this.turnRedINSURANCE.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.turnRedINSURANCE.ImageSize = new System.Drawing.Size(10, 10);
            this.turnRedINSURANCE.Location = new System.Drawing.Point(367, 32);
            this.turnRedINSURANCE.Margin = new System.Windows.Forms.Padding(2);
            this.turnRedINSURANCE.Name = "turnRedINSURANCE";
            this.turnRedINSURANCE.Size = new System.Drawing.Size(15, 16);
            this.turnRedINSURANCE.TabIndex = 89;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(289, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 88;
            this.label8.Text = "INSURANCE";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(226, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 87;
            this.label7.Text = "LTFRB";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(172, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 86;
            this.label6.Text = "LTO";
            // 
            // renewreport
            // 
            this.renewreport.AllowUserToAddRows = false;
            this.renewreport.AllowUserToDeleteRows = false;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.White;
            this.renewreport.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle18;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Arial Narrow", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.renewreport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle19;
            this.renewreport.ColumnHeadersHeight = 29;
            this.renewreport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PLATE,
            this.RENEW,
            this.DATE});
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.renewreport.DefaultCellStyle = dataGridViewCellStyle20;
            this.renewreport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.renewreport.Location = new System.Drawing.Point(11, 53);
            this.renewreport.Margin = new System.Windows.Forms.Padding(2);
            this.renewreport.Name = "renewreport";
            this.renewreport.ReadOnly = true;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.renewreport.RowHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.renewreport.RowHeadersVisible = false;
            this.renewreport.RowHeadersWidth = 51;
            this.renewreport.RowTemplate.Height = 24;
            this.renewreport.Size = new System.Drawing.Size(454, 313);
            this.renewreport.TabIndex = 85;
            this.renewreport.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.renewreport.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.renewreport.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.renewreport.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.renewreport.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.renewreport.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.renewreport.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.renewreport.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.renewreport.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.renewreport.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renewreport.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.renewreport.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.renewreport.ThemeStyle.HeaderStyle.Height = 29;
            this.renewreport.ThemeStyle.ReadOnly = true;
            this.renewreport.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.renewreport.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.renewreport.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renewreport.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.renewreport.ThemeStyle.RowsStyle.Height = 24;
            this.renewreport.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.renewreport.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.renewreport.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.renewreport_CellContentClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 40);
            this.label1.TabIndex = 84;
            this.label1.Text = "Renewals";
            // 
            // pnlSales
            // 
            this.pnlSales.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSales.Controls.Add(this.chartYearlyAmount);
            this.pnlSales.Controls.Add(this.btnMonthly);
            this.pnlSales.Controls.Add(this.btnYearly);
            this.pnlSales.Controls.Add(this.EditBulkInfo);
            this.pnlSales.Location = new System.Drawing.Point(20, 21);
            this.pnlSales.Margin = new System.Windows.Forms.Padding(2);
            this.pnlSales.Name = "pnlSales";
            this.pnlSales.Size = new System.Drawing.Size(668, 382);
            this.pnlSales.TabIndex = 1;
            // 
            // chartYearlyAmount
            // 
            chartArea3.Name = "ChartArea1";
            this.chartYearlyAmount.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartYearlyAmount.Legends.Add(legend3);
            this.chartYearlyAmount.Location = new System.Drawing.Point(17, 79);
            this.chartYearlyAmount.Name = "chartYearlyAmount";
            this.chartYearlyAmount.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Berry;
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartYearlyAmount.Series.Add(series3);
            this.chartYearlyAmount.Size = new System.Drawing.Size(637, 300);
            this.chartYearlyAmount.TabIndex = 88;
            this.chartYearlyAmount.Text = "chart1";
            // 
            // btnMonthly
            // 
            this.btnMonthly.Location = new System.Drawing.Point(579, 51);
            this.btnMonthly.Name = "btnMonthly";
            this.btnMonthly.Size = new System.Drawing.Size(75, 23);
            this.btnMonthly.TabIndex = 87;
            this.btnMonthly.Text = "Monthly";
            this.btnMonthly.UseVisualStyleBackColor = true;
            this.btnMonthly.Click += new System.EventHandler(this.btnMonthly_Click);
            // 
            // btnYearly
            // 
            this.btnYearly.Location = new System.Drawing.Point(579, 20);
            this.btnYearly.Name = "btnYearly";
            this.btnYearly.Size = new System.Drawing.Size(75, 23);
            this.btnYearly.TabIndex = 86;
            this.btnYearly.Text = "Yearly";
            this.btnYearly.UseVisualStyleBackColor = true;
            this.btnYearly.Click += new System.EventHandler(this.btnYearly_Click);
            // 
            // EditBulkInfo
            // 
            this.EditBulkInfo.AutoSize = true;
            this.EditBulkInfo.BackColor = System.Drawing.Color.Transparent;
            this.EditBulkInfo.Font = new System.Drawing.Font("Times New Roman", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditBulkInfo.Location = new System.Drawing.Point(10, 20);
            this.EditBulkInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.EditBulkInfo.Name = "EditBulkInfo";
            this.EditBulkInfo.Size = new System.Drawing.Size(208, 40);
            this.EditBulkInfo.TabIndex = 83;
            this.EditBulkInfo.Text = "Sales Report";
            // 
            // PLATE
            // 
            this.PLATE.HeaderText = "PLATE NUMBER";
            this.PLATE.MinimumWidth = 6;
            this.PLATE.Name = "PLATE";
            this.PLATE.ReadOnly = true;
            // 
            // RENEW
            // 
            this.RENEW.HeaderText = "RENEWAL MODULES";
            this.RENEW.MinimumWidth = 6;
            this.RENEW.Name = "RENEW";
            this.RENEW.ReadOnly = true;
            // 
            // DATE
            // 
            this.DATE.HeaderText = "DATE EXPIRY";
            this.DATE.Name = "DATE";
            this.DATE.ReadOnly = true;
            // 
            // asOfDateFuel
            // 
            this.asOfDateFuel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.asOfDateFuel.BackColor = System.Drawing.Color.Transparent;
            this.asOfDateFuel.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.asOfDateFuel.Location = new System.Drawing.Point(42, 67);
            this.asOfDateFuel.Margin = new System.Windows.Forms.Padding(2);
            this.asOfDateFuel.Name = "asOfDateFuel";
            this.asOfDateFuel.Size = new System.Drawing.Size(82, 24);
            this.asOfDateFuel.TabIndex = 89;
            this.asOfDateFuel.Text = "as of date";
            // 
            // txtRemainingFuel
            // 
            this.txtRemainingFuel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemainingFuel.BackColor = System.Drawing.Color.Transparent;
            this.txtRemainingFuel.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRemainingFuel.Location = new System.Drawing.Point(94, 139);
            this.txtRemainingFuel.Margin = new System.Windows.Forms.Padding(2);
            this.txtRemainingFuel.Name = "txtRemainingFuel";
            this.txtRemainingFuel.Size = new System.Drawing.Size(13, 22);
            this.txtRemainingFuel.TabIndex = 88;
            this.txtRemainingFuel.Text = "..";
            // 
            // guna2HtmlLabel5
            // 
            this.guna2HtmlLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel5.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel5.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel5.Location = new System.Drawing.Point(37, 95);
            this.guna2HtmlLabel5.Margin = new System.Windows.Forms.Padding(2);
            this.guna2HtmlLabel5.Name = "guna2HtmlLabel5";
            this.guna2HtmlLabel5.Size = new System.Drawing.Size(200, 26);
            this.guna2HtmlLabel5.TabIndex = 87;
            this.guna2HtmlLabel5.Text = "REMAINING FUEL :";
            // 
            // dashboardform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1292, 852);
            this.Controls.Add(this.guna2Panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "dashboardform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "dashboardform";
            this.Load += new System.EventHandler(this.dashboardform_Load);
            this.guna2Panel1.ResumeLayout(false);
            this.pnlLeave.ResumeLayout(false);
            this.pnlLeave.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataempLeave)).EndInit();
            this.pnlFuel.ResumeLayout(false);
            this.pnlFuel.PerformLayout();
            this.pnlRenewals.ResumeLayout(false);
            this.pnlRenewals.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.renewreport)).EndInit();
            this.pnlSales.ResumeLayout(false);
            this.pnlSales.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartYearlyAmount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel pnlSales;
    
        private Guna.UI2.WinForms.Guna2Panel pnlRenewals;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label EditBulkInfo;
        private Guna.UI2.WinForms.Guna2Panel pnlFuel;
        private Guna.UI2.WinForms.Guna2DataGridView renewreport;
        private Guna.UI2.WinForms.Guna2Panel pnlLeave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2DataGridView dataempLeave;
        private System.Windows.Forms.Button btnMonthly;
        private System.Windows.Forms.Button btnYearly;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartYearlyAmount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private Guna.UI2.WinForms.Guna2Button turnRedLTO;
        private Guna.UI2.WinForms.Guna2Button turnRedLTFRB;
        private Guna.UI2.WinForms.Guna2Button turnRedINSURANCE;
        private System.Windows.Forms.DataGridViewTextBoxColumn SERIES;
        private System.Windows.Forms.DataGridViewTextBoxColumn EMPID;
        private System.Windows.Forms.DataGridViewTextBoxColumn FULLNAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn POSITION;
        private System.Windows.Forms.DataGridViewTextBoxColumn LT;
        private System.Windows.Forms.DataGridViewTextBoxColumn FROM;
        private System.Windows.Forms.DataGridViewTextBoxColumn TO;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLATE;
        private System.Windows.Forms.DataGridViewTextBoxColumn RENEW;
        private System.Windows.Forms.DataGridViewTextBoxColumn DATE;
        private Guna.UI2.WinForms.Guna2HtmlLabel asOfDateFuel;
        private Guna.UI2.WinForms.Guna2HtmlLabel txtRemainingFuel;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel5;
    }
}