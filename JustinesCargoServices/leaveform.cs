using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    public partial class leaveform : Form
    {
        MySqlConnection con = new MySqlConnection(
          "datasource=localhost;" +
          "port=3306;" +
          "database=jcsdb;" +
          "username=root;" +
          "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public leaveform()
        {
            InitializeComponent();
        }

        private void leaveform_Load(object sender, EventArgs e)
        {
            LoadEmployeeIDs();
            UpdateDriverNamesBasedOnLeave();
         
            loadLeaveList();
        }
        public void loadLeaveList()
        {
            string query = "SELECT leaveID, empID, lname, fname, mname, position, leaveType, Reason, fromdate, todate, empLeavePay, leaveStatus FROM empleave;";

            try
            {
                dt_leaveList.Rows.Clear(); // Clear existing rows in the DataGridView

                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                dt_leaveList.Rows.Add(
                                    rdr.GetInt32("leaveID"),     // leaveID
                                    rdr.GetInt32("empID"),      // empID
                                    rdr.GetString("lname"),     // lname
                                    rdr.GetString("fname"),     // fname
                                    rdr.GetString("mname"),     // mname
                                    rdr.GetString("position"),  // position
                                    rdr.GetString("leaveType"), // leaveType
                                    rdr.GetString("Reason"),    // Reason
                                    rdr.GetDateTime("fromdate"), // fromdate
                                    rdr.GetDateTime("todate"),   // todate
                                    rdr.GetDecimal("empLeavePay"), // empLeavePay
                                    rdr.GetString("leaveStatus")  // leaveStatus
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading leave list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void UpdateDriverNamesBasedOnLeave()
        {
            string query = @"
UPDATE delivery d
JOIN empleave e
ON d.DriversName = CONCAT(e.lname, ', ', e.fname)
SET d.DriversName = 'Driver On Leave'
WHERE e.leaveStatus = 'Approved' AND e.leaveType IS NOT NULL AND e.leaveType != '';";

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        // Optionally, you can inform the user of the rows affected
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating driver names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadEmployeeIDs()
        {
            string query = "SELECT EmployeeID FROM empprofiling";

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
                                cbmempID.Items.Add(rdr["EmployeeID"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Employee IDs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void cbmempID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbmempID.SelectedItem != null)
            {
                string selectedEmployeeID = cbmempID.SelectedItem.ToString();
                PopulateEmployeeDetails(selectedEmployeeID);
            }
        }
        private void PopulateEmployeeDetails(string employeeID)
        {
            string query = "SELECT empFname, empLname, empMname, position FROM empprofiling WHERE EmployeeID = @EmployeeID";

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                lblFname.Text = rdr["empFname"].ToString();
                                lblLname.Text = rdr["empLname"].ToString();
                                lblMname.Text = rdr["empMname"].ToString();
                                lblPosition.Text = rdr["position"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Employee details not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching employee details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                // Insert into empleave
                using (cmd = new MySqlCommand("INSERT INTO empleave (empID, lname, fname, mname, position, leaveType, Reason, fromdate, todate, empLeavePay, leaveStatus) " +
                    "VALUES (@empID, @lname, @fname, @mname, @position, @leaveType, @Reason, @fromDate, @toDate, @empLeavePay,@leaveStatus);", con))
                {
                    cmd.Parameters.AddWithValue("@empID", cbmempID.Text.Trim());
                    cmd.Parameters.AddWithValue("@lname", lblLname.Text.Trim());
                    cmd.Parameters.AddWithValue("@fname", lblFname.Text.Trim());
                    cmd.Parameters.AddWithValue("@mname", lblMname.Text.Trim());
                    cmd.Parameters.AddWithValue("@position", lblPosition.Text.Trim());
                    cmd.Parameters.AddWithValue("@leaveType", cbmLeaveType.Text.Trim());
                    cmd.Parameters.AddWithValue("@Reason", txtReason.Text.Trim());
                    cmd.Parameters.AddWithValue("@fromDate", dateFrom.Value.Date);
                    cmd.Parameters.AddWithValue("@toDate", dateTo.Value.Date);
                    cmd.Parameters.AddWithValue("@leaveStatus", "Pending Approval");
                    cmd.Parameters.AddWithValue("@empLeavePay", Convert.ToDecimal(txtWithPay.Text.Trim()));

                    cmd.ExecuteNonQuery();
                }

                // Insert into empprofiling
                using (cmd = new MySqlCommand("UPDATE empprofiling SET LeavePay = @LeavePay WHERE EmployeeID = @empID;", con))
                {
                    cmd.Parameters.AddWithValue("@empID", cbmempID.Text.Trim());
                    cmd.Parameters.AddWithValue("@LeavePay", Convert.ToDecimal(txtWithPay.Text.Trim()));

                    cmd.ExecuteNonQuery();
                }

             
                UpdateDriverNamesBasedOnLeave();
                
                loadLeaveList();

                MessageBox.Show("Leave Details Added!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                // Update empleave
                using (cmd = new MySqlCommand(
                    "UPDATE empleave " +
                    "SET lname = @lname, fname = @fname, mname = @mname, position = @position, " +
                    "leaveType = @leaveType, Reason = @Reason, fromdate = @fromDate, todate = @toDate, empLeavePay = @empLeavePay " +
                    "WHERE empID = @empID;", con))
                {
                    cmd.Parameters.AddWithValue("@empID", cbmempID.Text);
                    cmd.Parameters.AddWithValue("@lname", lblLname.Text);
                    cmd.Parameters.AddWithValue("@fname", lblFname.Text);
                    cmd.Parameters.AddWithValue("@mname", lblMname.Text);
                    cmd.Parameters.AddWithValue("@position", lblPosition.Text);
                    cmd.Parameters.AddWithValue("@leaveType", cbmLeaveType.Text);
                    cmd.Parameters.AddWithValue("@Reason", txtReason.Text.Trim());
                    cmd.Parameters.AddWithValue("@fromDate", dateFrom.Value.Date);
                    cmd.Parameters.AddWithValue("@toDate", dateTo.Value.Date);
                    cmd.Parameters.AddWithValue("@empLeavePay", Convert.ToDecimal(txtWithPay.Text.Trim()));

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("No matching record found for the specified Employee ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Update empprofiling
                using (cmd = new MySqlCommand("UPDATE empprofiling SET LeavePay = @LeavePay WHERE EmployeeID = @empID;", con))
                {
                    cmd.Parameters.AddWithValue("@empID", cbmempID.Text.Trim());
                    cmd.Parameters.AddWithValue("@LeavePay", Convert.ToDecimal(txtWithPay.Text.Trim()));

                    cmd.ExecuteNonQuery();
                }

                UpdateDriverNamesBasedOnLeave();
             
                loadLeaveList();

                MessageBox.Show("Leave Details Updated!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void check_CheckedChanged(object sender, EventArgs e)
        {
            if (check.Checked)
            {
                CalculateAndSetLeavePay();
            }
        }
        private void CalculateAndSetLeavePay()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cbmempID.Text))
                {
                    MessageBox.Show("Please select an employee to calculate leave pay.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                con.Open();

                // Retrieve RatePerDay for the selected employee from empprofiling
                cmd = new MySqlCommand("SELECT RatePerDay FROM empprofiling WHERE EmployeeID = @empID;", con);
                cmd.Parameters.AddWithValue("@empID", cbmempID.Text);

                object ratePerDayObj = cmd.ExecuteScalar();
                if (ratePerDayObj == null)
                {
                    MessageBox.Show("RatePerDay not found for the selected employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal ratePerDay = Convert.ToDecimal(ratePerDayObj);
                decimal leavePay = ratePerDay * 5; // Calculate LeavePay for 5 days

                // Update LeavePay in empprofiling
                cmd = new MySqlCommand("UPDATE empprofiling SET LeavePay = @leavePay WHERE EmployeeID = @empID;", con);
                cmd.Parameters.AddWithValue("@leavePay", leavePay);
                cmd.Parameters.AddWithValue("@empID", cbmempID.Text);
                cmd.ExecuteNonQuery();

                // Update empLeavePay in empleave
                cmd = new MySqlCommand("UPDATE empleave SET empLeavePay = @empLeavePay WHERE empID = @empID;", con);
                cmd.Parameters.AddWithValue("@empLeavePay", leavePay);
                cmd.Parameters.AddWithValue("@empID", cbmempID.Text);
                cmd.ExecuteNonQuery();

                // Update the UI fields
                txtWithPay.Text = leavePay.ToString("F2"); // Set the calculated value in the textbox

                MessageBox.Show("Leave Pay calculated and updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while calculating leave pay: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private void dt_leaveList_SelectionChanged(object sender, EventArgs e)
        {
            if (dt_leaveList.SelectedRows.Count > 0)
            {

                cbmempID.Text = dt_leaveList.SelectedRows[0].Cells[1].Value.ToString();
                lblFname.Text = dt_leaveList.SelectedRows[0].Cells[2].Value.ToString();
                lblLname.Text = dt_leaveList.SelectedRows[0].Cells[3].Value.ToString();
                lblMname.Text = dt_leaveList.SelectedRows[0].Cells[4].Value.ToString();
                lblPosition.Text = dt_leaveList.SelectedRows[0].Cells[5].Value.ToString();
                cbmLeaveType.Text = dt_leaveList.SelectedRows[0].Cells[6].Value.ToString();
                txtReason.Text = dt_leaveList.SelectedRows[0].Cells[7].Value.ToString();
                dateFrom.Text = dt_leaveList.SelectedRows[0].Cells[8].Value.ToString();
                dateTo.Text = dt_leaveList.SelectedRows[0].Cells[9].Value.ToString();
                txtWithPay.Text = dt_leaveList.SelectedRows[0].Cells[10].Value.ToString();
            }
        }

        private void dateFrom_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void dateTo_ValueChanged(object sender, EventArgs e)
        {
          

        }
        private void ValidateDateRange()
        {
            // Retrieve selected dates
            DateTime fromDate = dateFrom.Value.Date;
            DateTime toDate = dateTo.Value.Date;

            // Calculate the difference in days
            TimeSpan difference = toDate - fromDate;

            // Check if the selected date range exceeds 5 days
            if (difference.TotalDays > 4)
            {
                // Show warning message
                MessageBox.Show(this, "The leave you set exceeded more than 5 days.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Reset the end date to the maximum valid range
                dateTo.Value = fromDate.AddDays(4); // Limit to 5 days including the start date
            }
        }

        private void addDel_Click(object sender, EventArgs e)
        {
            pnlDelIdent.Visible = !pnlDelIdent.Visible;
        }
    }
}
