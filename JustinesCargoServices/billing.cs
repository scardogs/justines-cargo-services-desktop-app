using DinkToPdf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    public partial class billing : Form
    {
        MySqlConnection con = new MySqlConnection(
        "datasource=localhost;" +
        "port=3306;" +
        "database=jcsdb;" +
        "username=root;" +
        "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public billing()
        {
            InitializeComponent();
            this.Resize += billing_Resize;
        }
        private void billing_Load(object sender, EventArgs e)
        {
            InitializeWaybillDataGrid();
            populateCompanyNames();
            loaddataWaybill();
            loaddataBilling();
            loaddataBillingg();
            LoadInvoiceNumbers();
            LoadInvoiceNumberss();
            txtInvoiceNo.DropDownStyle = ComboBoxStyle.DropDown;
            txtInvoiceNo.Enabled = true;
            txtGross.TextChanged += new EventHandler(CalculateBilling);
            txtVat.TextChanged += new EventHandler(CalculateBilling);
            txtNet.TextChanged += new EventHandler(CalculateBilling);
            txtwithTax.TextChanged += new EventHandler(CalculateBilling);
            txtNetAmount.TextChanged += new EventHandler(CalculateBilling);

        }
        private void LoadInvoiceNumberss()
        {
            string query = @"
        SELECT InvoiceNum 
        FROM invoicenum 
        WHERE InvoiceNum NOT IN (SELECT InvoiceNo FROM billing)";

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;"))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Clear and populate the ComboBox
                        txtInvoiceNo.Items.Clear();

                        while (reader.Read())
                        {
                            txtInvoiceNo.Items.Add(reader["InvoiceNum"].ToString());
                        }
                    }
                }

                // Ensure the ComboBox is enabled and usable
                txtInvoiceNo.Enabled = true;
                txtInvoiceNo.DropDownStyle = ComboBoxStyle.DropDown;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoice numbers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtInvoiceNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtInvoiceNo.SelectedItem != null)
            {
                MessageBox.Show($"Selected Invoice: {txtInvoiceNo.SelectedItem}");
            }
            else
            {
                MessageBox.Show("No invoice selected.");
            }
        }

        private void CalculateBilling(object sender, EventArgs e)
        {

            if (decimal.TryParse(txtGross.Text, out decimal gross))
            {

                decimal net = gross / 1.12m;
                decimal vat = gross - net;
                decimal withTax = net * 0.02m;
                decimal amountDue = gross - withTax;


                txtNet.Text = net.ToString("0.00");
                txtVat.Text = vat.ToString("0.00");
                txtwithTax.Text = withTax.ToString("0.00");
                txtNetAmount.Text = amountDue.ToString("0.00");
            }
            else
            {

                txtNet.Text = "0.00";
                txtVat.Text = "0.00";
                txtwithTax.Text = "0.00";
                txtNetAmount.Text = "0.00";
            }
        }
        private void InitializeWaybillDataGrid()
        {
            // Ensure the first column is a DataGridViewCheckBoxColumn
            if (waybilldata.Columns.Count == 3 || !(waybilldata.Columns[3] is DataGridViewCheckBoxColumn))
            {
                DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn
                {
                    Name = "Check",
                    HeaderText = "Select",
                    Width = 50,
                    ReadOnly = false,
                    FillWeight = 10,
                };
                waybilldata.Columns.Insert(3, checkColumn);
            }
        }
        private void populateCompanyNames()
        {
            try
            {
                con.Open();
                cmd = new MySqlCommand("SELECT CompanyName FROM companyinfo;", con);
                rdr = cmd.ExecuteReader();

                cbCompanyName.Items.Clear();

                while (rdr.Read())
                {
                    cbCompanyName.Items.Add(rdr["CompanyName"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Company Names", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        private void loaddataWaybill()
        {
            waybilldata.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT waybillNo, TotalPercentage, TotalAmount FROM waybill;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                waybilldata.Rows.Add(rdr.GetString(0),

                                    rdr.GetString(1),

                                    rdr.GetString(2));

            }
            con.Close();
        }
        private void loaddataBilling()
        {
            dataBilling.Rows.Clear();

            string query = "SELECT ID, Particulars, WaybillNo, InvoiceNo, InvoiceDate, CompanyNames, TinNo, Address, Gross, Vat, Net, WithTAX, Amount FROM billing";

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
                                dataBilling.Rows.Add(
                                    rdr.GetInt32("ID"),  // ID
                                    rdr.GetString("Particulars"),  // FWNo
                                    rdr.GetString("WaybillNo"),  // WaybillNo
                                    rdr.GetString("InvoiceNo"),  // InvoiceNo
                                    rdr.IsDBNull(rdr.GetOrdinal("InvoiceDate")) ? (object)DBNull.Value : rdr.GetDateTime("InvoiceDate").ToString("yyyy-MM-dd"), // InvoiceDate
                                    rdr.GetString("CompanyNames"),  // CompanyNames
                                    rdr.GetString("TinNo"),  // TinNo
                                    rdr.GetString("Address"),  // Address
                                    rdr.GetDecimal("Gross"),  // Gross
                                    rdr.GetDecimal("Vat"),  // Vat
                                    rdr.GetDecimal("Net"),  // Net
                                    rdr.GetDecimal("WithTAX"),  // WithTAX
                                    rdr.GetDecimal("Amount")  // Amount
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
        private void loaddataBillingg()
        {
            dataBillingg.Rows.Clear();

            string query = "SELECT ID, Particulars, WaybillNo, InvoiceNo, InvoiceDate, CompanyNames, TinNo, Address, Gross, Vat, Net, WithTAX, Amount FROM billing";

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
                                dataBillingg.Rows.Add(
                                    rdr.GetInt32("ID"),  // ID
                                    rdr.GetString("Particulars"),  // FWNo
                                    rdr.GetString("WaybillNo"),  // WaybillNo
                                    rdr.GetString("InvoiceNo"),  // InvoiceNo
                                    rdr.IsDBNull(rdr.GetOrdinal("InvoiceDate")) ? (object)DBNull.Value : rdr.GetDateTime("InvoiceDate").ToString("yyyy-MM-dd"), // InvoiceDate
                                    rdr.GetString("CompanyNames"),  // CompanyNames
                                    rdr.GetString("TinNo"),  // TinNo
                                    rdr.GetString("Address"),  // Address
                                    rdr.GetDecimal("Gross"),  // Gross
                                    rdr.GetDecimal("Vat"),  // Vat
                                    rdr.GetDecimal("Net"),  // Net
                                    rdr.GetDecimal("WithTAX"),  // WithTAX
                                    rdr.GetDecimal("Amount")  // Amount
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
        private void billing_Resize(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                if (this.Parent is Dashboard parentDashboard)
                {
                    var sidebarControl = parentDashboard.Controls["pnlSidebar"] as Panel;

                    if (sidebarControl != null)
                    {
                        AdjustLayout(sidebarControl.Width);
                    }
                }
            }
        }

        public void AdjustLayout(int sidebarWidth)
        {
            // Calculate the available width (accounting for the sidebar width)
            int availableWidth = this.Parent.ClientSize.Width - sidebarWidth;
            this.Width = availableWidth;
            this.Height = this.Parent.ClientSize.Height;

            // Set the width of dataBilling (fixed width)
            int dataBillingWidth = 200;
            dataBilling.Width = dataBillingWidth;

            // Calculate the remaining width for dataParticulars
            int remainingWidth = availableWidth - dataBillingWidth;
        }

            
        private void dataParticulars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataBilling_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnlResult_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2TextBox13_TextChanged(object sender, EventArgs e)
        {

        }
        private bool CheckDuplicatesInBilling(string[] waybillNumbers, string companyName)
        {
            string waybillNoList = string.Join(",", waybillNumbers.Select(w => $"'{w}'"));
            string query = $"SELECT WaybillNo, CompanyNames FROM billing WHERE WaybillNo IN ({waybillNoList}) AND CompanyNames = @CompanyNames";

            try
            {
                con.Open(); // Open MySQL connection
                cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CompanyNames", companyName);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    List<string> duplicateFWNos = new List<string>();

                    while (rdr.Read())
                    {
                        duplicateFWNos.Add(rdr["FWNo"].ToString());
                    }

                    if (duplicateFWNos.Any())
                    {
                        MessageBox.Show($"The following WaybillNo# numbers already exist under {companyName}: {string.Join(", ", duplicateFWNos)}",
                                        "Duplicate Found",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking duplicates: " + ex.Message);
            }
            finally
            {
                con.Close(); // Close the connection
            }

            return false;
        }
        private decimal GetTotalAmountForWaybillsByCompany(string[] waybillNumbers, string companyName)
        {
            decimal totalAmount = 0m;

            // Create a comma-separated list of waybill numbers for the SQL query
            string waybillNoList = string.Join(",", waybillNumbers.Select(w => $"'{w.Trim()}'"));

            // SQL query to select Company1, Amount1, Company2, Amount2, Company3, Amount3 for the waybill numbers
            string query = $"SELECT Company1, Amount1, Company2, Amount2, Company3, Amount3 FROM waybill " +
                           $"WHERE waybillNo IN ({waybillNoList})";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();  // Open the connection
                    }

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Iterate through the result set
                        while (rdr.Read())
                        {
                            // Check if the company matches Company1, Company2, or Company3, and add the corresponding amount
                            if (rdr["Company1"].ToString().Trim() == companyName)
                            {
                                totalAmount += Convert.ToDecimal(rdr["Amount1"]);
                            }
                            else if (rdr["Company2"].ToString().Trim() == companyName)
                            {
                                totalAmount += Convert.ToDecimal(rdr["Amount2"]);
                            }
                            else if (rdr["Company3"].ToString().Trim() == companyName)
                            {
                                totalAmount += Convert.ToDecimal(rdr["Amount3"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);  // Display any error message
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();  // Ensure connection is closed
                }
            }

            return totalAmount;  // Return the total amount
        }
        private void ClearAll()
        {
            if (txtParticulars.Text.Length > 3)
            {
                txtParticulars.Text = txtParticulars.Text.Substring(0, 3);
            }
            else
            {
                txtParticulars.Clear();
            }

           

        }
        private bool ValidateLTOInput()
        {

            if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
            {
                MessageBox.Show("Invoice number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(cbCompanyName.Text))
            {
                MessageBox.Show("Company Name is required.", "Error");
                return false;
            }




            return true;
        }
        private bool ValidateWayBillInput()
        {

            if (string.IsNullOrWhiteSpace(txtInvoiceNo2.Text))
            {
                MessageBox.Show("Invoice number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(cbCompanyName2.Text))
            {
                MessageBox.Show("Company Name is required.", "Error");
                return false;
            }




            return true;
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            // Validate input before proceeding
            if (!ValidateLTOInput()) return;

            string companyName = cbCompanyName.Text.Trim();

            // Collect all selected waybill numbers
            List<string> selectedWaybillNumbers = new List<string>();

            foreach (DataGridViewRow row in waybilldata.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["Check"].Value); // Ensure "Check" is the checkbox column name
                if (isChecked)
                {
                    selectedWaybillNumbers.Add(row.Cells["WaybillNo"].Value.ToString()); // Use "WaybillNo" column name
                }
            }

            // Check if no waybill numbers were selected
            if (!selectedWaybillNumbers.Any())
            {
                MessageBox.Show("No waybill numbers selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check for duplicate waybill numbers in the billing table
            if (CheckDuplicatesInBilling(selectedWaybillNumbers.ToArray(), companyName))
            {
                return; // Stop if duplicates are found
            }

            try
            {
                con.Open(); // Open MySQL connection

                // Calculate the total gross amount for all selected waybills
                decimal totalGross = 0m;

                // SQL query to fetch the total amount
                string fetchAmountQuery = "SELECT SUM(Amount1) FROM waybill WHERE waybillNo IN (" +
                                          string.Join(",", selectedWaybillNumbers.Select(waybill => $"'{waybill}'")) + ");";

                using (MySqlCommand fetchAmountCmd = new MySqlCommand(fetchAmountQuery, con))
                {
                    object result = fetchAmountCmd.ExecuteScalar();

                    // Handle DBNull values gracefully
                    totalGross = result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
                }

                // Check if the total gross amount is zero
                if (totalGross == 0m)
                {
                    MessageBox.Show("The total gross amount is zero. Please check the selected waybill numbers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Perform calculations for the combined totals
                decimal net = totalGross / 1.12m; // Net amount excluding VAT
                decimal vat = totalGross - net; // VAT amount
                decimal withTax = net * 0.02m; // Withholding tax (2%)
                decimal netAmount = totalGross - withTax; // Net amount after withholding tax

                // Combine selected waybill numbers in the format FW#/1/2
                string waybillNosCombined = "FW#/" + string.Join("/", selectedWaybillNumbers);

                // Insert the combined billing record into the billing table
                using (MySqlCommand insertCmd = new MySqlCommand("INSERT INTO billing (Particulars, WayBillNo, InvoiceNo, InvoiceDate, CompanyNames, TinNo, Address, Gross, Vat, Net, WithTAX, Amount) " +
                                                                 "VALUES (@Particulars, @WayBillNo, @InvoiceNo, @InvoiceDate, @CompanyNames, @TinNo, @Address, @Gross, @Vat, @Net, @WithTAX, @Amount);", con))
                {
                    insertCmd.Parameters.AddWithValue("@Particulars", txtParticulars.Text); // Insert the combined waybill numbers into Particulars
                    insertCmd.Parameters.AddWithValue("@WayBillNo", waybillNosCombined); // Combine waybill numbers as a single entry
                    insertCmd.Parameters.AddWithValue("@InvoiceNo", txtInvoiceNo.Text);
                    insertCmd.Parameters.AddWithValue("@InvoiceDate", PickerInvoiceDate.Value.ToString("yyyy-MM-dd"));
                    insertCmd.Parameters.AddWithValue("@CompanyNames", companyName);
                    insertCmd.Parameters.AddWithValue("@TinNo", txtTin.Text);
                    insertCmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    insertCmd.Parameters.AddWithValue("@Gross", totalGross);
                    insertCmd.Parameters.AddWithValue("@Vat", vat);
                    insertCmd.Parameters.AddWithValue("@Net", net);
                    insertCmd.Parameters.AddWithValue("@WithTAX", withTax);
                    insertCmd.Parameters.AddWithValue("@Amount", netAmount);

                    insertCmd.ExecuteNonQuery(); // Execute the insert query
                }

                MessageBox.Show("Billing record added successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during insert: " + ex.Message);
            }
            finally
            {
                con.Close(); // Ensure the connection is closed
            }

            // Reload the billing data after the insert
            loaddataBilling();
            loaddataBillingg();

            // Clear form inputs
            ClearAll();
            unhideBill();
        }








        private void waybilldata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0) // Ensure it's a valid row and the checkbox column
            {
                waybilldata.CommitEdit(DataGridViewDataErrorContexts.Commit); // Commit changes
            }
        }

        private void waybilldata_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0) // Ensure it's the checkbox column
            {
                var isChecked = Convert.ToBoolean(waybilldata.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

            }
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            pnlAddBill.Visible = !pnlAddBill.Visible;
        }

        private void cbCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFilteredWaybills();
            filltinandaddress();


        }
        private void filltinandaddress()
        {
            // Get the selected company name from the ComboBox
            string selectedCompanyName = cbCompanyName.SelectedItem.ToString();

            // Define the query to fetch company details
            string query = @"
    SELECT ID, CompanyName, Address, TinNo 
    FROM companyinfo 
    WHERE CompanyName = @CompanyName;";

            try
            {
                // Ensure the database connection is open
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CompanyName", selectedCompanyName);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            // Fill the textboxes with data from the database
                            txtAddress.Text = rdr["Address"].ToString();
                            txtTin.Text = rdr["TinNo"].ToString();
                        }
                        else
                        {
                            // If no data is found, clear the textboxes
                            txtAddress.Clear();
                            txtTin.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the database connection is closed
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        // Method to load filtered waybills into the DataGridView
        private void LoadFilteredWaybills()
        {
            string companyName = cbCompanyName.Text.Trim();

            // Define the query to filter waybill numbers
            string query = @"
    SELECT 
        waybill.waybillNo, 
        waybill.Company1, waybill.Percentage AS Percentage1, waybill.Amount1,
        waybill.Company2, waybill.Percentage2 AS Percentage2, waybill.Amount2,
        waybill.Company3, waybill.Percentage3 AS Percentage3, waybill.Amount3
    FROM waybill
    WHERE 
        (waybill.Company1 = @CompanyName OR waybill.Company2 = @CompanyName OR waybill.Company3 = @CompanyName)
        AND NOT EXISTS (
            SELECT 1
            FROM billing
            WHERE 
                billing.WaybillNo LIKE 'FW#/%'
                AND FIND_IN_SET(waybill.waybillNo, REPLACE(SUBSTRING(billing.WaybillNo, 4), '/', ',')) > 0
        );";

            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                // Clear and configure DataGridView columns
                ConfigureWaybillDataGrid();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CompanyName", companyName);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string waybillNo = rdr["waybillNo"].ToString();
                            string company1 = rdr["Company1"]?.ToString() ?? string.Empty;
                            string percentage1 = rdr["Percentage1"]?.ToString() ?? "0%";
                            string amount1 = rdr["Amount1"]?.ToString() ?? "0";

                            string company2 = rdr["Company2"]?.ToString() ?? string.Empty;
                            string percentage2 = rdr["Percentage2"]?.ToString() ?? "0%";
                            string amount2 = rdr["Amount2"]?.ToString() ?? "0";

                            string company3 = rdr["Company3"]?.ToString() ?? string.Empty;
                            string percentage3 = rdr["Percentage3"]?.ToString() ?? "0%";
                            string amount3 = rdr["Amount3"]?.ToString() ?? "0";

                            // Add rows for the matching company
                            if (company1 == companyName)
                            {
                                waybilldata.Rows.Add(false, waybillNo, company1, percentage1, amount1);
                            }
                            if (company2 == companyName)
                            {
                                waybilldata.Rows.Add(false, waybillNo, company2, percentage2, amount2);
                            }
                            if (company3 == companyName)
                            {
                                waybilldata.Rows.Add(false, waybillNo, company3, percentage3, amount3);
                            }
                        }
                    }
                }

                // Notify if no waybills found
                if (waybilldata.Rows.Count == 0)
                {
                    MessageBox.Show("No waybills found for the specified company or all waybills are already billed.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // Configure DataGridView for displaying waybill data
        private void ConfigureWaybillDataGrid()
        {
            waybilldata.Rows.Clear();
            waybilldata.Columns.Clear();

            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.Name = "Check";
            checkBoxColumn.HeaderText = "Select";
            waybilldata.Columns.Add(checkBoxColumn);

            waybilldata.Columns.Add("WaybillNo", "Waybill No");
            waybilldata.Columns.Add("Company", "Company");
            waybilldata.Columns.Add("Percentage", "Percentage");
            waybilldata.Columns.Add("Amount", "Amount");

            waybilldata.Columns["Percentage"].ValueType = typeof(string);
            waybilldata.Columns["Amount"].ValueType = typeof(decimal);
        }

        // Handle DataGridView DataError event to prevent crashes
        private void waybilldata_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Suppress the error or log it if necessary
            e.ThrowException = false;
        }
        // Load all waybill data (default method)
        private void LoadDataWaybill()
        {
            try
            {
                waybilldata.Rows.Clear();
                ConfigureWaybillDataGrid();

                using (MySqlCommand cmd = new MySqlCommand("SELECT waybillNo, TotalPercentage, TotalAmount FROM waybill;", con))
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            waybilldata.Rows.Add(
                                rdr["waybillNo"].ToString(),
                                string.Empty, // No specific company here
                                rdr["TotalPercentage"]?.ToString() ?? "0",
                                rdr["TotalAmount"]?.ToString() ?? "0"
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // Handle DataGridView errors
       




        private void unhideBill()
        {
            dataBillingg.Visible = !dataBillingg.Visible;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            unhideBill();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            pnlAddBill.Visible = !pnlAddBill.Visible;

        }

        

        private void dataBillingg_SelectionChanged(object sender, EventArgs e)
        {
            if (dataBillingg.SelectedRows.Count > 0)
            {

                txtID2.Text = dataBillingg.SelectedRows[0].Cells[0].Value.ToString();
                txtParticulars2.Text = dataBillingg.SelectedRows[0].Cells[1].Value.ToString();

                txtInvoiceNo2.Text = dataBillingg.SelectedRows[0].Cells[3].Value.ToString();
                PickerInvoiceDate2.Value = dataBillingg.SelectedRows[0].Cells[4].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataBilling.SelectedRows[0].Cells[4].Value);
                cbCompanyName2.Text = dataBillingg.SelectedRows[0].Cells[5].Value.ToString();
                txtTin2.Text = dataBillingg.SelectedRows[0].Cells[6].Value.ToString();
                txtAddress2.Text = dataBillingg.SelectedRows[0].Cells[7].Value.ToString();
                txtGross2.Text = dataBilling.SelectedRows[0].Cells[8].Value.ToString();
                txtVat2.Text = dataBilling.SelectedRows[0].Cells[9].Value.ToString();
                txtNet2.Text = dataBilling.SelectedRows[0].Cells[10].Value.ToString();
                txtwithTax2.Text = dataBilling.SelectedRows[0].Cells[11].Value.ToString();
                txtNetAmount2.Text = dataBilling.SelectedRows[0].Cells[12].Value.ToString();








            }
        }

        private void dataBilling_SelectionChanged(object sender, EventArgs e)
        {
            if (dataBilling.SelectedRows.Count > 0)
            {

                txtID2.Text = dataBilling.SelectedRows[0].Cells[0].Value.ToString();
                txtParticulars2.Text = dataBilling.SelectedRows[0].Cells[1].Value.ToString();

                txtInvoiceNo2.Text = dataBilling.SelectedRows[0].Cells[3].Value.ToString();
                PickerInvoiceDate2.Value = dataBilling.SelectedRows[0].Cells[4].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataBilling.SelectedRows[0].Cells[4].Value);
                cbCompanyName2.Text = dataBilling.SelectedRows[0].Cells[5].Value.ToString();
                txtTin2.Text = dataBilling.SelectedRows[0].Cells[6].Value.ToString();
                txtAddress2.Text = dataBilling.SelectedRows[0].Cells[7].Value.ToString();
                txtGross2.Text = dataBilling.SelectedRows[0].Cells[8].Value.ToString();
                txtVat2.Text = dataBilling.SelectedRows[0].Cells[9].Value.ToString();
                txtNet2.Text = dataBilling.SelectedRows[0].Cells[10].Value.ToString();
                txtwithTax2.Text = dataBilling.SelectedRows[0].Cells[11].Value.ToString();
                txtNetAmount2.Text = dataBilling.SelectedRows[0].Cells[12].Value.ToString();









            }
        }

        
        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            pnlEdit.Visible = !pnlEdit.Visible;
          

        }

        private void btnUpdate2_Click(object sender, EventArgs e)
        {

        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = !pnlEdit.Visible;
        }

        private void waybilldata_SelectionChanged(object sender, EventArgs e)
        {
            if (waybilldata.SelectedRows.Count > 0)
            {

                txtGross.Text = waybilldata.SelectedRows[0].Cells[2].Value.ToString();
                








            }
        }

        private void btnEdit2_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = !pnlEdit.Visible;

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateWayBillInput()) return;
            if (dataBilling.SelectedRows.Count > 0)
            {
                try
                {
                    // Open the database connection
                    con.Open();

                    // Corrected SQL query with spaces between the statements
                    cmd = new MySqlCommand("UPDATE billing " +
                                           "SET " +
                                           "Particulars = @Particulars, " +
                                           "InvoiceNo = @InvoiceNo, " +
                                           "InvoiceDate = @InvoiceDate, " +
                                           "CompanyNames = @CompanyNames, " +
                                           "Gross = @Gross, " +
                                           "Vat = @Vat, " +
                                           "Net = @Net, " +
                                           "WithTAX = @WithTAX, " +
                                           "Amount = @Amount, " +
                                           "Address = @Address " + // Added space before WHERE clause
                                           "WHERE ID = @ID;", con);

                    // Set the parameters for the query using values from the form controls
                    cmd.Parameters.AddWithValue("@ID", txtID2.Text);
                    cmd.Parameters.AddWithValue("@Particulars", txtParticulars2.Text);
                    cmd.Parameters.AddWithValue("@InvoiceNo", txtInvoiceNo2.Text);
                    cmd.Parameters.AddWithValue("@InvoiceDate", PickerInvoiceDate2.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@CompanyNames", cbCompanyName2.Text);
                    cmd.Parameters.AddWithValue("@TinNO", txtTin2.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress2.Text);
                    cmd.Parameters.AddWithValue("@Vat", txtVat2.Text);
                    cmd.Parameters.AddWithValue("@Net", txtNet2.Text);
                    cmd.Parameters.AddWithValue("@WithTAX", txtwithTax2.Text);
                    cmd.Parameters.AddWithValue("@Amount", txtNetAmount2.Text);

                    // Execute the update query
                    cmd.ExecuteNonQuery();

                    // Close the database connection after successful update
                    con.Close();

                    // Show a success message and reload the data
                    MessageBox.Show("Update successful!");
                    loaddataBilling();
                    ClearAll();
                    backfromedit();
                }
                catch (Exception ex)
                {
                    // Handle any potential errors by showing an error message
                    MessageBox.Show("Error during update: " + ex.Message);
                }
                finally
                {
                    // Ensure the connection is closed in case of an error
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                // Show an error message if no row is selected
                MessageBox.Show("Please select a record to update.");
            }
        }
        private void backfromedit()
        {
            pnlEdit.Visible = !pnlEdit.Visible;
        }
        private void btnBack2_Click_1(object sender, EventArgs e)
        {
            pnlEdit.Visible = !pnlEdit.Visible;
        }

        private void btnPrintBilling_Click(object sender, EventArgs e)
        {
            if (dataBilling.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to generate the billing receipt.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = dataBilling.SelectedRows.Count;

                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through selected rows in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = dataBilling.SelectedRows.Cast<DataGridViewRow>()
                                                      .Skip(i)
                                                      .Take(batchSize)
                                                      .Where(row => row.Visible); // Filter only visible rows

                    StringBuilder htmlBillingReport = new StringBuilder();
                    htmlBillingReport.AppendLine("<html>");
                    htmlBillingReport.AppendLine("<head><style>");
                    htmlBillingReport.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlBillingReport.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlBillingReport.AppendLine("th { background-color: #f2f2f2; }");
                    htmlBillingReport.AppendLine("h2, h3 { text-align: center; }");
                    htmlBillingReport.AppendLine(".section-header { margin-top: 20px; font-weight: bold; }");
                    htmlBillingReport.AppendLine("</style></head>");
                    htmlBillingReport.AppendLine("<body>");
                    htmlBillingReport.AppendLine("<h2>Billing Receipt</h2>");
                    htmlBillingReport.AppendLine("<h3>JUSTIN'S CARGO SERVICES</h3>");
                    htmlBillingReport.AppendLine("<p>123 Company Address, City, Country</p>");
                    htmlBillingReport.AppendLine("<p>Contact Info: (000) 123-4567</p>");

                    // Loop through the selected rows and build the receipt for each
                    foreach (var row in rows)
                    {
                        htmlBillingReport.AppendLine("<h3 class='section-header'>Billing Information</h3>");
                        htmlBillingReport.AppendLine("<table style='width: 100%;'>");
                        htmlBillingReport.AppendLine("<tr>");
                        htmlBillingReport.AppendLine("<td><b>Invoice No:</b> " + (row.Cells["INVOICE"].Value ?? "N/A") + "</td>");
                        htmlBillingReport.AppendLine($"<td><b>Company Name:</b> {row.Cells["CN"].Value ?? "N/A"}</td>");
                        htmlBillingReport.AppendLine("</tr>");
                        htmlBillingReport.AppendLine("<tr>");
                        htmlBillingReport.AppendLine($"<td><b>Invoice Date:</b> {(row.Cells["InvoiceDate"].Value != null ? Convert.ToDateTime(row.Cells["InvoiceDate"].Value).ToString("MMMM d, yyyy") : "N/A")}</td>");
                        htmlBillingReport.AppendLine($"<td><b>FW No:</b> {row.Cells["Waybill"].Value ?? "N/A"}</td>");
                        htmlBillingReport.AppendLine("</tr>");
                        htmlBillingReport.AppendLine("<tr>");
                        htmlBillingReport.AppendLine($"<td><b>TIN:</b> {row.Cells["Tin"].Value ?? "N/A"}</td>");
                        htmlBillingReport.AppendLine($"<td><b>Address:</b> {row.Cells["Address"].Value ?? "N/A"}</td>");
                        htmlBillingReport.AppendLine("</tr>");
                        htmlBillingReport.AppendLine("</table>");

                        // Amounts section
                        htmlBillingReport.AppendLine("<h3 class='section-header'>Billing Summary</h3>");
                        htmlBillingReport.AppendLine("<table>");
                        htmlBillingReport.AppendLine("<tr><th>Description</th><th>Amount</th></tr>");
                        htmlBillingReport.AppendLine($"<tr><td>Gross</td><td>{(row.Cells["GROSS"].Value != null ? Convert.ToDecimal(row.Cells["GROSS"].Value).ToString("C2") : "₱0.00")}</td></tr>");
                        htmlBillingReport.AppendLine($"<tr><td>VAT</td><td>{(row.Cells["VAT"].Value != null ? Convert.ToDecimal(row.Cells["VAT"].Value).ToString("C2") : "₱0.00")}</td></tr>");
                        htmlBillingReport.AppendLine($"<tr><td>Net</td><td>{(row.Cells["NET"].Value != null ? Convert.ToDecimal(row.Cells["NET"].Value).ToString("C2") : "₱0.00")}</td></tr>");
                        htmlBillingReport.AppendLine($"<tr><td>With Tax</td><td>{(row.Cells["WT"].Value != null ? Convert.ToDecimal(row.Cells["WT"].Value).ToString("C2") : "₱0.00")}</td></tr>");
                        htmlBillingReport.AppendLine($"<tr><td><b>Total Amount</b></td><td><b>{(row.Cells["NA"].Value != null ? Convert.ToDecimal(row.Cells["NA"].Value).ToString("C2") : "₱0.00")}</b></td></tr>");
                        htmlBillingReport.AppendLine("</table>");
                    }

                    // Close the HTML structure for the current batch
                    htmlBillingReport.AppendLine("</body>");
                    htmlBillingReport.AppendLine("</html>");

                    // Add the generated HTML content for the current batch to the list
                    htmlContents.Add(htmlBillingReport.ToString());
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Passing the List<string> of HTML contents

                MessageBox.Show("Billing report has been generated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GeneratePdf(List<string> htmlContents)
        {
            try
            {
                var converter = new BasicConverter(new PdfTools()); // Initialize the converter outside the loop

                foreach (var htmlContent in htmlContents)
                {
                    var doc = new HtmlToPdfDocument()
                    {
                        GlobalSettings = {
                    ColorMode = DinkToPdf.ColorMode.Color,
                    Orientation = DinkToPdf.Orientation.Landscape,
                    PaperSize = DinkToPdf.PaperKind.A4
                },
                        Objects = {
                    new DinkToPdf.ObjectSettings() {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
                    };

                    // Generate the PDF
                    byte[] pdf = converter.Convert(doc);

                    // Save and open the PDF with a unique name
                    SaveAndOpenPdf(pdf);

                    // Cleanup HtmlToPdfDocument explicitly
                    doc = null;
                }

                // Cleanup the converter explicitly
                converter = null;
                GC.Collect(); // Force garbage collection
                GC.WaitForPendingFinalizers();
            }
            catch (AccessViolationException ex)
            {
                MessageBox.Show($"Access violation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveAndOpenPdf(byte[] pdfContent)
        {
            try
            {
                // Specify the download directory
                string downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                // Generate a unique file name using a timestamp and GUID
                string uniqueName = $"GeneratedReport_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString().Substring(0, 8)}.pdf";
                string filePath = Path.Combine(downloadDirectory, uniqueName);

                // Save the file
                File.WriteAllBytes(filePath, pdfContent);

                // Open the PDF in the default browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving/opening PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnShowAddIN_Click(object sender, EventArgs e)
        {
            pnlShowIN.Visible = !pnlShowIN.Visible;
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
        private void btnAddInvoice_Click(object sender, EventArgs e)
        {
            if (!ValidateAddWaybillInput()) return;

            try
            {
                int stubNo = int.Parse(txtSTubNo.Text);
                int from = int.Parse(txtFrom.Text);
                int to = int.Parse(txtTo.Text);

                bool hasDuplicates = false;

                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;"))
                {
                    con.Open(); // Open the connection once at the beginning

                    for (int wayBillNo = from; wayBillNo <= to; wayBillNo++)
                    {
                        // Check if the waybill already exists
                        if (WaybillExists(con, stubNo, wayBillNo))
                        {
                            hasDuplicates = true;
                            continue;
                        }

                        // Insert the new waybill into the invoicenum table
                        using (MySqlCommand cmd = new MySqlCommand("INSERT INTO invoicenum (StubNo, InvoiceNum) VALUES (@StubNo, @InvoiceNum);", con))
                        {
                            cmd.Parameters.AddWithValue("@StubNo", stubNo);
                            cmd.Parameters.AddWithValue("@InvoiceNum", wayBillNo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                if (hasDuplicates)
                {
                    MessageBox.Show($"Some waybill numbers under stub number {stubNo} already exist.", "Duplicate Waybill", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Waybills added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadInvoiceNumbers(); // Refresh the data grid
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Check if the waybill exists
        private bool WaybillExists(MySqlConnection con, int stubNo, int wayBillNo)
        {
            bool exists = false;

            try
            {
                // Check if the waybill already exists in the invoicenum table
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM invoicenum WHERE StubNo = @StubNo AND InvoiceNum = @InvoiceNum;", con))
                {
                    cmd.Parameters.AddWithValue("@StubNo", stubNo);
                    cmd.Parameters.AddWithValue("@InvoiceNum", wayBillNo);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    exists = count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return exists;
        }


        private bool InvoiceExists(int id, string stubNo, string invoiceNum)
        {
            string query = "SELECT COUNT(*) FROM invoiceNum WHERE ID = @ID AND StubNo = @StubNo AND InvoiceNum = @InvoiceNum;";
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@StubNo", stubNo);
                cmd.Parameters.AddWithValue("@InvoiceNum", invoiceNum);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
        private void LoadInvoiceNumbers()
        {
            dataInvoiceInfo.Rows.Clear();
            string query = "SELECT ID, StubNo, InvoiceNum FROM invoiceNum;";
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        waybilldata.Rows.Add(
                            rdr["ID"],
                            rdr["StubNo"],
                            rdr["InvoiceNum"]
                           
                        );
                    }
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            dataInvoiceInfo.Visible = !dataInvoiceInfo.Visible;
        }

       
    }
}
