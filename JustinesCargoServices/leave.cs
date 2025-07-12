using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    public partial class leave : Form
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

        public leave()
        {
            InitializeComponent();
        }

        // Load
        private void leave_Load(object sender, EventArgs e)
        {
            UpdateDriverNamesBasedOnLeave();
            dt_leaveList.CellValueChanged += dt_leaveList_CellValueChanged;
            dt_leaveList.CurrentCellDirtyStateChanged += dt_leaveList_CurrentCellDirtyStateChanged; // To detect immediate change in cell
            SetupDataGridView();
            loadLeaveList();
            cbFilterByStatus.Items.Add("All");
            cbFilterByStatus.Items.Add("Approved");
            cbFilterByStatus.Items.Add("Rejected");
            LoadDataCounts();
            cbFilterByStatus.Items.Add("Pending Approval");
            cbFilterByStatus.SelectedIndex = 0; // Default to 'All' or first item
        }

        // Load the leave list
        public void loadLeaveList()
        {
            try
            {
                dt_leaveList.Rows.Clear();
                con.Open();

                string filterCondition = cbFilterByStatus.SelectedItem?.ToString() ?? "All";
                string query;

                if (filterCondition == "All")
                {
                    query = "SELECT leaveID, empID, lname, fname, mname, position, leaveType, fromdate, todate, empLeavePay, leaveStatus FROM empleave;";
                }
                else
                {
                    query = "SELECT leaveID, empID, lname, fname, mname, position, leaveType, fromdate, todate, empLeavePay, leaveStatus " +
                            "FROM empleave WHERE leaveStatus = @status;";
                }

                cmd = new MySqlCommand(query, con);

                if (filterCondition != "All")
                {
                    cmd.Parameters.AddWithValue("@status", filterCondition);
                }

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    dt_leaveList.Rows.Add(
                        rdr.GetInt32(0), // leaveID
                        rdr.GetInt32(1), // empID
                        rdr.GetString(2), // lname
                        rdr.GetString(3), // fname
                        rdr.GetString(4), // mname
                        rdr.GetString(5), // position
                        rdr.GetString(6), // leaveType
                        rdr.GetDateTime(7), // fromdate
                        rdr.GetDateTime(8), // todate
                        rdr.GetDecimal(9), // empLeavePay
                        rdr.GetString(10)  // leaveStatus
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

        // Event handler for status selection change
        private void cbFilterByStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadLeaveList();
        }

        // Refresh button click
        private void refreshBtn_Click(object sender, EventArgs e)
        {
            loadLeaveList();
        }

        // Search button click
        private void searchBtn_Click(object sender, EventArgs e)
        {
            SearchLeave();
        }

        private void UpdateDriverNamesBasedOnLeave()
        {
            try
            {
                con.Open();

                string query = @"
            UPDATE delivery d
            JOIN empleave e
            ON d.DriversName = CONCAT(e.lname, ', ', e.fname)
            SET d.DriversName = ''
            WHERE e.leaveType IS NOT NULL AND e.leaveType != '';";

                MySqlCommand cmd = new MySqlCommand(query, con);
                int rowsAffected = cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating driver names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }


        // Function to perform the search
        public void SearchLeave()
        {
            try
            {
                string searchKeyword = txtSearch.Text.Trim();
                dt_leaveList.Rows.Clear();
                con.Open();

                string query = "SELECT leaveID, empID, lname, fname, mname, position, leaveType, fromdate, todate, empLeavePay, leaveStatus FROM empleave";

                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    query += " WHERE lname LIKE @search OR fname LIKE @search OR mname LIKE @search OR leaveType LIKE @search";
                }

                cmd = new MySqlCommand(query, con);

                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchKeyword + "%");
                }

                rdr = cmd.ExecuteReader();

                bool dataFound = false;

                while (rdr.Read())
                {
                    dt_leaveList.Rows.Add(
                        rdr.GetInt32(0),
                        rdr.GetInt32(1),
                        rdr.GetString(2),
                        rdr.GetString(3),
                        rdr.GetString(4),
                        rdr.GetString(5),
                        rdr.GetString(6),
                        rdr.GetDateTime(7),
                        rdr.GetDateTime(8),
                        rdr.GetDecimal(9),
                        rdr.GetString(10)
                    );
                    dataFound = true;
                }

                if (!dataFound)
                {
                    MessageBox.Show("No data found for the search name: " + searchKeyword, "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching leave data: " + ex.Message);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        // Function to load data counts (approved, rejected, pending, total)
        public void LoadDataCounts()
        {
            try
            {
                con.Open();

                string leaveTypeQuery = "SELECT COUNT(DISTINCT leaveType) FROM empleave;";
                cmd = new MySqlCommand(leaveTypeQuery, con);
                lblLeaveType.Text = " " + cmd.ExecuteScalar().ToString();

                string employeeQuery = "SELECT COUNT(DISTINCT empID) FROM empleave;";
                cmd = new MySqlCommand(employeeQuery, con);
                lblEmployee.Text = " " + cmd.ExecuteScalar().ToString();

                string approveQuery = "SELECT COUNT(*) FROM empleave WHERE leaveStatus = 'Approved';";
                cmd = new MySqlCommand(approveQuery, con);
                lblApprove.Text = " " + cmd.ExecuteScalar().ToString();

                string rejectedQuery = "SELECT COUNT(*) FROM empleave WHERE leaveStatus = 'Rejected';";
                cmd = new MySqlCommand(rejectedQuery, con);
                lblRejected.Text = " " + cmd.ExecuteScalar().ToString();

                string pendingQuery = "SELECT COUNT(*) FROM empleave WHERE leaveStatus = 'Pending Approval';";
                cmd = new MySqlCommand(pendingQuery, con);
                lblPending.Text = " " + cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data counts: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        // Setup the DataGridView
        private void SetupDataGridView()
        {
            if (dt_leaveList.Columns["status"] == null)
            {
                DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn();
                statusColumn.Name = "status";
                statusColumn.HeaderText = "STATUS";
                statusColumn.Items.AddRange("Pending Approval", "Approved", "Rejected");
                statusColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                statusColumn.ReadOnly = false;

                dt_leaveList.Columns.Add(statusColumn);
            }
            else
            {
                DataGridViewComboBoxColumn statusColumn = (DataGridViewComboBoxColumn)dt_leaveList.Columns["status"];
                statusColumn.Items.Clear();
                statusColumn.Items.AddRange("Pending Approval", "Approved", "Rejected");
            }

            dt_leaveList.ReadOnly = false;
        }

        // Cell value changed handler (for updating leave status)
        private void dt_leaveList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dt_leaveList.Columns["status"].Index)
            {
                string leaveID = dt_leaveList.Rows[e.RowIndex].Cells["SERIES"].Value.ToString();
                string newStatus = dt_leaveList.Rows[e.RowIndex].Cells["status"].Value.ToString();

                UpdateLeaveStatus(leaveID, newStatus);
                LoadDataCounts();
                loadLeaveList();
            }
        }

    
        private void dt_leaveList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dt_leaveList.IsCurrentCellDirty)
            {
                dt_leaveList.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // Update leave status in the database
        public void UpdateLeaveStatus(string leaveID, string newStatus)
        {
            try
            {
                con.Open();
                string query = "UPDATE empleave SET leaveStatus = @newStatus WHERE leaveID = @leaveID;";
                cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@newStatus", newStatus);
                cmd.Parameters.AddWithValue("@leaveID", leaveID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating leave status: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
