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
    public partial class renewalsLTO : Form
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
        public renewalsLTO()
        {
            InitializeComponent();
            timerSlide.Tick += timerSlide_Tick;
            timerSlide.Interval = 5; // Adjust for smoother or faster animation
            pnlLTO.Visible = false;   // Ensure the panel is initially hidden
            originalY = this.ClientSize.Height; // Set the starting position (below the form)
            pnlLTO.Top = originalY;
        }
        private void ClearAll()
        {
            txtPlate.Clear();
            txtVehicleT.Clear();
            txtColor.Clear();
            txtMRate.Clear();
            txtRegName.Clear();
            txtUpdate.Clear();
            txtORnum.Clear();
        }
        private void ToggleDeliveryVisibility()
        {

            dataLTOrecords.Visible = !dataLTOrecords.Visible;
        }
        private void btnRecords_Click(object sender, EventArgs e)
        {
            ToggleDeliveryVisibility();
        }

        private void timerSlide_Tick(object sender, EventArgs e)
        {
            if (isSlidingUp)
            {
                // Slide up
                if (pnlLTO.Top > targetY)
                {
                    pnlLTO.Top -= 5; // Adjust for smoother or faster sliding
                    if (pnlLTO.Top <= targetY)
                    {
                        pnlLTO.Top = targetY;
                        timerSlide.Stop();
                        isExpanded = true;
                    }
                }
            }
            else
            {
                // Slide down
                if (pnlLTO.Top < originalY)
                {
                    pnlLTO.Top += 5; // Adjust for smoother or faster sliding
                    if (pnlLTO.Top >= originalY)
                    {
                        pnlLTO.Top = originalY;
                        pnlLTO.Visible = false; // Hide the panel after sliding down
                        timerSlide.Stop();
                        isExpanded = false;
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!isExpanded)
            {
                // Slide up and show
                pnlLTO.Visible = true; // Ensure the panel is visible
                targetY = (this.ClientSize.Height - pnlLTO.Height) / 2;
                isSlidingUp = true;
            }
            else
            {
                // Slide down and hide
                isSlidingUp = false;
            }

            timerSlide.Start();
        }

        private void renewalsLTO_Load(object sender, EventArgs e)
        {
            renewalLTO_data();
            LTORecords_Data();


            PickerSortDate.ValueChanged += PickerSortDate_ValueChanged;
        }

        private void renewalLTO_data()
        {
            dataLTOrenew.Rows.Clear();
            con.Open();
            cmd = new MySqlCommand("SELECT LTO_ID, PlateNo, VehicleType, Color, MVUCrate, Duedate, Regname, ORnum, ORdate, Update_, Remarks FROM ltorenewal;", con);
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
                    rdr.IsDBNull(8) ? (object)DBNull.Value : rdr.GetDateTime(8).ToString("yyyy-MM-dd"),
                    rdr.GetString(9),
                    rdr.GetString(10)
                );
            }
            con.Close();
        }
        private void LTORecords_Data()
        {
            dataLTOrecords.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT plateNo, orNo, orDate, renewedDate FROM rnwlltorecords;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataLTOrecords.Rows.Add(rdr.GetString(0), rdr.GetString(1), rdr.GetDateTime(2).ToString("yyyy-MM-dd"), rdr.GetDateTime(3).ToString("yyyy-MM-dd"));
            }
            con.Close();
        }
        private void PickerSortDate_ValueChanged(object sender, EventArgs e)
        {
            FilterDataByMonthAndYear();
        }
        private void FilterDataByMonthAndYear()
        {
            int selectedMonth = PickerSortDate.Value.Month;
            int selectedYear = PickerSortDate.Value.Year;

            dataLTOrenew.Rows.Clear();

            try
            {
                con.Open();
                string query = "SELECT LTO_ID, PlateNo, VehicleType, Color, MVUCrate, Duedate, Regname, ORnum, ORdate, Update_ " +
                               "FROM ltorenewal " +
                               "WHERE MONTH(Duedate) = @Month AND YEAR(Duedate) = @Year;";
                cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Month", selectedMonth);
                cmd.Parameters.AddWithValue("@Year", selectedYear);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    dataLTOrenew.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2),
                        rdr.GetString(3), rdr.GetString(4), rdr.IsDBNull(5) ? (object)DBNull.Value : rdr.GetDateTime(5).ToString("yyyy-MM-dd"),
                        rdr.GetString(6), rdr.GetString(7), rdr.IsDBNull(8) ? (object)DBNull.Value : rdr.GetDateTime(8).ToString("yyyy-MM-dd"),
                        rdr.GetString(9));
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
        private void dataLTOrenew_SelectionChanged(object sender, EventArgs e)
        {
            if (dataLTOrenew.SelectedRows.Count > 0)
            {
                txtID.Text = dataLTOrenew.SelectedRows[0].Cells[0].Value.ToString();
                txtPlate.Text = dataLTOrenew.SelectedRows[0].Cells[1].Value.ToString();
                txtVehicleT.Text = dataLTOrenew.SelectedRows[0].Cells[2].Value.ToString();
                txtColor.Text = dataLTOrenew.SelectedRows[0].Cells[3].Value.ToString();
                txtMRate.Text = dataLTOrenew.SelectedRows[0].Cells[4].Value.ToString();
                txtDuedate.Value = dataLTOrenew.SelectedRows[0].Cells[5].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataLTOrenew.SelectedRows[0].Cells[5].Value);
                txtRegName.Text = dataLTOrenew.SelectedRows[0].Cells[6].Value.ToString();
                txtORnum.Text = dataLTOrenew.SelectedRows[0].Cells[7].Value.ToString();
                txtORdate.Value = dataLTOrenew.SelectedRows[0].Cells[8].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataLTOrenew.SelectedRows[0].Cells[8].Value);
                txtUpdate.Text = dataLTOrenew.SelectedRows[0].Cells[9].Value.ToString();


            }
            if (dataLTOrenew.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataLTOrenew.SelectedRows[0];

                // Ensure the DueDate column exists and is not null
                if (DateTime.TryParse(selectedRow.Cells["DueDate"].Value?.ToString(), out DateTime dueDate))
                {
                    // Calculate days remaining
                    int daysRemaining = (dueDate - DateTime.Now).Days;

                    // Display the result in lblDUEdays
                    lblDUEdays.Text = $"Days until Due Date: {daysRemaining}";
                }
                else
                {
                    // Handle case where DueDate is invalid
                    lblDUEdays.Text = "Invalid Due Date";
                }
            }
            else
            {
                // Clear label if no row is selected
                lblDUEdays.Text = "No row selected";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshStatus();
        }
        private void RefreshStatus()
        {
            MySqlTransaction transaction = null;
            try
            {
                con.Open();
                transaction = con.BeginTransaction();

                cmd = new MySqlCommand("UPDATE ltorenewal SET Update_ = " +
                                       "CASE " +
                                       "WHEN Duedate = CURDATE() AND ORdate = CURDATE() THEN 'DUE/OR' " +
                                       "WHEN Duedate = CURDATE() THEN 'DUE' " +
                                       "WHEN ORdate = CURDATE() THEN 'OR' " +
                                       "ELSE 'PENDING' " +
                                       "END;", con, transaction);
                cmd.ExecuteNonQuery();
                transaction.Commit();
                cmd = new MySqlCommand("UPDATE ltorenewal SET Remarks = REPLACE(Remarks, 'DONE', '')", con, transaction);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("UPDATE ltorenewal SET Remarks = REPLACE(Remarks, 'Or Date Renewed', '')", con, transaction);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Refresh Success!");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error updating status: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            renewalLTO_data();
        }
        private bool ValidateLTOInput()
        {

            if (string.IsNullOrWhiteSpace(txtPlate.Text))
            {
                MessageBox.Show("Plate number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtVehicleT.Text))
            {
                MessageBox.Show("Vehicle Type is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtColor.Text))
            {
                MessageBox.Show("color is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMRate.Text))
            {
                MessageBox.Show("MVUC rate is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDuedate.Text))
            {
                MessageBox.Show("Due Date is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtRegName.Text))
            {
                MessageBox.Show("Registered Name is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtORnum.Text))
            {
                MessageBox.Show("OR number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtORdate.Text))
            {
                MessageBox.Show("Or date is required.", "Error");
                return false;
            }



            return true;
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateLTOInput()) return;

            if (dataLTOrenew.SelectedRows.Count > 0)
            {
                try
                {

                    int orDateYear = txtORdate.Value.Year;


                    DateTime dueDate = new DateTime(orDateYear + 1, txtDuedate.Value.Month, txtDuedate.Value.Day);


                    string dueUpdateStatus = (txtDuedate.Value.Date == DateTime.Now.Date) ? "DUE" : "PENDING";
                    string orUpdateStatus = (txtORdate.Value.Date == DateTime.Now.Date) ? "OR" : "PENDING";

                    con.Open();
                    cmd = new MySqlCommand("UPDATE ltorenewal SET " +
                                           "PlateNo = @Plate, " +
                                           "VehicleType = @VehicleType, " +
                                           "Color = @Color, " +
                                           "MVUCrate = @MVUCrate, " +
                                           "Duedate = @Duedate, " +
                                           "Regname = @Regname, " +
                                           "ORnum = @ORnum, " +
                                           "ORdate = @ORdate, " +
                                           "Update_ = @Update, " +
                                           "Remarks = 'Or Date Renewed' " +
                                           "WHERE LTO_ID = @ID;", con);
                    cmd.Parameters.AddWithValue("@Plate", txtPlate.Text);
                    cmd.Parameters.AddWithValue("@VehicleType", txtVehicleT.Text);
                    cmd.Parameters.AddWithValue("@Color", txtColor.Text);
                    cmd.Parameters.AddWithValue("@MVUCrate", txtMRate.Text);
                    cmd.Parameters.AddWithValue("@Duedate", dueDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Regname", txtRegName.Text);
                    cmd.Parameters.AddWithValue("@ORnum", txtORnum.Text);
                    cmd.Parameters.AddWithValue("@ORdate", txtORdate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Update", (dueUpdateStatus == "DUE" && orUpdateStatus == "OR") ? "DUE/OR" : (dueUpdateStatus == "DUE" ? "DUE" : (orUpdateStatus == "OR" ? "OR" : "PENDING")));
                    cmd.Parameters.AddWithValue("@ID", txtID.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Record Updated Successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating record: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }

                renewalLTO_data();
                ClearAll();
            }
            else
            {
                MessageBox.Show("Please select a record to update!");
            }
        }

        private void btnRenew_Click(object sender, EventArgs e)
        {
            if (dataLTOrenew.SelectedRows.Count > 0)
            {
                con.Open();
                cmd = new MySqlCommand("UPDATE ltorenewal SET " +

                                       "Remarks = @Remarks " +
                                       "WHERE LTO_ID = @ID;", con);
                cmd.Parameters.AddWithValue("@Remarks", "Renewal in process");
                cmd.Parameters.AddWithValue("@ID", txtID.Text);
                cmd.ExecuteNonQuery();



                con.Close();
                MessageBox.Show("Remarks Updated Successfully!");
                renewalLTO_data();


            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                dataLTOrenew.Rows.Clear();
                con.Open();
                cmd = new MySqlCommand("SELECT LTO_ID, PlateNo, VehicleType, Color, MVUCrate, Duedate, Regname, ORnum, ORdate, Update_ " +
                                       "FROM ltorenewal " +
                                       "WHERE CONCAT(LTO_ID, PlateNo, VehicleType, Color, MVUCrate, Duedate, Regname, ORnum, ORdate, Update_) LIKE @searchText;", con);
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    dataLTOrenew.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2),
                        rdr.GetString(3), rdr.GetString(4), rdr.GetDateTime(5).ToString("yyyy-MM-dd"),
                        rdr.GetString(6), rdr.GetString(7), rdr.GetDateTime(8).ToString("yyyy-MM-dd"),
                        rdr.GetString(9));
                }
                con.Close();
            }
            else
            {
                renewalLTO_data();
            }
        }

        private void txtDuedate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtORdate_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}

