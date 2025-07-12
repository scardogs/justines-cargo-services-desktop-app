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
   
    public partial class TimeInLogIn1 : Form
    {
        MySqlConnection con = new MySqlConnection(
          "datasource=localhost;" +
          "port=3306;" +
          "database=jcsdb;" +
          "username=root;" +
          "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public TimeInLogIn1()
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

        private void btnTimeOUT_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=jcsdb;username=root;password=''"))
                {
                    con.Open();

                    // Query to check if the employee has a TimeIn entry for today
                    string queryCheckTimeIn = "SELECT 1 FROM attendance WHERE EmpID = @EmpID AND DATE(TimeIn) = CURDATE()";

                    using (MySqlCommand checkCmd = new MySqlCommand(queryCheckTimeIn, con))
                    {
                        checkCmd.Parameters.AddWithValue("@EmpID", empID.Text.Trim());
                        object result = checkCmd.ExecuteScalar();

                        if (result == null) // No TimeIn entry found
                        {
                            MessageBox.Show("The Employee did not Time In yet today.", "Time Out Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Update the Timeout field if TimeIn exists
                    string queryUpdateTimeOut = "UPDATE attendance SET Timeout = @Timeout WHERE EmpID = @EmpID AND DATE(TimeIn) = CURDATE()";

                    using (MySqlCommand updateCmd = new MySqlCommand(queryUpdateTimeOut, con))
                    {
                        updateCmd.Parameters.AddWithValue("@Timeout", DateTime.Now);
                        updateCmd.Parameters.AddWithValue("@EmpID", empID.Text.Trim());

                        updateCmd.ExecuteNonQuery();
                        MessageBox.Show("Time Out Success!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

