using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    public partial class renewalsLTFRB : Form
    {
        private int targetY;
        private int originalY;
        private bool isSlidingUp = false;
        private bool isExpanded = false;
        MySqlConnection con = new MySqlConnection(
        "datasource=localhost;" +
        "port=3306;" +
        "database=jcsdb;" +
        "username=root;" +
        "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public renewalsLTFRB()
        {
            InitializeComponent();
            timerSlide.Tick += timerSlide_Tick;
            timerSlide.Interval = 10; // Adjust for smoother or faster animation
            pnlLTFRB.Visible = false;   // Ensure the panel is initially hidden
            originalY = this.ClientSize.Height; // Set the starting position (below the form)
            pnlLTFRB.Top = originalY;
        }

        private void timerSlide_Tick(object sender, EventArgs e)
        {
            if (isSlidingUp)
            {
                // Slide up
                if (pnlLTFRB.Top > targetY)
                {
                    pnlLTFRB.Top -= 5; // Adjust for smoother or faster sliding
                    if (pnlLTFRB.Top <= targetY)
                    {
                        pnlLTFRB.Top = targetY;
                        timerSlide.Stop();
                        isExpanded = true;
                    }
                }
            }
            else
            {
                // Slide down
                if (pnlLTFRB.Top < originalY)
                {
                    pnlLTFRB.Top += 5; // Adjust for smoother or faster sliding
                    if (pnlLTFRB.Top >= originalY)
                    {
                        pnlLTFRB.Top = originalY;
                        pnlLTFRB.Visible = false; // Hide the panel after sliding down
                        timerSlide.Stop();
                        isExpanded = false;
                    }
                }
            }
        }

        private void btnEditss_Click(object sender, EventArgs e)
        {
            if (!isExpanded)
            {
                // Slide up and show
                pnlLTFRB.Visible = true; // Ensure the panel is visible
                targetY = (this.ClientSize.Height - pnlLTFRB.Height) / 2;
                isSlidingUp = true;
            }
            else
            {
                // Slide down and hide
                isSlidingUp = false;
            }

            timerSlide.Start();
        }

        

        private void RefreshStatus()
        {
            con.Open();
            cmd = new MySqlCommand("UPDATE ltfrbrenewal SET Update_ = " +
                                   "CASE " +
                                   "WHEN DecisionDate = CURDATE() AND ExpiryDate = CURDATE() THEN 'DEC/EX' " +
                                   "WHEN DecisionDate = CURDATE() THEN 'DECISION_D' " +
                                   "WHEN ExpiryDate = CURDATE() THEN 'EXPIRY_D' " +
                                   "ELSE 'PENDING' " +
                                   "END;", con);
            cmd.ExecuteNonQuery();

            cmd = new MySqlCommand("UPDATE ltfrbrenewal SET Remarks = REPLACE(Remarks, 'DONE', '')", con);
            cmd.ExecuteNonQuery();

            con.Close();
            MessageBox.Show("Update refresh Successfully!");
            renewalLTFRB_data();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void renewalsLTFRB_Load(object sender, EventArgs e)
        {
            renewalLTFRB_data();
            LTFRBRecords_Data();
            ClearAll();
        }
        private bool ValidateLTFRBInput()
        {

            if (string.IsNullOrWhiteSpace(txtCaseNum.Text))
            {
                MessageBox.Show("Case number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDecisionDate.Text))
            {
                MessageBox.Show("Decision Date is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPlateNum.Text))
            {
                MessageBox.Show("PlateNum by field is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtExpiryDate.Text))
            {
                MessageBox.Show("Expiry Date is required.", "Error");
                return false;
            }


            return true;
        }
        private void ClearAll()
        {
            txtID.Clear();
            txtCaseNum.Clear();

            txtUpdate.Clear();

        }
        private void renewalLTFRB_data()
        {
            dataLTFRB.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT ID, CaseNum, DecisionDate, PlateNum, ExpiryDate, Update_, Remarks FROM ltfrbrenewal ;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataLTFRB.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.IsDBNull(2) ? (object)DBNull.Value : rdr.GetDateTime(2).ToString("yyyy-MM-dd"),
                    rdr.GetString(3), rdr.IsDBNull(4) ? (object)DBNull.Value : rdr.GetDateTime(4).ToString("yyyy-MM-dd"), rdr.GetString(5), rdr.GetString(6));

            }
            con.Close();
        }
        private void LTFRBRecords_Data()
        {
            dataLTFRBrecords.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT caseNo, plateNo, decisionDate, expiryDate, renewedDate FROM rnwlltfrbrecords;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataLTFRBrecords.Rows.Add(rdr.GetString(0), rdr.GetString(1), rdr.GetDateTime(2).ToString("yyyy-MM-dd"), rdr.GetDateTime(3).ToString("yyyy-MM-dd"), rdr.GetDateTime(4).ToString("yyyy-MM-dd"));
            }
            con.Close();
        }

        private void btnRenew_Click(object sender, EventArgs e)
        {
            if (dataLTFRBrecords.SelectedRows.Count > 0)
            {
                con.Open();
                cmd = new MySqlCommand("UPDATE ltfrbrenewal SET " +

                                       "Remarks = @Remarks " +
                                       "WHERE ID = @ID;", con);
                cmd.Parameters.AddWithValue("@Remarks", "Renewal in process");
                cmd.Parameters.AddWithValue("@ID", txtID.Text);
                cmd.ExecuteNonQuery();



                con.Close();
                MessageBox.Show("Remarks Updated Successfully!");
                renewalLTFRB_data();


            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateLTFRBInput()) return;
            if (dataLTFRBrecords.SelectedRows.Count > 0)
            {
                string decUpdateStatus = (txtDecisionDate.Value.Date == DateTime.Now.Date) ? "DECISION_D" : "PENDING";
                string exUpdateStatus = (txtExpiryDate.Value.Date == DateTime.Now.Date) ? "EXPIRY_D" : "PENDING";
                string updateStatus = (decUpdateStatus == "DECISION_D" && exUpdateStatus == "EXPIRY_D") ? "DEC/EX" : (decUpdateStatus == "DECISION_D" ? "DECISION_D" : (exUpdateStatus == "EXPIRY_D" ? "EXPIRY_D" : "PENDING"));
                con.Open();
                cmd = new MySqlCommand("UPDATE ltfrbrenewal SET " +
                    "CaseNum = @CaseNum, " +
                    "DecisionDate = @DecisionDate, " +
                    "PlateNum =  @PlateNum, " +
                    "ExpiryDate = @ExpiryDate, " +
                    "Update_ = @Update " +
                    "WHERE ID = @ID;", con);
                cmd.Parameters.AddWithValue("@CaseNum", txtCaseNum.Text);
                cmd.Parameters.AddWithValue("@DecisionDate", txtDecisionDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@PlateNum", txtPlateNum.Text);
                cmd.Parameters.AddWithValue("@ExpiryDate", txtExpiryDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Update", updateStatus);
                cmd.Parameters.AddWithValue("@ID", txtID.Text);

                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Record Updated Successfully!");
                renewalLTFRB_data();
                ClearAll();
            }
            else
            {
                MessageBox.Show("Please select a record to update!");
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (dataLTFRBrecords.SelectedRows.Count > 0)
            {

                con.Open();
                cmd = new MySqlCommand("UPDATE ltfrbrenewal SET Remarks = @Remarks WHERE  PlateNum = @PlateNo;", con);
                cmd.Parameters.AddWithValue("@Remarks", "DONE");
                cmd.Parameters.AddWithValue("@PlateNo", txtPlateNum.Text);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("INSERT INTO rnwlltfrbrecords(caseNo, plateNo, decisionDate, expiryDate, renewedDate) " +
                 "VALUES(@caseNo, @plateNo, @decisionDate, @expiryDate, @renewedDate)", con);


                cmd.Parameters.AddWithValue("@caseNo", txtCaseNum.Text);
                cmd.Parameters.AddWithValue("@plateNo", txtPlateNum.Text);
                cmd.Parameters.AddWithValue("@decisionDate", txtDecisionDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@expiryDate", txtExpiryDate.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@renewedDate", DateTime.Now);

                cmd.ExecuteNonQuery();



                con.Close();

                MessageBox.Show("Remarks updated successfully!");

                LTFRBRecords_Data();
                renewalLTFRB_data();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                dataLTFRB.Rows.Clear();
                con.Open();
                cmd = new MySqlCommand("SELECT ID, CaseNum, DecisionDate, PlateNum, ExpiryDate, Update_ " +
                                       "FROM ltfrbrenewal " +
                                       "WHERE CONCAT(ID, CaseNum, DecisionDate, PlateNum, ExpiryDate, Update_) LIKE @searchText;", con);
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    dataLTFRB.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetDateTime(2).ToString("yyyy-MM-dd"),
                        rdr.GetString(3), rdr.GetDateTime(4).ToString("yyyy-MM-dd"), rdr.GetString(5));
                }
                con.Close();
            }
            else
            {
                renewalLTFRB_data();
            }
        }

        private void PickerSortDate_ValueChanged(object sender, EventArgs e)
        {
            FilterDataByMonthAndYear();
        }
        private void FilterDataByMonthAndYear()
        {
            int selectedMonth = PickerSortDate.Value.Month;
            int selectedYear = PickerSortDate.Value.Year;

            dataLTFRBrecords.Rows.Clear();

            try
            {
                con.Open();
                string query = "SELECT ID, CaseNum, DecisionDate, PlateNum, ExpiryDate, Update_, Remarks FROM ltfrbrenewal " +
                               "WHERE MONTH(ExpiryDate) = @Month AND YEAR(ExpiryDate) = @Year;";
                cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Month", selectedMonth);
                cmd.Parameters.AddWithValue("@Year", selectedYear);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    dataLTFRBrecords.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.IsDBNull(2) ? (object)DBNull.Value : rdr.GetDateTime(2).ToString("yyyy-MM-dd"),
                    rdr.GetString(3), rdr.IsDBNull(4) ? (object)DBNull.Value : rdr.GetDateTime(4).ToString("yyyy-MM-dd"), rdr.GetString(5), rdr.GetString(6));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering data: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void dataLTFRB_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataLTFRB_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dataLTFRB.SelectedRows.Count > 0)
            {
                txtID.Text = dataLTFRB.SelectedRows[0].Cells[0].Value.ToString();
                txtCaseNum.Text = dataLTFRB.SelectedRows[0].Cells[1].Value.ToString();
                txtDecisionDate.Value = dataLTFRB.SelectedRows[0].Cells[2].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataLTFRB.SelectedRows[0].Cells[2].Value);
                txtPlateNum.Text = dataLTFRB.SelectedRows[0].Cells[3].Value.ToString();
                txtExpiryDate.Value = dataLTFRB.SelectedRows[0].Cells[4].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataLTFRB.SelectedRows[0].Cells[4].Value);
                txtUpdate.Text = dataLTFRB.SelectedRows[0].Cells[5].Value.ToString();
            }
            if (dataLTFRB.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataLTFRB.SelectedRows[0];

                // Ensure the DueDate column exists and is not null
                if (DateTime.TryParse(selectedRow.Cells["ED"].Value?.ToString(), out DateTime dueDate))
                {
                    // Calculate days remaining
                    int daysRemaining = (dueDate - DateTime.Now).Days;

                    // Display the result in lblDUEdays
                    lblDaysUntilR.Text = $"Days until Renewal Date: {daysRemaining}";
                }
                else
                {
                    // Handle case where DueDate is invalid
                    lblDaysUntilR.Text = "Invalid Renewal Date";
                }
            }
            else
            {
                // Clear label if no row is selected
                lblDaysUntilR.Text = "No row selected";
            }
        }

        private void btnRecords_Click(object sender, EventArgs e)
        {

        }
    }
}
