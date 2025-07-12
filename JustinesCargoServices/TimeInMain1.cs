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
using DPFP;
using MySql.Data.MySqlClient;
using JustinesCargoServices;

namespace FinalGUI
{
  

    public partial class TimeInMain1 : Form
    {
        private DPFP.Template Template;
        MySqlConnection con = new MySqlConnection(
        "datasource=localhost;" +
        "port=3306;" +
        "database=jcsdb;" +
        "username=root;" +
        "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        MySqlDataAdapter adapter;
        DataTable table;

        public TimeInMain1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
       
       
        private void verify_btn_Click(object sender, EventArgs e)
        {
           
            

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            login login = new login();
            login.Show();
            this.Close();
        }

        private void Biometrics_Load(object sender, EventArgs e)
        {
            
        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            TimeInMain timeInMain = new TimeInMain();
            timeInMain.Show();
            this.Close();
        }

        private void btnTimeOut_Click(object sender, EventArgs e)
        {
            TimeInMain2 timeInMain = new TimeInMain2(); 
            timeInMain.Show();
            this.Close();
        }
    }
}
