using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using System.Windows.Forms;
using best;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;

namespace JustinesCargoServices
{
    public partial class Dashboard : Form
    {
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
        private string username;
        private string workLevel;
        private byte[] image;
        public string testVar;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // Constants for dragging the form
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private Guna2Button lastClickedButton = null;
        private Color originalBackColor;
        private Color originalForeColor;

        private Dictionary<Guna2Button, (Color originalFillColor, Color originalForeColor)> buttonColors = new Dictionary<Guna2Button, (Color, Color)>();

        public Dashboard(string username, string workLevel, byte[] image)
        {
            InitializeComponent();
            this.MouseDown += Form_MouseDown;
            pnlHeader.MouseDown += Form_MouseDown;
            label1.MouseDown += Form_MouseDown;
            this.Load += Dashboard_Load;
            this.Resize += Dashboard_Resize;
            this.StartPosition = FormStartPosition.CenterScreen;
            // Enable double buffering for smoother rendering
            this.DoubleBuffered = true;
            EnableDoubleBuffering(pnlHeader);
            EnableDoubleBuffering(pnlContent);
            this.username = username;
            this.workLevel = workLevel;
            this.image = image;

            DisplayUserDetails();
        }
        private void DisplayUserDetails()
        {
            lbUsername.Text = username;
            lbWorklevel.Text = workLevel;

            if (image != null)
            {

                using (MemoryStream ms = new MemoryStream(image))
                {
                    showImage.Image = Image.FromStream(ms);
                }
            }
        }
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture and send a drag message
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        private void EnableDoubleBuffering(Control control)
        {
            PropertyInfo property = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null)
            {
                property.SetValue(control, true, null);
            }
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
         

            AdjustLayout();
            InitializeButtonColors();
            LoadFormInPanel(new dashboardform());

        }

        private void Dashboard_Resize(object sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            AdjustHeaderWidth();
            AdjustContentWidth();
            AdjustDynamicControls();
        }

        private void AdjustHeaderWidth()
        {
            pnlHeader.Left = pnlSidebar.Width; // Header starts after sidebar
            pnlHeader.Width = this.ClientSize.Width - pnlSidebar.Width; // Occupy remaining width
        }

        private void AdjustContentWidth()
        {
            pnlContent.Left = pnlSidebar.Width;  // Content starts after the sidebar
            pnlContent.Width = this.ClientSize.Width - pnlSidebar.Width;  // Dynamic width based on parent form's width
            pnlContent.Height = this.ClientSize.Height; // Ensure content panel height adjusts to the form's height
        }




        private void AdjustDynamicControls()
        {
            int rightMargin = 50;
            int topMargin = 10;
            int spacing = 5;
            int picboxSpacing = 50;

            showImage.Left = pnlHeader.Width - showImage.Width - rightMargin;
            showImage.Top = pnlHeader.Top + (pnlHeader.Height - showImage.Height) / 2;

            lbUsername.Left = showImage.Left - lbUsername.Width - picboxSpacing;
            lbUsername.Top = topMargin;

            lbWorklevel.Left = lbUsername.Left;
            lbWorklevel.Top = lbUsername.Bottom + spacing;
        }

        private void InitializeButtonColors()
        {
            buttonColors[btnDash] = (btnDash.FillColor, btnDash.ForeColor);
            buttonColors[btnDel] = (btnDel.FillColor, btnDel.ForeColor);
        }

        private void ResetPreviousButton()
        {
            if (lastClickedButton != null)
            {
                lastClickedButton.FillColor = originalBackColor;
                lastClickedButton.ForeColor = originalForeColor;
            }
        }

        private void LoadFormInPanel(Form form)
        {
            pnlContent.Controls.Clear();  // Clear previous controls
            form.TopLevel = false;        // Allow form to act as a child control
            form.Dock = DockStyle.Fill;   // Fill the content panel
            pnlContent.Controls.Add(form);  // Add form to the panel
            form.BringToFront();          // Bring the form to the front
            form.Show();                  // Display the form
        }





        // Button Click Event Handlers
        private void btnDash_Click(object sender, EventArgs e) => HandleButtonClick(sender, new dashboardform());
        private void btnDel_Click(object sender, EventArgs e) => HandleButtonClick(sender, new deliveries());
        private void btnWay_Click(object sender, EventArgs e) => HandleButtonClick(sender, new waybill());
        private void btnBill_Click(object sender, EventArgs e) => HandleButtonClick(sender, new billing());                                  
       
        private void btnTru_Click(object sender, EventArgs e) => HandleButtonClick(sender, new trucks());
        private void btnAd_Click(object sender, EventArgs e) => HandleButtonClick(sender, new admin());
        private void btnPay_Click(object sender, EventArgs e) => HandleButtonClick(sender, new payroll()); 
        private void btnRep_Click(object sender, EventArgs e) => HandleButtonClick(sender, new reports());
        private void btnLTFRB_Click(object sender, EventArgs e) => HandleButtonClick(sender, new renewalsLTFRB());
        private void btnLTO_Click(object sender, EventArgs e) => HandleButtonClick(sender, new renewalsLTO());
       







        private void HandleButtonClick(object sender, Form formToLoad = null)
        {
            var clickedButton = (Guna2Button)sender;

            ResetPreviousButton();

            originalBackColor = clickedButton.FillColor;
            originalForeColor = clickedButton.ForeColor;

            clickedButton.FillColor = Color.White;
            clickedButton.ForeColor = Color.Black;

            lastClickedButton = clickedButton;

            if (formToLoad != null)
            {
                LoadFormInPanel(formToLoad);
            }
        }


        // Event Handlers for Dynamic Controls
        private void txtusername_Click(object sender, EventArgs e) { }
        private void txtworkposition_Click(object sender, EventArgs e) { }
        private void picboximage_Click(object sender, EventArgs e) { }

        // Declare 'expand' variable at the class level (outside of any methods)
        private bool expand = false;  // Track the expansion state

        // Renewals tick method to handle the expansion and collapsing
        private void renewals_Tick(object sender, EventArgs e)
        {
            int expandedHeight = 194;  // Height when expanded
            int collapsedHeight = 39;// Height when collapsed

            if (!expand)
            {
                // Expanding the container
                renewalContainer.Height += 5;  // Gradually increase height

                if (renewalContainer.Height >= expandedHeight)
                {
                    renewalContainer.Height = expandedHeight;  // Stop at expanded height
                    renewals.Stop();  // Stop the timer once fully expanded
                    expand = true;  // Set the state to expanded
                }
            }
            else
            {
                // Collapsing the container
                renewalContainer.Height -= 5;  // Gradually decrease height

                if (renewalContainer.Height <= collapsedHeight)
                {
                    renewalContainer.Height = collapsedHeight;  // Stop at collapsed height
                    renewals.Stop();  // Stop the timer once fully collapsed
                    expand = false;  // Set the state to collapsed
                }
            }
        }
        private void btnRenew_Click(object sender, EventArgs e)
        {
            renewals.Start();
            HandleButtonClick(sender);
        }


        

        private void btnEmp_Click(object sender, EventArgs e)
        {
            HandleButtonClick(sender, new employee());

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {

        }

        

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnInsurance_Click(object sender, EventArgs e)
        {
            HandleButtonClick(sender, new renewalsInsurance());
        }

        private void btnLogOUT_Click(object sender, EventArgs e)
        {
          login login = new login();
          login.Show();
          this.Close();
        }

        private void pnlContent_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Dashboard_Load_1(object sender, EventArgs e)
        {

        }

        private void btnFu_Click(object sender, EventArgs e)
        {
            HandleButtonClick(sender, new fuelmonitoring());
        }

        private void btnEmpLeave_Click(object sender, EventArgs e)
        {
            HandleButtonClick(sender, new leave());
        }

        private void btnFileLeave_Click(object sender, EventArgs e)
        {
            HandleButtonClick(sender, new leaveform());
        }





        //private void btnLTFRB_Click(object sender, EventArgs e)
        //{

        //}
    }
}
