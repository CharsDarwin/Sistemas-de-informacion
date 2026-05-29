using AnyStore.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyStore
{
    public partial class frmUserDashboard : Form
    {
        public frmUserDashboard()
        {
            InitializeComponent();
        }

        private void frmUserDashboard_Load(object sender, EventArgs e)
        {
            lblLoggedInUser.Text = frmLogin.loggedIn;
        }

        private void frmUserDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmLogin login = new frmLogin();
            login.Show();
            this.Hide();
        }

        private void LoadFormInPanel(Form frm)
        {
            if (this.pnlContent.Controls.Count > 0)
            {
                var currentForm = this.pnlContent.Tag as Form;
                if (currentForm != null)
                {
                    currentForm.Close();
                }
                this.pnlContent.Controls.Clear();
            }
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            this.pnlContent.Controls.Add(frm);
            this.pnlContent.Tag = frm;
            frm.Show();
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new frmProducts());
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new frmInventory());
        }
    }
}

