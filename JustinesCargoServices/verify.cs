using DPFP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;


namespace FinalGUI
{
    public partial class verify : capture
    {
        MySqlConnection con = new MySqlConnection(
         "datasource=localhost;" +
         "port=3306;" +
         "database=jcsdb;" +
         "username=root;" +
         "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;


        public void Verify(DPFP.Template template)
        {
          Template = template;
          ShowDialog();
            
        }

        protected override void Process(Sample sample)
        {
            try
            {
                string MyConnection = "datasource=localhost;port=3306;username=root;password='';";
                string Query = "SELECT empFName, empLName, EmployeeID, finger_print1, finger_print2, finger_print3 FROM jcsdb.biometrics";

                using (MySqlConnection MyConn = new MySqlConnection(MyConnection))
                {
                    MySqlCommand MyCommand = new MySqlCommand(Query, MyConn);
                    MySqlDataAdapter MyAdapter = new MySqlDataAdapter { SelectCommand = MyCommand };
                    DataTable dTable = new DataTable();

                    MyAdapter.Fill(dTable);
                    MyConn.Open();
                    MySqlDataReader myReader = MyCommand.ExecuteReader();

                    foreach (DataRow row in dTable.Rows)
                    {
                        string[] fingerprintFields = { "finger_print1", "finger_print2", "finger_print3" };
                        bool isVerified = false;

                        foreach (var field in fingerprintFields)
                        {
                            if (!row.Table.Columns.Contains(field))
                            {
                                MakeReport($"Column '{field}' does not exist in the data table.");
                                continue;
                            }

                            byte[] fingerprintData = row[field] as byte[];
                            if (fingerprintData == null)
                            {
                                MakeReport($"Fingerprint data is null for {field} in a record.");
                                continue;
                            }

                            using (MemoryStream ms = new MemoryStream(fingerprintData))
                            {
                                DPFP.Template dbTemplate = new DPFP.Template();
                                dbTemplate.DeSerialize(ms);

                                DPFP.FeatureSet features = ExtractFeatures(sample, DPFP.Processing.DataPurpose.Verification);
                                if (features == null)
                                {
                                    MakeReport("Failed to extract features from the fingerprint sample.");
                                    continue;
                                }

                                if (Verificator == null)
                                {
                                    MakeReport("Fingerprint Verificator is not initialized.");
                                    return;
                                }

                                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                                Verificator.Verify(features, dbTemplate, ref result);
                                UpdateStatus(result.FARAchieved);

                                if (result.Verified)
                                {
                                    MakeReport($"The fingerprint from {field} was verified as {row["empFName"]} {row["empLName"]}.");
                                    Setfname(row["empFName"].ToString());
                                    SetLname(row["empLName"].ToString());
                                    SetEmpname(row["EmployeeID"].ToString());

                                    isVerified = true;
                                    break;
                                }
                            }
                        }

                        if (isVerified)
                        {
                            break; // Exit the main loop if a match is found
                        }
                        else
                        {
                            MakeReport("The fingerprint is NOT verified.");
                            Setfname("NO DATA");
                            SetLname("NO DATA");
                            SetEmpname("NO DATA");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void Init()
        {
            base.Init();
            base.Text = "Fingerprint Verification";
            Verificator = new DPFP.Verification.Verification();
            UpdateStatus(0);
        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            // Implementation for Time-In functionality
        }

        private void UpdateStatus(int FAR)
        {
            SetStatus(String.Format("False Accept Rate (FAR) = {0}", FAR));
        }
    }
}
