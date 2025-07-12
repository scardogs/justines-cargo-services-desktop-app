using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace best
{
    public partial class fuelmonitoring : Form
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

        public fuelmonitoring()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            cbSortBy.SelectedIndexChanged += cbSortBy_SelectedIndexChanged;

            loadFuelBulk();
            loadFuel();
            loadBulkFuelRemaining();

            txtPlateNum.Size = new System.Drawing.Size(140, 106);

            
            txtPlateNum.DropDownHeight = 200;
        }

        //Fuel Consumption Section
        public void loadFuel()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                // Check if lblDept.Text is set to "All Companies"
                string selectedCompany = lblDept.Text;
                string query = "SELECT fm.ID, fm.fuel_date, fm.plateNum, fm.department, fm.requestedBy, fm.qty, fb.PricePerLitter, fm.totalFuelAmount " +
                               "FROM fuelmonitoring fm " +
                               "JOIN fuelbulk fb ON fm.bulkID = fb.bulkID ";

                // Add a condition only if a specific company is selected
                if (selectedCompany != "All Companies")
                {
                    query += "WHERE fm.department = @selectedCompany ";
                }

                cmd = new MySqlCommand(query, con);

                // Add parameter only if a specific company is selected
                if (selectedCompany != "All Companies")
                {
                    cmd.Parameters.AddWithValue("@selectedCompany", selectedCompany);
                }

                rdr = cmd.ExecuteReader();

                // Clear previous data
                data_Fuel.Rows.Clear();

                while (rdr.Read())
                {
                    data_Fuel.Rows.Add(
                        rdr.GetInt32(0), rdr.GetDateTime(1), rdr.GetString(2), rdr.GetString(3),
                        rdr.GetString(4), rdr.GetDecimal(5), rdr.GetDecimal(6), rdr.GetDecimal(7));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading fuel data: {ex.Message}", "Error");
            }
            finally
            {
                if (rdr != null && !rdr.IsClosed)
                {
                    rdr.Close();
                }
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

            CalculateFuelTotal(); // Calculate the total fuel
        }




        //Fuel Bulk Section
        public void loadFuelBulk()
        {
            FuelBulk_Grid.Rows.Clear();
            con.Open();
            cmd = new MySqlCommand("SELECT bulkID, dateOrdered, amount, PricePerLitter, totalBulkPrice FROM fuelbulk;", con);
            rdr = cmd.ExecuteReader();


            while (rdr.Read())
            {
                FuelBulk_Grid.Rows.Add(rdr.GetInt32(0), rdr.GetDateTime(1),
                    rdr.GetDecimal(2), rdr.GetDecimal(3), rdr.GetDecimal(4));
            }
            con.Close();
        }

        private void fuelmonitoring_Load(object sender, EventArgs e)
        {
            cbSortBy.SelectedItem = "All Companies";
            Fuelmonitoring_load();
            clearAll();
            loadFuel();
            loadBulkFuelRemaining();
            UpdateRemainingFuelTextBox();
            this.WindowState = FormWindowState.Maximized;
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            data_Fuel.ClearSelection();
            txtPlateNum.SelectedItem = "0";
            FuelBulk_Grid.ClearSelection();
            asOfDateFuel.Text = $"Fuel Remaining as of  {DateTime.Now:MMMM dd, yyyy}";
        }
        



        public void loadBulkFuelRemaining()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd = new MySqlCommand("SELECT bulkRemainFuelID, bulkRemainingFuel FROM bulkremainingfuel ORDER BY bulkRemainFuelID DESC;", con);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading remaining fuel: {ex.Message}", "Error");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            // update remaining fuel textbox
            UpdateRemainingFuelTextBox();
        }


        //buttons
        // Buttons
  
        private void btnFuelAdd_Click_1(object sender, EventArgs e)
        {
            if (!fuelTrucksValidateInput()) return;

            if (!decimal.TryParse(txtQty.Text, out decimal fuelConsumed))
            {
                MessageBox.Show("Invalid fuel consumption quantity. Please enter a valid number.", "Error");
                return;
            }

            if (!verifyFuelConsumption(fuelConsumed))
            {
                MessageBox.Show("Fuel consumption is not enough. Please check and try again.", "Error");
                return;
            }

            try
            {
                con.Open();

                // Fetch the latest bulkID
                cmd = new MySqlCommand("SELECT MAX(bulkID) FROM fuelbulk;", con);
                int latestBulkID = Convert.ToInt32(cmd.ExecuteScalar());

                if (latestBulkID <= 0)
                {
                    throw new Exception("No valid bulk entry found. Please add fuel bulk information first.");
                }

                decimal latestPricePerLitter = GetLatestPricePerLitter();
                decimal qty = decimal.Parse(txtQty.Text);
                decimal totalFuelAmount = qty * latestPricePerLitter;

                cmd = new MySqlCommand(
                    "INSERT INTO fuelmonitoring (fuel_date, plateNum, department, requestedBy, qty, bulkID, totalFuelAmount) " +
                    "VALUES (@fdate, @pnum, @dept, @rb, @qty, @bulkID, @totalFuelAmount);", con);

                cmd.Parameters.AddWithValue("@fdate", txtFuelDatePicker.Value.Date);
                cmd.Parameters.AddWithValue("@pnum", txtPlateNum.Text);
                cmd.Parameters.AddWithValue("@dept", cbDepartment.Text);
                cmd.Parameters.AddWithValue("@rb", txtRequestedBy.Text);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.Parameters.AddWithValue("@bulkID", latestBulkID);
                cmd.Parameters.AddWithValue("@totalFuelAmount", totalFuelAmount);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    UpdateRemainingFuel(-qty);  // Subtract the fuel consumed
                    MessageBox.Show("Fuel Consumption Added!", "Success");
                    loadBulkFuelRemaining();
                }
                else
                {
                    MessageBox.Show("Failed to add fuel consumption. No rows were affected.", "Error");
                }
                Fuelmonitoring_load();
                loadFuelBulk();
                loadFuel();
                clearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
            finally
            {
                con.Close();
            }
        }

        private void btnFuelEdit_Click_1(object sender, EventArgs e)
        {

            if (!fuelTrucksValidateInput()) return;

            if (!decimal.TryParse(txtQty.Text, out decimal newFuelConsumed))
            {
                MessageBox.Show("Invalid fuel consumption quantity. Please enter a valid number.", "Error");
                return;
            }

            if (!verifyFuelConsumption(newFuelConsumed))
            {
                MessageBox.Show("Fuel consumption is not enough. Please check and try again.", "Error");
                return;
            }

            try
            {
                decimal oldFuelConsumed = GetOldFuelConsumption(int.Parse(txtFuelID.Text));

                decimal latestPricePerLitter = GetLatestPricePerLitter();
                decimal totalFuelAmount = newFuelConsumed * latestPricePerLitter;

                if (con.State == ConnectionState.Open)
                    con.Close();

                cmd = new MySqlCommand("UPDATE fuelmonitoring SET plateNum = @pnum, department = @dept, fuel_date = @fdate, " +
                    "requestedBy = @rb, qty = @qty, totalFuelAmount = @totalFuelAmount " +
                    "WHERE ID = @id;", con);

                cmd.Parameters.AddWithValue("@fdate", txtFuelDatePicker.Value.Date);
                cmd.Parameters.AddWithValue("@pnum", txtPlateNum.Text);
                cmd.Parameters.AddWithValue("@dept", cbDepartment.Text);
                cmd.Parameters.AddWithValue("@rb", txtRequestedBy.Text);
                cmd.Parameters.AddWithValue("@qty", newFuelConsumed);
                cmd.Parameters.AddWithValue("@totalFuelAmount", totalFuelAmount);
                cmd.Parameters.AddWithValue("@id", int.Parse(txtFuelID.Text));

                con.Open();
                cmd.ExecuteNonQuery();

                decimal fuelDifference = oldFuelConsumed - newFuelConsumed;

                UpdateRemainingFuel(fuelDifference);  // Adjust for the difference

                con.Close(); // Close the connection after the operation

                MessageBox.Show("Details Edited Successfully", "Success");
                Fuelmonitoring_load();
                loadFuelBulk();
                loadBulkFuelRemaining();
                loadFuel();
                clearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }


        private void btnBulkADD_Click_1(object sender, EventArgs e)
        {
            if (!fuelBulkValidateInput()) return;
            decimal bulkAmount = decimal.Parse(txtBulkAmount.Text);

            try
            {
                con.Open();
                cmd = new MySqlCommand("INSERT INTO fuelbulk (dateOrdered, amount, PricePerLitter, totalBulkPrice) " +
                                       "VALUES (@dto, @amount, @ppp, @tbp);", con);

                cmd.Parameters.AddWithValue("@dto", DTBulk.Value.Date);
                cmd.Parameters.AddWithValue("@amount", bulkAmount);
                cmd.Parameters.AddWithValue("@ppp", decimal.Parse(txtPricePerLitter.Text));
                cmd.Parameters.AddWithValue("@tbp", decimal.Parse(txtTotalBP.Text));

                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Fuel Bulk Information Added!", "Success");
                loadFuelBulk();
                UpdateRemainingFuelOnBulkAdd(bulkAmount);
                UpdateRemainingFuelTextBox();
                loadFuel();
                clearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }

        private void btnBulkEdit_Click(object sender, EventArgs e)
        {
            if (!fuelBulkValidateInput()) return;


            decimal newBulkAmount = decimal.Parse(txtBulkAmount.Text);
            int bulkID = int.Parse(txtBulkID.Text);
            decimal oldBulkAmount = GetOldBulkAmount(bulkID);

            // Validate the edit
            if (!ValidateBulkEdit(newBulkAmount, out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Validation Error");
                return;
            }


            try
            {
                con.Open();
                cmd = new MySqlCommand("UPDATE fuelbulk SET dateOrdered = @dto, amount = @amount, PricePerLitter = @ppp, totalBulkPrice = @tbp " +
                    "WHERE bulkID = @id;", con);

                cmd.Parameters.AddWithValue("@dto", DTBulk.Value.Date);
                cmd.Parameters.AddWithValue("@amount", newBulkAmount);
                cmd.Parameters.AddWithValue("@ppp", decimal.Parse(txtPricePerLitter.Text));
                cmd.Parameters.AddWithValue("@tbp", decimal.Parse(txtTotalBP.Text));
                cmd.Parameters.AddWithValue("@id", bulkID);

                cmd.ExecuteNonQuery();
                con.Close();

                // Adjust remaining fuel based on the difference
                decimal amountDifference = newBulkAmount - oldBulkAmount;
                UpdateRemainingFuelOnBulkEdit(amountDifference);

                MessageBox.Show("Fuel Bulk Information Edited!", "Success");
                loadFuelBulk();
                UpdateRemainingFuelTextBox();
                loadBulkFuelRemaining();
                loadFuel();
                clearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }


        // Methods for remaining Fuel

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

        private void UpdateRemainingFuel(decimal amount)
        {
            decimal currentRemainingFuel = 0;
            if (con.State != ConnectionState.Open)
                con.Open();

            cmd = new MySqlCommand("SELECT bulkRemainingFuel FROM bulkremainingfuel ORDER BY bulkRemainFuelID DESC LIMIT 1;", con);
            object result = cmd.ExecuteScalar();
            if (result != null)
                currentRemainingFuel = Convert.ToDecimal(result);

            decimal newRemainingFuel = currentRemainingFuel + amount; // Adjust by adding or subtracting

            cmd = new MySqlCommand("INSERT INTO bulkremainingfuel (bulkRemainingFuel) VALUES (@remainingFuel);", con);
            cmd.Parameters.AddWithValue("@remainingFuel", newRemainingFuel);
            cmd.ExecuteNonQuery();

            con.Close();
        }

        private void UpdateRemainingFuelOnBulkAdd(decimal newBulkFuel)
        {
            // Just add the new bulk amount to the existing fuel
            UpdateRemainingFuel(newBulkFuel);
        }

        private void UpdateRemainingFuelOnBulkEdit(decimal amountDifference)
        {
            if (con.State != ConnectionState.Open)
                con.Open();

            // Fetch the current remaining fuel
            decimal currentRemainingFuel = 0;
            cmd = new MySqlCommand("SELECT bulkRemainingFuel FROM bulkremainingfuel ORDER BY bulkRemainFuelID DESC LIMIT 1;", con);
            object result = cmd.ExecuteScalar();
            if (result != null)
                currentRemainingFuel = Convert.ToDecimal(result);

            // Adjust the remaining fuel based on the amount difference
            decimal newRemainingFuel = currentRemainingFuel + amountDifference;

            // Update the bulkremainingfuel table with the new remaining fuel value
            cmd = new MySqlCommand("INSERT INTO bulkremainingfuel (bulkRemainingFuel) VALUES (@remainingFuel);", con);
            cmd.Parameters.AddWithValue("@remainingFuel", newRemainingFuel);
            cmd.ExecuteNonQuery();

            con.Close();
        }



        // Fetch the old fuel consumption for the specific truck by fuel ID
        private decimal GetOldFuelConsumption(int fuelID)
        {
            decimal oldFuelConsumed = 0;
            if (con.State != ConnectionState.Open)
                con.Open();

            cmd = new MySqlCommand("SELECT qty FROM fuelmonitoring WHERE ID = @id", con);
            cmd.Parameters.AddWithValue("@id", fuelID);

            object result = cmd.ExecuteScalar();
            if (result != null)
                oldFuelConsumed = Convert.ToDecimal(result);

            con.Close();
            return oldFuelConsumed;
        }

        // Fetch the old bulk amount for the specific bulkID
        private decimal GetOldBulkAmount(int bulkID)
        {
            decimal oldBulkAmount = 0;
            if (con.State != ConnectionState.Open)
                con.Open();

            cmd = new MySqlCommand("SELECT amount FROM fuelbulk WHERE bulkID = @id", con);
            cmd.Parameters.AddWithValue("@id", bulkID);

            object result = cmd.ExecuteScalar();
            if (result != null)
                oldBulkAmount = Convert.ToDecimal(result);

            con.Close();
            return oldBulkAmount;
        }



        // methods for remaining fuel


        public void clearAll()
        {
           
           txtRequestedBy.Clear();
           cbDepartment.SelectedIndex = 0;
           txtQty.Clear();
           txtBulkAmount.Clear();
           txtPricePerLitter.Clear();
           txtTotalBP.Clear();
           txtRequestedBy.Clear();
           txtQty.Clear();
       
        }

        // Selection Change
        private void data_Fuel_SelectionChanged(object sender, EventArgs e)
        {
            if (data_Fuel.SelectedRows.Count > 0)
            {
                txtFuelID.Text = data_Fuel.SelectedRows[0].Cells[0].Value.ToString();
                txtFuelDatePicker.Value = Convert.ToDateTime(data_Fuel.SelectedRows[0].Cells[1].Value);
                txtPlateNum.Text = data_Fuel.SelectedRows[0].Cells[2].Value.ToString();

                string department = data_Fuel.SelectedRows[0].Cells[3].Value.ToString();
                int departmentIndex = cbDepartment.FindStringExact(department);

                if (departmentIndex != -1)
                {
                    cbDepartment.SelectedIndex = departmentIndex;
                }
                else
                {
                    cbDepartment.SelectedIndex = -1;
                }

                txtRequestedBy.Text = data_Fuel.SelectedRows[0].Cells[4].Value.ToString();
                txtQty.Text = data_Fuel.SelectedRows[0].Cells[5].Value.ToString();
            }
        }

        private void FuelBulk_Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (FuelBulk_Grid.SelectedRows.Count > 0)
            {
                txtBulkID.Text = FuelBulk_Grid.SelectedRows[0].Cells[0].Value.ToString();
                DTBulk.Value = Convert.ToDateTime(FuelBulk_Grid.SelectedRows[0].Cells[1].Value);
                txtBulkAmount.Text = FuelBulk_Grid.SelectedRows[0].Cells[2].Value.ToString();
                txtPricePerLitter.Text = FuelBulk_Grid.SelectedRows[0].Cells[3].Value.ToString();
                txtTotalBP.Text = FuelBulk_Grid.SelectedRows[0].Cells[4].Value.ToString();
            }

        }

      
        private void Fuelmonitoring_load()
        {
            data_Fuel.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT ID, fuel_date, plateNum, department, requestedBy, qty, totalFuelAmount FROM fuelmonitoring;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                data_Fuel.Rows.Add(rdr.GetInt32(0), rdr.GetDateTime(1).ToString("yyyy-MM-dd"),
                rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetDecimal(5), rdr.GetDecimal(6));

            }
            con.Close();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }
        private void PerformSearch()
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                Fuelmonitoring_load();
                return;
            }

            data_Fuel.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT ID, fuel_date, plateNum, department, requestedBy, qty, totalFuelAmount FROM fuelmonitoring WHERE CONCAT(ID, ' ', fuel_date, ' ', plateNum, ' ', department, ' ', requestedBy, ' ', qty, ' ', totalFuelAmount) LIKE @searchText;", con);
            cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                data_Fuel.Rows.Add(rdr.GetInt32(0), rdr.GetDateTime(1).ToString("yyyy-MM-dd"),
                rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetDecimal(5), rdr.GetDecimal(6));
            }

            con.Close();
        }
        private decimal GetLatestPricePerLitter()
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT PricePerLitter FROM fuelbulk ORDER BY bulkID DESC LIMIT 1;", con))
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                object result = cmd.ExecuteScalar();
                return (result != null && result != DBNull.Value) ? Convert.ToDecimal(result) : 0;
            }
        }
        private void cbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSortBy.SelectedItem == null)
            {
                return; // or handle this case as needed
            }

            string selectedSortCriteria = cbSortBy.SelectedItem.ToString();
            string orderByClause = "";

            switch (selectedSortCriteria)
            {

                case "Justine's Cargo":
                    lblDept.Text = "Justine's Cargo";
                    orderByClause = "ORDER BY totalFuelAmount DESC";
                    break;
                case "Justine's Scrap":
                    lblDept.Text = "Justine's Scrap";
                    orderByClause = "ORDER BY totalFuelAmount ASC";
                    break;
                case "Bodega 3":
                    lblDept.Text = "Bodega 3";
                    orderByClause = "ORDER BY totalFuelAmount DESC";
                    break;
                default:
                    lblDept.Text = "All Companies";
                    orderByClause = "";
                    break;
            }

            LoadFuelDeptData(orderByClause);
        }



        private void LoadFuelDeptData(string orderByClause)
        {
            try
            {
                con.Open();
                string selectedDeptFuel = "SELECT fm.ID, fm.fuel_date, fm.plateNum, fm.department, fm.requestedBy, fm.qty, fb.PricePerLitter, fm.totalFuelAmount " +
                                          "FROM fuelmonitoring fm " +
                                          "JOIN fuelbulk fb ON fm.bulkID = fb.bulkID ";

                if (lblDept.Text != "All Companies")
                {
                    selectedDeptFuel += "WHERE fm.department = @selectedCompany ";
                }

                selectedDeptFuel += orderByClause + ";";

                cmd = new MySqlCommand(selectedDeptFuel, con);

                if (lblDept.Text != "All Companies")
                {
                    cmd.Parameters.AddWithValue("@selectedCompany", lblDept.Text);
                }

                rdr = cmd.ExecuteReader();
                data_Fuel.Rows.Clear();

                while (rdr.Read())
                {
                    data_Fuel.Rows.Add(
                        rdr.GetInt32(0), rdr.GetDateTime(1), rdr.GetString(2), rdr.GetString(3),
                        rdr.GetString(4), rdr.GetDecimal(5), rdr.GetDecimal(6), rdr.GetDecimal(7));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading fuel data: {ex.Message}", "Error");
            }
            finally
            {
                if (rdr != null && !rdr.IsClosed)
                {
                    rdr.Close();
                }
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

            CalculateFuelTotal(); // Calculate the total fuel
        }


        //Calculate Total Fuel Cost By deparment
        private void CalculateFuelTotal()
        {
            try
            {
                con.Open();
                string selectedCompany = lblDept.Text;
                string selectedDeptFuel = "SELECT SUM(fm.totalFuelAmount) " +
                                          "FROM fuelmonitoring fm " +
                                          "JOIN fuelbulk fb ON fm.bulkID = fb.bulkID ";

                if (selectedCompany != "All Companies")
                {
                    selectedDeptFuel += "WHERE fm.department = @selectedCompany ";
                }

                cmd = new MySqlCommand(selectedDeptFuel, con);

                if (selectedCompany != "All Companies")
                {
                    cmd.Parameters.AddWithValue("@selectedCompany", selectedCompany);
                }

                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    decimal totalFuelAmount = Convert.ToDecimal(result);
                    txtFuelTotal.Text = $"₱{totalFuelAmount:N2}";

                }
                else
                {
                    txtFuelTotal.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while calculating the total fuel: {ex.Message}", "Error");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private bool fuelTrucksValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtPlateNum.Text))
            {
                MessageBox.Show("Plate number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(cbDepartment.Text))
            {
                MessageBox.Show("Department is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtRequestedBy.Text))
            {
                MessageBox.Show("Requested by field is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtQty.Text))
            {
                MessageBox.Show("Quantity is required.", "Error");
                return false;
            }
            if (!decimal.TryParse(txtQty.Text, out _))
            {
                MessageBox.Show("Quantity must be a valid number.", "Error");
                return false;
            }


            return true;
        }

        private bool fuelBulkValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtBulkAmount.Text))
            {
                MessageBox.Show("Amount is required.", "Error");
                return false;
            }
            if (!decimal.TryParse(txtBulkAmount.Text, out _))
            {
                MessageBox.Show("Amount must be a valid number.", "Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPricePerLitter.Text))
            {
                MessageBox.Show("Price Per Litter is required.", "Error");
                return false;
            }
            if (!decimal.TryParse(txtPricePerLitter.Text, out _))
            {
                MessageBox.Show("Price Per Litter must be a valid number.", "Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTotalBP.Text))
            {
                MessageBox.Show("Total bulk price is required.", "Error");
                return false;
            }
            if (!decimal.TryParse(txtTotalBP.Text, out _))
            {
                MessageBox.Show("Total bulk price must be a valid number.", "Error");
                return false;
            }
            return true;
        }

        //Validate Fuel consumption
        private bool verifyFuelConsumption(decimal fuelConsumed)
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
                    if (remainingFuel >= fuelConsumed)
                    {
                        // There's enough remaining fuel
                        return true;
                    }
                    else
                    {
                        // Not enough remaining fuel
                        MessageBox.Show("Fuel not enough. Please refill before adding fuel consumption.", "Error");
                        return false;
                    }
                }
                else
                {
                    // No data found for remaining fuel
                    MessageBox.Show("No remaining fuel data found.", "Error");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while checking remaining fuel: {ex.Message}", "Error");
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private bool ValidateBulkEdit(decimal newBulkAmount, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Fetch the current remaining fuel
            decimal currentRemainingFuel = 0;
            if (con.State != ConnectionState.Open)
                con.Open();

            cmd = new MySqlCommand("SELECT bulkRemainingFuel FROM bulkremainingfuel ORDER BY bulkRemainFuelID DESC LIMIT 1;", con);
            object result = cmd.ExecuteScalar();
            if (result != null)
                currentRemainingFuel = Convert.ToDecimal(result);

            con.Close();

            // Calculate the difference between the new bulk amount and the current remaining fuel
            decimal oldBulkAmount = GetOldBulkAmount(int.Parse(txtBulkID.Text));
            decimal amountDifference = newBulkAmount - oldBulkAmount;

            // Check if the remaining fuel will become negative
            decimal newRemainingFuel = currentRemainingFuel + amountDifference;
            if (newRemainingFuel < 0)
            {
                errorMessage = "Error: Bulk amount will be set to negative. Please adjust the amount.";
                return false;
            }

            return true;
        }


        private void guna2ContextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        
        private void PopulateAvailablePlate()
        {
            txtPlateNum.Items.Clear();
            HashSet<string> uniqueNames = new HashSet<string>();

            con.Open();
            cmd = new MySqlCommand("SELECT CONCAT(plateNum) AS Plate FROM truck;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                string fullName = rdr.GetString("Plate");
                if (uniqueNames.Add(fullName))
                {
                    txtPlateNum.Items.Add(fullName);
                }
            }
            con.Close();
        }

        private void txtPlateNum_DropDown(object sender, EventArgs e)
        {
            PopulateAvailablePlate();
        }



        private void TruckConShowInt_Click(object sender, EventArgs e)
        {
            if (sender == TruckConShowIntAdd)
            {
                panelFuelConAdd.Visible = true;
                panelFuelConAdd.BringToFront();
                btnFuelEdit.Visible = false;
                btnFuelAdd.Visible = true;
                lblTxtFuelID.Visible = false;
                fuelConAddInfo.Visible = true;
                fuelConEditInfo.Visible = false;
                clearAll();
            }
        }

        private void fuelAddBackBtn_Click(object sender, EventArgs e)
        {
            if (sender == fuelAddBackBtn)
            {
                panelFuelConAdd.Visible = false;
            }
        }

      

        private void viewFuelBulk_Click(object sender, EventArgs e)
        {
            if (sender == viewFuelBulk)
            {
                fuelBulkMainPanel.BringToFront();    
                fuelBulkMainPanel.Visible = true;
            }     
        }

        private void newFuelBulkShow_Click(object sender, EventArgs e)
        {
            if (sender == newFuelBulkShow)
            {
                txtBulkID.Visible = false;
                FuelBulkPanelInput.Visible = true;
                FuelBulkPanelInput.BringToFront();
                EditBulkInfo.Visible = false;
                newBulkFuelnfo.Visible = true;
                btnBulkADD.Visible = true;
                btnBulkEdit.Visible = false;
                clearAll();

            }
        }

        private void truckShowConEdit_Click(object sender, EventArgs e)
        {
            if (data_Fuel.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select data in the DataGrid first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
            btnFuelAdd.Visible = false;
            btnFuelEdit.Visible = true;
            panelFuelConAdd.Visible = true;
            panelFuelConAdd.BringToFront();
            fuelConAddInfo.Visible = false;
            fuelConEditInfo.Visible = true;
        }

        private void editFuelBulkShow_Click(object sender, EventArgs e)
        {
            if (FuelBulk_Grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select data in the DataGrid first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            if (sender == editFuelBulkShow)
            {
                txtBulkID.Visible = false;
                FuelBulkPanelInput.Visible = true;
                FuelBulkPanelInput.BringToFront();
                newBulkFuelnfo.Visible = false;
                btnBulkADD.Visible = false;
                btnBulkEdit.Visible=true;
                EditBulkInfo.Visible = true;
   

            }
        }

        private void returnFuelCon_Click(object sender, EventArgs e)
        {
            if (sender == returnFuelCon)
            {
                fuelBulkMainPanel.Visible = false;
            }
        }

        private void FuelBulkPanelInput_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fuelBulkBack_Click(object sender, EventArgs e)
        {
            if (sender == fuelBulkBack)
            {
                FuelBulkPanelInput.Visible = false;
            }
        }

        
    }
}