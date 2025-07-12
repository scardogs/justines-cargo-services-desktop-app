using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;

namespace FinalGUI
{
   
    public partial class TimeInLogIn : Form
    {
        MySqlConnection con = new MySqlConnection(
          "datasource=localhost;" +
          "port=3306;" +
          "database=jcsdb;" +
          "username=root;" +
          "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public TimeInLogIn()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
       
       
        private void verify_btn_Click(object sender, EventArgs e)
        {
           
            

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Biometrics_Load(object sender, EventArgs e)
        {
            
        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            pnlPassword.Visible = !pnlPassword.Visible;
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {

            // Validate input
            if (string.IsNullOrWhiteSpace(empID.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill in both Employee ID and Password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if EmployeeID and Password exist in `empprofiling`
            string queryCheckCredentials = "SELECT 1 FROM empprofiling WHERE EmployeeID = @EmployeeID AND passWord = @Password";

            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand(queryCheckCredentials, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", empID.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());

                        object result = cmd.ExecuteScalar();

                        if (result != null) // Credentials are valid
                        {
                            // Insert into `attendance`
                            string insertAttendanceQuery = "INSERT INTO attendance (EmpID, Fname, Lname, TimeIn) " +
                                                           "VALUES (@EmpID, @Fname, @Lname, @TimeIn)";
                            using (MySqlCommand insertCmd = new MySqlCommand(insertAttendanceQuery, con))
                            {
                                insertCmd.Parameters.AddWithValue("@EmpID", empID.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@Fname", fname.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@Lname", lname.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@TimeIn", DateTime.Now);

                                insertCmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Time In Success!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Invalid Employee ID or Password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
