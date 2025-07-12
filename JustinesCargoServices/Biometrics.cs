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

namespace FinalGUI
{
    delegate void Function();
    public partial class Biometrics : Form
    {
        private DPFP.Template Template;
        public Biometrics()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
       
        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Function(delegate ()
            {
                Template = template;

                if (template != null)
                {
                    MessageBox.Show("The fingerprint template is ready for fingerprint verification", "Fingerprint Enrollment");
                }
                else
                {
                    MessageBox.Show("The fingerprint template is not valid, Repeat Fingerprinnt scannning", "Fingerprint Enrollment");
                }
            }
            ));

        }
        private void enroll_btn_Click(object sender, EventArgs e)
        {
           

        }
       
        private void verify_btn_Click(object sender, EventArgs e)
        {
            verify VeFrm = new verify();
            VeFrm.HideButton();
            VeFrm.HideButton1();
            VeFrm.HideButton2();
            VeFrm.Verify(Template);
            

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Biometrics_Load(object sender, EventArgs e)
        {
            SetFormRoundEdge(30);
        }
        private void SetFormRoundEdge(int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(this.Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(this.Width - radius, this.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, this.Height - radius, radius, radius), 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);
        }
    }
}
