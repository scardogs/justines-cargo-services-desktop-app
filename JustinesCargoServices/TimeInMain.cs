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
    delegate void Function1();

    public partial class TimeInMain : Form
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

        public TimeInMain()
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
            
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {

        }

        private void btnEnter_Click_1(object sender, EventArgs e)
        {
            verify VeFrm = new verify();
            VeFrm.HideButton1();
            VeFrm.HideButton4();
            VeFrm.HideButton3();
            VeFrm.Verify(Template);
            this.Close();
        }

        private void btnPasswordTimeIn_Click(object sender, EventArgs e)
        {
            TimeInLogIn timeInLogIn = new TimeInLogIn();
            timeInLogIn.Show();
            this.Close();
        }
    }
}
