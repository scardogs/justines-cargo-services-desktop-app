using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using DinkToPdf;
using System.IO;


namespace JustinesCargoServices
{
    public partial class reports : Form
    {
        MySqlConnection con = new MySqlConnection(
          "datasource=localhost;" +
          "port=3306;" +
          "database=JCSdb;" +
          "username=root;" +
          "password=;" +
          "Convert Zero Datetime=True;");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public reports()
        {
            InitializeComponent();
            this.Load += Reports_Load;
        }
        private void Reports_Load(object sender, EventArgs e)
        {
            SetLongMonthYearFormat();
            PopulateAllDriverNames();
            billing_data();
            payrollinfoLoad();
            UpdateTotalNet();
            populateCompanyName();
            renewalLTO_data();
            renewalLTFRB_data();
            renewalInsurance_data();
            loaddataWaybill();
            Delivery_data();
            truck_data();
            fuel_data();
            UpdateFuelData();
            UpdateColumnVisibility2();

        }
        private void UpdateFuelData()
        {
            string selectedDepartment = cbSortBy.SelectedItem?.ToString() ?? "All Departments";
            DateTime? startDate = dateFrom2.Value;
            DateTime? endDate = dateTo2.Value;

            // Pass the selected department and date range into fuel_data
            fuel_data(selectedDepartment, startDate, endDate);
        }
        private void loaddataWaybill()
        {
            waybilldata.Rows.Clear();

            string query = "SELECT ID, waybillNo, ReferenceNo, DispatchDate, TotalLoadorCbm, CbmorPalleteOccupied, Rate, ADDTL, Company1, percentage, Amount1, Company2, percentage2, Amount2, Company3, percentage3, Amount3, TotalPercentage, TotalRate,  TotalAmount, Status FROM waybill";

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
                                    rdr.GetString(20)

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
        private void payrollinfoLoad()
        {
            dataPayrollInfo.Rows.Clear();



            con.Open();
            cmd = new MySqlCommand("SELECT EmployeeID, empFname, empLname, modeOfPay, grossPay, netPay FROM empprofiling;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataPayrollInfo.Rows.Add(
                            rdr.GetInt32(0),
                            rdr.GetString(1),
                            rdr.GetString(2),
                            rdr.GetString(3),
                            rdr.GetDecimal(4),
                            rdr.GetDecimal(5)

                        );

            }
            con.Close();

        }
        private void PopulateAllDriverNames()
        {
            txtDriverName.Items.Clear(); // Clear existing items

            string connectionString = "datasource=localhost;port=3306;database=jcsdb;username=root;password=''";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    // Query to retrieve all drivers' names
                    string query = "SELECT CONCAT(empLname, ', ', empFname) AS FullName " +
                                   "FROM empprofiling " +
                                   "WHERE Position = 'Driver';";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string fullName = rdr.GetString("FullName");

                                // Add the driver's full name directly to the ComboBox
                                txtDriverName.Items.Add(fullName);
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SetLongMonthYearFormat()
        {
            // Set the Format type to Custom
            dateDelivery.Format = DateTimePickerFormat.Custom;

            // Use "MMMM yyyy" for long month name and year
            dateDelivery.CustomFormat = "MMMM yyyy";

            // Use ShowUpDown to disable the calendar dropdown
            dateDelivery.ShowUpDown = true;
        }
        private void fuel_data(string departmentFilter = "All Departments", DateTime? startDate = null, DateTime? endDate = null)
        {
            data_Fuel.Rows.Clear();

            decimal totalFuelConsumption = 0;
            decimal totalPrice = 0;

            try
            {
                con.Open();

                // Base query
                string query = "SELECT ID, fuel_date, plateNum, department, qty, totalFuelAmount FROM fuelmonitoring WHERE 1=1";

                // Add date filter
                if (startDate.HasValue && endDate.HasValue)
                {
                    query += " AND fuel_date BETWEEN @StartDate AND @EndDate";
                }

                // Add department filter
                if (departmentFilter != "All Departments")
                {
                    query += " AND department = @Department";
                }

                cmd = new MySqlCommand(query, con);

                // Add parameters for filtering
                if (startDate.HasValue && endDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@EndDate", endDate.Value.ToString("yyyy-MM-dd"));
                }

                if (departmentFilter != "All Departments")
                {
                    cmd.Parameters.AddWithValue("@Department", departmentFilter);
                }

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    // Populate the grid
                    var row = new object[]
                    {
                rdr.GetInt32(0),
                rdr.IsDBNull(1) ? (object)DBNull.Value : rdr.GetDateTime(1).ToString("yyyy-MM-dd"),
                rdr.GetString(2),
                rdr.GetString(3),
                rdr.GetDecimal(4),
                rdr.GetDecimal(5)
                    };
                    data_Fuel.Rows.Add(row);

                    // Accumulate totals
                    totalFuelConsumption += rdr.GetDecimal(4);
                    totalPrice += rdr.GetDecimal(5);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving fuel data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

            // Update textboxes
            txtTotalFuelCost.Text = totalFuelConsumption.ToString("N2");
            txtTotalPrice.Text = totalPrice.ToString("C2");
        }
        private void Delivery_data()
        {
            deliveryData.Rows.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "SELECT LiquidationNO, PlateNo, PayDate,Reference,  " +
                                   "ArrivalDate, Driver " +
                                   "FROM delivery_multi;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                                deliveryData.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.IsDBNull(2) ? (object)DBNull.Value : rdr.GetDateTime(2).ToString("yyyy-MM-dd"), // LoadingDate

                                    rdr.GetString(3),

                                    rdr.IsDBNull(4) ? (object)DBNull.Value : rdr.GetDateTime(4).ToString("yyyy-MM-dd"), // ArrivalDate

                                    rdr.GetString(5)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void renewalInsurance_data()
        {
            dataInsurance.Rows.Clear(); // Clear existing rows

            con.Open();
            cmd = new MySqlCommand("SELECT ID, plateNo, vehicleUnit, insuranceType, policyNo, from_, to_, cvNo, checkNo, checkDate, orNo, orDate FROM insurancerenewal;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataInsurance.Rows.Add(
                    rdr.GetInt32(0),  // ID (column 0)
                    rdr.GetString(1), // plateNo (column 1)
                    rdr.GetString(2), // vehicleUnit (column 2)
                    rdr.GetString(3), // insuranceType (column 3)
                    rdr.GetString(4), // policyNo (column 4)
                    rdr.IsDBNull(5) ? (object)DBNull.Value : rdr.GetDateTime(5).ToString("yyyy-MM-dd"), // from_ (column 5)
                    rdr.IsDBNull(6) ? (object)DBNull.Value : rdr.GetDateTime(6).ToString("yyyy-MM-dd"), // to_ (column 6)
                    rdr.GetString(7), // cvNo (column 7)
                    rdr.GetString(8), // checkNo (column 8)
                    rdr.IsDBNull(9) ? (object)DBNull.Value : rdr.GetDateTime(9).ToString("yyyy-MM-dd"), // checkDate (column 9)
                    rdr.GetString(10), // orNo (column 10)
                    rdr.IsDBNull(11) ? (object)DBNull.Value : rdr.GetDateTime(11).ToString("yyyy-MM-dd") // orDate (column 11)
                );
            }
            con.Close();
        }
        private void renewalLTFRB_data()
        {
            dataLTFRB.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT ID, CaseNum, DecisionDate, PlateNum, ExpiryDate FROM ltfrbrenewal ;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataLTFRB.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.IsDBNull(2) ? (object)DBNull.Value : rdr.GetDateTime(2).ToString("yyyy-MM-dd"),
                    rdr.GetString(3), rdr.IsDBNull(4) ? (object)DBNull.Value : rdr.GetDateTime(4).ToString("yyyy-MM-dd"));

            }
            con.Close();
        }
        private void renewalLTO_data()
        {
            dataLTOrenew.Rows.Clear();
            con.Open();
            cmd = new MySqlCommand("SELECT LTO_ID, PlateNo, VehicleType, Color, MVUCrate, Duedate, Regname, ORnum, ORdate FROM ltorenewal;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataLTOrenew.Rows.Add(
                    rdr.GetInt32(0),
                    rdr.GetString(1),
                    rdr.GetString(2),
                    rdr.GetString(3),
                    rdr.GetString(4),
                    rdr.IsDBNull(5) ? (object)DBNull.Value : rdr.GetDateTime(5).ToString("yyyy-MM-dd"),
                    rdr.GetString(6),
                    rdr.GetString(7),
                    rdr.IsDBNull(8) ? (object)DBNull.Value : rdr.GetDateTime(8).ToString("yyyy-MM-dd")

                );
            }
            con.Close();
        }
        private void billing_data()
        {
            dataBilling.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT ID, WaybillNo, InvoiceNo, InvoiceDate, CompanyNames, Gross, Vat, Net, WithTAX, Amount FROM billing;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataBilling.Rows.Add(
                                    rdr.GetInt32(0),
                                     rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.IsDBNull(3) ? (object)DBNull.Value : rdr.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    rdr.GetString(4),
                                    rdr.GetDecimal(5),
                                    rdr.GetDecimal(6),
                                    rdr.GetDecimal(7),
                                    rdr.GetDecimal(8),
                                    rdr.GetDecimal(9));

            }
            con.Close();
        }
        private void populateCompanyName()
        {
            try
            {
                con.Open();
                cmd = new MySqlCommand("SELECT CompanyName FROM companyinfo;", con);
                rdr = cmd.ExecuteReader();

                cbmCompanyName.Items.Clear();

                while (rdr.Read())
                {
                    cbmCompanyName.Items.Add(rdr["CompanyName"].ToString());
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
        private void dateFrom_ValueChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void dateTo_ValueChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void FilterData()
        {
            try
            {
                dataBilling.Rows.Clear(); // Clear existing rows in the DataGridView

                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                // Get the selected dates from the DateTimePickers
                DateTime fromDate = dateFrom.Value.Date;
                DateTime toDate = dateTo.Value.Date.AddDays(1).AddSeconds(-1); // Include the full end date

                // SQL query to filter data
                string query = "SELECT ID, WaybillNo, InvoiceNo, InvoiceDate, CompanyNames, Gross, Vat, Net, WithTAX, Amount " +
                               "FROM billing " +
                               "WHERE InvoiceDate BETWEEN @dateFrom AND @dateTo";

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@dateFrom", fromDate);
                    command.Parameters.AddWithValue("@dateTo", toDate);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataBilling.Rows.Add(
                                reader.GetInt32(0),  // ID
                                reader.GetString(1), // FWNo
                                reader.GetString(2), // InvoiceNo
                                reader.IsDBNull(3) ? (object)DBNull.Value : reader.GetDateTime(3).ToString("yyyy-MM-dd"), // InvoiceDate
                                reader.GetString(4), // CompanyNames
                                reader.GetDecimal(5), // Gross
                                reader.GetDecimal(6), // Vat
                                reader.GetDecimal(7), // Net
                                reader.GetDecimal(8), // WithTAX
                                reader.GetDecimal(9)  // Amount
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    UpdateTotalNet();
                }
            }
        }
        private void UpdateTotalNet()
        {
            decimal totalNet = 0;

            // Iterate through the rows of the DataGridView and sum the values in the 'Amount' column
            foreach (DataGridViewRow row in dataBilling.Rows)
            {
                // Check if the 'Amount' cell is not null or empty
                if (row.Cells["AMOUNTD"].Value != null)
                {
                    // Add the value to the total
                    totalNet += Convert.ToDecimal(row.Cells["AMOUNTD"].Value);
                }
            }

            // Display the total in txtTotalNet
            txtTotalNet.Text = $"₱{totalNet:N2}"; // Format as currency
        }

        private void checkGross_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnVisibility();
        }

        private void checkVat_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnVisibility();
        }

        private void checkNet_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnVisibility();
        }

        private void checkWTax_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnVisibility();
        }
        private void UpdateColumnVisibility()
        {
            foreach (DataGridViewColumn column in dataBilling.Columns)
            {
                column.Visible = true;
            }

            // Show the column based on the checkbox state
            if (checkGross.Checked)
                dataBilling.Columns["GROSS"].Visible = true;
            if (checkVat.Checked)
                dataBilling.Columns["VAT"].Visible = true;
            if (checkNet.Checked)
                dataBilling.Columns["NET"].Visible = true;
            if (checkWTax.Checked)
                dataBilling.Columns["WTAX"].Visible = true;
        }
        private void UpdateColumnVisibility2()
        {
            foreach (DataGridViewColumn column in dataBilling.Columns)
            {
                column.Visible = true;
            }

          
          
                dataBilling.Columns["GROSS"].Visible = false;
           
                dataBilling.Columns["VAT"].Visible = false;
           
                dataBilling.Columns["NET"].Visible = false;
        
                dataBilling.Columns["WTAX"].Visible = false;
        }
        private void btnPrintBilling_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in dataBilling
            if (dataBilling.Rows.Count == 0)
            {
                MessageBox.Show("No data available for the receipt.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = dataBilling.Rows.Count;
                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through data in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = dataBilling.Rows.Cast<DataGridViewRow>()
                                               .Skip(i)
                                               .Take(batchSize);

                    StringBuilder htmlReceipt = new StringBuilder();

                    htmlReceipt.AppendLine("<html>");
                    htmlReceipt.AppendLine("<head><style>");
                    htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
                    htmlReceipt.AppendLine("</style></head>");
                    htmlReceipt.AppendLine("<body>");
                    htmlReceipt.AppendLine("<h2 style='text-align: center;'>BILLING RECEIPT</h2>");
                    htmlReceipt.AppendLine("<table>");

                    // Build table headers dynamically based on visible columns
                    htmlReceipt.AppendLine("<tr>");
                    foreach (DataGridViewColumn column in dataBilling.Columns)
                    {
                        if (column.Visible) // Only add visible columns to the header
                        {
                            htmlReceipt.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
                        }
                    }
                    htmlReceipt.AppendLine("</tr>");

                    // Loop through the rows of the DataGridView and extract data for visible columns
                    foreach (var row in rows)
                    {
                        if (row.Cells[0].Value != null) // Ensure the row is not empty
                        {
                            htmlReceipt.AppendLine("<tr>");
                            foreach (DataGridViewColumn column in dataBilling.Columns)
                            {
                                if (column.Visible) // Only add cells for visible columns
                                {
                                    var cellValue = row.Cells[column.Name].Value?.ToString() ?? "";
                                    htmlReceipt.AppendLine($"<td>{HttpUtility.HtmlEncode(cellValue)}</td>");
                                }
                            }
                            htmlReceipt.AppendLine("</tr>");
                        }
                    }

                    // Check if the AMOUNTD column is visible and calculate the total if it is
                    if (dataBilling.Columns["AMOUNTD"].Visible)
                    {
                        decimal totalAmount = dataBilling.Rows.Cast<DataGridViewRow>()
                                                  .Where(r => r.Cells["AMOUNTD"].Value != null)
                                                  .Sum(r => Convert.ToDecimal(r.Cells["AMOUNTD"].Value ?? 0));

                        // Add total amount row
                        int visibleColumnCount = dataBilling.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
                        htmlReceipt.AppendLine("<tr>");
                        htmlReceipt.AppendLine($"<td colspan='{visibleColumnCount - 1}' style='text-align: right;'><strong>Total Amount:</strong></td>");
                        htmlReceipt.AppendLine($"<td><strong>{totalAmount.ToString("C2")}</strong></td>");
                        htmlReceipt.AppendLine("</tr>");
                    }

                    // Close table and HTML tags
                    htmlReceipt.AppendLine("</table>");
                    htmlReceipt.AppendLine("</body>");
                    htmlReceipt.AppendLine("</html>");

                    htmlContents.Add(htmlReceipt.ToString()); // Add the generated HTML content to the list
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Now passing the List<string> of HTML contents
                UpdateTotalNet(); // Update total net (if needed)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateFromLto_ValueChanged(object sender, EventArgs e)
        {
            FilterLtoData();
        }

        private void dateToLto_ValueChanged(object sender, EventArgs e)
        {
            FilterLtoData();
        }

        private void btnPrintLTOrep_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in dataLTOrenew
            if (dataLTOrenew.Rows.Count == 0)
            {
                MessageBox.Show("No data available to print.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = dataLTOrenew.Rows.Count;
                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through data in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = dataLTOrenew.Rows.Cast<DataGridViewRow>()
                                               .Skip(i)
                                               .Take(batchSize);

                    StringBuilder htmlReceipt = new StringBuilder();

                    htmlReceipt.AppendLine("<html>");
                    htmlReceipt.AppendLine("<head><style>");
                    htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
                    htmlReceipt.AppendLine("</style></head>");
                    htmlReceipt.AppendLine("<body>");
                    htmlReceipt.AppendLine("<h2 style='text-align: center;'>LTO RENEWAL REPORT</h2>");
                    htmlReceipt.AppendLine("<table>");

                    // Build table headers dynamically based on visible columns
                    htmlReceipt.AppendLine("<tr>");
                    foreach (DataGridViewColumn column in dataLTOrenew.Columns)
                    {
                        if (column.Visible) // Only add visible columns to the header
                        {
                            htmlReceipt.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
                        }
                    }
                    htmlReceipt.AppendLine("</tr>");

                    // Loop through the rows of the DataGridView and extract data for visible columns
                    foreach (var row in rows)
                    {
                        if (row.Cells[0].Value != null) // Ensure the row is not empty
                        {
                            htmlReceipt.AppendLine("<tr>");
                            foreach (DataGridViewColumn column in dataLTOrenew.Columns)
                            {
                                if (column.Visible) // Only add cells for visible columns
                                {
                                    var cellValue = row.Cells[column.Name].Value?.ToString() ?? "N/A";
                                    if (column.Name == "Duedate" || column.Name == "ORdate")
                                    {
                                        // Format date columns
                                        cellValue = row.Cells[column.Name].Value != null
                                                    ? Convert.ToDateTime(row.Cells[column.Name].Value).ToString("yyyy-MM-dd")
                                                    : "N/A";
                                    }
                                    htmlReceipt.AppendLine($"<td>{HttpUtility.HtmlEncode(cellValue)}</td>");
                                }
                            }
                            htmlReceipt.AppendLine("</tr>");
                        }
                    }

                    // Close table and HTML tags
                    htmlReceipt.AppendLine("</table>");
                    htmlReceipt.AppendLine("</body>");
                    htmlReceipt.AppendLine("</html>");

                    htmlContents.Add(htmlReceipt.ToString()); // Add the generated HTML content to the list
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Now passing the List<string> of HTML contents
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterLtoData()
        {
            try
            {
                dataLTOrenew.Rows.Clear(); // Clear existing rows in the DataGridView

                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                // Get the selected dates from the DateTimePickers
                DateTime fromDate = dateFromLto.Value.Date;
                DateTime toDate = dateToLto.Value.Date.AddDays(1).AddSeconds(-1); // Include the full end date

                // SQL query to filter data
                string query = "SELECT LTO_ID, PlateNo, VehicleType, Color, MVUCrate, Duedate, Regname, ORnum, ORdate " +
                               "FROM ltorenewal " +
                               "WHERE Duedate BETWEEN @dateFrom AND @dateTo";

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@dateFrom", fromDate);
                    command.Parameters.AddWithValue("@dateTo", toDate);

                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            dataLTOrenew.Rows.Add(
                                rdr.GetInt32(0),  // LTO_ID
                                rdr.GetString(1), // PlateNo
                                rdr.GetString(2), // VehicleType
                                rdr.GetString(3), // Color
                                rdr.GetString(4),
                                rdr.IsDBNull(5) ? (object)DBNull.Value : rdr.GetDateTime(5).ToString("yyyy-MM-dd"), // Duedate
                                rdr.GetString(6), // Regname
                                rdr.GetString(7), // ORnum
                                rdr.IsDBNull(8) ? (object)DBNull.Value : rdr.GetDateTime(8).ToString("yyyy-MM-dd") // ORdate
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private void dateFromLTFRB_ValueChanged(object sender, EventArgs e)
        {
            renewalLTFRB_filter();
        }

        private void dateToLTFRB_ValueChanged(object sender, EventArgs e)
        {
            renewalLTFRB_filter();
        }

        private void btnPrintLTFRB_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in dataLTFRB
            if (dataLTFRB.Rows.Count == 0)
            {
                MessageBox.Show("No data available for the LTFRB report.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = dataLTFRB.Rows.Count;
                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through data in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = dataLTFRB.Rows.Cast<DataGridViewRow>()
                                             .Skip(i)
                                             .Take(batchSize);

                    StringBuilder htmlReceipt = new StringBuilder();

                    htmlReceipt.AppendLine("<html>");
                    htmlReceipt.AppendLine("<head><style>");
                    htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
                    htmlReceipt.AppendLine("</style></head>");
                    htmlReceipt.AppendLine("<body>");
                    htmlReceipt.AppendLine("<h2 style='text-align: center;'>LTFRB RENEWAL REPORT</h2>");
                    htmlReceipt.AppendLine("<table>");

                    // Build table headers dynamically based on visible columns
                    htmlReceipt.AppendLine("<tr>");
                    foreach (DataGridViewColumn column in dataLTFRB.Columns)
                    {
                        if (column.Visible) // Only add visible columns to the header
                        {
                            htmlReceipt.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
                        }
                    }
                    htmlReceipt.AppendLine("</tr>");

                    // Loop through the rows of the DataGridView and extract data for visible columns
                    foreach (var row in rows)
                    {
                        if (row.Cells[0].Value != null) // Ensure the row is not empty
                        {
                            htmlReceipt.AppendLine("<tr>");
                            foreach (DataGridViewColumn column in dataLTFRB.Columns)
                            {
                                if (column.Visible) // Only add cells for visible columns
                                {
                                    var cellValue = row.Cells[column.Name].Value?.ToString() ?? "";
                                    htmlReceipt.AppendLine($"<td>{HttpUtility.HtmlEncode(cellValue)}</td>");
                                }
                            }
                            htmlReceipt.AppendLine("</tr>");
                        }
                    }

                    // Close table and HTML tags
                    htmlReceipt.AppendLine("</table>");
                    htmlReceipt.AppendLine("</body>");
                    htmlReceipt.AppendLine("</html>");

                    htmlContents.Add(htmlReceipt.ToString()); // Add the generated HTML content to the list
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Now passing the List<string> of HTML contents
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void renewalLTFRB_filter()
        {
            try
            {
                dataLTFRB.Rows.Clear(); // Clear existing rows in the DataGridView

                // Open the database connection if it's not open
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                // Get the selected dates from the DateTimePickers
                DateTime fromDate = dateFromLTFRB.Value.Date;
                DateTime toDate = dateToLTFRB.Value.Date.AddDays(1).AddSeconds(-1); // Include the full end date

                // SQL query to filter data based on ExpiryDate
                string query = "SELECT ID, CaseNum, DecisionDate, PlateNum, ExpiryDate " +
                               "FROM ltfrbrenewal " +
                               "WHERE ExpiryDate BETWEEN @dateFrom AND @dateTo";

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    // Adding parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@dateFrom", fromDate);
                    command.Parameters.AddWithValue("@dateTo", toDate);

                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // Add rows to the DataGridView, ensuring ExpiryDate is filtered within the date range
                            dataLTFRB.Rows.Add(
                                rdr.GetInt32(0),  // ID
                                rdr.GetString(1), // CaseNum
                                rdr.IsDBNull(2) ? (object)DBNull.Value : rdr.GetDateTime(2).ToString("yyyy-MM-dd"), // DecisionDate
                                rdr.GetString(3), // PlateNum
                                rdr.IsDBNull(4) ? (object)DBNull.Value : rdr.GetDateTime(4).ToString("yyyy-MM-dd") // ExpiryDate
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the connection is closed
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private void dateFromINSURANCE_ValueChanged(object sender, EventArgs e)
        {
            renewalInsurance_filter();
        }

        private void dateToINSURANCE_ValueChanged(object sender, EventArgs e)
        {
            renewalInsurance_filter();
        }

        private void btnPrintInsurance_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in dataInsurance
            if (dataInsurance.Rows.Count == 0)
            {
                MessageBox.Show("No data available to print.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = dataInsurance.Rows.Count;
                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through data in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = dataInsurance.Rows.Cast<DataGridViewRow>()
                                                 .Skip(i)
                                                 .Take(batchSize);

                    StringBuilder htmlReceipt = new StringBuilder();

                    htmlReceipt.AppendLine("<html>");
                    htmlReceipt.AppendLine("<head><style>");
                    htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
                    htmlReceipt.AppendLine("</style></head>");
                    htmlReceipt.AppendLine("<body>");
                    htmlReceipt.AppendLine("<h2 style='text-align: center;'>INSURANCE RENEWAL REPORT</h2>");
                    htmlReceipt.AppendLine("<table>");

                    // Build table headers dynamically based on visible columns
                    htmlReceipt.AppendLine("<tr>");
                    foreach (DataGridViewColumn column in dataInsurance.Columns)
                    {
                        if (column.Visible) // Only add visible columns to the header
                        {
                            htmlReceipt.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
                        }
                    }
                    htmlReceipt.AppendLine("</tr>");

                    // Loop through the rows of the DataGridView and extract data for visible columns
                    foreach (var row in rows)
                    {
                        if (row.Cells[0].Value != null) // Ensure the row is not empty
                        {
                            htmlReceipt.AppendLine("<tr>");
                            foreach (DataGridViewColumn column in dataInsurance.Columns)
                            {
                                if (column.Visible) // Only add cells for visible columns
                                {
                                    var cellValue = row.Cells[column.Name].Value?.ToString() ?? "";

                                    // Handle nullable DateTime columns (same as LTFRB)
                                    if (column.Name == "from_" || column.Name == "to_" || column.Name == "checkDate" || column.Name == "orDatee")
                                    {
                                        cellValue = (row.Cells[column.Name].Value == DBNull.Value || row.Cells[column.Name].Value == null)
                                            ? "N/A"
                                            : Convert.ToDateTime(row.Cells[column.Name].Value).ToString("yyyy-MM-dd");
                                    }

                                    htmlReceipt.AppendLine($"<td>{HttpUtility.HtmlEncode(cellValue)}</td>");
                                }
                            }
                            htmlReceipt.AppendLine("</tr>");
                        }
                    }

                    // Close table and HTML tags
                    htmlReceipt.AppendLine("</table>");
                    htmlReceipt.AppendLine("</body>");
                    htmlReceipt.AppendLine("</html>");

                    // Add the generated HTML content to the list for the current batch
                    htmlContents.Add(htmlReceipt.ToString());
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Now passing the List<string> of HTML contents
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void renewalInsurance_filter()
        {
            dataInsurance.Rows.Clear(); // Clear existing rows

            try
            {
                // Open the database connection
                con.Open();

                // Get the selected dates from the DateTimePickers
                DateTime fromDate = dateFromINSURANCE.Value.Date;
                DateTime toDate = dateToINSURANCE.Value.Date.AddDays(1).AddSeconds(-1); // Include the full end date

                // SQL query to fetch data, filtering based on `to_` date
                string query = "SELECT ID, plateNo, vehicleUnit, insuranceType, policyNo, from_, to_, cvNo, checkNo, checkDate, orNo, orDate " +
                               "FROM insurancerenewal " +
                               "WHERE to_ BETWEEN @dateFrom AND @dateTo";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    // Adding parameters for date range
                    cmd.Parameters.AddWithValue("@dateFrom", fromDate);
                    cmd.Parameters.AddWithValue("@dateTo", toDate);

                    // Execute the query and load data
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            dataInsurance.Rows.Add(
                                rdr.GetInt32(0),  // ID (column 0)
                                rdr.GetString(1), // plateNo (column 1)
                                rdr.GetString(2), // vehicleUnit (column 2)
                                rdr.GetString(3), // insuranceType (column 3)
                                rdr.GetString(4), // policyNo (column 4)
                                rdr.IsDBNull(5) ? (object)DBNull.Value : rdr.GetDateTime(5).ToString("yyyy-MM-dd"), // from_ (column 5)
                                rdr.IsDBNull(6) ? (object)DBNull.Value : rdr.GetDateTime(6).ToString("yyyy-MM-dd"), // to_ (column 6)
                                rdr.GetString(7), // cvNo (column 7)
                                rdr.GetString(8), // checkNo (column 8)
                                rdr.IsDBNull(9) ? (object)DBNull.Value : rdr.GetDateTime(9).ToString("yyyy-MM-dd"), // checkDate (column 9)
                                rdr.GetString(10), // orNo (column 10)
                                rdr.IsDBNull(11) ? (object)DBNull.Value : rdr.GetDateTime(11).ToString("yyyy-MM-dd") // orDate (column 11)
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the connection is closed
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private void dateDelivery_ValueChanged(object sender, EventArgs e)
        {
            filterdelivery();
        }

        private void txtDriverName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataByDriver();
        }

        private void btnPrintDelivery_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in deliveryData
            if (deliveryData.Rows.Count == 0)
            {
                MessageBox.Show("No data available to print.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = deliveryData.Rows.Count;
                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through data in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = deliveryData.Rows.Cast<DataGridViewRow>()
                                                 .Skip(i)
                                                 .Take(batchSize)
                                                 .Where(row => row.Visible); // Filter only visible rows

                    StringBuilder htmlReceipt = new StringBuilder();

                    htmlReceipt.AppendLine("<html>");
                    htmlReceipt.AppendLine("<head><style>");
                    htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
                    htmlReceipt.AppendLine("</style></head>");
                    htmlReceipt.AppendLine("<body>");
                    htmlReceipt.AppendLine("<h2 style='text-align: center;'>DELIVERY REPORT</h2>");
                    htmlReceipt.AppendLine("<table>");

                    // Build table headers dynamically based on visible columns
                    htmlReceipt.AppendLine("<tr>");
                    foreach (DataGridViewColumn column in deliveryData.Columns)
                    {
                        if (column.Visible) // Only add visible columns to the header
                        {
                            htmlReceipt.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
                        }
                    }
                    htmlReceipt.AppendLine("</tr>");

                    // Loop through the rows of the DataGridView and extract data for visible columns
                    foreach (var row in rows)
                    {
                        if (row.Cells[0].Value != null) // Ensure the row is not empty
                        {
                            htmlReceipt.AppendLine("<tr>");
                            foreach (DataGridViewColumn column in deliveryData.Columns)
                            {
                                if (column.Visible) // Only add cells for visible columns
                                {
                                    var cellValue = row.Cells[column.Name].Value?.ToString() ?? "";

                                    // Handle nullable DateTime columns
                                    if (column.Name == "LoadingDate" || column.Name == "ArrivalDate")
                                    {
                                        cellValue = (row.Cells[column.Name].Value == DBNull.Value || row.Cells[column.Name].Value == null)
                                            ? "N/A"
                                            : Convert.ToDateTime(row.Cells[column.Name].Value).ToString("yyyy-MM-dd");
                                    }

                                    htmlReceipt.AppendLine($"<td>{HttpUtility.HtmlEncode(cellValue)}</td>");
                                }
                            }
                            htmlReceipt.AppendLine("</tr>");
                        }
                    }

                    // Close table and HTML tags
                    htmlReceipt.AppendLine("</table>");
                    htmlReceipt.AppendLine("</body>");
                    htmlReceipt.AppendLine("</html>");

                    // Add the generated HTML content to the list for the current batch
                    htmlContents.Add(htmlReceipt.ToString());
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Now passing the List<string> of HTML contents
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterDataByDriver()
        {
            // Get the selected driver name from the ComboBox
            string selectedDriver = txtDriverName.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedDriver))
            {
                // If no driver is selected, show all rows
                foreach (DataGridViewRow row in deliveryData.Rows)
                {
                    row.Visible = true;
                }
            }
            else
            {
                // Loop through all rows in the DataGridView and filter based on driver name
                foreach (DataGridViewRow row in deliveryData.Rows)
                {
                    // Check if the driver column matches the selected driver
                    if (row.Cells["Driver"].Value?.ToString() == selectedDriver)
                    {
                        row.Visible = true; // Show the row if the driver matches
                    }
                    else
                    {
                        row.Visible = false; // Hide the row if the driver doesn't match
                    }
                }
            }
        }
        private void filterdelivery()
        {
            deliveryData.Rows.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    // Get the selected month and year from the DateTimePicker
                    DateTime selectedDate = dateDelivery.Value;
                    int selectedMonth = selectedDate.Month;
                    int selectedYear = selectedDate.Year;

                    // Query to filter data based on the ArrivalDate's month and year
                    string query = "SELECT LiquidationNO, PlateNo, LoadingDate, Reference, ArrivalDate, Driver " +
                                   "FROM deliverydata " +
                                   "WHERE MONTH(ArrivalDate) = @month AND YEAR(ArrivalDate) = @year;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@month", selectedMonth);
                        cmd.Parameters.AddWithValue("@year", selectedYear);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                deliveryData.Rows.Add(
                                    rdr.GetInt32(0), // LiquidationNO
                                    rdr.GetString(1), // PlateNo
                                    rdr.IsDBNull(2) ? (object)DBNull.Value : rdr.GetDateTime(2).ToString("yyyy-MM-dd"), // LoadingDate
                                    rdr.GetString(3), // Reference
                                    rdr.IsDBNull(4) ? (object)DBNull.Value : rdr.GetDateTime(4).ToString("yyyy-MM-dd"), // ArrivalDate
                                    rdr.GetString(5) // Driver
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbmStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterWaybillData();
        }

        private void btnPrintWaybill_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in waybilldata
            if (waybilldata.Rows.Count == 0)
            {
                MessageBox.Show("No data available to print.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = waybilldata.Rows.Count;
                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through data in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = waybilldata.Rows.Cast<DataGridViewRow>()
                                               .Skip(i)
                                               .Take(batchSize)
                                               .Where(row => row.Visible); // Filter only visible rows

                    StringBuilder htmlReceipt = new StringBuilder();
                    htmlReceipt.AppendLine("<html>");
                    htmlReceipt.AppendLine("<head><style>");
                    htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
                    htmlReceipt.AppendLine("</style></head>");
                    htmlReceipt.AppendLine("<body>");
                    htmlReceipt.AppendLine("<h2 style='text-align: center;'>Waybill Report</h2>");
                    htmlReceipt.AppendLine("<table><thead><tr>");

                    // Add table headers dynamically based on visible columns
                    foreach (DataGridViewColumn column in waybilldata.Columns)
                    {
                        if (column.Visible) // Only print visible columns
                        {
                            htmlReceipt.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
                        }
                    }
                    htmlReceipt.AppendLine("</tr></thead><tbody>");

                    // Loop through the visible rows of the DataGridView and add data to the HTML table
                    foreach (var row in rows)
                    {
                        htmlReceipt.AppendLine("<tr>");
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.OwningColumn.Visible) // Only print visible cells
                            {
                                string cellValue = cell.Value?.ToString() ?? "";

                                // Handle nullable DateTime columns (if applicable)
                                if (cell.OwningColumn.Name == "LoadingDate" || cell.OwningColumn.Name == "ArrivalDate")
                                {
                                    cellValue = (cell.Value == DBNull.Value || cell.Value == null)
                                        ? "N/A"
                                        : Convert.ToDateTime(cell.Value).ToString("yyyy-MM-dd");
                                }

                                htmlReceipt.AppendLine($"<td>{HttpUtility.HtmlEncode(cellValue)}</td>");
                            }
                        }
                        htmlReceipt.AppendLine("</tr>");
                    }

                    htmlReceipt.AppendLine("</tbody></table></body></html>");
                    htmlContents.Add(htmlReceipt.ToString()); // Add the generated HTML content to the list
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Now passing the List<string> of HTML contents
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterWaybillData()
        {
            string selectedStatus = cbmStatus.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedStatus) || selectedStatus == "ALL")
            {
                // If "ALL" is selected, show all rows
                foreach (DataGridViewRow row in waybilldata.Rows)
                {
                    row.Visible = true;
                }
            }
            else
            {
                // Filter rows based on the selected status
                foreach (DataGridViewRow row in waybilldata.Rows)
                {
                    string status = row.Cells["Statuss"].Value?.ToString();
                    row.Visible = status == selectedStatus;
                }
            }
        }

        private void checkModofPay_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterPayrollInfoFromDatabase();
        }

        private void btnPrintPayroll_Click(object sender, EventArgs e)
        {
            if (dataPayrollInfo.Rows.Count == 0)
            {
                MessageBox.Show("No data available for the payroll report.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prepare the HTML structure for the payroll report
            StringBuilder htmlPayrollReport = new StringBuilder();
            htmlPayrollReport.AppendLine("<html><head><style>");
            htmlPayrollReport.AppendLine("table { border-collapse: collapse; width: 100%; }");
            htmlPayrollReport.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            htmlPayrollReport.AppendLine("th { background-color: #f2f2f2; }</style></head>");
            htmlPayrollReport.AppendLine("<body><h2 style='text-align: center;'>Payroll Report</h2>");
            htmlPayrollReport.AppendLine("<table><tr>");

            // Add table headers dynamically
            foreach (DataGridViewColumn column in dataPayrollInfo.Columns)
            {
                htmlPayrollReport.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
            }
            htmlPayrollReport.AppendLine("</tr>");

            // Loop through each row in the DataGridView and add the data to the HTML table
            foreach (DataGridViewRow row in dataPayrollInfo.Rows)
            {
                if (row.Cells[0].Value != null) // Ensure the row has data
                {
                    htmlPayrollReport.AppendLine("<tr>");
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        // Safely handle null values in the cells
                        htmlPayrollReport.AppendLine($"<td>{HttpUtility.HtmlEncode(cell.Value?.ToString() ?? "")}</td>");
                    }
                    htmlPayrollReport.AppendLine("</tr>");
                }
            }

            // Close the HTML tags
            htmlPayrollReport.AppendLine("</table></body></html>");

            // Convert the HTML content into a List<string> for GeneratePdf
            List<string> htmlContents = new List<string> { htmlPayrollReport.ToString() };

            // Call the GeneratePdf function
            GeneratePdf(htmlContents);

           
        }
        private void FilterPayrollInfoFromDatabase()
        {
            string selectedMode = checkModofPay.SelectedItem?.ToString().Trim();
            dataPayrollInfo.Rows.Clear();

            string query = "SELECT EmployeeID, empFname, empLname, modeOfPay, grossPay, netPay FROM empprofiling";

            // Apply filter only if not "ALL"
            if (!string.IsNullOrEmpty(selectedMode) && selectedMode != "ALL")
            {
                query += " WHERE TRIM(modeOfPay) = @modeOfPay";
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        if (!string.IsNullOrEmpty(selectedMode) && selectedMode != "ALL")
                        {
                            cmd.Parameters.AddWithValue("@modeOfPay", selectedMode);
                        }

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                // Safely add all rows to the DataGridView
                                dataPayrollInfo.Rows.Add(
                                    rdr.GetInt32(0),  // EmployeeID
                                    rdr.GetString(1), // empFname
                                    rdr.GetString(2), // empLname
                                    rdr.GetString(3), // modeOfPay
                                    rdr.GetDecimal(4), // grossPay
                                    rdr.GetDecimal(5)  // netPay
                                );
                            }
                        }
                    }
                }

                // Force the DataGridView to refresh
                dataPayrollInfo.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            truck_data();
        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            truck_data();
        }

        private void truck_data()
        {
            data_trucks.Rows.Clear();

            try
            {
                con.Open();

                // Base query to select all truck data
                string query = "SELECT id, plateNum, yearModel, category, Color, RegName, MVUCRate, VehicleType, status, cbm FROM truck WHERE 1=1";

                // Apply category filter if selected
                if (cbCategory.SelectedItem != null && cbCategory.SelectedItem.ToString() != "All Categories")
                {
                    query += " AND category = @Category";
                }

                // Apply status filter if selected
                if (cbStatus.SelectedItem != null && cbStatus.SelectedItem.ToString() != "All Status")
                {
                    query += " AND status = @Status";
                }

                cmd = new MySqlCommand(query, con);

                // Add parameters for the filters
                if (cbCategory.SelectedItem != null && cbCategory.SelectedItem.ToString() != "All Categories")
                {
                    cmd.Parameters.AddWithValue("@Category", cbCategory.SelectedItem.ToString());
                }

                if (cbStatus.SelectedItem != null && cbStatus.SelectedItem.ToString() != "All Status")
                {
                    cmd.Parameters.AddWithValue("@Status", cbStatus.SelectedItem.ToString());
                }

                rdr = cmd.ExecuteReader();

                // Populate the DataGridView
                while (rdr.Read())
                {
                    var row = new object[]
                    {
                rdr.GetInt32(0),    // id
                rdr.GetString(1),   // plateNum
                rdr.GetString(2),    // yearModel
                rdr.GetString(3),   // category
                rdr.GetString(4),   // color
                rdr.GetString(5),   // regName
                rdr.GetString(6),  // MVUCRate
                rdr.GetString(7),   // vehicleType
                rdr.GetString(8),   // status
                rdr.GetDecimal(9)   // cbm
                    };

                    data_trucks.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving truck data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void dateFrom2_ValueChanged(object sender, EventArgs e)
        {
            FilterFuelByDateRange();
        }

        private void dateTo2_ValueChanged(object sender, EventArgs e)
        {
            FilterFuelByDateRange();
        }

        private void cbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFuelData();

        }

        private void checkBoxPrice_CheckedChanged(object sender, EventArgs e)
        {
            FilterFuelByDateRange();
        }
        private void FilterFuelByDateRange()
        {
            if (dateFrom2.Value > dateTo2.Value)
            {
                MessageBox.Show("The 'From' date must be earlier than the 'To' date.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Clearing old data before repopulating
            data_Fuel.Rows.Clear();
            decimal totalAmount = 0;
            decimal totalPrice = 0;

            // Ensure the price column is added if the checkbox is checked
            if (checkBoxPrice.Checked && !data_Fuel.Columns.Contains("Total Price"))
            {
                data_Fuel.Columns.Add("Total Price", "Total Price");
            }
            else if (!checkBoxPrice.Checked && data_Fuel.Columns.Contains("Total Price"))
            {
                data_Fuel.Columns.Remove("Total Price");
            }

            try
            {
                con.Open();
                // Adjusted query to filter by both date and department
                string query = "SELECT ID, fuel_date, plateNum, department, qty, totalFuelAmount FROM fuelmonitoring WHERE fuel_date BETWEEN @FromDate AND @ToDate";

                // Add department filter if necessary
                if (cbSortBy.SelectedItem != null && cbSortBy.SelectedItem.ToString() != "All Departments")
                {
                    query += " AND department = @Department";
                }

                cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FromDate", dateFrom2.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@ToDate", dateTo2.Value.ToString("yyyy-MM-dd"));

                // Add department filter if necessary
                if (cbSortBy.SelectedItem != null && cbSortBy.SelectedItem.ToString() != "All Departments")
                {
                    cmd.Parameters.AddWithValue("@Department", cbSortBy.SelectedItem.ToString());
                }

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var row = new object[] {
                rdr.GetInt32(0),
                rdr.IsDBNull(1) ? (object)DBNull.Value : rdr.GetDateTime(1).ToString("yyyy-MM-dd"),
                rdr.GetString(2),
                rdr.GetString(3),
                rdr.GetDecimal(4),
                rdr.GetDecimal(5)
            };

                    totalAmount += rdr.GetDecimal(4);  // Add total fuel amount

                    // Add the total price if checkbox is checked
                    if (checkBoxPrice.Checked)
                    {
                        var fuelPrice = rdr.GetDecimal(5);
                        row = row.Concat(new object[] { fuelPrice }).ToArray();
                        totalPrice += fuelPrice;
                    }

                    // Add the row to the data grid
                    data_Fuel.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering fuel data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

            // Update the total fuel cost and total price textboxes
            txtTotalFuelCost.Text = totalAmount.ToString("N2");
            if (checkBoxPrice.Checked)
            {
                txtTotalPrice.Text = $"₱{totalPrice:N2}";
            }
        }

        private void btnReciept_Click(object sender, EventArgs e)
        {
            if (data_Fuel.Rows.Count == 0)
            {
                MessageBox.Show("No data available for the fuel report.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prepare the HTML structure for the fuel report
            StringBuilder htmlFuelReport = new StringBuilder();
            htmlFuelReport.AppendLine("<html><head><style>");
            htmlFuelReport.AppendLine("table { border-collapse: collapse; width: 100%; }");
            htmlFuelReport.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            htmlFuelReport.AppendLine("th { background-color: #f2f2f2; }</style></head>");
            htmlFuelReport.AppendLine("<body><h2 style='text-align: center;'>Fuel Monitoring Report</h2>");
            htmlFuelReport.AppendLine("<table><tr>");

            // Add table headers dynamically
            foreach (DataGridViewColumn column in data_Fuel.Columns)
            {
                htmlFuelReport.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
            }
            htmlFuelReport.AppendLine("</tr>");

            // Loop through each row in the DataGridView and add the data to the HTML table
            foreach (DataGridViewRow row in data_Fuel.Rows)
            {
                if (row.Cells[0].Value != null) // Ensure the row has data
                {
                    htmlFuelReport.AppendLine("<tr>");
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        // Safely handle null values in the cells
                        htmlFuelReport.AppendLine($"<td>{HttpUtility.HtmlEncode(cell.Value?.ToString() ?? "")}</td>");
                    }
                    htmlFuelReport.AppendLine("</tr>");
                }
            }

            // Close the HTML tags
            htmlFuelReport.AppendLine("</table></body></html>");

            // Convert the HTML content into a List<string> for GeneratePdf
            List<string> htmlContents = new List<string> { htmlFuelReport.ToString() };

            // Call the GeneratePdf function
            GeneratePdf(htmlContents);

            
        }

        private void btnPrintTruck_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in data_trucks
            if (data_trucks.Rows.Count == 0)
            {
                MessageBox.Show("No data available for the truck report.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int batchSize = 50; // Number of rows per batch
                int totalRows = data_trucks.Rows.Count;

                var htmlContents = new List<string>(); // List to hold HTML content for each batch

                // Iterate through data in batches
                for (int i = 0; i < totalRows; i += batchSize)
                {
                    var rows = data_trucks.Rows.Cast<DataGridViewRow>()
                                                .Skip(i)
                                                .Take(batchSize)
                                                .Where(row => row.Visible); // Filter only visible rows

                    StringBuilder htmlTruckReport = new StringBuilder();

                    htmlTruckReport.AppendLine("<html>");
                    htmlTruckReport.AppendLine("<head><style>");
                    htmlTruckReport.AppendLine("table { border-collapse: collapse; width: 100%; }");
                    htmlTruckReport.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
                    htmlTruckReport.AppendLine("th { background-color: #f2f2f2; }</style></head>");
                    htmlTruckReport.AppendLine("<body>");
                    htmlTruckReport.AppendLine("<h2 style='text-align: center;'>Truck Report</h2>");
                    htmlTruckReport.AppendLine("<table><thead><tr>");

                    // Add table headers dynamically based on visible columns
                    foreach (DataGridViewColumn column in data_trucks.Columns)
                    {
                        if (column.Visible)  // Only print visible columns
                        {
                            htmlTruckReport.AppendLine($"<th>{HttpUtility.HtmlEncode(column.HeaderText)}</th>");
                        }
                    }
                    htmlTruckReport.AppendLine("</tr></thead><tbody>");

                    // Loop through the visible rows of the DataGridView and add data to the HTML table
                    foreach (var row in rows)
                    {
                        htmlTruckReport.AppendLine("<tr>");
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.OwningColumn.Visible)  // Only print visible cells
                            {
                                string cellValue = cell.Value?.ToString() ?? "";

                                // Handle nullable DateTime columns (if applicable)
                                if (cell.OwningColumn.Name == "LoadingDate" || cell.OwningColumn.Name == "ArrivalDate")
                                {
                                    cellValue = (cell.Value == DBNull.Value || cell.Value == null)
                                        ? "N/A"
                                        : Convert.ToDateTime(cell.Value).ToString("yyyy-MM-dd");
                                }

                                htmlTruckReport.AppendLine($"<td>{HttpUtility.HtmlEncode(cellValue)}</td>");
                            }
                        }
                        htmlTruckReport.AppendLine("</tr>");
                    }

                    htmlTruckReport.AppendLine("</tbody></table></body></html>");

                    // Add the generated HTML content to the list for the current batch
                    htmlContents.Add(htmlTruckReport.ToString());
                }

                // Pass the batched HTML contents to GeneratePdf
                GeneratePdf(htmlContents); // Now passing the List<string> of HTML contents
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
