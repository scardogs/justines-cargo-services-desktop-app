using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace JustinesCargoServices
{
    public partial class waybill : Form
    {
        MySqlConnection con = new MySqlConnection(
        "datasource=localhost;" +
        "port=3306;" +
        "database=jcsdb;" +
        "username=root;" +
        "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public waybill()
        {
            InitializeComponent();
        }

        private int DisUnit = 1;
        private const int panelSpacing = 10;



        private void guna2Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                using (Pen pen = new Pen(Color.Gray, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            }
        }

        private void label_Click(object sender, EventArgs e)
        {
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void pnlCompany_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            }
        }



        private void waybill_Load(object sender, EventArgs e)
        {
            InitializeColumnVisibility();
            InitializeColumnVisibility2();
            PopulateLiquidationNo();
            waybillNumbers();
            loaddataWaybill();
            loaddataWaybills();
            LoadPlateNumbers();
            ClearAll();
            PopulateAllCompanies();
            PopulateCompanyNames();
            SetComboBoxEventHandlers(true);

            txtAmount1.TextChanged += new EventHandler(CalculateTotalRate);
            txtAmount2.TextChanged += new EventHandler(CalculateTotalRate);
            txtAmount3.TextChanged += new EventHandler(CalculateTotalRate);
            txtAmount4.TextChanged += new EventHandler(CalculateTotalRate);
            txtAmount5.TextChanged += new EventHandler(CalculateTotalRate);
            txtADDTL.TextChanged += new EventHandler(CalculateTotalRate);
            txtRate.TextChanged += new EventHandler(CalculateTotalRate);

            txtLoadorcbm.TextChanged += new EventHandler(CalculatePercentage);
            txtCbmorPallete.TextChanged += new EventHandler(CalculatePercentage);
            txtLoadorcbm.TextChanged += new EventHandler(CalculatePercentage2);
            txtCbmorPallete2.TextChanged += new EventHandler(CalculatePercentage2);
            txtLoadorcbm.TextChanged += new EventHandler(CalculatePercentage3);
            txtCbmorPallete3.TextChanged += new EventHandler(CalculatePercentage3);
            txtLoadorcbm.TextChanged += new EventHandler(CalculatePercentage4);
            txtCbmorPallete4.TextChanged += new EventHandler(CalculatePercentage4);
            txtLoadorcbm.TextChanged += new EventHandler(CalculatePercentage5);
            txtCbmorPallete5.TextChanged += new EventHandler(CalculatePercentage5);
            txtLoadorcbm.TextChanged += new EventHandler(SubtracttoLoad);
            txtCbmorPallete.TextChanged += new EventHandler(SubtracttoLoad);
            txtCbmorPallete2.TextChanged += new EventHandler(SubtracttoLoad);
            txtCbmorPallete3.TextChanged += new EventHandler(SubtracttoLoad);
            txtCbmorPallete4.TextChanged += new EventHandler(SubtracttoLoad);
            txtCbmorPallete5.TextChanged += new EventHandler(SubtracttoLoad);

            txtCbmorPallete.TextChanged += new EventHandler(CalculateTotalLoad);
            txtCbmorPallete2.TextChanged += new EventHandler(CalculateTotalLoad);
            txtCbmorPallete3.TextChanged += new EventHandler(CalculateTotalLoad);
            txtCbmorPallete4.TextChanged += new EventHandler(CalculateTotalLoad);
            txtCbmorPallete5.TextChanged += new EventHandler(CalculateTotalLoad);

            txtTotalPercent.TextChanged += new EventHandler(CalculateTotalAmount);
            txtTotalRate.TextChanged += new EventHandler(CalculateTotalAmount);
            txtPercentage.TextChanged += new EventHandler(CalculateTotalPercentage);
            txtPercentage2.TextChanged += new EventHandler(CalculateTotalPercentage);
            txtPercentage4.TextChanged += new EventHandler(CalculateTotalPercentage);
            txtPercentage5.TextChanged += new EventHandler(CalculateTotalPercentage);
        }
        private void PopulateCompanyNames()
        {
            try
            {
                // Open the database connection
                con.Open();

                // SQL query to fetch company names
                string query = "SELECT CompanyName FROM companyinfo";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Clear existing items in the ComboBox
                        cbmCompanyName.Items.Clear();

                        // Add each company name to the ComboBox
                        while (reader.Read())
                        {
                            cbmCompanyName.Items.Add(reader["CompanyName"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the database connection
                con.Close();
            }
        }

        private void CalculateTotalRate(object sender, EventArgs e)
        {
            decimal Amount1 = 0, Amount2 = 0, Amount3 = 0, Amount4 = 0, Amount5 = 0, additional = 0;


            bool isAmount1Valid = decimal.TryParse(txtAmount1.Text, out Amount1);
            bool isAmount2Valid = decimal.TryParse(txtAmount2.Text, out Amount2);
            bool isAmount3Valid = decimal.TryParse(txtAmount3.Text, out Amount3);
            bool isAmount4Valid = decimal.TryParse(txtAmount4.Text, out Amount4);
            bool isAmount5Valid = decimal.TryParse(txtAmount5.Text, out Amount5);
            bool isAdditionalValid = decimal.TryParse(txtADDTL.Text, out additional);


            if (isAmount1Valid && isAmount2Valid && isAmount3Valid && isAmount4Valid && isAmount5Valid && isAdditionalValid)
            {
                decimal TotalRate = Amount1 + Amount2 + Amount3 + Amount4 + Amount5 + additional;
                txtTotalRate.Text = TotalRate.ToString("0.00");

            }

            else if (isAmount1Valid && isAmount2Valid && isAmount3Valid && isAmount4Valid && isAmount5Valid)
            {
                decimal TotalRate = Amount1 + Amount2 + Amount3 + Amount4 + Amount5;
                txtTotalRate.Text = TotalRate.ToString("0.00");

            }

            else if (isAmount1Valid && isAmount2Valid && isAmount3Valid && isAmount4Valid)
            {
                decimal TotalRate = Amount1 + Amount2 + Amount3 + Amount4 + (isAdditionalValid ? additional : 0);
                txtTotalRate.Text = TotalRate.ToString("0.00");

            }
            else if (isAmount1Valid && isAmount2Valid && isAmount3Valid)
            {
                decimal TotalRate = Amount1 + Amount2 + Amount3 + (isAdditionalValid ? additional : 0);
                txtTotalRate.Text = TotalRate.ToString("0.00");

            }
            else if (isAmount1Valid && isAmount2Valid)
            {
                decimal TotalRate = Amount1 + Amount2 + (isAdditionalValid ? additional : 0);
                txtTotalRate.Text = TotalRate.ToString("0.00");

            }
            else if (isAmount1Valid)
            {
                decimal TotalRate = Amount1 + (isAdditionalValid ? additional : 0);
                txtTotalRate.Text = TotalRate.ToString("0.00");

            }
            else
            {
                txtTotalRate.Text = "0.00";
            }
        }

        private void CalculatePercentage(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtLoadorcbm.Text, out decimal totalcbm)
       && decimal.TryParse(txtCbmorPallete.Text, out decimal part)
       && totalcbm != 0)
            {
                decimal percentage = (part / totalcbm) * 100;
                txtPercentage.Text = percentage.ToString("0.00") + "%";
            }
            else
            {
                txtPercentage.Text = "0.00%";
            }
        }
        private void CalculateTotalLoad(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtCbmorPallete.Text, out decimal occupied1)
       && decimal.TryParse(txtCbmorPallete2.Text, out decimal occupied2) && decimal.TryParse(txtCbmorPallete3.Text, out decimal occupied3)
       && decimal.TryParse(txtCbmorPallete4.Text, out decimal occupied4) && decimal.TryParse(txtCbmorPallete5.Text, out decimal occupied5)
       )
            {
                decimal TotalOccupied = occupied1 + occupied2 + occupied3 + occupied4 + occupied5;
                txtTotalOccupied.Text = TotalOccupied.ToString("0.00");
            }
            else
            {
                txtTotalOccupied.Text = "0.00";
            }
        }

        private void SubtracttoLoad(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtLoadorcbm.Text, out decimal totalcbm))
            {
                decimal part1 = 0, part2 = 0, part3 = 0, part4 = 0, part5 = 0;


                decimal.TryParse(txtCbmorPallete.Text, out part1);
                decimal.TryParse(txtCbmorPallete2.Text, out part2);
                decimal.TryParse(txtCbmorPallete3.Text, out part3);
                decimal.TryParse(txtCbmorPallete4.Text, out part4);
                decimal.TryParse(txtCbmorPallete5.Text, out part5);


                decimal totalload = totalcbm - (part1 + part2 + part3 + part4 + part5);


                txtRemaining.Text = totalload.ToString("0.00");
            }
            else
            {

                txtRemaining.Text = "0.00";
            }
        }

        private void CalculatePercentage2(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtLoadorcbm.Text, out decimal totalcbm)
       && decimal.TryParse(txtCbmorPallete2.Text, out decimal part)
       && totalcbm != 0)
            {
                decimal percentage = (part / totalcbm) * 100;
                txtPercentage2.Text = percentage.ToString("0.00") + "%";
            }
            else
            {
                txtPercentage2.Text = "0.00%";
            }
        }
        private void CalculatePercentage3(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtLoadorcbm.Text, out decimal totalcbm)
       && decimal.TryParse(txtCbmorPallete3.Text, out decimal part)
       && totalcbm != 0)
            {
                decimal percentage = (part / totalcbm) * 100;
                txtPercentage3.Text = percentage.ToString("0.00") + "%";
            }
            else
            {
                txtPercentage3.Text = "0.00%";
            }
        }
        private void CalculatePercentage4(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtLoadorcbm.Text, out decimal totalcbm)
       && decimal.TryParse(txtCbmorPallete4.Text, out decimal part)
       && totalcbm != 0)
            {
                decimal percentage = (part / totalcbm) * 100;
                txtPercentage4.Text = percentage.ToString("0.00") + "%";
            }
            else
            {
                txtPercentage4.Text = "0.00%";
            }
        }
        private void CalculatePercentage5(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtLoadorcbm.Text, out decimal totalcbm)
       && decimal.TryParse(txtCbmorPallete5.Text, out decimal part)
       && totalcbm != 0)
            {
                decimal percentage = (part / totalcbm) * 100;
                txtPercentage5.Text = percentage.ToString("0.00") + "%";
            }
            else
            {
                txtPercentage5.Text = "0.00%";
            }
        }
        private void CalculateTotalPercentage(object sender, EventArgs e)
        {
            decimal totalPercentage = 0;

            // Parse the individual percentages (percent1, percent2, etc.)
            decimal percent1 = decimal.TryParse(txtPercentage.Text.Replace("%", ""), out percent1) ? percent1 : 0;
            decimal percent2 = decimal.TryParse(txtPercentage2.Text.Replace("%", ""), out percent2) ? percent2 : 0;
            decimal percent3 = decimal.TryParse(txtPercentage3.Text.Replace("%", ""), out percent3) ? percent3 : 0;
            decimal percent4 = decimal.TryParse(txtPercentage4.Text.Replace("%", ""), out percent4) ? percent4 : 0;
            decimal percent5 = decimal.TryParse(txtPercentage5.Text.Replace("%", ""), out percent5) ? percent5 : 0;

            // Sum all the percentages
            totalPercentage = percent1 + percent2 + percent3 + percent4 + percent5;

            // Calculate the remaining CBM
            decimal totalcbm = decimal.TryParse(txtLoadorcbm.Text, out totalcbm) ? totalcbm : 0;
            decimal filledCBM = (percent1 + percent2 + percent3 + percent4 + percent5) * totalcbm / 100;
            decimal remainingCBM = totalcbm - filledCBM;

            // If remaining CBM is 0, set total percentage to 100%
            if (remainingCBM == 0)
            {
                totalPercentage = 100; // Automatically set total percentage to 100 if all CBM is filled
            }

            // Display the total percentage (if it's under 100%, it will show the calculated total, otherwise 100%)
            txtTotalPercent.Text = totalPercentage.ToString("0.00") + "%";
        }
        private void CalculateTotalAmount(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtTotalRate.Text, out decimal totalRate)
        && decimal.TryParse(txtTotalPercent.Text.TrimEnd('%'), out decimal percentage))
            {

                decimal percentageAsDecimal = percentage / 100;


                decimal totalAmount = totalRate * percentageAsDecimal;

                txtTotalAmount.Text = totalAmount.ToString("0.00");
            }
            else
            {
                txtTotalAmount.Text = "0.00";
            }
        }
        private void waybillNumbers()
        {

            dataWaybillInfo.Rows.Clear();

            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            try
            {

                cmd = new MySqlCommand("SELECT ID, StubNo, WayBillNo, Status FROM waybillnum;", con);
                rdr = cmd.ExecuteReader();


                while (rdr.Read())
                {
                    dataWaybillInfo.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Waybills", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        // Make sure the relevant columns are identified in the DataGridView
        private void InitializeColumnVisibility()
        {
            // Set all relevant columns to be invisible initially
            dataWaybill.Columns["company2"].Visible = false;
            dataWaybill.Columns["percentage2"].Visible = false;
            dataWaybill.Columns["amount2"].Visible = false;
            dataWaybill.Columns["company3"].Visible = false;
            dataWaybill.Columns["percentage3"].Visible = false;
            dataWaybill.Columns["amount3"].Visible = false;
            dataWaybill.Columns["company4"].Visible = false;
            dataWaybill.Columns["percentage4"].Visible = false;
            dataWaybill.Columns["amount4"].Visible = false;
            dataWaybill.Columns["company5"].Visible = false;
            dataWaybill.Columns["percentage5"].Visible = false;
            dataWaybill.Columns["amount5"].Visible = false;
        }

        private void InitializeColumnVisibility2()
        {


            waybilldata.Columns["company44"].Visible = false;
            waybilldata.Columns["percentage44"].Visible = false;
            waybilldata.Columns["amount44"].Visible = false;
            waybilldata.Columns["company555"].Visible = false;
            waybilldata.Columns["percentage55"].Visible = false;
            waybilldata.Columns["amount55"].Visible = false;
        }

        private int currentPanelIndex = 0; // Start with the first panel (index 0 for pnlCompany2)

        // Panel-Column mapping
        private Dictionary<int, string[]> panelColumnMapping = new Dictionary<int, string[]>
{
    { 0, new string[] { "company2", "percentage2", "amount2" } },  // pnlCompany2
    { 1, new string[] { "company3", "percentage3", "amount3" } },  // pnlCompany3
    { 2, new string[] { "company4", "percentage4", "amount4" } },  // pnlCompany4
    { 3, new string[] { "company5", "percentage5", "amount5" } }   // pnlCompany5
};

        // Show panels and columns progressively
        private void btnAddCompany_Click(object sender, EventArgs e)
        {
            // List of panels to manage visibility
            Panel[] panels = { pnlCompany2, pnlCompany3, pnlCompany4, pnlCompany5 };

            if (currentPanelIndex < panels.Length) // Check if more panels can be shown
            {
                panels[currentPanelIndex].Visible = true; // Show the next panel

                // Show the corresponding columns for this panel
                string[] columnsToShow = panelColumnMapping[currentPanelIndex];
                foreach (var columnName in columnsToShow)
                {
                    dataWaybill.Columns[columnName].Visible = true;
                }

                currentPanelIndex++; // Move to the next panel and columns
            }
            else
            {
                MessageBox.Show("All panels are already visible!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private void addDel_Click(object sender, EventArgs e)
        {
            pnlTransaction.Visible = !pnlTransaction.Visible;

        }
        private void btnSubtractCompany_Click(object sender, EventArgs e)
        {
            // List of panels to manage visibility
            Panel[] panels = { pnlCompany2, pnlCompany3, pnlCompany4, pnlCompany5 };

            if (currentPanelIndex > 0) // Ensure we're not below index 0
            {
                currentPanelIndex--; // Move to the previous panel

                panels[currentPanelIndex].Visible = false; // Hide the current panel

                // Hide the corresponding columns for this panel
                string[] columnsToHide = panelColumnMapping[currentPanelIndex];
                foreach (var columnName in columnsToHide)
                {
                    dataWaybill.Columns[columnName].Visible = false;
                }
            }
            else
            {
                MessageBox.Show("All panels are already hidden!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optionally reset to the first panel when all are hidden and the button is clicked again
                currentPanelIndex = 0; // Reset the index so that we can start adding panels again
            }
        }

        // Reset panel visibility and columns visibility
        private void ResetPanelVisibility()
        {
            // If you want to reset all panels to be hidden, ensure that the index is also reset
            Panel[] panels = { pnlCompany2, pnlCompany3, pnlCompany4, pnlCompany5 };

            // Hide all panels
            foreach (var panel in panels)
            {
                panel.Visible = false;
            }

            // Hide all columns
            foreach (var columnName in dataWaybill.Columns)
            {
                dataWaybill.Columns[columnName.ToString()].Visible = false;
            }

            currentPanelIndex = 0; // Reset index to start from the first panel when adding panels
        }

        private void btnShowAddWB_Click(object sender, EventArgs e)
        {
            pnlShowWB.Visible = !pnlShowWB.Visible;
        }

        private void btnWaybillList_Click(object sender, EventArgs e)
        {
            pnlWaybillinfo.Visible = !pnlWaybillinfo.Visible;
        }
        private bool ValidateAddWaybillInput()
        {

            if (string.IsNullOrWhiteSpace(txtSTubNo.Text))
            {
                MessageBox.Show("Stub Number is required.", "Error");
                return false;

            }
            if (string.IsNullOrWhiteSpace(txtFrom.Text))
            {
                MessageBox.Show("First parameter for Add Waybill is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTo.Text))
            {
                MessageBox.Show("Second parameter for Add Waybill is required.", "Error");
                return false;
            }


            return true;
        }
        private bool WaybillExists(int stubNo, int wayBillNo)
        {
            bool exists = false;

            try
            {

                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }


                cmd = new MySqlCommand("SELECT COUNT(*) FROM waybillnum WHERE StubNo = @StubNo AND WayBillNo = @WayBillNo;", con);
                cmd.Parameters.AddWithValue("@StubNo", stubNo);
                cmd.Parameters.AddWithValue("@WayBillNo", wayBillNo);


                int count = Convert.ToInt32(cmd.ExecuteScalar());
                exists = count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return exists;
        }
        private void btnAddWaybill_Click(object sender, EventArgs e)
        {
            if (!ValidateAddWaybillInput()) return;
            try
            {

                int stubNo = int.Parse(txtSTubNo.Text);
                int from = int.Parse(txtFrom.Text);
                int to = int.Parse(txtTo.Text);


                bool hasDuplicates = false;


                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }


                for (int wayBillNo = from; wayBillNo <= to; wayBillNo++)
                {

                    if (WaybillExists(stubNo, wayBillNo))
                    {
                        hasDuplicates = true;
                        continue;
                    }


                    cmd = new MySqlCommand("INSERT INTO waybillnum (StubNo, WayBillNo) VALUES (@StubNo, @WayBillNo);", con);
                    cmd.Parameters.AddWithValue("@StubNo", stubNo);
                    cmd.Parameters.AddWithValue("@WayBillNo", wayBillNo);
                    cmd.ExecuteNonQuery();
                }


                if (hasDuplicates)
                {
                    MessageBox.Show($"The waybill number(s) under stub number {stubNo} already exist.", "Duplicate Waybill", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Waybills added successfully.");
                }

                waybillNumbers();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        private void SearchWaybillNumbers(string searchText)
        {
            // Clear the previous data in the DataGridView
            dataWaybillInfo.Rows.Clear();

            // Open connection if not already open
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            try
            {
                // Build the query with a WHERE clause if searchText is provided
                string query = "SELECT ID, StubNo, WayBillNo, Status FROM waybillnum";

                // Add a WHERE clause if searchText is not empty
                if (!string.IsNullOrEmpty(searchText))
                {
                    query += " WHERE CONCAT_WS(' ', ID, StubNo, WayBillNo, Status) LIKE @searchText";
                }

                // Create the MySqlCommand object
                cmd = new MySqlCommand(query, con);

                // Add the parameter to the query if there is a searchText
                if (!string.IsNullOrEmpty(searchText))
                {
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                }

                // Execute the query
                rdr = cmd.ExecuteReader();

                // Loop through the results and add rows to the DataGridView
                while (rdr.Read())
                {
                    dataWaybillInfo.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3));
                }
            }
            catch (Exception ex)
            {
                // Handle any errors during data retrieval
                MessageBox.Show(ex.Message, "Error Loading Waybills", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure that the connection is closed
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private void btnSearchWBN_Click(object sender, EventArgs e)
        {
            string searchText = txtSearchWBN.Text.Trim(); // Assuming you have a TextBox named txtSearch
            SearchWaybillNumbers(searchText);
        }

        private void btnRefreshWBN_Click(object sender, EventArgs e)
        {
            waybillNumbers();
        }

        private void cbmCompany1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbmCompany1.SelectedIndex != -1)
            {
                txtCbmorPallete.Enabled = true;
                txtAmount1.Enabled = true;

            }
            else
            {
                txtCbmorPallete.Enabled = false;
                txtAmount1.Enabled = false;
            }

        }

        private void cbmCompany2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbmCompany2.SelectedIndex != -1)
            {
                txtCbmorPallete2.Enabled = true;
                txtAmount2.Enabled = true;

            }
            else
            {
                txtCbmorPallete2.Enabled = false;
                txtAmount2.Enabled = false;
            }
        }

        private void cbmCompany3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbmCompany3.SelectedIndex != -1)
            {
                txtCbmorPallete3.Enabled = true;
                txtAmount3.Enabled = true;

            }
            else
            {
                txtCbmorPallete3.Enabled = false;
                txtAmount3.Enabled = false;

            }
        }

        private void cbmCompany4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbmCompany4.SelectedIndex != -1)
            {
                txtCbmorPallete4.Enabled = true;
                txtAmount4.Enabled = true;

            }
            else
            {
                txtCbmorPallete4.Enabled = false;
                txtAmount4.Enabled = false;
            }
        }

        private void cbmCompany5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbmCompany5.SelectedIndex != -1)
            {
                txtCbmorPallete5.Enabled = true;
                txtAmount5.Enabled = true;

            }
            else
            {
                txtCbmorPallete5.Enabled = false;
                txtAmount5.Enabled = false;
            }
        }
        private List<string> allCompanies = new List<string>();

        private void PopulateAllCompanies()
        {
            try
            {
                con.Open();
                cmd = new MySqlCommand("SELECT CompanyName FROM companyinfo;", con);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    allCompanies.Add(rdr["CompanyName"].ToString());
                }
                rdr.Close();

                // Assign all companies to each ComboBox
                AssignCompaniesToComboBox(cbmCompany1);
                AssignCompaniesToComboBox(cbmCompany2);
                AssignCompaniesToComboBox(cbmCompany3);
                AssignCompaniesToComboBox(cbmCompany4);
                AssignCompaniesToComboBox(cbmCompany5);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Company Names", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void AssignCompaniesToComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(allCompanies.ToArray());
            comboBox.Tag = new List<string>(allCompanies); // Store available options in Tag
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox currentComboBox)
            {
                // Temporarily disable events to prevent recursion
                SetComboBoxEventHandlers(false);

                string selectedValue = currentComboBox.Text;

                // Update other ComboBoxes
                foreach (ComboBox comboBox in new[] { cbmCompany1, cbmCompany2, cbmCompany3, cbmCompany4, cbmCompany5 })
                {
                    if (comboBox != currentComboBox)
                    {
                        UpdateComboBoxOptions(comboBox, selectedValue);
                    }
                }

                // Re-enable events
                SetComboBoxEventHandlers(true);
            }
        }
        private void UpdateComboBoxOptions(ComboBox comboBox, string excludedValue)
        {
            if (comboBox.Tag is List<string> availableOptions)
            {
                string currentSelection = comboBox.Text;

                // Filter options, excluding the selected value
                var updatedOptions = allCompanies
                    .Where(company => company != excludedValue && !IsSelectedInOtherComboBoxes(company, comboBox))
                    .ToList();

                comboBox.Tag = updatedOptions;
                comboBox.Items.Clear();
                comboBox.Items.AddRange(updatedOptions.ToArray());

                // Retain valid selection if possible
                if (!string.IsNullOrEmpty(currentSelection) && updatedOptions.Contains(currentSelection))
                {
                    comboBox.Text = currentSelection;
                }
                else
                {
                    comboBox.Text = string.Empty; // Clear invalid selection
                }
            }
        }

        private bool IsSelectedInOtherComboBoxes(string company, ComboBox currentComboBox)
        {
            return new[] { cbmCompany1, cbmCompany2, cbmCompany3, cbmCompany4, cbmCompany5 }
                .Where(comboBox => comboBox != currentComboBox)
                .Any(comboBox => comboBox.Text == company);
        }


        // Inside the waybill class
        private void SetComboBoxEventHandlers(bool enable)
        {
            if (enable)
            {
                cbmCompany1.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                cbmCompany2.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                cbmCompany3.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                cbmCompany4.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                cbmCompany5.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            }
            else
            {
                cbmCompany1.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                cbmCompany2.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                cbmCompany3.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                cbmCompany4.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                cbmCompany5.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
            }
        }

        private void pnlTransaction_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbmCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCompanyName = cbmCompanyName.SelectedItem.ToString();
            LoadCompanyInfo(selectedCompanyName);
        }
        private void LoadCompanyInfo(string CompanyName)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT ID, CompanyName, Address, TinNo FROM companyinfo WHERE CompanyName = @CompanyName;", con))
                {
                    cmd.Parameters.AddWithValue("@CompanyName", CompanyName);

                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }

                    using (rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            txtCompID.Text = rdr["ID"].ToString();
                            txtTin.Text = rdr["TinNo"].ToString();
                            txtAddress.Text = rdr["Address"].ToString();
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }
        private bool ValidateCompanyInfoInput()
        {

            if (string.IsNullOrWhiteSpace(cbmCompanyName.Text))
            {
                MessageBox.Show("The Company Name for the Add Company is required.", "Error");
                return false;

            }
            if (string.IsNullOrWhiteSpace(txtTin.Text))
            {
                MessageBox.Show("Tin is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Address is required.", "Error");
                return false;
            }


            return true;
        }
        private void btnAddCI_Click(object sender, EventArgs e)
        {
            if (!ValidateCompanyInfoInput()) return;
            con.Open();
            cmd = new MySqlCommand("INSERT INTO companyinfo (CompanyName, Address, TinNo) VALUES (@CompanyName, @Address, @TinNo);", con);



            cmd.Parameters.AddWithValue("@companyName", cbmCompanyName.Text);
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
            cmd.Parameters.AddWithValue("@TinNo", txtTin.Text);



            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Company Info Added Successfully");

            waybilldata.Rows.Clear();
            loaddataWaybill();
            PopulateCompanyNames();
            LoadPlateNumbers();
            ClearAll();
        }
        private void ClearAll()
        {


            txtReferenceNo.Clear();
            txtLoadorcbm.Clear();
            txtTotalOccupied.Clear();
            txtCbmorPallete.Clear();
            txtCbmorPallete2.Clear();
            txtCbmorPallete3.Clear();
            txtRate.Clear();
            txtADDTL.Clear();
            txtPercentage.Clear();
            txtAmount1.Clear();
            txtPercentage2.Clear();
            txtAmount2.Clear();
            txtPercentage3.Clear();
            txtAmount3.Clear();
            txtTotalPercent.Clear();
            txtTotalAmount.Clear();


            cbmCompany1.SelectedIndex = -1;
            cbmCompany1.Text = "";
            cbmCompany2.SelectedIndex = -1;
            cbmCompany2.Text = "";
            cbmCompany3.SelectedIndex = -1;
            cbmCompany3.Text = "";

        }
        private void LoadPlateNumbers()
        {
            try
            {
                cbPlateNum.Items.Clear(); // Clear existing items
                con.Open();
                cmd = new MySqlCommand("SELECT plateNum FROM truck;", con);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cbPlateNum.Items.Add(rdr.GetString(0)); // Add plateNum to ComboBox
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading plate numbers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (con.State == ConnectionState.Open) con.Close();
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateCompanyInfoInput()) return;
            con.Open();


            cmd = new MySqlCommand("UPDATE companyinfo SET CompanyName = @CompanyName, Address = @Address, TinNo = @TinNo WHERE ID = @CompanyID;", con);


            cmd.Parameters.AddWithValue("@CompanyName", cbmCompanyName.Text);
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
            cmd.Parameters.AddWithValue("@TinNo", txtTin.Text);


            cmd.Parameters.AddWithValue("@CompanyID", txtCompID.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Company Info Updated Successfully");

            waybilldata.Rows.Clear();
            loaddataWaybill();
            PopulateCompanyNames();
            LoadPlateNumbers();
            ClearAll();
        }
        private void loaddataWaybill()
        {
            waybilldata.Rows.Clear();

            string query = "SELECT ID, waybillNo, ReferenceNo, DispatchDate, TotalLoadorCbm, CbmorPalleteOccupied, Rate, ADDTL, Company1, percentage, Amount1, Company2, percentage2, Amount2, Company3, percentage3, Amount3, Company4, percentage4, Amount4, Company5, percentage5, Amount5, TotalPercentage,   TotalAmount, Status FROM waybill";

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                waybilldata.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.IsDBNull(3) ? (object)DBNull.Value : rdr.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6),
                                    rdr.GetString(7),
                                    rdr.GetString(8),
                                    rdr.GetString(9),
                                    rdr.GetString(10),
                                    rdr.GetString(11),
                                    rdr.GetString(12),
                                    rdr.GetString(13),
                                    rdr.GetString(14),
                                    rdr.GetString(15),
                                    rdr.GetString(16),
                                    rdr.GetString(17),
                                    rdr.GetString(18),
                                    rdr.GetString(19),
                                    rdr.GetString(20),
                                    rdr.GetString(21),
                                    rdr.GetString(22),
                                    rdr.GetString(23),
                                    rdr.GetString(24),
                                    rdr.GetString(25)







                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void loaddataWaybills()
        {
            dataWaybill.Rows.Clear();

            string query = "SELECT ID, waybillNo, ReferenceNo, DispatchDate, TotalLoadorCbm, CbmorPalleteOccupied, Rate, ADDTL, Company1, percentage, Amount1, Company2, percentage2, Amount2, Company3, percentage3, Amount3, Company4, percentage4, Amount4, Company5, percentage5, Amount5, TotalPercentage,   TotalAmount, Status FROM waybill";

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                dataWaybill.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.IsDBNull(3) ? (object)DBNull.Value : rdr.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6),
                                    rdr.GetString(7),
                                    rdr.GetString(8),
                                    rdr.GetString(9),
                                    rdr.GetString(10),
                                    rdr.GetString(11),
                                    rdr.GetString(12),
                                    rdr.GetString(13),
                                    rdr.GetString(14),
                                    rdr.GetString(15),
                                    rdr.GetString(16),
                                    rdr.GetString(17),
                                    rdr.GetString(18),
                                    rdr.GetString(19),
                                    rdr.GetString(20),
                                    rdr.GetString(21),
                                    rdr.GetString(22),
                                    rdr.GetString(23),
                                    rdr.GetString(24),
                                    rdr.GetString(25)







                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnSearchWB_Click(object sender, EventArgs e)
        {
            string searchKeyword = txtSearchWB.Text.Trim();
            searchWaybill(searchKeyword);
        }
        private void searchWaybill(string searchKeyword = "")
        {
            waybilldata.Rows.Clear();

            // SQL query with CONCAT to search across multiple columns
            string query = @"SELECT ID, waybillNo, ReferenceNo, DispatchDate, TotalLoadorCbm, 
                     CbmorPalleteOccupied, Rate, ADDTL, Company1, percentage, Amount1, 
                     Company2, percentage2, Amount2, Company3, percentage3, Amount3, 
                     Company4, percentage4, Amount4, Company5, percentage5, Amount5, 
                     TotalPercentage, TotalAmount, Status 
                     FROM waybill";

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query += @" WHERE CONCAT(waybillNo, ' ', ReferenceNo, ' ', DispatchDate, ' ', 
                   Company1, ' ', Company2, ' ', Company3, ' ', Company4, ' ', Company5) 
                   LIKE @searchKeyword";
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        if (!string.IsNullOrEmpty(searchKeyword))
                        {
                            cmd.Parameters.AddWithValue("@searchKeyword", "%" + searchKeyword + "%");
                        }

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                waybilldata.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.IsDBNull(3) ? (object)DBNull.Value : rdr.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6),
                                    rdr.GetString(7),
                                    rdr.GetString(8),
                                    rdr.GetString(9),
                                    rdr.GetString(10),
                                    rdr.GetString(11),
                                    rdr.GetString(12),
                                    rdr.GetString(13),
                                    rdr.GetString(14),
                                    rdr.GetString(15),
                                    rdr.GetString(16),
                                    rdr.GetString(17),
                                    rdr.GetString(18),
                                    rdr.GetString(19),
                                    rdr.GetString(20),
                                    rdr.GetString(21),
                                    rdr.GetString(22),
                                    rdr.GetString(23),
                                    rdr.GetString(24),
                                    rdr.GetString(25)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void PopulateLiquidationNo()
        {
            try
            {
                con.Open();
                string query = "SELECT DISTINCT LiquidationNo FROM delivery_multi;";
                cmd = new MySqlCommand(query, con);
                rdr = cmd.ExecuteReader();

                cbmLiquiNo.Items.Clear();

                while (rdr.Read())
                {
                    cbmLiquiNo.Items.Add(rdr["LiquidationNo"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading LiquidationNo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (con.State == ConnectionState.Open) con.Close();
            }
        }
        private void populateWaybillNo(string liquidationNo = null)
        {
            try
            {
                con.Open();

                string query;

                // If a LiquidationNo is provided, fetch WaybillNos for that specific LiquidationNo
                if (!string.IsNullOrEmpty(liquidationNo))
                {
                    query = @"
            SELECT WaybillNo 
            FROM delivery_multi 
            WHERE LiquidationNo = @LiquidationNo
            AND WaybillNo NOT IN (
                SELECT WaybillNo 
                FROM waybill
            );";
                }
                else
                {
                    // Otherwise, fetch unused WaybillNos from waybillnum
                    query = @"
            SELECT WayBillNo 
            FROM waybillnum 
            WHERE WayBillNo NOT IN (
                SELECT WaybillNo 
                FROM waybill
            );";
                }

                cmd = new MySqlCommand(query, con);

                if (!string.IsNullOrEmpty(liquidationNo))
                {
                    cmd.Parameters.AddWithValue("@LiquidationNo", liquidationNo);
                }

                rdr = cmd.ExecuteReader();

                cbmWaybillNo.Items.Clear();

                while (rdr.Read())
                {
                    string waybillNo = rdr["WaybillNo"].ToString();

                    if (!cbmWaybillNo.Items.Contains(waybillNo))
                    {
                        cbmWaybillNo.Items.Add(waybillNo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Waybill Numbers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void cbmLiquiNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbmLiquiNo.SelectedItem != null)
            {
                string selectedLiquidationNo = cbmLiquiNo.SelectedItem.ToString();
                populateWaybillNo(selectedLiquidationNo);
            }
        }

        private void cbPlateNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPlateNum.SelectedItem == null) return;

            try
            {
                con.Open();
                cmd = new MySqlCommand("SELECT CBM FROM truck WHERE plateNum = @plateNum;", con);
                cmd.Parameters.AddWithValue("@plateNum", cbPlateNum.SelectedItem.ToString());
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    txtLoadorcbm.Text = rdr.GetDecimal(0).ToString(); // Display CBM in TextBox
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CBM: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void btnAddCInfo_Click(object sender, EventArgs e)
        {
            pnlAddCI.Visible = !pnlAddCI.Visible;

        }
        private bool ValidateWaybillInput()
        {

            if (string.IsNullOrWhiteSpace(txtReferenceNo.Text))
            {
                MessageBox.Show("Reference number is required.", "Error");
                return false;
            }




            return true;
        }
        private void btnAddWB_Click(object sender, EventArgs e)
        {
            if (!ValidateWaybillInput()) return;

            try
            {
                con.Open();

                // Check for duplicate entries
                string checkQuery = "SELECT COUNT(*) FROM waybill WHERE waybillNo = @waybillNo OR ReferenceNo = @ReferenceNo;";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@waybillNo", cbmWaybillNo.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@ReferenceNo", txtReferenceNo.Text.Trim());

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("This waybill number or reference number already exists. Please enter unique values.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Insert new waybill record
                string insertQuery = @"INSERT INTO waybill(waybillNo, ReferenceNo, DispatchDate, TotalLoadorCbm, 
                               CbmorPalleteOccupied, Rate, ADDTL, Company1, percentage, Amount1, Company2, percentage2, 
                               Amount2, Company3, percentage3, Company4, percentage4, Amount4, Company5, percentage5, 
                               Amount5, TotalPercentage, Amount3, TotalRate, TotalAmount) 
                               VALUES (@waybillNo, @ReferenceNo, @DispatchDate, @TotalLoadorCbm, 
                               @CbmorPalleteOccupied, @Rate, @ADDTL, @Company1, @percentage, @Amount1, 
                               @Company2, @percentage2, @Amount2, @Company3, @percentage3, @Company4, 
                               @percentage4, @Amount4, @Company5, @percentage5, @Amount5, @TotalPercentage, 
                               @Amount3, @TotalRate, @TotalAmount);";

                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, con))
                {
                    insertCmd.Parameters.AddWithValue("@waybillNo", cbmWaybillNo.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@ReferenceNo", txtReferenceNo.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@DispatchDate", PickerDispatchDate.Value.ToString("yyyy-MM-dd"));
                    insertCmd.Parameters.AddWithValue("@TotalLoadorCbm", txtLoadorcbm.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@CbmorPalleteOccupied", txtTotalOccupied.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Rate", txtRate.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@ADDTL", txtADDTL.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Company1", cbmCompany1.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@percentage", txtPercentage.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Amount1", txtAmount1.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Company2", cbmCompany2.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@percentage2", txtPercentage2.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Amount2", txtAmount2.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Company3", cbmCompany3.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@percentage3", txtPercentage3.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Amount3", txtAmount3.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Company4", cbmCompany4.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@percentage4", txtPercentage4.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Amount4", txtAmount4.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Company5", cbmCompany5.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@percentage5", txtPercentage5.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@Amount5", txtAmount5.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@TotalPercentage", txtTotalPercent.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@TotalRate", txtTotalRate.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@TotalAmount", txtTotalAmount.Text.Trim());

                    insertCmd.ExecuteNonQuery();
                }

                // Update waybill number status
                string updateQuery = "UPDATE waybillnum SET Status = 'USED' WHERE WayBillNo = @WayBillNo;";
                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, con))
                {
                    updateCmd.Parameters.AddWithValue("@WayBillNo", cbmWaybillNo.Text.Trim());
                    updateCmd.ExecuteNonQuery();
                }

                // Reload data
                loaddataWaybill();
                waybillNumbers();
                LoadPlateNumbers();
                loaddataWaybills();

                MessageBox.Show("Waybill added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            pnlWaybillinfo.Visible = !pnlWaybillinfo.Visible;
        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            pnlTransaction.Visible = !pnlTransaction.Visible;
        }

        private void waybilldata_SelectionChanged(object sender, EventArgs e)
        {
            if (waybilldata.SelectedRows.Count > 0 && cbmWaybillNo.Enabled)
            {
                txtID.Text = waybilldata.SelectedRows[0].Cells[0].Value.ToString();
                cbmWaybillNo.Text = waybilldata.SelectedRows[0].Cells[1].Value.ToString();
                
                txtReferenceNo.Text = waybilldata.SelectedRows[0].Cells[2].Value.ToString();
                PickerDispatchDate.Value = waybilldata.SelectedRows[0].Cells[3].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(waybilldata.SelectedRows[0].Cells[3].Value);
                txtLoadorcbm.Text = waybilldata.SelectedRows[0].Cells[4].Value.ToString();
                txtTotalOccupied.Text = waybilldata.SelectedRows[0].Cells[5].Value.ToString();
                txtRate.Text = waybilldata.SelectedRows[0].Cells[6].Value.ToString();
                txtADDTL.Text = waybilldata.SelectedRows[0].Cells[7].Value.ToString();
                cbmCompany1.Text = waybilldata.SelectedRows[0].Cells[8].Value.ToString();
                txtPercentage.Text = waybilldata.SelectedRows[0].Cells[9].Value.ToString();
                txtAmount1.Text = waybilldata.SelectedRows[0].Cells[10].Value.ToString();
                cbmCompany2.Text = waybilldata.SelectedRows[0].Cells[11].Value.ToString();
                txtPercentage2.Text = waybilldata.SelectedRows[0].Cells[12].Value.ToString();
                txtAmount2.Text = waybilldata.SelectedRows[0].Cells[13].Value.ToString();
                cbmCompany3.Text = waybilldata.SelectedRows[0].Cells[14].Value.ToString();
                txtPercentage3.Text = waybilldata.SelectedRows[0].Cells[15].Value.ToString();
                txtAmount3.Text = waybilldata.SelectedRows[0].Cells[16].Value.ToString();
                cbmCompany4.Text = waybilldata.SelectedRows[0].Cells[17].Value.ToString();
                txtPercentage4.Text = waybilldata.SelectedRows[0].Cells[18].Value.ToString();
                txtAmount4.Text = waybilldata.SelectedRows[0].Cells[19].Value.ToString();
                cbmCompany5.Text = waybilldata.SelectedRows[0].Cells[20].Value.ToString();
                txtPercentage5.Text = waybilldata.SelectedRows[0].Cells[21].Value.ToString();
                txtAmount5.Text = waybilldata.SelectedRows[0].Cells[22].Value.ToString();
                txtTotalPercent.Text = waybilldata.SelectedRows[0].Cells[23].Value.ToString();
                txtTotalAmount.Text = waybilldata.SelectedRows[0].Cells[24].Value.ToString();
            }
        }

        private void dataWaybill_SelectionChanged(object sender, EventArgs e)
        {
            if (dataWaybill.SelectedRows.Count > 0 && cbmWaybillNo.Enabled)
            {
                txtID.Text = dataWaybill.SelectedRows[0].Cells[0].Value.ToString();
                cbmWaybillNo.Text = dataWaybill.SelectedRows[0].Cells[1].Value.ToString();
               
                txtReferenceNo.Text = dataWaybill.SelectedRows[0].Cells[2].Value.ToString();
                PickerDispatchDate.Value = dataWaybill.SelectedRows[0].Cells[3].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(waybilldata.SelectedRows[0].Cells[3].Value);
                txtLoadorcbm.Text = dataWaybill.SelectedRows[0].Cells[4].Value.ToString();
                txtTotalOccupied.Text = dataWaybill.SelectedRows[0].Cells[5].Value.ToString();
                txtRate.Text = dataWaybill.SelectedRows[0].Cells[6].Value.ToString();
                txtADDTL.Text = dataWaybill.SelectedRows[0].Cells[7].Value.ToString();
                cbmCompany1.Text = dataWaybill.SelectedRows[0].Cells[8].Value.ToString();
                txtPercentage.Text = dataWaybill.SelectedRows[0].Cells[9].Value.ToString();
                txtAmount1.Text = dataWaybill.SelectedRows[0].Cells[10].Value.ToString();
                cbmCompany2.Text = dataWaybill.SelectedRows[0].Cells[11].Value.ToString();
                txtPercentage2.Text = dataWaybill.SelectedRows[0].Cells[12].Value.ToString();
                txtAmount2.Text = dataWaybill.SelectedRows[0].Cells[13].Value.ToString();
                cbmCompany3.Text = dataWaybill.SelectedRows[0].Cells[14].Value.ToString();
                txtPercentage3.Text = dataWaybill.SelectedRows[0].Cells[15].Value.ToString();
                txtAmount3.Text = dataWaybill.SelectedRows[0].Cells[16].Value.ToString();
                cbmCompany4.Text = dataWaybill.SelectedRows[0].Cells[17].Value.ToString();
                txtPercentage4.Text = dataWaybill.SelectedRows[0].Cells[18].Value.ToString();
                txtAmount4.Text = dataWaybill.SelectedRows[0].Cells[19].Value.ToString();
                cbmCompany5.Text = dataWaybill.SelectedRows[0].Cells[20].Value.ToString();
                txtPercentage5.Text = dataWaybill.SelectedRows[0].Cells[21].Value.ToString();
                txtAmount5.Text = dataWaybill.SelectedRows[0].Cells[22].Value.ToString();
                txtTotalPercent.Text = dataWaybill.SelectedRows[0].Cells[23].Value.ToString();
                txtTotalAmount.Text = dataWaybill.SelectedRows[0].Cells[24].Value.ToString();
            }
        }

        private void btnUpdatee_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                // Update the waybill record
                string updateQuery = @"UPDATE waybill SET 
                                ReferenceNo = @ReferenceNo, 
                                DispatchDate = @DispatchDate, 
                                TotalLoadorCbm = @TotalLoadorCbm, 
                                CbmorPalleteOccupied = @CbmorPalleteOccupied, 
                                Rate = @Rate, 
                                ADDTL = @ADDTL, 
                                Company1 = @Company1, 
                                percentage = @percentage, 
                                Amount1 = @Amount1, 
                                Company2 = @Company2, 
                                percentage2 = @percentage2, 
                                Amount2 = @Amount2, 
                                Company3 = @Company3, 
                                percentage3 = @percentage3, 
                                Company4 = @Company4, 
                                percentage4 = @percentage4, 
                                Amount4 = @Amount4, 
                                Company5 = @Company5, 
                                percentage5 = @percentage5, 
                                Amount5 = @Amount5, 
                                TotalPercentage = @TotalPercentage, 
                                Amount3 = @Amount3, 
                                TotalRate = @TotalRate, 
                                TotalAmount = @TotalAmount 
                              WHERE ID = @ID;";

                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, con))
                {
                    updateCmd.Parameters.AddWithValue("@ID", txtID.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@ReferenceNo", txtReferenceNo.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@DispatchDate", PickerDispatchDate.Value.ToString("yyyy-MM-dd"));
                    updateCmd.Parameters.AddWithValue("@TotalLoadorCbm", txtLoadorcbm.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@CbmorPalleteOccupied", txtTotalOccupied.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Rate", txtRate.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@ADDTL", txtADDTL.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Company1", cbmCompany1.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@percentage", txtPercentage.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Amount1", txtAmount1.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Company2", cbmCompany2.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@percentage2", txtPercentage2.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Amount2", txtAmount2.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Company3", cbmCompany3.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@percentage3", txtPercentage3.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Amount3", txtAmount3.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Company4", cbmCompany4.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@percentage4", txtPercentage4.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Amount4", txtAmount4.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Company5", cbmCompany5.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@percentage5", txtPercentage5.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@Amount5", txtAmount5.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@TotalPercentage", txtTotalPercent.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@TotalRate", txtTotalRate.Text.Trim());
                    updateCmd.Parameters.AddWithValue("@TotalAmount", txtTotalAmount.Text.Trim());

                    updateCmd.ExecuteNonQuery();
                }

                // Reload data
                loaddataWaybill();
                loaddataWaybills();
                waybillNumbers();
                LoadPlateNumbers();

                MessageBox.Show("Waybill updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void checkUnused_CheckedChanged(object sender, EventArgs e)
        {
            if (checkUnused.Checked)
            {
                UpdateStatus("UNUSED");
                checkCancelled.Checked = false;
                checkRemoveStat.Checked = false;
            }
            loaddataWaybill();
        }

        private void checkCancelled_CheckedChanged(object sender, EventArgs e)
        {
            if (checkCancelled.Checked)
            {
                UpdateStatus("CANCELLED");
                checkUnused.Checked = false;
                checkRemoveStat.Checked = false;
            }
            loaddataWaybill();
        }

        private void checkRemoveStat_CheckedChanged(object sender, EventArgs e)
        {
            if (checkRemoveStat.Checked)
            {
                UpdateStatus(""); // Set status to an empty string
                checkUnused.Checked = false;
                checkCancelled.Checked = false;
            }
            loaddataWaybill();
        }

        private void UpdateStatus(string status)
        {
            try
            {
                if (dataWaybill.SelectedRows.Count > 0) // Ensure a row is selected
                {
                    DataGridViewRow selectedRow = dataWaybill.SelectedRows[0]; // Get the selected row
                    int id = Convert.ToInt32(selectedRow.Cells["IDDD"].Value); // Get the ID

                    // Open the database connection
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }

                    // Update query
                    string query = "UPDATE waybill SET Status = @status WHERE ID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery(); // Execute the query
                    }

                    // Update the status in the DataGridView
                    selectedRow.Cells["Statusss"].Value = status;

                    MessageBox.Show($"Status updated to '{status}' ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select a row to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the connection is closed
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    loaddataWaybill();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            pnlTransaction.Visible = !pnlTransaction.Visible;
        }
        private void ShowAdditionalColumns()
        {
            waybilldata.Columns["company44"].Visible = true;
            waybilldata.Columns["percentage44"].Visible = true;
            waybilldata.Columns["amount44"].Visible = true;
            waybilldata.Columns["company555"].Visible = true;
            waybilldata.Columns["percentage55"].Visible = true;
            waybilldata.Columns["amount55"].Visible = true;
        }
        private void ShowLessColumns()
        {
            waybilldata.Columns["company44"].Visible = false;
            waybilldata.Columns["percentage44"].Visible = false;
            waybilldata.Columns["amount44"].Visible = false;
            waybilldata.Columns["company555"].Visible = false;
            waybilldata.Columns["percentage55"].Visible = false;
            waybilldata.Columns["amount55"].Visible = false;
        }
        private void btnMore_Click(object sender, EventArgs e)
        {
            ShowAdditionalColumns();
        }

        private void btnLess_Click(object sender, EventArgs e)
        {
            ShowLessColumns();
        }

        private void cbmCompanyName_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selectedCompanyName = cbmCompanyName.SelectedItem.ToString();
            LoadCompanyInfo(selectedCompanyName);
        }
    }
}
