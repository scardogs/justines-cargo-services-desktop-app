using DPFP;
using DPFP.Capture;
using DPUruNet;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalGUI
{
    public partial class capture : Form, DPFP.Capture.EventHandler
    {
        MySqlConnection con = new MySqlConnection(
          "datasource=localhost;" +
          "port=3306;" +
          "database=jcsdb;" +
          "username=root;" +
          "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        private DPFP.Capture.Capture Capturer;
        
        public string FirstName
        {
            get { return fname.Text; }
            set { fname.Text = value; }
        }

        public string LastName
        {
            get { return lname.Text; }
            set { lname.Text = value; }
        }

        public string EmpID
        {
            get { return empID.Text; }
            set { empID.Text = value; }
        }

        public event Action TimeInClicked;

        public capture()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

        }
        public void HideFinger2()
        {
            lb3rdFinger.Visible = false;   
            lbFirstFinger.Visible = false;
        }
        public void HideFinger3()
        {
            lb2ndfinger.Visible = false;
            lbFirstFinger.Visible = false;
        }
        public void HideButton()
        {
       
            btnTimeIn.Visible = false;
            btnTimeOut.Visible = false;
           
        }
        public void HideButton2()
        {
           lbFirstFinger.Visible = false ;
            lb2ndfinger.Visible = false;
            lb3rdFinger.Visible = false;
        }
        public void HideButton1()
        {
            lbFirstFinger.Visible = false;
            lb2ndfinger.Visible = false;
            lb3rdFinger.Visible = false;
            btn2ndFinger.Visible = false;
            btn3rdFinger.Visible = false;
            StatusLabel.Visible = false;
        }
        public void HideButton3()
        {
          
            btn2ndFinger.Visible = false;
            btn3rdFinger.Visible = false;
            StatusLabel.Visible = false;
        }
        public void HideButton4()
        {
            btnTimeOut.Visible = false;
           
        }
        protected void SetPrompt(string prompt)
        {
            this.Invoke(new Function(delegate ()
            {
                Prompt.Text = prompt;
            }
            ));
        }
        protected void SetStatus(string status)
        {
            this.Invoke(new Function(delegate ()
            {
                StatusLabel.Text = status;
            }
            ));
        }

        private void DrawPicture(Bitmap bitmap)
        {
            this.Invoke(new Function(delegate ()
            {
                fImage.Image = new Bitmap(bitmap, fImage.Size);
            }
            ));
        }
        protected void Setfname(string value)
        {
            this.Invoke(new Function(delegate ()
            {
                fname.Text = value;
            }));
        }
        protected void SetLname(string value)
        {
            this.Invoke(new Function(delegate ()
            {
                lname.Text = value;
            }));
        }
        protected void SetEmpname(string value)
        {
            this.Invoke(new Function(delegate ()
            {
                empID.Text = value;
            }));
        }
        protected virtual void Init()
        {
            try
            {          
                Capturer = new DPFP.Capture.Capture();
                if (null != Capturer)

                    Capturer.EventHandler = this;
                else
                    SetPrompt("Can't iniate capture operation");

            }catch{
                MessageBox.Show("Can't iniate  capture operation");
            }
        }

        protected virtual void Process(DPFP.Sample Sample)
        {
            DrawPicture(ConvertSampleToBitmap(Sample));
        }

        protected virtual void Processs(DPFP.Sample Sample)
        {
            DrawPicture(ConvertSampleToBitmap(Sample));
        }

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Converter = new DPFP.Capture.SampleConversion();
            Bitmap bitmap = null;
            Converter.ConvertToPicture(Sample, ref bitmap);
            return bitmap;
        }
        protected void Start()
        {
            if(null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    SetPrompt("Using the fingerprint reader, scan your fingerprint");
                }
                catch
                {
                    SetPrompt("Can't iniate Capture");
                }
            }
        }

        protected void Stop()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                    timer1.Dispose();
                    
                }
                catch
                {
                    SetPrompt("Can't terminate Capture");
                }
            }
        }

        protected void MakeReport(string message)
        {
            this.Invoke(new Function(delegate ()
            {
                StatusText.AppendText(message + "\r\n");
            }
            ));
        }

        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new  DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;

        }











        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            MakeReport("The fingerprint was removed from the Fingerprint reader");
            SetPrompt("Scan the same fingerprint again");
            Process(Sample);
            Processs(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint was removed from the Fingerprint reader");
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was touch");
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was connected");
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            MakeReport("The fingerprint reader was disconnected");
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if(CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
            
                MakeReport("The quality of the fingerprint sample if good");
            else
                MakeReport("The quality of the fingerprint sample if poor");

        }

        

        private void start_scan_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Start();
        }

        private void testingg_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        

        private void fname_TextChanged(object sender, EventArgs e)
        {
            FirstName = fname.Text;
        }

        private void capture_Load(object sender, EventArgs e)
        {
           
            Init();
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lname_TextChanged(object sender, EventArgs e)
        {
            LastName = lname.Text;
        }

        private void empID_TextChanged(object sender, EventArgs e)
        {
            EmpID = empID.Text;
        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            RecordAttendance();
        }
        protected void RecordAttendance()
        {
             con.Open();
                cmd = new MySqlCommand("INSERT INTO attendance (EmpID, Fname, Lname, TimeIn) " +
                    "VALUES (@EmpID, @Fname, @Lname, @TimeIn);", con);
                cmd.Parameters.AddWithValue("@EmpID", EmpID);
                cmd.Parameters.AddWithValue("@Fname", FirstName);
                cmd.Parameters.AddWithValue("@Lname", LastName);
                cmd.Parameters.AddWithValue("@TimeIn", DateTime.Now);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Time In Success!!");
                con.Close();
            
        }

        private void btnTimeOut_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new MySqlCommand("UPDATE attendance SET " +
                    "Timeout = @Timeout " +
                    "WHERE EmpID = @EmpID AND DATE(TimeIn) = CURDATE();", con);
            cmd.Parameters.AddWithValue("@Timeout", DateTime.Now);
            cmd.Parameters.AddWithValue("@EmpID", EmpID);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Time Out Success!!");
            con.Close();
        }

        private void btn2ndFinger_Click(object sender, EventArgs e)
        {
            string empIDD = EmpID;
            string firstName = FirstName;
            string lastName = LastName;

            // Create an instance of the enroll form and pass the values
            enrolll enrollForm = new enrolll(empIDD, firstName, lastName);

            enrollForm.HideFinger2();
            enrollForm.HideButton();
            enrollForm.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string empIDD = EmpID;
            string firstName = FirstName;
            string lastName = LastName;

            // Create an instance of the enroll form and pass the values
            enrollll enrollForm = new enrollll(empIDD, firstName, lastName);
            enrollForm.HideFinger3();
            enrollForm.HideButton();
            enrollForm.Show();
            this.Close();
        }
    }
}
