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

namespace FinalGUI
{
    public partial class enrollll : capture
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

        public enrollll(string empID, string firstName, string lastName)
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
        
        protected override void Processs(DPFP.Sample Sample)
        {
            base.Process(Sample);

            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);
            if (features != null)
            {
                try
                {
                    MakeReport("The fingerprint feature set created");
                    Enroller.AddFeatures(features);
                }
                finally
                {
                    UpdateStatus();

                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:
                            {
                                OnTemplate?.Invoke(Enroller.Template);

                                using (MemoryStream fingerprintData = new MemoryStream())
                                {
                                    Enroller.Template.Serialize(fingerprintData);
                                    byte[] bytes = fingerprintData.ToArray();

                                    MessageBox.Show($"FirstName: {FirstName}, LastName: {LastName}");

                                    try
                                    {
                                        string MyConnection = "datasource=localhost;port=3306;username=root;password='';database=jcsdb;";
                                        using (MySqlConnection MyConn = new MySqlConnection(MyConnection))
                                        {
                                            string Query = "UPDATE biometrics SET finger_print3 = @finger WHERE EmployeeID = @EmpID";
                                            using (MySqlCommand Mycommand1 = new MySqlCommand(Query, MyConn))
                                            {
                                                Mycommand1.Parameters.AddWithValue("@EmpID", EmpID);

                                                Mycommand1.Parameters.AddWithValue("@finger", bytes).DbType = DbType.Binary;

                                                MyConn.Open();
                                                Mycommand1.ExecuteNonQuery();
                                                MessageBox.Show("Fingerprint data successfully saved.");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Error: " + ex.Message);
                                    }
                                }
                                Stop();
                                break;
                            }
                        case DPFP.Processing.Enrollment.Status.Failed:
                            {
                                Enroller.Clear();
                                Stop();
                                UpdateStatus();
                                OnTemplate?.Invoke(null);
                                Start();
                                break;
                            }
                    }
                }
            }
        }


        private void UpdateStatus()
        {
            SetStatus(String.Format("Fingerprint sample needed: {0}", Enroller.FeaturesNeeded));
        }

    }
}
