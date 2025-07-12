using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X500;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    public partial class admin : Form
    {
        private bool isGridVisible = false;
        private Timer slideTimer;
        private int targetTop;
        MySqlConnection con = new MySqlConnection(
          "datasource=localhost;" +
          "port=3306;" +
          "database=jcsdb;" +
          "username=root;" +
          "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public admin()
        {
            InitializeComponent();
            SlideTimer0 = new Timer();
            SlideTimer0.Interval = 1; // Adjust interval for the animation speed (10ms)
            SlideTimer0.Tick += SlideTimer0_Tick; // Ad
            SlideTimer1 = new Timer();
            SlideTimer1.Interval = 1; // Adjust interval for the animation speed (10ms)
            SlideTimer1.Tick += SlideTimer1_Tick; // Ad
        }
        private void admin_Load(object sender, EventArgs e)
        {
            Admin_data();
            ClearAll();
        }
        private void ClearAll()
        {
            txtID.Clear();

            txtName.Clear();
            txtUserName.Clear();
            txtPassword.Clear();


        }
        private void Admin_data()
        {
            dataAdmin.Rows.Clear();

            con.Open();
            cmd = new MySqlCommand("SELECT ID, Image, Name, username, password, workLevel FROM users ;", con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                byte[] imageData = (byte[])rdr["Image"];
                Image image = ByteArrayToImage(imageData);
                string name = rdr.GetString(2);
                string username = rdr.GetString(3);
                string password = rdr.GetString(4);
                string workLevel = rdr.GetString(5);

                dataAdmin.Rows.Add(id, image, name, username, password, workLevel);
            }
            con.Close();
        }
        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                if (byteArrayIn != null && byteArrayIn.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(byteArrayIn))
                    {
                        Image returnImage = Image.FromStream(ms);
                        return returnImage;
                    }
                }
                else
                {

                    return null;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error converting byte array to image: " + ex.Message);
                return null;
            }
        }
        private void SlideTimer0_Tick(object sender, EventArgs e)
        {
            // Check if the grid should slide up or down
            if (isGridVisible)
            {
                // Slide down (move to the bottom of the form)
                if (pnlAdminAdd.Top < this.ClientSize.Height)
                {
                    pnlAdminAdd.Top += 15;  // Adjust the speed of sliding down
                }
                else
                {
                    pnlAdminAdd.Visible = false;  // Hide the DataGridView once it reaches the bottom
                    SlideTimer0.Stop();  // Stop the timer when sliding down is complete
                    isGridVisible = false;  // Update the visibility flag
                }
            }
            else
            {
                // Slide up (move towards the targetTop position)
                if (pnlAdminAdd.Top > targetTop)
                {
                    pnlAdminAdd.Top -= 15;  // Adjust the speed of sliding up
                }
                else
                {
                    SlideTimer0.Stop();  // Stop the timer when sliding up is complete
                    isGridVisible = true;  // Update the visibility flag
                }
            }
        }

        private void SlideTimer1_Tick(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down (move to the bottom of the form)
                if (pnlAdminEdit.Top < this.ClientSize.Height)
                {
                    pnlAdminEdit.Top += 15;  // Adjust the speed of sliding down
                }
                else
                {
                    pnlAdminEdit.Visible = false;  // Hide the DataGridView once it reaches the bottom
                    SlideTimer1.Stop();  // Stop the timer when sliding down is complete
                    isGridVisible = false;  // Update the visibility flag
                }
            }
            else
            {
                // Slide up (move towards the targetTop position)
                if (pnlAdminEdit.Top > targetTop)
                {
                    pnlAdminEdit.Top -= 15;  // Adjust the speed of sliding up
                }
                else
                {
                    SlideTimer1.Stop();  // Stop the timer when sliding up is complete
                    isGridVisible = true;  // Update the visibility flag
                }
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down if the grid is visible
                targetTop = this.ClientSize.Height; // Target position is off-screen (bottom)
            }
            else
            {
                // Slide up if the grid is hidden
                pnlAdminAdd.Visible = true;  // Make the DataGridView visible before sliding up
                targetTop = 500; // Adjust this value to where you want the grid to end up (e.g., 100px from the top)
            }

            // Start the sliding animation in the appropriate direction
            SlideTimer0.Start();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down if the grid is visible
                targetTop = this.ClientSize.Height; // Target position is off-screen (bottom)
            }
            else
            {
                // Slide up if the grid is hidden
                pnlAdminEdit.Visible = true;  // Make the DataGridView visible before sliding up
                targetTop = 500; // Adjust this value to where you want the grid to end up (e.g., 100px from the top)
            }

            // Start the sliding animation in the appropriate direction
            SlideTimer1.Start();
            ClearAll();
        }
        private void btnSetImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image selectedImage = Image.FromFile(openFileDialog.FileName);
                    showImage.Image = selectedImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private byte[] ConvertImageToBytes(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                ImageFormat format = image.RawFormat;


                image.Save(ms, format);

                return ms.ToArray();
            }
        }
        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            if (dataAdmin.SelectedRows.Count > 0)
            {
                con.Open();
                cmd = new MySqlCommand("UPDATE users SET " +
                   "Image = @Image " +
                   "WHERE ID = @ID;", con);

                if (showImage.Image != null)
                {
                    byte[] imageData = ConvertImageToBytes(showImage.Image);
                    cmd.Parameters.AddWithValue("@Image", imageData);
                }
                else
                {
                    MessageBox.Show("Please upload an image first.");
                    con.Close();
                    return;
                }

                cmd.Parameters.AddWithValue("@ID", txtID.Text);

                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Image updated successfully.");
                Admin_data();
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }
        private bool ValidateAdminInput()
        {

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Username is required.", "Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtWorkLevel.Text))
            {
                MessageBox.Show("Worklevel is required.", "Error");
                return false;
            }




            return true;
        }
        private void btnAddd_Click(object sender, EventArgs e)
        {
            if (!ValidateAdminInput()) return;
            con.Open();
            cmd = new MySqlCommand("INSERT INTO users(Name, username, password, workLevel) " +
                "VALUES (@Name, @username, @password, @workLevel);", con);

            cmd.Parameters.AddWithValue("@Name", txtName.Text);
            cmd.Parameters.AddWithValue("@username", txtUserName.Text);
            cmd.Parameters.AddWithValue("@password", txtPassword.Text);
            cmd.Parameters.AddWithValue("@workLevel", txtWorkLevel.Text);
            cmd.ExecuteNonQuery();
            con.Close();
            Admin_data();
            ClearAll();
        }

        private void btnSetImage1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image selectedImage = Image.FromFile(openFileDialog.FileName);
                    showImage1.Image = selectedImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnUploadImage1_Click(object sender, EventArgs e)
        {
            if (dataAdmin.SelectedRows.Count > 0)
            {
                con.Open();
                cmd = new MySqlCommand("UPDATE users SET " +
                   "Image = @Image " +
                   "WHERE ID = @ID;", con);

                if (showImage.Image != null)
                {
                    byte[] imageData = ConvertImageToBytes(showImage1.Image);
                    cmd.Parameters.AddWithValue("@Image", imageData);
                }
                else
                {
                    MessageBox.Show("Please upload an image first.");
                    con.Close();
                    return;
                }

                cmd.Parameters.AddWithValue("@ID", txtID1.Text);

                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Image updated successfully.");
                Admin_data();
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }
        private bool ValidateAdminInput1()
        {

            if (string.IsNullOrWhiteSpace(txtName1.Text))
            {
                MessageBox.Show("Name is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUserName1.Text))
            {
                MessageBox.Show("Username is required.", "Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword1.Text))
            {
                MessageBox.Show("Password is required.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtWorkLevel1.Text))
            {
                MessageBox.Show("Worklevel is required.", "Error");
                return false;
            }




            return true;
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateAdminInput1()) return;
            if (dataAdmin.SelectedRows.Count > 0)
            {
                con.Open();
                cmd = new MySqlCommand("UPDATE users SET " +
                   "Name = @Name, " +
                   "username = @Username, " +
                   "password = @password, " +
                   "workLevel = @workLevel " +
                   "WHERE ID = @ID;", con);

                cmd.Parameters.AddWithValue("@Name", txtName1.Text);
                cmd.Parameters.AddWithValue("@Username", txtUserName1.Text);
                cmd.Parameters.AddWithValue("@password", txtPassword1.Text);
                cmd.Parameters.AddWithValue("@workLevel", txtWorkLevel1.Text);
                cmd.Parameters.AddWithValue("@ID", txtID1.Text);



                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Success");
                Admin_data();
                ClearAll();
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private void dataAdmin_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataAdmin.Rows[e.RowIndex];
                Bitmap imageBitmap = (Bitmap)selectedRow.Cells[1].Value;
                showImage.Image = imageBitmap;

                txtID.Text = selectedRow.Cells[0].Value?.ToString();
                txtName.Text = selectedRow.Cells[2].Value?.ToString();
                txtUserName.Text = selectedRow.Cells[3].Value?.ToString();
                txtPassword.Text = selectedRow.Cells[4].Value?.ToString();
                txtWorkLevel.Text = selectedRow.Cells[5].Value?.ToString();
            }
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataAdmin.Rows[e.RowIndex];
                Bitmap imageBitmap = (Bitmap)selectedRow.Cells[1].Value;
                showImage1.Image = imageBitmap;

                txtID1.Text = selectedRow.Cells[0].Value?.ToString();
                txtName1.Text = selectedRow.Cells[2].Value?.ToString();
                txtUserName1.Text = selectedRow.Cells[3].Value?.ToString();
                txtPassword1.Text = selectedRow.Cells[4].Value?.ToString();
                txtWorkLevel1.Text = selectedRow.Cells[5].Value?.ToString();
            }
        }

        private void dataAdmin_SelectionChanged(object sender, EventArgs e)
        {
            if (dataAdmin.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataAdmin.SelectedRows[0];

                txtID.Text = selectedRow.Cells[0].Value != null ? selectedRow.Cells[0].Value.ToString() : "";

                Bitmap imageBitmap = (Bitmap)selectedRow.Cells[1].Value;
                showImage.Image = imageBitmap;

                txtName.Text = selectedRow.Cells[2].Value != null ? selectedRow.Cells[2].Value.ToString() : "";
                txtUserName.Text = selectedRow.Cells[3].Value != null ? selectedRow.Cells[3].Value.ToString() : "";
                txtPassword.Text = selectedRow.Cells[4].Value != null ? selectedRow.Cells[4].Value.ToString() : "";
                txtWorkLevel.Text = selectedRow.Cells[5].Value != null ? selectedRow.Cells[5].Value.ToString() : "";
            }
            if (dataAdmin.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataAdmin.SelectedRows[0];

                txtID1.Text = selectedRow.Cells[0].Value != null ? selectedRow.Cells[0].Value.ToString() : "";

                Bitmap imageBitmap = (Bitmap)selectedRow.Cells[1].Value;
                showImage1.Image = imageBitmap;

                txtName1.Text = selectedRow.Cells[2].Value != null ? selectedRow.Cells[2].Value.ToString() : "";
                txtUserName1.Text = selectedRow.Cells[3].Value != null ? selectedRow.Cells[3].Value.ToString() : "";
                txtPassword1.Text = selectedRow.Cells[4].Value != null ? selectedRow.Cells[4].Value.ToString() : "";
                txtWorkLevel1.Text = selectedRow.Cells[5].Value != null ? selectedRow.Cells[5].Value.ToString() : "";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Clear existing rows
            dataAdmin.Rows.Clear();

            // Get the search term
            string searchTerm = txtSearch.Text.Trim();

            // Open the database connection
            con.Open();

            // Prepare the query with the search term
            string query = @"SELECT ID, Name, username, password, workLevel 
                     FROM users 
                     WHERE Name LIKE @search OR username LIKE @search OR workLevel LIKE @search";

            // Create a MySQL command object
            cmd = new MySqlCommand(query, con);

            // Add a parameter to prevent SQL injection
            cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

            // Execute the reader
            MySqlDataReader rdr = cmd.ExecuteReader();

            // Populate DataGridView with the search results
            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                string username = rdr.GetString(2);
                string password = rdr.GetString(3);
                string workLevel = rdr.GetString(4);

                // Add the row to the DataGridView
                dataAdmin.Rows.Add(id, null, name, username, password, workLevel); // Image is excluded
            }

            // Close the reader and connection
            rdr.Close();
            con.Close();
        }
    }
}
