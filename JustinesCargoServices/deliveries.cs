using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    public partial class deliveries : Form
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
        public deliveries()
        {
            InitializeComponent();
            this.Resize += Deliveries_Resize;
            timerSlide.Tick += timerSlide_Tick;
            timerSlide.Interval = 10; 
            pnlEdit.Visible = false;   
            originalY = this.ClientSize.Height; 
            pnlEdit.Top = originalY;

        }

        private void deliveries_Load(object sender, EventArgs e)
        {
            Delivery_ID();
            LoadWaybillNumbers1();
            LoadWaybillNumbers();
            Delivery_data();
            //Delivery_dataa();
           
            Delivery_IDD();
            Delivery_multi();
            PopulateAvailableNames();
            delivery_multii.Columns["IDD"].Visible = false;
        }
        private void LoadWaybillNumbers()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    // Query to fetch waybillNo from waybill table excluding those in deliverydata and delivery_multi
                    string query = @"
            SELECT waybillNo 
            FROM waybill 
            WHERE waybillNo NOT IN (
                SELECT waybillNo FROM deliverydata
                UNION
                SELECT waybillNo FROM delivery_multi
            );";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            txtWB.Items.Clear(); // Clear existing items in the ComboBox

                            while (reader.Read())
                            {
                                txtWB.Items.Add(reader["waybillNo"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading waybill numbers: " + ex.Message);
            }
        }

        private void LoadWaybillNumbers1()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    // Query to fetch waybillNo from waybill table excluding those in deliverydata and delivery_multi
                    string query = @"
            SELECT waybillNo 
            FROM waybill 
            WHERE waybillNo NOT IN (
                SELECT waybillNo FROM deliverydata
                UNION
                SELECT waybillNo FROM delivery_multi
            );";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            txtW.Items.Clear(); // Clear existing items in the ComboBox

                            while (reader.Read())
                            {
                                txtW.Items.Add(reader["waybillNo"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading waybill numbers: " + ex.Message);
            }
        }


        private void ClearAll()
        {
            txtID.Clear();
          
            txtAmount_.Clear();

          



            txtD.Clear();
            txtA.Clear();
        
            txtPT.SelectedIndex = -1;
            txtPT.Text = "";
            txtOrigin.Clear();
            txtmop.SelectedIndex = -1;
            txtmop.Text = "";
            txtRM.Clear();



            dropRemarks.Clear();

            txtdriversName.Clear();
            txtOrigin.Clear();
            dropMop1.SelectedIndex = -1;
            dropMop1.Text = "";
            cbmPaymentTerms.SelectedIndex = -1;
            cbmPaymentTerms.Text = "";
            txtWaybillNo.SelectedIndex = -1;
            txtWaybillNo.Text = "";
        }
        private void populateWaybillNo()
        {
            try
            {
                // Use a local connection string as in Delivery_data
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    // Query to fetch WayBillNo not in delivery_multi and deliverydata
                    string query = @"
                SELECT WayBillNo 
                FROM waybillnum 
                WHERE WayBillNo NOT IN (
                    SELECT waybillNo FROM delivery_multi
                )
                AND WayBillNo NOT IN (
                    SELECT waybillNo FROM deliverydata
                );";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            txtW.Items.Clear(); // Clear existing items

                            while (rdr.Read())
                            {
                                txtW.Items.Add(rdr["WayBillNo"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Waybill Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void populateWaybillNo1()
        {
            try
            {
                // Use a local connection string as in Delivery_data
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    // Query to fetch WayBillNo not in delivery_multi and deliverydata
                    string query = @"
                SELECT WayBillNo 
                FROM waybillnum 
                WHERE WayBillNo NOT IN (
                    SELECT waybillNo FROM delivery_multi
                )
                AND WayBillNo NOT IN (
                    SELECT waybillNo FROM deliverydata
                );";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            txtW.Items.Clear(); // Clear existing items

                            while (rdr.Read())
                            {
                                txtW.Items.Add(rdr["WayBillNo"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Waybill Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PopulateAvailableNames()
        {
            txtDriverName.Items.Clear();
            HashSet<string> uniqueNames = new HashSet<string>();
            HashSet<string> existingDrivers = new HashSet<string>();

            string connectionString = "datasource=localhost;port=3306;database=jcsdb;username=root;password=''";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();


                    using (MySqlCommand cmd = new MySqlCommand("SELECT DriversName FROM delivery;", con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string driverName = rdr.GetString("DriversName");
                                existingDrivers.Add(driverName);
                            }
                        }
                    }


                    string query = "SELECT CONCAT(empLname, ', ', empFname) AS FullName " +
                                   "FROM empprofiling " +
                                   "WHERE Position = 'Driver' AND dateSeperated IS NULL;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string fullName = rdr.GetString("FullName");


                                if (uniqueNames.Add(fullName) && !existingDrivers.Contains(fullName))
                                {
                                    txtDriverName.Items.Add(fullName);
                                }
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        public void AdjustLayout(int sidebarWidth)
        {
            int availableWidth = this.Parent.ClientSize.Width - sidebarWidth;
            this.Width = availableWidth;
            this.Height = this.Parent.ClientSize.Height;
        }

        private void Deliveries_Resize(object sender, EventArgs e)
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
                    else
                    {
                        MessageBox.Show("Sidebar control not found.");
                    }
                }
            }
        }

        private void LoadFormInDashboardPanel(Form form)
        {
            if (this.Parent is Dashboard parentDashboard)
            {
                // Find the panelContent in the parent (Dashboard)
                var panelContent = parentDashboard.Controls["panelContent"] as Panel;

                if (panelContent != null)
                {
                    // Clear existing controls in panelContent
                    panelContent.Controls.Clear();

                    // Set the new form as a child of panelContent
                    form.TopLevel = false;
                    form.Dock = DockStyle.Fill;
                    panelContent.Controls.Add(form);
                    form.Show();
                }
                else
                {
                    MessageBox.Show("PanelContent not found in Dashboard.");
                }
            }
            else
            {
                MessageBox.Show("This form is not loaded in a Dashboard form.");
            }
        }

        // Updated method for the button 'addDel'
        private void addDel_Click(object sender, EventArgs e)
        {
            // Instantiate addDelivery and load it into the panelContent of the Dashboard
           
        }

        private void pnlDelContents_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Delivery_data()
        {
            deliveryData.Rows.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "SELECT LiquidationNO, waybillNo, Reference, PlateNo, Origin, Destination1, Amount1, " +
                                   "ModOfpay, PaymentTerms, Remarks, PayDate, LoadingDate, ArrivalDate, Driver " +
                                   "FROM deliverydata;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                deliveryData.Rows.Add(
                                    rdr.GetInt32(0), // LiquidationNO
                                    rdr.GetString(1), // waybillNo
                                    rdr.GetString(2), // Reference
                                    rdr.GetString(3), // PlateNo
                                    rdr.GetString(4), // Origin
                                    rdr.GetString(5), // Destination1
                                    rdr.GetString(6), // Amount1
                                    rdr.GetString(7), // ModOfpay
                                    rdr.GetString(8), // PaymentTerms
                                    rdr.GetString(9), // Remarks
                                                      // Safely handle null or invalid DateTime values:
                                    rdr.IsDBNull(10) ? (object)DBNull.Value : rdr.GetDateTime(10).ToString("yyyy-MM-dd"),
                                    rdr.IsDBNull(11) ? (object)DBNull.Value : rdr.GetDateTime(11).ToString("yyyy-MM-dd"),
                                    rdr.IsDBNull(12) ? (object)DBNull.Value : rdr.GetDateTime(12).ToString("yyyy-MM-dd"),
                                    rdr.GetString(13) // Driver
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


        private void Delivery_dataa()
        {
            deliveryDataa.Rows.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "SELECT LiquidationNO, PlateNo, Reference, LoadingDate, Destination1, Amount1, Driver FROM deliverydata;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                                deliveryDataa.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                     rdr.GetString(2),
                                    rdr.IsDBNull(3) ? (object)DBNull.Value : rdr.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6)

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
        private void Delivery_ID()
        {
            dataDeliveryId.Rows.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "SELECT ID, PlateNo, Category, DriversName, Status FROM delivery;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                                dataDeliveryId.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.GetString(3),
                                    rdr.GetString(4)
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
        private void Delivery_IDD()
        {
            Driver_ID.Rows.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "SELECT ID, PlateNo, Category, DriversName, Status FROM delivery;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                                Driver_ID.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.GetString(3),
                                    rdr.GetString(4)
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
        private void Delivery_multi()
        {
            delivery_multii.Rows.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();
                     
                    string query = "SELECT ID, waybillNo, LiquidationNO, PlateNo, Reference, origin, Destination, Amount2, ModOfpay, PaymentTerms, Remarks,LoadingDate,  ArrivalDate,PayDate, Driver FROM delivery_multi;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                                delivery_multii.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetInt32(2),
                                    rdr.GetString(3),
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6),
                                    rdr.GetString(7),
                                    rdr.GetString(8),
                                    rdr.GetString(9),
                                    rdr.GetString(10),
                                    rdr.IsDBNull(11) ? (object)DBNull.Value : rdr.GetDateTime(11).ToString("yyyy-MM-dd"), 
                                    rdr.IsDBNull(12) ? (object)DBNull.Value : rdr.GetDateTime(12).ToString("yyyy-MM-dd"), 
                                    rdr.IsDBNull(13) ? (object)DBNull.Value : rdr.GetDateTime(13).ToString("yyyy-MM-dd"), 
                                    rdr.GetString(14)
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
        private void btnAssignDrivers_Click(object sender, EventArgs e)
        {
            pnlAssignD.Visible = !pnlAssignD.Visible;
          

        }

       

        private void btnAssignD_Click(object sender, EventArgs e)
        {
            pnlDIdenti.Visible = !pnlDIdenti.Visible;
        }

        private void dataDeliveryId_SelectionChanged(object sender, EventArgs e)
        {
            if (dataDeliveryId.SelectedRows.Count > 0)
            {
                txtID.Text = dataDeliveryId.SelectedRows[0].Cells[0].Value.ToString();
                txtPlate.Text = dataDeliveryId.SelectedRows[0].Cells[1].Value.ToString();
               
                txtCategory.Text = dataDeliveryId.SelectedRows[0].Cells[2].Value.ToString();
                txtDriverName.Text = dataDeliveryId.SelectedRows[0].Cells[3].Value.ToString();
               
                txtStatus.Text = dataDeliveryId.SelectedRows[0].Cells[4].Value.ToString();





            }
        }

        private void delivery_multi_SelectionChanged(object sender, EventArgs e)
        {

        }
        private bool ValidateDeliveriesInput()
        {
            if (string.IsNullOrWhiteSpace(txtPlate.Text))
            {
                MessageBox.Show("Plate number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Category is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDriverName.Text))
            {
                MessageBox.Show("Driver's Name is required.", "Error");
                return false;
            }
           

            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateDeliveriesInput()) return;
            if (dataDeliveryId.SelectedRows.Count > 0)
            {
                con.Open();
                cmd = new MySqlCommand("UPDATE delivery SET " +
                   "PlateNo = @PlateNo, " +
                   "Category = @Category, " +
                   "DriversName = @DriversName, " +
                   "Status = @Status " +
                   "WHERE ID = @ID;", con);
                cmd.Parameters.AddWithValue("@PlateNo", txtPlate.Text);
                cmd.Parameters.AddWithValue("@Category", txtCategory.Text);
                cmd.Parameters.AddWithValue("@DriversName", txtDriverName.Text);
                cmd.Parameters.AddWithValue("@Status", txtStatus.Text);
                cmd.Parameters.AddWithValue("@ID", txtID.Text);

                cmd.ExecuteNonQuery();


                if (txtStatus.Text == "Completed")
                {
                    cmd = new MySqlCommand("UPDATE truck SET status = 'Available' WHERE plateNum = @PlateNo;", con);
                    cmd.Parameters.AddWithValue("@PlateNo", txtPlate.Text);
                    cmd.ExecuteNonQuery();
                }

                else if (txtStatus.Text == "On Delivery")
                {
                    cmd = new MySqlCommand("UPDATE truck SET status = 'On delivery' WHERE plateNum = @PlateNo;", con);
                    cmd.Parameters.AddWithValue("@PlateNo", txtPlate.Text);
                    cmd.ExecuteNonQuery();
                }
                else if (txtStatus.Text == "Under Maintenance")
                {
                    cmd = new MySqlCommand("UPDATE truck SET status = 'Under Maintenance' WHERE plateNum = @PlateNo;", con);
                    cmd.Parameters.AddWithValue("@PlateNo", txtPlate.Text);
                    cmd.ExecuteNonQuery();
                }

                con.Close();
                MessageBox.Show("Success");

                Delivery_ID();
                ClearAll();
                PopulateAvailableNames();
            }
            else
            {
                MessageBox.Show("No record selected!");
            }
        }

        private void deliveryData_SelectionChanged(object sender, EventArgs e)
        {
            if (deliveryData.SelectedRows.Count > 0)
            {
                txtTID.Text = deliveryData.SelectedRows[0].Cells[0].Value.ToString(); // LiquidationNO
                txtL.Text = deliveryData.SelectedRows[0].Cells[0].Value.ToString(); // LiquidationNO
                txtWB.Text = deliveryData.SelectedRows[0].Cells[1].Value.ToString(); // waybillNo
                txtRef.Text = deliveryData.SelectedRows[0].Cells[2].Value.ToString();
                txtR.Text = deliveryData.SelectedRows[0].Cells[2].Value.ToString();
                txtPlateee.Text = deliveryData.SelectedRows[0].Cells[3].Value.ToString(); // PlateNo
                txtP.Text = deliveryData.SelectedRows[0].Cells[3].Value.ToString(); // PlateNo
                txtoriginn.Text = deliveryData.SelectedRows[0].Cells[4].Value.ToString(); // Origin
                txtDestinationn.Text = deliveryData.SelectedRows[0].Cells[5].Value.ToString(); // Destination1
                txtAmountt.Text = deliveryData.SelectedRows[0].Cells[6].Value.ToString(); // Amount1
                txtMOPP.Text = deliveryData.SelectedRows[0].Cells[7].Value.ToString(); // ModOfpay
                txtPAYMENTERMS.Text = deliveryData.SelectedRows[0].Cells[8].Value.ToString(); // PaymentTerms
                txtRemarks.Text = deliveryData.SelectedRows[0].Cells[9].Value.ToString(); // Remarks

                txtPDD.Value = deliveryData.SelectedRows[0].Cells[10].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(deliveryData.SelectedRows[0].Cells[10].Value);
                dateLdate.Value = deliveryData.SelectedRows[0].Cells[11].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(deliveryData.SelectedRows[0].Cells[11].Value); // LoadingDate
                dateArrival.Value = deliveryData.SelectedRows[0].Cells[12].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(deliveryData.SelectedRows[0].Cells[12].Value); // ArrivalDate
                txtDriverss.Text = deliveryData.SelectedRows[0].Cells[13].Value.ToString(); // Driver
            }

        }

        private void btnSaveA_Click(object sender, EventArgs e)
        {
            if (!ValidateDeliveryDataInput()) return; // Validate input before proceeding

            if (deliveryData.SelectedRows.Count > 0)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                    {
                        con.Open();

                        // Prepare the SQL command
                        string query = "UPDATE delivery_multi SET " +
                                       "ArrivalDate = @ArrivalDate, " +
                                        "LoadingDate = @LoadingDate, " +
                                       "Destination = @Destination, " +
                                       "Amount2 = @Amount2, " +
                                       "ModOfPay = @ModOfPay, " +
                                       "PayDate = @PayDate, " +
                                       "PaymentTerms = @PaymentTerms, " +
                                       "Remarks = @Remarks, " +
                                       "PlateNo = @PlateNo, " +
                                       "Origin = @Origin, " +
                                       "Reference = @Reference, " +
                                       "WaybillNo = @WaybillNo, " +
                                       "Driver = @Driver " +
                                       "WHERE ID = @ID;";

                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            // Bind parameters
                            cmd.Parameters.AddWithValue("@ID", txtI.Text); // ID from the txtI TextBox
                            cmd.Parameters.AddWithValue("@ArrivalDate", dateA.Value.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@LoadingDate", dateLoadingDatee.Value.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@Destination", txtD.Text);
                            cmd.Parameters.AddWithValue("@Amount2", txtA.Text);
                            cmd.Parameters.AddWithValue("@ModOfPay", txtmop.Text);
                            cmd.Parameters.AddWithValue("@PayDate", datePD.Value.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@PaymentTerms", txtPT.Text);
                            cmd.Parameters.AddWithValue("@Remarks", txtRM.Text);
                            cmd.Parameters.AddWithValue("@PlateNo", txtP.Text);
                            cmd.Parameters.AddWithValue("@Origin", txtOrigin.Text);
                            cmd.Parameters.AddWithValue("@Reference", txtR.Text);
                            cmd.Parameters.AddWithValue("@WaybillNo", txtW.Text);
                            cmd.Parameters.AddWithValue("@Driver", txtDRI.Text);

                            // Execute the query
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Record updated successfully!", "Success");

                    // Reload data and reset fields

                    LoadWaybillNumbers1();
                    LoadWaybillNumbers();
                    loadmultiDel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error");
                }
            }
            else
            {
                MessageBox.Show("Please select a record to update!", "Error");
            }
        }

        private bool ValidateDeliveryDataInput()
        {

            if (string.IsNullOrWhiteSpace(txtD.Text))
            {
                MessageBox.Show("Destination is required.", "Error");
                return false;

            }
            if (string.IsNullOrWhiteSpace(txtA.Text))
            {
                MessageBox.Show("Amount is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtmop.Text))
            {
                MessageBox.Show("Mode of Payment is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPT.Text))
            {
                MessageBox.Show("Payment Terms is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtRM.Text))
            {
                MessageBox.Show("Remarks is required.", "Error");
                return false;
            }

            return true;
        }
        private void btnAddDeliveries_Click(object sender, EventArgs e)
        {
            if (!ValidateDeliveryDataInput()) return;

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    // Debug date values
                    Console.WriteLine($"LoadingDate: {dateLoadingDatee.Value.ToString("yyyy-MM-dd")}");
                    Console.WriteLine($"PD: {datePD.Value.ToString("yyyy-MM-dd")}");

                    // Insert into delivery_multi table
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO delivery_multi (" +
                        " LiquidationNO, PlateNo, origin, Destination, Reference, Amount2, ModOfPay, waybillNo, PaymentTerms, LoadingDate, ArrivalDate, Remarks, PayDate, Driver) " +
                        "VALUES (@LiquidationNO, @PlateNo, @origin, @Destination, @Reference, @Amount2, @ModOfPay, @WaybillNo, @PaymentTerms, @LoadingDate, @ArrivalDate, @Remarks, @PD, @Driver);", con))
                    {
                        cmd.Parameters.AddWithValue("@LiquidationNO", txtL.Text);
                        cmd.Parameters.AddWithValue("@PlateNo", txtP.Text);
                        cmd.Parameters.AddWithValue("@origin", txtOrigin.Text);
                        cmd.Parameters.AddWithValue("@Destination", txtD.Text);
                        cmd.Parameters.AddWithValue("@Reference", txtR.Text);
                        cmd.Parameters.AddWithValue("@Amount2", txtA.Text);
                        cmd.Parameters.AddWithValue("@ModOfPay", txtmop.Text);
                        cmd.Parameters.AddWithValue("@WaybillNo", txtW.Text);
                        cmd.Parameters.AddWithValue("@PaymentTerms", txtPT.Text);
                        cmd.Parameters.AddWithValue("@Remarks", txtRM.Text);

                        // Ensure date values are valid
                        cmd.Parameters.AddWithValue("@PD", datePD.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@LoadingDate", dateLoadingDatee.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@ArrivalDate", PickerAdate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Driver", txtDRI.Text);

                        cmd.ExecuteNonQuery();
                    }

                    // Update waybillnum table
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE waybillnum SET Status = 'USED' WHERE WayBillNo = @WayBillNo;", con))
                    {
                        cmd.Parameters.AddWithValue("@WayBillNo", txtW.Text);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Delivery Data Inserted Successfully!", "Success");

                    // Refresh the UI
                    LoadWaybillNumbers1();
                    LoadWaybillNumbers();
                    loadmultiDel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error");
            }
        }




            private void btnHasArrive_Click(object sender, EventArgs e)
        {
         
            if (deliveryData.SelectedRows.Count > 0)
            {
                con.Open();

                int selectedId = Convert.ToInt32(deliveryData.SelectedRows[0].Cells[0].Value);
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");


                cmd = new MySqlCommand("UPDATE delivery_multi SET " +
                    "ArrivalDate = @ArrivalDate " +
                    
                    "WHERE ID = @ID;", con);

                cmd.Parameters.AddWithValue("@ID", txtI.Text);

                cmd.Parameters.AddWithValue("@ArrivalDate", PickerAdate.Value.ToString("yyyy-MM-dd"));
                
       

                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("UPDATE delivery SET " +
                   "Status = @Status " +
                   "WHERE plateNo = @PlateNo;", con);
                cmd.Parameters.AddWithValue("@Status", "Available");
                cmd.Parameters.AddWithValue("@PlateNo", txtPlateNo.Text);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("UPDATE truck SET " +
                   "status = @Status " +
                   "WHERE plateNum = @PlateNo;", con);
                cmd.Parameters.AddWithValue("@Status", "Available");
                cmd.Parameters.AddWithValue("@PlateNo", txtPlateNo.Text);
                cmd.ExecuteNonQuery();



                cmd.ExecuteNonQuery();


                con.Close();

                MessageBox.Show("Arrival Date Updated Successfully!", "Success");

            
                populateWaybillNo();
                loadmultiDel();

            }
            else
            {
                MessageBox.Show("Please select a record to update!", "Error");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (deliveryData.SelectedRows.Count > 0)
            {
                con.Open();
                int selectedId = Convert.ToInt32(txtLiquidation.Text);
                MySqlCommand cmd = new MySqlCommand("DELETE FROM delivery_multi WHERE LiquidationNO = @selectedId", con);
                cmd.Parameters.AddWithValue("@selectedId", selectedId);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                txtL.Text = string.Empty;


               
                con.Close();
                MessageBox.Show("Successfully deleted!");
                Delivery_data();
            }
            else
            {
                MessageBox.Show("FAILED!!");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            Delivery_multi();
        }

        private void btnAddLoading_Click(object sender, EventArgs e)
        {
            
         
            pnlDeliveryList.Visible = !pnlDeliveryList.Visible;
        }
        private operationalexpenses opform;
        private void btnOperationalEx_Click(object sender, EventArgs e)
        {
            if (opform == null || opform.IsDisposed)
            {
                opform = new operationalexpenses();
                
                opform.Show();
                
            }
            else
           {
               opform.BringToFront();
           }
        }
        private void loadmultiDel()
        {
            string loadData = txtL.Text.Trim();
            if (!string.IsNullOrEmpty(loadData))
            {
                delivery_multii.Rows.Clear();
                con.Open();
                cmd = new MySqlCommand("SELECT ID, waybillNo, LiquidationNO, PlateNo, Reference, origin, Destination, Amount2, ModOfpay, PaymentTerms, Remarks, LoadingDate,  ArrivalDate,PayDate, Driver FROM delivery_multi WHERE LiquidationNO LIKE @loadData;", con);
                cmd.Parameters.AddWithValue("@loadData", "%" + loadData + "%");
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    delivery_multii.Rows.Add(
                                    rdr.GetInt32(0),
                                   rdr.GetString(1),
                                   rdr.GetInt32(2),
                                   rdr.GetString(3),
                                   rdr.GetString(4),
                                   rdr.GetString(5),
                                   rdr.GetString(6),
                                   rdr.GetString(7),
                                   rdr.GetString(8),
                                   rdr.GetString(9),
                                   rdr.GetString(10),
                                   rdr.IsDBNull(11) ? (object)DBNull.Value : rdr.GetDateTime(11).ToString("yyyy-MM-dd"), // ArrivalDate
                                   rdr.IsDBNull(12) ? (object)DBNull.Value : rdr.GetDateTime(12).ToString("yyyy-MM-dd"), // PayDate
                                    rdr.IsDBNull(13) ? (object)DBNull.Value : rdr.GetDateTime(13).ToString("yyyy-MM-dd"), // PayD
                                   rdr.GetString(14));
                }
                con.Close();
            }
            else
            {
                Delivery_data();
            }
        }
        private void btnLoadDeliveries_Click(object sender, EventArgs e)
        {
            string loadData = txtL.Text.Trim();
            if (!string.IsNullOrEmpty(loadData))
            {
                delivery_multii.Rows.Clear();
                con.Open();
                cmd = new MySqlCommand("SELECT LiquidationNO, PlateNo, Reference, ArrivalDate, Destination, Amount2, ModOfpay, PaymentTerms, PayDate, Remarks, waybillNo, Driver FROM delivery_multi WHERE LiquidationNO LIKE @loadData;", con);
                cmd.Parameters.AddWithValue("@loadData", "%" + loadData + "%");
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    delivery_multii.Rows.Add(rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.IsDBNull(3) ? (object)DBNull.Value : rdr.GetDateTime(3).ToString("yyyy-MM-dd"), // ArrivalDate
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6),
                                    rdr.GetString(7),
                                    rdr.IsDBNull(8) ? (object)DBNull.Value : rdr.GetDateTime(8).ToString("yyyy-MM-dd"), // PayDate
                                    rdr.GetString(9),
                                    rdr.GetString(10),
                                    rdr.GetString(11));
                }
                con.Close();
            }
            else
            {
                Delivery_data();
            }
        }

        private void deliveryDataa_SelectionChanged(object sender, EventArgs e)
        {
            if (deliveryDataa.SelectedRows.Count > 0)
            {
                txtLiquidationNo.Text = deliveryDataa.SelectedRows[0].Cells[0].Value.ToString();
                txtPlatee.Text = deliveryDataa.SelectedRows[0].Cells[1].Value.ToString();
                txtReference2.Text = deliveryDataa.SelectedRows[0].Cells[2].Value.ToString();
                PickerLdate2.Value = deliveryDataa.SelectedRows[0].Cells[3].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(deliveryData.SelectedRows[0].Cells[3].Value);
                txtDestination1.Text = deliveryDataa.SelectedRows[0].Cells[4].Value.ToString();
                txtAmount2.Text = deliveryDataa.SelectedRows[0].Cells[5].Value.ToString();

            }
        }

        private void btnRefreshLoadD_Click(object sender, EventArgs e)
        {
            Delivery_dataa();
        }

        private void btnLoadAssgn_Click(object sender, EventArgs e)
        {
            pnlLoadingD.Visible = !pnlLoadingD.Visible;
        }
        private bool ValidateLTOInputDIdentification()
        {

            if (string.IsNullOrWhiteSpace(txtReference2.Text))
            {
                MessageBox.Show("Reference number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAmount2.Text))
            {
                MessageBox.Show("Amount is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDestination1.Text))
            {
                MessageBox.Show("Destination is required.", "Error");
                return false;
            }
            return true;
        }
        private void btnAddLoadingData_Click(object sender, EventArgs e)
        {
            if (!ValidateLTOInputDIdentification()) return;
            if (dataDeliveryId.SelectedRows.Count > 0)
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string plateNumber = txtPlate.Text;
                string referenceNumber = txtReference2.Text;

                try
                {
                    con.Open();


                    string checkPlateQuery = "SELECT COUNT(*) FROM deliverydata WHERE PlateNo = @PlateNo AND LoadingDate = @LoadingDate;";
                    using (MySqlCommand checkPlateCmd = new MySqlCommand(checkPlateQuery, con))
                    {
                        checkPlateCmd.Parameters.AddWithValue("@PlateNo", plateNumber);
                        checkPlateCmd.Parameters.AddWithValue("@LoadingDate", currentDate);

                        int plateCount = Convert.ToInt32(checkPlateCmd.ExecuteScalar());

                        if (plateCount > 0)
                        {
                            MessageBox.Show("The plate number already exists for today. Please use a different plate number.", "REMINDER");
                            return;
                        }
                    }
                    string checkReferenceQuery = "SELECT COUNT(*) FROM deliverydata WHERE Reference = @Reference;";
                    using (MySqlCommand checkReferenceCmd = new MySqlCommand(checkReferenceQuery, con))
                    {
                        checkReferenceCmd.Parameters.AddWithValue("@Reference", referenceNumber);

                        int referenceCount = Convert.ToInt32(checkReferenceCmd.ExecuteScalar());

                        if (referenceCount > 0)
                        {
                            MessageBox.Show("Reference number already exists. Please use a different reference number.", "REMINDER");
                            return;
                        }
                    }




                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO deliverydata (PlateNo, LoadingDate, Destination1, Driver, Reference, Amount1) " +
                        "VALUES (@PlateNo, @LoadingDate, @Destination1, @Driver, @Reference, @Amount1);", con))
                    {
                        cmd.Parameters.AddWithValue("@PlateNo", plateNumber);
                        cmd.Parameters.AddWithValue("@LoadingDate", PickerLdate2.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Destination1", txtDestination1.Text);
                        cmd.Parameters.AddWithValue("@Driver", txtDriversname2.Text);
                        cmd.Parameters.AddWithValue("@Reference", referenceNumber);
                        cmd.Parameters.AddWithValue("@Amount1", txtAmount2.Text);
                        cmd.ExecuteNonQuery();
                    }

                    using (MySqlCommand cmd = new MySqlCommand("UPDATE delivery SET Status = @Status WHERE PlateNo = @PlateNo;", con))
                    {
                        cmd.Parameters.AddWithValue("@Status", "On Delivery");
                        cmd.Parameters.AddWithValue("@PlateNo", plateNumber);
                        cmd.ExecuteNonQuery();
                    }


                    using (MySqlCommand cmd = new MySqlCommand("UPDATE truck SET Status = @Status WHERE PlateNum = @PlateNum;", con))
                    {
                        cmd.Parameters.AddWithValue("@Status", "On Delivery");
                        cmd.Parameters.AddWithValue("@PlateNum", plateNumber);
                        cmd.ExecuteNonQuery();
                    }


                    Delivery_data();
                    Delivery_ID();
                    Delivery_IDD();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a record from Delivery ID to proceed.", "Error");
            }
        }

        private void btnShowDrivers_Click(object sender, EventArgs e)
        {
            Driver_ID.Visible = !Driver_ID.Visible;
        }

        private void Driver_ID_SelectionChanged(object sender, EventArgs e)
        {
            if (Driver_ID.SelectedRows.Count > 0)
            {
                txtLiquidationNo.Text = Driver_ID.SelectedRows[0].Cells[0].Value.ToString();
                txtPlateee.Text = Driver_ID.SelectedRows[0].Cells[1].Value.ToString();

                txtDriverss.Text = Driver_ID.SelectedRows[0].Cells[3].Value.ToString();
              

            }
        }

        private void pnlLoadingD_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnBackFromAssignD_Click(object sender, EventArgs e)
        {
            pnlAssignD.Visible = !pnlAssignD.Visible;

        }

        private void btnBackFromArrival_Click(object sender, EventArgs e)
        {
            pnlArrivalD.Visible = !pnlArrivalD.Visible;
            Delivery_data();
            Delivery_ID();
        }


        private void btnBackfromLoad_Click(object sender, EventArgs e)
        {
            pnlLoadingD.Visible = !pnlLoadingD.Visible;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            try
            {
                deliveryData.Rows.Clear(); // Clear the DataGridView before loading new data

                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "SELECT LiquidationNO, PlateNo, Reference, LoadingDate, Destination1, Amount1, Driver " +
                                   "FROM deliverydata";

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " WHERE CONCAT_WS(',', LiquidationNO, PlateNo, Reference, LoadingDate, Destination1, Amount1, Driver) LIKE @searchText;";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                        }

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                deliveryData.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetString(1),
                                    rdr.GetString(2),
                                    rdr.IsDBNull(3) ? (object)DBNull.Value : rdr.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtWaybillNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtWaybillNo.SelectedIndex != -1)
            {
                string selectedWaybillNo = txtWaybillNo.SelectedItem.ToString();


                string query = "SELECT Company1, Company2, Company3, TotalAmount FROM waybill WHERE waybillNo = @waybillNo";

                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@waybillNo", selectedWaybillNo);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {

                                string companyInfo = $"{rdr["Company1"]} / {rdr["Company2"]} / {rdr["Company3"]}";
                                txtDestination.Text = companyInfo;


                                txtAmount_.Text = rdr["TotalAmount"].ToString();
                            }
                        }
                    }
                }
            }
        }

        private void btnArrivalD_Click(object sender, EventArgs e)
        {
            pnlArrivalD.Visible = !pnlArrivalD.Visible;
            loadmultiDel();
            ClearAll();

        }
        private void DisableAutoSelect()
        {
            if (delivery_multii.Rows.Count > 0)
            {
                // Clear any selection
                delivery_multii.ClearSelection();
            }
        }
        private void timerSlide_Tick(object sender, EventArgs e)
        {
            if (isSlidingUp)
            {
                // Slide up
                if (pnlDeliveryList.Top > targetY)
                {
                    pnlDeliveryList.Top -= 5; // Adjust for smoother or faster sliding
                    if (pnlDeliveryList.Top <= targetY)
                    {
                        pnlDeliveryList.Top = targetY;
                        timerSlide.Stop();
                        isExpanded = true;
                    }
                }
            }
            else
            {
                // Slide down
                if (pnlDeliveryList.Top < originalY)
                {
                    pnlDeliveryList.Top += 5; // Adjust for smoother or faster sliding
                    if (pnlDeliveryList.Top >= originalY)
                    {
                        pnlDeliveryList.Top = originalY;
                        pnlDeliveryList.Visible = false; // Hide the panel after sliding down
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
                pnlDeliveryList.Visible = true; // Ensure the panel is visible
                targetY = (this.ClientSize.Height - pnlDeliveryList.Height) / 2;
                isSlidingUp = true;
            }
            else
            {
                // Slide down and hide
                isSlidingUp = false;
            }

            timerSlide.Start();
        }

        private void btnUpdateee_Click(object sender, EventArgs e)
        {
            if (!ValidateLTOInputDIdentification()) return;

            try
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(
                    "UPDATE deliverydata SET " +
                    "LoadingDate = @LoadingDate, " +
                    "Destination1 = @Destination1, " +
                    "Driver = @Driver, " +
                    "Reference = @Reference, " +
                    "Amount1 = @Amount1 " +
                    "WHERE PlateNo = @PlateNo;", con))
                {
                    // Bind parameters
                    cmd.Parameters.AddWithValue("@PlateNo", txtPlatee2.Text); 
                    cmd.Parameters.AddWithValue("@LoadingDate", PickerLdate3.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Destination1", txtDestination2.Text);
                    cmd.Parameters.AddWithValue("@Driver", txtDriversname3.Text);
                    cmd.Parameters.AddWithValue("@Reference", txtReference3.Text);
                    cmd.Parameters.AddWithValue("@Amount1", txtAmount3.Text);

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                // Refresh the data or perform other operations
                Delivery_data();
                Delivery_ID();
                Delivery_IDD();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
           
        }

        private void delivery_multii_SelectionChanged(object sender, EventArgs e)
        {
            if (delivery_multii.SelectedRows.Count > 0)
            {
                txtI.Text = delivery_multii.SelectedRows[0].Cells[1].Value.ToString();
                txtW.Text = delivery_multii.SelectedRows[0].Cells[2].Value.ToString();


                txtOrigin.Text = delivery_multii.SelectedRows[0].Cells[5].Value.ToString();
                txtD.Text = delivery_multii.SelectedRows[0].Cells[6].Value.ToString();
                txtA.Text = delivery_multii.SelectedRows[0].Cells[7].Value.ToString();
              
                txtmop.Text = delivery_multii.SelectedRows[0].Cells[8].Value.ToString();
                txtPT.Text = delivery_multii.SelectedRows[0].Cells[9].Value.ToString();
            
                txtRM.Text = delivery_multii.SelectedRows[0].Cells[10].Value.ToString();
                dateLoadingDatee.Value = delivery_multii.SelectedRows[0].Cells[11].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(delivery_multii.SelectedRows[0].Cells[11].Value);
                datePD.Value = delivery_multii.SelectedRows[0].Cells[13].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(delivery_multii.SelectedRows[0].Cells[13].Value);
                dateA.Value = delivery_multii.SelectedRows[0].Cells[12].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(delivery_multii.SelectedRows[0].Cells[12].Value);
                txtDRI.Text = delivery_multii.SelectedRows[0].Cells[14].Value.ToString();

            }
        }

        private void pnlArrivalD_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnEdit5_Click(object sender, EventArgs e)
        {
            pnlEditArrival.Visible = !pnlEditArrival.Visible;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlEditArrival.Visible = !pnlEditArrival.Visible;
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "INSERT INTO deliverydata " +
                                   "( waybillNo, Reference, PlateNo, Origin, Destination1, Amount1, " +
                                   "ModOfpay, PaymentTerms, Remarks, PayDate, LoadingDate, ArrivalDate, Driver) " +
                                   "VALUES (@waybillNo, @Reference, @PlateNo, @Origin, @Destination1, @Amount1, " +
                                   "@ModOfpay, @PaymentTerms, @Remarks, @PayDate, @LoadingDate, @ArrivalDate, @Driver);";

                    using (MySqlCommand cmdInsert = new MySqlCommand(query, con)) // Renamed 'cmd' to 'cmdInsert'
                    {
                        // Assign values from textboxes to parameters
                       
                        cmdInsert.Parameters.AddWithValue("@waybillNo", txtWB.Text);
                        cmdInsert.Parameters.AddWithValue("@Reference", txtRef.Text);
                        cmdInsert.Parameters.AddWithValue("@PlateNo", txtPlateee.Text);
                        cmdInsert.Parameters.AddWithValue("@Origin", txtoriginn.Text);
                        cmdInsert.Parameters.AddWithValue("@Destination1", txtDestinationn.Text);
                        cmdInsert.Parameters.AddWithValue("@Amount1", txtAmountt.Text); // Parse to decimal if needed
                        cmdInsert.Parameters.AddWithValue("@ModOfpay", txtMOPP.Text);
                        cmdInsert.Parameters.AddWithValue("@PaymentTerms", txtPAYMENTERMS.Text);
                        cmdInsert.Parameters.AddWithValue("@Remarks", txtRemarks.Text);

                        // Handle nullable date fields
                        cmdInsert.Parameters.AddWithValue("@PayDate",
                            string.IsNullOrEmpty(txtPDD.Text) ? (object)DBNull.Value : DateTime.Parse(txtPDD.Text));
                        cmdInsert.Parameters.AddWithValue("@LoadingDate",
                            dateLdate.Value != null ? dateLdate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                        cmdInsert.Parameters.AddWithValue("@ArrivalDate",
                            dateArrival.Value != null ? dateArrival.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);

                        cmdInsert.Parameters.AddWithValue("@Driver", txtDriverss.Text);

                        // Execute the query
                        cmdInsert.ExecuteNonQuery();
                    }

                    // Update the delivery table
                    using (MySqlCommand cmdUpdateDelivery = new MySqlCommand("UPDATE delivery SET Status = @Status WHERE PlateNo = @PlateNo;", con))
                    {
                        cmdUpdateDelivery.Parameters.AddWithValue("@Status", "On Delivery");
                        cmdUpdateDelivery.Parameters.AddWithValue("@PlateNo", txtPlateee.Text);
                        cmdUpdateDelivery.ExecuteNonQuery();
                    }

                    // Update the truck table
                    using (MySqlCommand cmdUpdateTruck = new MySqlCommand("UPDATE truck SET Status = @Status WHERE PlateNum = @PlateNum;", con))
                    {
                        cmdUpdateTruck.Parameters.AddWithValue("@Status", "On Delivery");
                        cmdUpdateTruck.Parameters.AddWithValue("@PlateNum", txtPlateee.Text);
                        cmdUpdateTruck.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Record Inserted Successfully!", "Success");
                LoadWaybillNumbers1();
                LoadWaybillNumbers();
                Delivery_data();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }



        private void Driver_ID_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    string query = "UPDATE deliverydata SET " +
                                   "waybillNo = @waybillNo, " +
                                   "Reference = @Reference, " +
                                   "PlateNo = @PlateNo, " +
                                   "Origin = @Origin, " +
                                   "Destination1 = @Destination1, " +
                                   "Amount1 = @Amount1, " +
                                   "ModOfpay = @ModOfpay, " +
                                   "PaymentTerms = @PaymentTerms, " +
                                   "Remarks = @Remarks, " +
                                   "PayDate = @PayDate, " +
                                   "LoadingDate = @LoadingDate, " +
                                   "ArrivalDate = @ArrivalDate, " +
                                   "Driver = @Driver " +
                                   "WHERE LiquidationNO = @LiquidationNO;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        // Assign values from textboxes to parameters
                        cmd.Parameters.AddWithValue("@waybillNo", txtWB.Text);
                        cmd.Parameters.AddWithValue("@Reference", txtRef.Text);
                        cmd.Parameters.AddWithValue("@PlateNo", txtPlateee.Text);
                        cmd.Parameters.AddWithValue("@Origin", txtoriginn.Text);
                        cmd.Parameters.AddWithValue("@Destination1", txtDestinationn.Text);
                        cmd.Parameters.AddWithValue("@Amount1", txtAmountt.Text); // Parse to decimal if necessary
                        cmd.Parameters.AddWithValue("@ModOfpay", txtMOPP.Text);
                        cmd.Parameters.AddWithValue("@PaymentTerms", txtPAYMENTERMS.Text);
                        cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);

                        // Handle nullable date fields
                        cmd.Parameters.AddWithValue("@PayDate",
                            string.IsNullOrEmpty(txtPDD.Text) ? (object)DBNull.Value : DateTime.Parse(txtPDD.Text));
                        cmd.Parameters.AddWithValue("@LoadingDate",
                            dateLdate.Value != null ? dateLdate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ArrivalDate",
                            dateArrival.Value != null ? dateArrival.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);

                        cmd.Parameters.AddWithValue("@Driver", txtDriverss.Text);

                        // Use LiquidationNO as the condition
                        cmd.Parameters.AddWithValue("@LiquidationNO", txtTID.Text);

                        // Execute the query
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Record Updated Successfully!", "Success");
                Delivery_data();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void deliveryData1_SelectionChanged(object sender, EventArgs e)
        {
           
        }

        private void guna2Panel41_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
