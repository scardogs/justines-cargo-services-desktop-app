using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X500;
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


    public partial class trucks : Form
    {
        private bool isGridVisible = false;
        private Timer slideTimer;
        private int targetTop;
        MySqlConnection con = new MySqlConnection(
            "datasource=localhost;" +
            "port=3306;" +
            "database=JCSdb;" +
            "username=root;" +
            "password=;" +
            "Convert Zero Datetime=True;");
        MySqlCommand cmd;
        MySqlDataReader rdr;

        public trucks()
        {
            InitializeComponent();
            SlideTimer0 = new Timer();
            SlideTimer0.Interval = 1; // Adjust interval for the animation speed (10ms)
            SlideTimer0.Tick += SlideTimer0_Tick; // Ad
            this.StartPosition = FormStartPosition.CenterScreen;
            data_trucks.SelectionChanged += data_trucks_SelectionChanged;
            SlideTimer1 = new Timer();
            SlideTimer1.Interval = 1; // Adjust interval for the animation speed (10ms)
            SlideTimer1.Tick += SlideTimer1_Tick; // Ad
        }

        //Navigation
        private void newTruckShow_Click(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down if the grid is visible
                targetTop = this.ClientSize.Height; // Target position is off-screen (bottom)
            }
            else
            {
                // Slide up if the grid is hidden
                truckInputPanel.Visible = true;  // Make the DataGridView visible before sliding up
                targetTop = 500; // Adjust this value to where you want the grid to end up (e.g., 100px from the top)
            }

            // Start the sliding animation in the appropriate direction
            SlideTimer1.Start();
            if (sender == newTruckShow)
            {
              
              
                btnEdit.Visible = false;
                btnAddTruck.Visible = true;

                TruckEditInfo.Visible = false;
                TruckNewInfo.Visible = true;
                data_trucks.ClearSelection();
                txtTruckID.Visible = false;
                ClearAll();
              

            }
        }

        private void newBackBTn_Click(object sender, EventArgs e)
        {
            
            truckInputPanel.Visible = !truckInputPanel.Visible;
        }

        private void truckEditShow_Click(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down if the grid is visible
                targetTop = this.ClientSize.Height; // Target position is off-screen (bottom)
            }
            else
            {
                // Slide up if the grid is hidden
                truckInputPanel.Visible = true;  // Make the DataGridView visible before sliding up
                targetTop = 500; // Adjust this value to where you want the grid to end up (e.g., 100px from the top)
            }

            // Start the sliding animation in the appropriate direction
            SlideTimer0.Start();
            if (sender == truckEditShow)
            {
                
                truckInputPanel.BringToFront();
                TruckNewInfo.Visible = false;
                TruckEditInfo.Visible = true;
                btnAddTruck.Visible = false;
                btnEdit.Visible = true;
                txtTruckID.Visible = true;
            }
        }

        //load 
        private void loadTruck()
        {
            data_trucks.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT id, plateNum, CBM, yearModel, category, Color, RegName,MVUCRate, VehicleType, status FROM truck;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                data_trucks.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2),
                rdr.GetString(3), rdr.GetString(4), rdr.GetString(5)
                , rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9));
            }
            con.Close();
        }

        private void trucks_Load(object sender, EventArgs e)
        {
            loadTruck();
            ClearAll();
            this.WindowState = FormWindowState.Maximized;
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        //buttons
        private void btnAddTruck_Click(object sender, EventArgs e)
        {
            addTrucks();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!ValidateTrucksInput()) return;

            if (data_trucks.SelectedRows.Count > 0)
            {
                string oldPlateNum = data_trucks.SelectedRows[0].Cells[1].Value.ToString();
                DateTime dueDate = GetDueDateFromPlateNumber(txtPlateNum.Text);

                con.Open();
                MySqlTransaction transaction = con.BeginTransaction(); // Begin a transaction

                try
                {
                    // Update truck table
                    using (MySqlCommand cmd = new MySqlCommand(
                        "UPDATE truck SET plateNum = @plateNum,CBM = @CBM, yearModel = @yearModel, category = @category, " +
                        "Color = @Color, RegName = @RegName, MVUCRate = @MVUCRate, VehicleType = @VehicleType, " +
                        "status = @status WHERE ID = @id;", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@plateNum", txtPlateNum.Text);
                        cmd.Parameters.AddWithValue("@yearModel", txtYearModel.Text);
                        cmd.Parameters.AddWithValue("@CBM", txtCBM.Text);
                        cmd.Parameters.AddWithValue("@category", cbCategory.Text);
                        cmd.Parameters.AddWithValue("@Color", txtColor.Text);
                        cmd.Parameters.AddWithValue("@RegName", txtRegName.Text);
                        cmd.Parameters.AddWithValue("@MVUCRate", txtMVUCrate.Text);
                        cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text);
                        cmd.Parameters.AddWithValue("@status", cbStatus.Text);
                        cmd.Parameters.AddWithValue("@id", txtTruckID.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Update delivery table
                    using (MySqlCommand cmd = new MySqlCommand(
                        "UPDATE delivery SET PlateNo = @plateNum, Category = @category WHERE PlateNo = @oldPlateNum;", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@plateNum", txtPlateNum.Text);
                        cmd.Parameters.AddWithValue("@category", cbCategory.Text);
                        cmd.Parameters.AddWithValue("@oldPlateNum", oldPlateNum);
                        cmd.ExecuteNonQuery();
                    }

                    // Update ltorenewal table
                    using (MySqlCommand cmd = new MySqlCommand(
                        "UPDATE ltorenewal SET PlateNo = @plateNum, Duedate = @Duedate, Color = @Color, " +
                        "MVUCRate = @MVUCRate, VehicleType = @VehicleType, RegName = @RegName WHERE PlateNo = @oldPlateNum;", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@plateNum", txtPlateNum.Text);
                        cmd.Parameters.AddWithValue("@Duedate", dueDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Color", txtColor.Text);
                        cmd.Parameters.AddWithValue("@RegName", txtRegName.Text);
                        cmd.Parameters.AddWithValue("@MVUCRate", txtMVUCrate.Text);
                        cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text);
                        cmd.Parameters.AddWithValue("@oldPlateNum", oldPlateNum);
                        cmd.ExecuteNonQuery();
                    }

                    // Update insurancerenewal table
                    using (MySqlCommand cmd = new MySqlCommand(
                        "UPDATE insurancerenewal SET plateNo = @plateNum WHERE plateNo = @oldPlateNum;", con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@plateNum", txtPlateNum.Text);
                        cmd.Parameters.AddWithValue("@oldPlateNum", oldPlateNum);
                        cmd.ExecuteNonQuery();
                    }

                    // Update status in delivery table if required   
                    if (cbStatus.Text == "Under Repair" || cbStatus.Text == "For Repair" || cbStatus.Text == "Available")
                    {
                        using (MySqlCommand cmd = new MySqlCommand(
                            "UPDATE delivery SET Status = @status WHERE PlateNo = @plateNum;", con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@status", cbStatus.Text);
                            cmd.Parameters.AddWithValue("@plateNum", txtPlateNum.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();

                    MessageBox.Show("Truck details updated successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback the transaction on error
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
                finally
                {
                    con.Close(); // Ensure the connection is closed
                }

                loadTruck(); // Reload the truck data
                ClearAll();
            }
            else
            {
                MessageBox.Show("Please select a truck to update.");
            }
        }
        //methods
        private void addTrucks()
        {
            if (!ValidateTrucksInput()) return;
            DateTime dueDate = GetDueDateFromPlateNumber(txtPlateNum.Text);
            con.Open();
            cmd = new MySqlCommand("INSERT INTO " +
                "truck (plateNum,CBM, yearModel, category, Color, RegName,MVUCRate, VehicleType,status) " +
                "VALUES (@pnum,@CBM, @ym, @category, @Color, @RegName,@MVUCRate, @VehicleType, @stat);", con);
            cmd.Parameters.AddWithValue("@pnum", txtPlateNum.Text);
            cmd.Parameters.AddWithValue("@CBM", txtCBM.Text);
            cmd.Parameters.AddWithValue("@ym", txtYearModel.Text);
            cmd.Parameters.AddWithValue("@category", cbCategory.Text);
            cmd.Parameters.AddWithValue("@Color", txtColor.Text);
            cmd.Parameters.AddWithValue("@RegName", txtRegName.Text);
            cmd.Parameters.AddWithValue("@MVUCRate", txtMVUCrate.Text);
            cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text);
            cmd.Parameters.AddWithValue("@stat", cbStatus.Text);
            cmd.ExecuteNonQuery();

            cmd = new MySqlCommand("INSERT INTO ltorenewal (PlateNo, MVUCrate, VehicleType, Duedate, Color, Regname) " +
                                   "VALUES (@PlateNo,@MVUCrate, @VehicleType, @Duedate, @Color, @Regname);", con);
            cmd.Parameters.AddWithValue("@PlateNo", txtPlateNum.Text);
            cmd.Parameters.AddWithValue("@MVUCrate", txtMVUCrate.Text);
            cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text);
            cmd.Parameters.AddWithValue("@Color", txtColor.Text);
            cmd.Parameters.AddWithValue("@Regname", txtRegName.Text);
            cmd.Parameters.AddWithValue("@Duedate", dueDate.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();

            cmd = new MySqlCommand("INSERT INTO delivery (PlateNo, Category, Status) " +
                                   "VALUES (@PlateNo, @Category, @Status);", con);
            cmd.Parameters.AddWithValue("@PlateNo", txtPlateNum.Text);
            cmd.Parameters.AddWithValue("@Category", cbCategory.Text);
            cmd.Parameters.AddWithValue("@Status", cbStatus.Text);

            cmd.ExecuteNonQuery();

            cmd = new MySqlCommand("INSERT INTO insurancerenewal(plateNo)  " +
             "VALUES (@plateNo);", con);
            cmd.Parameters.AddWithValue("@plateNo", txtPlateNum.Text);
            cmd.ExecuteNonQuery();



            con.Close();
            MessageBox.Show("Truck Added!", "Success");
            loadTruck();
        }

        private DateTime GetDueDateFromPlateNumber(string plateNumber)
        {

            if (plateNumber.Length < 2)
                throw new ArgumentException("Plate number must be at least 2 characters long.");

            string lastTwoDigits = plateNumber.Substring(plateNumber.Length - 2);
            int lastDigit = int.Parse(lastTwoDigits.Substring(1, 1));
            int secondLastDigit = int.Parse(lastTwoDigits.Substring(0, 1));


            int month;
            switch (lastDigit)
            {
                case 1: month = 1; break;
                case 2: month = 2; break;
                case 3: month = 3; break;
                case 4: month = 4; break;
                case 5: month = 5; break;
                case 6: month = 6; break;
                case 7: month = 7; break;
                case 8: month = 8; break;
                case 9: month = 9; break;
                case 0: month = 10; break;
                default: throw new InvalidOperationException("Invalid plate number");
            }


            int day;
            if (secondLastDigit >= 1 && secondLastDigit <= 3)
            {
                day = 1;
            }
            else if (secondLastDigit >= 4 && secondLastDigit <= 6)
            {
                day = 8;
            }
            else if (secondLastDigit == 7 || secondLastDigit == 8)
            {
                day = 15;
            }
            else if (secondLastDigit == 9 || secondLastDigit == 0)
            {
                day = 22;
            }
            else
            {
                throw new InvalidOperationException("Invalid plate number");
            }

            int year = DateTime.Now.Year + 1;


            return new DateTime(year, month, day);
        }

        //selection change

        private void data_trucks_SelectionChanged(object sender, EventArgs e)
        {
            // Check if at least one row is selected
            if (data_trucks.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = data_trucks.SelectedRows[0];


                txtTruckID.Text = selectedRow.Cells[0]?.Value?.ToString() ?? "";
                txtPlateNum.Text = selectedRow.Cells[1]?.Value?.ToString() ?? "";
                txtCBM.Text = selectedRow.Cells[2]?.Value?.ToString() ?? "";
                txtYearModel.Text = selectedRow.Cells[3]?.Value?.ToString() ?? "";
                cbCategory.Text = selectedRow.Cells[4]?.Value?.ToString() ?? "";
                txtColor.Text = selectedRow.Cells[5]?.Value?.ToString() ?? "";
                txtRegName.Text = selectedRow.Cells[6]?.Value?.ToString() ?? "";
                txtMVUCrate.Text = selectedRow.Cells[7]?.Value?.ToString() ?? "";
                txtVehicleType.Text = selectedRow.Cells[8]?.Value?.ToString() ?? "";
                cbStatus.Text = selectedRow.Cells[9]?.Value?.ToString() ?? "";
            }
            else
            {
                
                ClearAll();
            }
        }



        //others
        private void ClearAll()
        {
            txtPlateNum.Clear();
            txtYearModel.Clear();
            txtColor.Clear();
            cbStatus.SelectedIndex = 0; 
            cbCategory.SelectedIndex = 0;   

            txtCBM.Clear();

            txtRegName.Clear();
            txtMVUCrate.Clear();
            txtVehicleType.Clear();
            txtRegName.Clear();
        }
        private void searchBtn_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            loadTruck(searchQuery);
         
            {
            }
        }

        private void loadTruck(string searchQuery = "")
        {
            data_trucks.Rows.Clear();

            string query = "SELECT id, plateNum, yearModel, category, Color, RegName, status FROM truck";

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query += " WHERE CONCAT(id, plateNum, yearModel, category, Color, RegName, status) LIKE @searchQuery";
            }

            con.Open();
            cmd = new MySqlCommand(query, con);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                cmd.Parameters.AddWithValue("@searchQuery", "%" + searchQuery + "%");
            }

            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                data_trucks.Rows.Add(rdr.GetInt32(0), rdr.GetString(1),
                    rdr.GetString(2), rdr.GetString(3), rdr.GetString(4),
                    rdr.GetString(5), rdr.GetString(6));
            }
            con.Close();
        }

        private bool ValidateTrucksInput()
        {
            if (string.IsNullOrWhiteSpace(txtPlateNum.Text))
            {
                MessageBox.Show("Plate Number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCBM.Text))
            {
                MessageBox.Show("CBM is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtYearModel.Text))
            {
                MessageBox.Show("Year Model is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(cbCategory.Text))
            {
                MessageBox.Show("Category is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtColor.Text))
            {
                MessageBox.Show("Color is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtRegName.Text))
            {
                MessageBox.Show("Reg Name is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMVUCrate.Text))
            {
                MessageBox.Show("MVUC rate is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtVehicleType.Text))
            {
                MessageBox.Show("Vehicle Type is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(cbStatus.Text))
            {
                MessageBox.Show("Status is required.", "Error");
                return false;
            }

            

            return true; // Add this to indicate that the validation passed
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            loadTruck();
        }

        private void SlideTimer0_Tick(object sender, EventArgs e)
        {
            // Check if the grid should slide up or down
            if (isGridVisible)
            {
                // Slide down (move to the bottom of the form)
                if (truckInputPanel.Top < this.ClientSize.Height)
                {
                    truckInputPanel.Top += 15;  // Adjust the speed of sliding down
                }
                else
                {
                    truckInputPanel.Visible = false;  // Hide the DataGridView once it reaches the bottom
                    SlideTimer0.Stop();  // Stop the timer when sliding down is complete
                    isGridVisible = false;  // Update the visibility flag
                }
            }
            else
            {
                // Slide up (move towards the targetTop position)
                if (truckInputPanel.Top > targetTop)
                {
                    truckInputPanel.Top -= 15;  // Adjust the speed of sliding up
                }
                else
                {
                    SlideTimer0.Stop();  // Stop the timer when sliding up is complete
                    isGridVisible = true;  // Update the visibility flag
                }
            }
        }

        private void SlideTimer1_Tick(object sender, EventArgs e)
        {
            // Check if the grid should slide up or down
            if (isGridVisible)
            {
                // Slide down (move to the bottom of the form)
                if (truckInputPanel.Top < this.ClientSize.Height)
                {
                    truckInputPanel.Top += 15;  // Adjust the speed of sliding down
                }
                else
                {
                    truckInputPanel.Visible = false;  // Hide the DataGridView once it reaches the bottom
                    SlideTimer1.Stop();  // Stop the timer when sliding down is complete
                    isGridVisible = false;  // Update the visibility flag
                }
            }
            else
            {
                // Slide up (move towards the targetTop position)
                if (truckInputPanel.Top > targetTop)
                {
                    truckInputPanel.Top -= 15;  // Adjust the speed of sliding up
                }
                else
                {
                    SlideTimer1.Stop();  // Stop the timer when sliding up is complete
                    isGridVisible = true;  // Update the visibility flag
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            truckInputPanel.Visible = !truckInputPanel.Visible;
        }
    }
}
