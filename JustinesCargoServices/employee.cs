using FinalGUI;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
using System;
using System.Data;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    delegate void Function();
    public partial class employee : Form
    {
        private DPFP.Template Template;
        MySqlConnection con = new MySqlConnection(
           "datasource=localhost;" +
           "port=3306;" +
           "database=jcsdb;" +
           "username=root;" +
           "password=;" +
           "Convert Zero Datetime=True;");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public employee()
        {

            InitializeComponent();


        }
        private void OnTemplate1(DPFP.Template template)
        {
            this.Invoke(new Function(delegate ()
            {
                Template = template;

                if (template != null)
                {
                    MessageBox.Show("The fingerprint template is ready for fingerprint verification", "Fingerprint Enrollment");
                }
                else
                {
                    MessageBox.Show("The fingerprint template is not valid, Repeat Fingerprinnt scannning", "Fingerprint Enrollment");
                }
            }
            ));

        }
        //buttons

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateDeliveriesInput()) return;
            con.Open();
            cmd = new MySqlCommand("INSERT INTO empprofiling (EmployeeID, empFname, empLname, empMname, address, civilStatus, sex, contactNum, dateOfBirth, contactPerson) " +
                "VALUES (@empID, @empFname, @empLname, @empMname, @empAddress, @civilStatus, @sex, @contactNum, @dateOfBirth, @contactPerson);", con);

            cmd.Parameters.AddWithValue("@empID", empID.Text);
            cmd.Parameters.AddWithValue("@empFname", empFname.Text);
            cmd.Parameters.AddWithValue("@empLname", empLname.Text);
            cmd.Parameters.AddWithValue("@empMname", empNname.Text);
            cmd.Parameters.AddWithValue("@empAddress", empAddress.Text);
            cmd.Parameters.AddWithValue("@civilStatus", cbCivilStat.SelectedItem.ToString());
            cmd.Parameters.AddWithValue("@sex", GetSelectedSex());
            cmd.Parameters.AddWithValue("@contactNum", empContactNum.Text);
            cmd.Parameters.AddWithValue("@dateOfBirth", empDOB.Value.Date);
            cmd.Parameters.AddWithValue("@contactPerson", empContactNum.Text);


            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Employee Details Added!", "Success");

            clearAll();
        }

        private void EditPersonalInfo_Click(object sender, EventArgs e)
        {
            if (!ValidateDeliveriesInput()) return;

            con.Open();
            cmd = new MySqlCommand("UPDATE empprofiling SET empFname = @empFname, empLname = @empLname, empMname = @empMname, " +
                                    "address = @empAddress, civilStatus = @civilStatus, sex = @sex, contactNum = @contactNum, " +
                                    "dateOfBirth = @dateOfBirth, contactPerson = @contactPerson WHERE EmployeeID = @empID;", con);

            cmd.Parameters.AddWithValue("@empID", empID.Text);
            cmd.Parameters.AddWithValue("@empFname", empFname.Text);
            cmd.Parameters.AddWithValue("@empLname", empLname.Text);
            cmd.Parameters.AddWithValue("@empMname", empNname.Text);
            cmd.Parameters.AddWithValue("@empAddress", empAddress.Text);
            cmd.Parameters.AddWithValue("@civilStatus", cbCivilStat.SelectedItem.ToString());
            cmd.Parameters.AddWithValue("@sex", GetSelectedSex());
            cmd.Parameters.AddWithValue("@contactNum", empContactNum.Text);
            cmd.Parameters.AddWithValue("@dateOfBirth", empDOB.Value.Date);
            cmd.Parameters.AddWithValue("@contactPerson", EmpContactPerson.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Employee details updated successfully!", "Success");
        }


        //load

        private void LoadPersonalInfo()
        {
            dataPersonalInfo.Rows.Clear();
            con.Open();
            cmd = new MySqlCommand("SELECT Id, EmployeeID, empFname, empLname, empMname, address, civilStatus, sex, contactNum, dateOfBirth, contactPerson FROM empprofiling;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dataPersonalInfo.Rows.Add(
                rdr.GetInt64(0),
                rdr.GetInt32(1),
                    rdr.GetString(2),
                    rdr.GetString(3),
                    rdr.GetString(4),
                    rdr.GetString(5),
                    rdr.GetString(6),
                    rdr.GetString(7),
                rdr.GetString(8),
                rdr.IsDBNull(9) ? (object)DBNull.Value : rdr.GetDateTime(9).ToString("yyyy-MM-dd"),
                    rdr.GetString(10)
                );
            }
            rdr.Close();
            con.Close();
        }

        private void employee_Load(object sender, EventArgs e)
        {
            LoadPersonalInfo();
            workinfoLoad();

            dataPersonalInfo.ClearSelection();
            dataPersonalInfo.ClearSelection();
            ShowInputPersonal.Visible = false;
            workInfoMainPanel.Visible = false;



        }

        private void ShowAddEmp_Click(object sender, EventArgs e)
        {
            if (sender == ShowAddEmp)
            {
                ShowInputPersonal.Visible = true;
                EditPersonalInfo.Visible = false;
                clearAll();
                keyID_Info.Visible = false;



            }
        }



        private void ShowEditEmp_Click(object sender, EventArgs e)
        {

            if (dataPersonalInfo.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select data in the DataGrid first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (sender == ShowEditEmp)
            {
                ShowInputPersonal.Visible = true;
                EditPersonalInfo.Visible = true;
                btnAdd.Visible = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.ToLower();  // Get the search text and convert it to lowercase

            // Loop through all the rows in the DataGridView
            foreach (DataGridViewRow row in dataPersonalInfo.Rows)
            {
                bool isMatch = false;

                // Check each cell in the row
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // If any cell contains the search query, set isMatch to true
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchQuery))
                    {
                        isMatch = true;
                        break;  // No need to check other cells if a match is found
                    }
                }

                // Show or hide the row based on whether a match was found
                row.Visible = isMatch;
            }
        }

        private void btnShowWorkInfo_Click(object sender, EventArgs e)
        {
            workInfoMainPanel.Visible = true;
            checkEmpPersonalProfileData();
            workInfoMainPanel.BringToFront();
            WorkInfoInputPanel.Visible = false;
            dataworkinfo.ClearSelection();
            
        }

        private void dataPersonalInfo_SelectionChanged(object sender, EventArgs e)
        {
            if (dataPersonalInfo.SelectedRows.Count > 0)
            {
                keyID.Text = dataPersonalInfo.SelectedRows[0].Cells[0].Value.ToString();
                empID.Text = dataPersonalInfo.SelectedRows[0].Cells[1].Value.ToString();
                empFname.Text = dataPersonalInfo.SelectedRows[0].Cells[2].Value.ToString();
                empLname.Text = dataPersonalInfo.SelectedRows[0].Cells[3].Value.ToString();
                empNname.Text = dataPersonalInfo.SelectedRows[0].Cells[4].Value.ToString();
                empAddress.Text = dataPersonalInfo.SelectedRows[0].Cells[5].Value.ToString();
                cbCivilStat.Text = dataPersonalInfo.SelectedRows[0].Cells[6].Value.ToString();
                string sex = dataPersonalInfo.SelectedRows[0].Cells[7].Value.ToString();
                if (sex.Equals("Male", StringComparison.OrdinalIgnoreCase))
                {
                    Rmale.Checked = true;
                }
                else if (sex.Equals("Female", StringComparison.OrdinalIgnoreCase))
                {
                    Rfemale.Checked = true;
                }
                empContactNum.Text = dataPersonalInfo.SelectedRows[0].Cells[8].Value.ToString();
                empDOB.Value = dataPersonalInfo.SelectedRows[0].Cells[9].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataPersonalInfo.SelectedRows[0].Cells[9].Value);
                EmpContactPerson.Text = dataPersonalInfo.SelectedRows[0].Cells[10].Value.ToString();
            }
        }

        public void clearAll()
        {
            empFname.Clear();
            empLname.Clear();
            empNname.Clear();
            EmpContactPerson.Clear();
            empAddress.Clear();
            empContactNum.Clear();
        }

        private void ShowInputPersonal_Paint(object sender, PaintEventArgs e)
        {


        }

        private string GetSelectedSex()
        {
            if (Rmale.Checked)
            {
                return "Male";
            }
            else if (Rfemale.Checked)
            {
                return "Female";
            }

            return string.Empty;
        }


        //Others

        private bool ValidateDeliveriesInput()
        {

            if (string.IsNullOrWhiteSpace(empFname.Text))
            {
                MessageBox.Show("First Name is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(empLname.Text))
            {
                MessageBox.Show("Last Name is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(empAddress.Text))
            {
                MessageBox.Show("Address is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(cbCivilStat.Text))
            {
                MessageBox.Show("Civil Status is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(GetSelectedSex()))
            {
                MessageBox.Show("Sex selection is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(empContactNum.Text))
            {
                MessageBox.Show("Contact Number is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(EmpContactPerson.Text))
            {
                MessageBox.Show("Contact Person is required.", "Error");
                return false;
            }

            return true;

        }

        private void btnBackToEmpList_Click(object sender, EventArgs e)
        {
            if (sender == btnBackToEmpList)
            {
                ShowInputPersonal.Visible = false;
            }
        }


        //Work Info

        //load

        private void workinfoLoad()
        {
            dataworkinfo.Rows.Clear();

            string connectionString = "datasource=localhost;port=3306;database=jcsdb;username=root;password=''";
            string query = "SELECT ID, EmployeeID, empLname, paymentMethod, bankAccName, bankAccNo, modeOfPay, position, dateHire, dateSeperated, workStatus, SSSnum, pagIbigNum, philHealthNum FROM empprofiling;";
            MySqlConnection con = new MySqlConnection(connectionString);

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    dataworkinfo.Rows.Add(
                        rdr.GetInt32(0),
                        rdr.GetInt32(1),
                        rdr.GetString(2),
                        rdr.GetString(3),
                        rdr.GetString(4),
                        rdr.GetString(5),
                        rdr.GetString(6),
                        rdr.GetString(7),
                        rdr.IsDBNull(8) ? (object)DBNull.Value : rdr.GetDateTime(8).ToString("yyyy-MM-dd"),
                        rdr.IsDBNull(9) ? (object)DBNull.Value : rdr.GetDateTime(9).ToString("yyyy-MM-dd"),
                        rdr.GetString(10),
                        rdr.GetString(11),
                        rdr.GetString(12),
                        rdr.GetString(13)
                    );
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }


        //Buttons

        private void btnDateSeparated_Click(object sender, EventArgs e)
        {
            if ((empDateHire.Enabled && empDateSeparate.Enabled) || (!empDateHire.Enabled && !empDateSeparate.Enabled))
            {
                MessageBox.Show("Please ensure only one DateTimePicker is enabled (either Date Hired or Date Separated).", "Error");
                return;
            }

            if (!empDateSeparate.Enabled)
            {
                MessageBox.Show("Can't Resign. You are a new employee.", "Error");
                return;
            }

            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                cmd = new MySqlCommand("UPDATE empprofiling SET workStatus = 'OLD EMPLOYEE', dateSeperated = @dateSeperated WHERE ID = @empID;", con);

                cmd.Parameters.AddWithValue("@dateSeperated", empDateSeparate.Value.Date);
                cmd.Parameters.AddWithValue("@empID", txtID_.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Employee Details Updated!", "Success");


                workinfoLoad();
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating details: " + ex.Message);
            }
            finally
            {

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        private void btnEdit__Click(object sender, EventArgs e)
        {
            if (!ValidateWorkInfoInput()) return;
            if (dataworkinfo.SelectedRows.Count > 0)
            {

                cmd = new MySqlCommand("UPDATE empprofiling SET  paymentMethod = @payMethod, bankAccName = @bankName, bankAccNo = @bankAccNo, modeOfPay = @modeOfPay, position = @position, dateHire = @dateHire,  workStatus = @workStatus, SSSnum = @SSSnum, pagIbigNum = @pagIbigNum, philHealthNum = @philHealthNum WHERE ID = @empID;", con);


                cmd.Parameters.AddWithValue("@payMethod", cbEmpPayMethod.Text);
                cmd.Parameters.AddWithValue("@position", empPosition.Text);
                cmd.Parameters.AddWithValue("@bankName", txtbankName.Text);
                cmd.Parameters.AddWithValue("@bankAccNo", txtbankNo.Text);
                cmd.Parameters.AddWithValue("@modeOfPay", cbModeOfPay.Text);
                cmd.Parameters.AddWithValue("@dateHire", empDateHire.Value.Date);
                cmd.Parameters.AddWithValue("@workStatus", cbEmpWorkStatus.Text);
                cmd.Parameters.AddWithValue("@SSSnum", empSSS.Text);
                cmd.Parameters.AddWithValue("@pagIbigNum", empPAGIBIG.Text);
                cmd.Parameters.AddWithValue("@philHealthNum", empPhilHealth.Text);
                cmd.Parameters.AddWithValue("@empID", txtID_.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Employee Details Updated!", "Success");
                workinfoLoad();
                ClearAll();
            }
        }

        //functions
        private void ClearAll()
        {

            empSSS.Clear();
            empPAGIBIG.Clear();
            empPhilHealth.Clear();
            txtbankName.Clear();
            txtbankNo.Clear();
            txtID_.Clear();


            cbEmpPayMethod.SelectedIndex = -1;
            cbModeOfPay.SelectedIndex = -1;
            cbEmpWorkStatus.SelectedIndex = -1;


            empPosition.Text = string.Empty;


            empDateHire.Value = DateTime.Now;
            empDateSeparate.Value = DateTime.Now;

            dataworkinfo.ClearSelection();
        }

        private bool ValidateWorkInfoInput()
        {

            if (string.IsNullOrWhiteSpace(cbEmpPayMethod.Text))
            {
                MessageBox.Show("Payment Method is required.", "Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(cbModeOfPay.Text))
            {
                MessageBox.Show("Mode of Pay is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(empPosition.Text))
            {
                MessageBox.Show("Position is required.", "Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(cbEmpWorkStatus.Text))
            {
                MessageBox.Show("Employment Status is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(empSSS.Text))
            {
                MessageBox.Show("SSS is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(empPAGIBIG.Text))
            {
                MessageBox.Show("PAG-IBIG is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(empPhilHealth.Text))
            {
                MessageBox.Show("Phil-Health is required.", "Error");
                return false;
            }

            return true;

        }

        private void ShowEditWorkInfo_Click(object sender, EventArgs e)
        {
            if (dataworkinfo.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select data in the DataGrid first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (sender == ShowEditWorkInfo)
            {

                WorkInfoInputPanel.Visible = true;
            }
        }

        private void dataworkinfo_SelectionChanged(object sender, EventArgs e)
        {
            if (dataworkinfo.SelectedRows.Count > 0)
            {
                txtID_.Text = dataworkinfo.SelectedRows[0].Cells[0].Value.ToString();
                cbEmpPayMethod.Text = dataworkinfo.SelectedRows[0].Cells[3].Value.ToString();
                empPosition.Text = dataworkinfo.SelectedRows[0].Cells[7].Value.ToString(); // Fixed: Position is in Cell[7]
                txtbankName.Text = dataworkinfo.SelectedRows[0].Cells[4].Value.ToString();
                txtbankNo.Text = dataworkinfo.SelectedRows[0].Cells[5].Value.ToString();
                cbModeOfPay.Text = dataworkinfo.SelectedRows[0].Cells[6].Value.ToString(); // Fixed: Mode of Pay is in Cell[6]
                empDateHire.Value = dataworkinfo.SelectedRows[0].Cells[8].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataworkinfo.SelectedRows[0].Cells[8].Value);
                empDateSeparate.Value = dataworkinfo.SelectedRows[0].Cells[9].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataworkinfo.SelectedRows[0].Cells[9].Value);
                cbEmpWorkStatus.Text = dataworkinfo.SelectedRows[0].Cells[10].Value.ToString();
                empSSS.Text = dataworkinfo.SelectedRows[0].Cells[11].Value.ToString();
                empPAGIBIG.Text = dataworkinfo.SelectedRows[0].Cells[12].Value.ToString();
                empPhilHealth.Text = dataworkinfo.SelectedRows[0].Cells[13].Value.ToString();



            }
        }


        private void refresh_Click(object sender, EventArgs e)
        {
            workinfoLoad();
        }

        private void checkEmpPersonalProfileData()
        {
            con.Open();

            // Query to check if the table is empty
            string query = "SELECT COUNT(*) FROM empprofiling";
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                // If the table is empty, show a prompt
                if (count == 0)
                {
                    MessageBox.Show(
                        "The Employee Profiling table is empty. Please add personal information first.",
                        "Information Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    con.Close();
                }
            }
        }

        private void SearchWorkInfo(string searchText)
        {
            dataworkinfo.Rows.Clear();

            string connectionString = "datasource=localhost;port=3306;database=jcsdb;username=root;password=''";
            string query = @"SELECT ID, EmployeeID, empLname, paymentMethod, bankAccName, bankAccNo, modeOfPay, 
                            position, dateHire, dateSeperated, workStatus, SSSnum, pagIbigNum, philHealthNum 
                     FROM empprofiling 
                     WHERE empLname LIKE @searchText OR EmployeeID LIKE @searchText 
                     OR position LIKE @searchText OR workStatus LIKE @searchText;";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                dataworkinfo.Rows.Add(
                                    rdr.GetInt32(0),
                                    rdr.GetInt32(1),
                                    rdr.GetString(2),
                                    rdr.GetString(3),
                                    rdr.GetString(4),
                                    rdr.GetString(5),
                                    rdr.GetString(6),
                                    rdr.GetString(7),
                                    rdr.IsDBNull(8) ? (object)DBNull.Value : rdr.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    rdr.IsDBNull(9) ? (object)DBNull.Value : rdr.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    rdr.GetString(10),
                                    rdr.GetString(11),
                                    rdr.GetString(12),
                                    rdr.GetString(13)
                                );
                            }
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching: " + ex.Message);
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                SearchWorkInfo(searchText);
            }
            else
            {
                workinfoLoad();
            }

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if(sender == btnBack) {
                WorkInfoInputPanel.Visible = false;
            }
        }

        private void workInfoMainPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            string empIDD = empID.Text;
            string firstName = empFname.Text;
            string lastName = empLname.Text;

            // Create an instance of the enroll form and pass the values
            enroll enrollForm = new enroll(empIDD, firstName, lastName);

            // Optionally, hook up an event or method for further actions
            enrollForm.OnTemplate += this.OnTemplate1;
            enrollForm.HideButton();

            // Show the enroll form
            enrollForm.Show();
        }

        private void btnBiometrics_Click(object sender, EventArgs e)
        {
            Biometrics biometrics = new Biometrics();
            biometrics.Show();
        }
    }
}




