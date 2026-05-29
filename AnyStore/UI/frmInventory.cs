using AnyStore.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyStore.UI
{
    public partial class frmInventory : Form
    {
        public frmInventory()
        {
            InitializeComponent();
        }
        categoriesDAL cdal = new categoriesDAL();
        productsDAL pdal = new productsDAL();
        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            //Addd Functionality to Close this form
            this.Hide();
        }

        private void frmInventory_Load(object sender, EventArgs e)
        {
            //Display the CAtegories in Combobox
            DataTable cDt = cdal.Select();

            cmbCategories.DataSource = cDt;

            //Specify the value member and display member for Combobox
            cmbCategories.DisplayMember = "title";
            cmbCategories.ValueMember = "title";

            //Display all the products in Datagrid view when the form is loaded
            DataTable pdt = pdal.Select();
            dgvProducts.DataSource = pdt;
            Translator.TranslateGrid(dgvProducts);
            CalculateTotal();

            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            btnFilterAudit_Click(sender, e);

        }

        
        private void btnFilterAudit_Click(object sender, EventArgs e)
        {
            AnyStore.DAL.transactionsDAL tDAL = new AnyStore.DAL.transactionsDAL();
            DataTable dt = tDAL.Display_Audit(dtpStartDate.Value, dtpEndDate.Value);
            dgvAudit.DataSource = dt;
        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Display all the Products Based on Selected CAtegory

            string category = cmbCategories.Text;

            DataTable dt = pdal.DisplayProductsByCategory(category);
            dgvProducts.DataSource = dt;
            Translator.TranslateGrid(dgvProducts);
            CalculateTotal();

            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            btnFilterAudit_Click(sender, e);

        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            //Display all the productswhen this button is clicked
            DataTable dt = pdal.Select();
            dgvProducts.DataSource = dt;
            Translator.TranslateGrid(dgvProducts);
            CalculateTotal();

            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            btnFilterAudit_Click(sender, e);

        }

        private void CalculateTotal()
        {
            decimal total = 0;
            if (dgvProducts.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    decimal rowQty = 0;
                    if (row.Cells["qty"] != null && row.Cells["qty"].Value != null && decimal.TryParse(row.Cells["qty"].Value.ToString(), out rowQty))
                    {
                        total += rowQty;
                    }
                }
            }
            lblTotalSummary.Text = "Total en Almacen: " + total.ToString("0.00");
        }
    }


}


