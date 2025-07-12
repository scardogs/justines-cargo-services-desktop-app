using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace JustinesCargoServices
{
    public partial class dashboardform : Form
    {
        MySqlConnection con = new MySqlConnection(
         "datasource=localhost;" +
         "port=3306;" +
         "database=jcsdb;" +
         "username=root;" +
         "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public dashboardform()
        {
            InitializeComponent();
           
        }
        private void dashboardform_Load(object sender, EventArgs e)
        {
            LoadYearlyOrMonthlyChart("Monthly");
            LoadRenewalDataAndUpdateButtons();
            loadLeaveList();
            UpdateRemainingFuelTextBox();
        }
        private void UpdateRemainingFuelTextBox()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd = new MySqlCommand("SELECT bulkRemainingFuel FROM bulkremainingfuel ORDER BY bulkRemainFuelID DESC LIMIT 1;", con);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    decimal remainingFuel = Convert.ToDecimal(result);
                    txtRemainingFuel.Text = $"{remainingFuel:N2} liters";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating remaining fuel: {ex.Message}", "Error");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        private void LoadYearlyOrMonthlyChart(string mode)
        {
            try
            {
                string query = "";

                if (mode == "Yearly")
                {
                    query = @"
                SELECT YEAR(InvoiceDate) AS Year, SUM(Amount) AS TotalAmount
                FROM billing
                GROUP BY YEAR(InvoiceDate)
                ORDER BY YEAR(InvoiceDate);";
                }
                else if (mode == "Monthly")
                {
                    query = @"
                SELECT YEAR(InvoiceDate) AS Year, MONTH(InvoiceDate) AS Month, SUM(Amount) AS TotalAmount
                FROM billing
                GROUP BY YEAR(InvoiceDate), MONTH(InvoiceDate)
                ORDER BY YEAR(InvoiceDate), MONTH(InvoiceDate);"
                    ;
                }

                con.Open();
                cmd = new MySqlCommand(query, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Clear existing data in the chart
                chartYearlyAmount.Series.Clear();
                chartYearlyAmount.Titles.Clear();

                // Add chart title based on mode
                chartYearlyAmount.Titles.Add(mode + " Total Amount");

                // Create the series for the chart
                Series series = new Series
                {
                    Name = "Total Amount",
                    XValueMember = (mode == "Yearly") ? "Year" : "Month",
                    YValueMembers = "TotalAmount",
                   
                    IsValueShownAsLabel = true, // This will display the value on the chart
                    BorderWidth = 5,
                    MarkerColor = Color.SaddleBrown, // Set marker color to SaddleBrown
                    MarkerStyle = MarkerStyle.Circle, // 
                    MarkerSize = 10

                };

                chartYearlyAmount.Series.Add(series);

                // Bind the data to the chart
                chartYearlyAmount.DataSource = dt;
                chartYearlyAmount.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
       

        
        private void renewreport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        private void LoadRenewalDataAndUpdateButtons()
        {
            // Clear existing rows in the DataGridView
            renewreport.Rows.Clear();

            // Get today's date
            DateTime today = DateTime.Today;

            // Flags to track if any button should turn red
            bool ltoRed = false;
            bool ltfrbRed = false;
            bool insuranceRed = false;

            // Open database connection
            con.Open();

            // Fetch and process data for LTO renewal
            string queryLTO = "SELECT PlateNo, Duedate FROM ltorenewal";
            using (MySqlCommand cmd = new MySqlCommand(queryLTO, con))
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string plateNo = rdr["PlateNo"].ToString();
                    DateTime? dueDate = rdr["Duedate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Duedate"]);

                    if (dueDate.HasValue && IsWithinRange(dueDate.Value, today, -1, 1))
                    {
                        renewreport.Rows.Add(plateNo, "LTO", dueDate.Value.ToShortDateString());
                        ltoRed = true;
                    }
                }
            }

            // Fetch and process data for LTFRB renewal
            string queryLTFRB = "SELECT PlateNum, ExpiryDate FROM ltfrbrenewal";
            using (MySqlCommand cmd = new MySqlCommand(queryLTFRB, con))
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string plateNum = rdr["PlateNum"].ToString();
                    DateTime? expiryDate = rdr["ExpiryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["ExpiryDate"]);

                    if (expiryDate.HasValue && IsWithinRange(expiryDate.Value, today, -1, 1))
                    {
                        renewreport.Rows.Add(plateNum, "LTFRB", expiryDate.Value.ToShortDateString());
                        ltfrbRed = true;
                    }
                }
            }

            // Fetch and process data for Insurance renewal
            string queryInsurance = "SELECT PlateNo, `to_` FROM insurancerenewal";
            using (MySqlCommand cmd = new MySqlCommand(queryInsurance, con))
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string plateNo = rdr["PlateNo"].ToString();
                    DateTime? toDate = rdr["to_"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["to_"]);

                    if (toDate.HasValue && IsWithinRange(toDate.Value, today, -12, 1))
                    {
                        renewreport.Rows.Add(plateNo, "Insurance", toDate.Value.ToShortDateString());
                        insuranceRed = true;
                    }
                }
            }

            // Close database connection
            con.Close();

            // Update button fill color based on the flags
            UpdateButtonFillColor(turnRedLTO, ltoRed);
            UpdateButtonFillColor(turnRedLTFRB, ltfrbRed);
            UpdateButtonFillColor(turnRedINSURANCE, insuranceRed);
        }


        // Helper function to check if a date is within a specified range
        private bool IsWithinRange(DateTime date, DateTime referenceDate, int monthsBefore, int monthsAfter)
        {
            DateTime startDate = referenceDate.AddMonths(monthsBefore);
            DateTime endDate = referenceDate.AddMonths(monthsAfter);
            return date >= startDate && date <= endDate;
        }

        // Helper function to update button fill color for Guna2Button
        private void UpdateButtonFillColor(Guna.UI2.WinForms.Guna2Button button, bool shouldTurnRed)
        {
            button.FillColor = shouldTurnRed ? Color.Red : SystemColors.Control; // Adjust fill color
            button.ForeColor = Color.White; // Optional: Makes text visible on red background
        }

        // Helper function to update button fill color
        private void UpdateButtonFillColor(Button button, bool shouldTurnRed)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = shouldTurnRed ? Color.Red : SystemColors.Control;
            button.ForeColor = Color.White; // Optional: Makes text visible on red background
        }



        private void btnYearly_Click(object sender, EventArgs e)
        {
            LoadYearlyOrMonthlyChart("Yearly");
        }

        private void btnMonthly_Click(object sender, EventArgs e)
        {
            LoadYearlyOrMonthlyChart("Monthly");
        }

        public void loadLeaveList()
        {
            try
            {
                dataempLeave.Rows.Clear();
                con.Open();
                cmd = new MySqlCommand("SELECT leaveID, empID, lname, fname, mname, position, leaveType, fromdate, todate FROM empleave;", con);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    // Combine lname, fname, mname into FullName
                    string fullName = $"{rdr.GetString(2)} {rdr.GetString(3)} {rdr.GetString(4)}".Trim();

                    // Add data to the DataGridView
                    dataempLeave.Rows.Add(
                        rdr.GetInt32(0),    // leaveID -> Column 0
                        rdr.GetInt32(1),    // empID -> Column 1
                        fullName,           // FullName -> Column 2
                        rdr.GetString(5),   // position -> Column 3
                        rdr.GetString(6),   // leaveType -> Column 4
                        rdr.GetDateTime(7), // fromdate -> Column 5
                        rdr.GetDateTime(8)  // todate -> Column 6
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading leave list data: " + ex.Message);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (con.State == ConnectionState.Open) con.Close();
            }
        }



    }
}
