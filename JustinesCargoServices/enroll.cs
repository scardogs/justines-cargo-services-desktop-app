using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using DPFP;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalGUI
{
    public partial class enroll : capture
    {
        private string empID;
        private string firstName;
        private string lastName;

        private int fingerprintCount = 1;
        private int currentSampleIndex = 1;
        public delegate void OnTemplateEventHandler(DPFP.Template template);
        public event OnTemplateEventHandler OnTemplate;
        private DPFP.Processing.Enrollment Enroller;
        private DPFP.Template Template;

        public enroll(string empID, string firstName, string lastName)
        {
            InitializeComponent();
            this.empID = empID;
            this.firstName = firstName;
            this.lastName = lastName;

            // Set the TextBoxes with the received values
            EmpID = this.empID;
            FirstName = this.firstName;
            LastName = this.lastName;
        }

        
        protected override void Init()
        {
            base.Init();
            base.Text = "Fingerprint Enrollment";
            Enroller = new DPFP.Processing.Enrollment();
            
           

            UpdateStatus();
        }



        protected override void Process(DPFP.Sample sample)
        {
            base.Process(sample);

            DPFP.FeatureSet features = ExtractFeatures(sample, DPFP.Processing.DataPurpose.Enrollment);
            if (features == null)
            {
                MakeReport("Failed to extract features. Please try again with a better sample.");
                return;
            }

            try
            {
                MakeReport("The fingerprint feature set created.");
                Enroller.AddFeatures(features);

                UpdateStatus();

                if (Enroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
                {
                    OnTemplate?.Invoke(Enroller.Template);

                    using (MemoryStream fingerprintData = new MemoryStream())
                    {
                        Enroller.Template.Serialize(fingerprintData);
                        byte[] bytes = fingerprintData.ToArray();

                        try
                        {
                            string MyConnection = "datasource=localhost;port=3306;username=root;password='';database=jcsdb;";
                            using (MySqlConnection MyConn = new MySqlConnection(MyConnection))
                            {
                                MyConn.Open();

                                string queryCheck = "SELECT * FROM biometrics WHERE UPPER(EmployeeID) = @EmpID AND UPPER(empFname) = @fname AND UPPER(empLname) = @lname";
                                using (MySqlCommand cmdCheck = new MySqlCommand(queryCheck, MyConn))
                                {
                                    cmdCheck.Parameters.AddWithValue("@EmpID", EmpID.ToUpper());
                                    cmdCheck.Parameters.AddWithValue("@fname", FirstName.ToUpper());
                                    cmdCheck.Parameters.AddWithValue("@lname", LastName.ToUpper());

                                    int count = 0;
                                    using (MySqlDataReader reader = cmdCheck.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            count++;
                                        }
                                    }

                                    if (count > 0)
                                    {
                                        MessageBox.Show("The person you want to register already exists.");
                                        Enroller.Clear();
                                        UpdateStatus();
                                        return;
                                    }
                                }

                                string queryInsert = "INSERT INTO biometrics (EmployeeID, empFname, empLname, finger_print1) VALUES (@EmpID, @fname, @lname, @finger)";
                                using (MySqlCommand cmdInsert = new MySqlCommand(queryInsert, MyConn))
                                {
                                    cmdInsert.Parameters.AddWithValue("@EmpID", EmpID);
                                    cmdInsert.Parameters.AddWithValue("@fname", FirstName);
                                    cmdInsert.Parameters.AddWithValue("@lname", LastName);
                                    cmdInsert.Parameters.AddWithValue("@finger", bytes).DbType = DbType.Binary;

                                    cmdInsert.ExecuteNonQuery();
                                    MessageBox.Show("Fingerprint data successfully saved.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Database error: " + ex.Message);
                        }
                    }
                }
                else if (Enroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Failed)
                {
                    MakeReport("Enrollment failed. Starting over...");
                    Enroller.Clear();
                    UpdateStatus();
                }
            }
            catch (Exception ex)
            {
                MakeReport("Error during enrollment: " + ex.Message);
            }
        }

        private void UpdateStatus()
        {
            SetStatus(String.Format("Fingerprint sample needed: {0}", Enroller.FeaturesNeeded));
        }

    }
}
