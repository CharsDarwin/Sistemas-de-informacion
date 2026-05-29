using AnyStore.BLL;
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
    public partial class frmProducts : Form
    {
        public frmProducts()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        categoriesDAL cdal = new categoriesDAL();
        productsBLL p = new productsBLL();
        productsDAL pdal = new productsDAL();
        userDAL udal = new userDAL();
        
        // Cart Memory
        DataTable dtCart = new DataTable();
        int cartSelectedRowIndex = -1;

        private void frmProducts_Load(object sender, EventArgs e)
        {
            txtName.Text = frmLogin.loggedInUserType + " - " + frmLogin.loggedIn;
            txtName.ReadOnly = true;
            
            // For batching, EVERYONE can use the Add/Update/Delete buttons.
            // But we will rename them in UI.
            btnAdd.Text = "GUARDAR LOTE";
            btnUpdate.Text = "ACTUALIZAR FILA";
            btnDelete.Text = "ELIMINAR FILA";
            
            DataTable categoriesDT = cdal.Select();
            cmbCategory.DataSource = categoriesDT;
            cmbCategory.DisplayMember = "title";
            cmbCategory.ValueMember = "title";

            // Initialize Cart Table
            dtCart.Columns.Add("product_id");
            dtCart.Columns.Add("Categoría");
            dtCart.Columns.Add("Transacción");
            dtCart.Columns.Add("Cantidad");
            
            dgvProducts.DataSource = dtCart;
            dgvProducts.Columns["product_id"].Visible = false;

            CalculateCategoryTotal();
        }

        public void Clear()
        {
            txtTransactionQty.Text = "";
            txtQty.Text = "";
        }

        private void dgvProducts_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = e.RowIndex;
            cartSelectedRowIndex = rowIndex;
            
            if(rowIndex >= 0 && rowIndex < dtCart.Rows.Count)
            {
                cmbCategory.Text = dgvProducts.Rows[rowIndex].Cells["Categoría"].Value.ToString();
                cmbTransactionType.Text = dgvProducts.Rows[rowIndex].Cells["Transacción"].Value.ToString();
                
                if(cmbTransactionType.Text == "ENTRADA")
                    txtQty.Text = dgvProducts.Rows[rowIndex].Cells["Cantidad"].Value.ToString();
                else
                    txtTransactionQty.Text = dgvProducts.Rows[rowIndex].Cells["Cantidad"].Value.ToString();
            }
        }

        // GUARDAR LOTE (Commit Batch)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dtCart.Rows.Count == 0)
            {
                MessageBox.Show("El carrito está vacío. Agregue pesajes primero.");
                return;
            }

            AnyStore.BLL.transactionsBLL tBLL = new AnyStore.BLL.transactionsBLL();
            tBLL.type = "BATCH"; // Mixed batch
            tBLL.transaction_date = DateTime.Now;
            tBLL.added_by = udal.GetIDFromUsername(frmLogin.loggedIn).id;

            List<AnyStore.BLL.transactionDetailBLL> listDetails = new List<transactionDetailBLL>();
            
            foreach (DataRow row in dtCart.Rows)
            {
                AnyStore.BLL.transactionDetailBLL tdBLL = new AnyStore.BLL.transactionDetailBLL();
                tdBLL.product_id = Convert.ToInt32(row["product_id"]);
                tdBLL.qty = Convert.ToDecimal(row["Cantidad"]);
                // If it's SALIDA, negative quantity? Actually the DAL should handle logic or we store the type!
                // Wait, transactionDetail doesn't have "type". We should just do Insert_Transaction for each row individually or update the DAL to handle it.
                // Let's do it right here using a loop of standard transactions to ensure consistency.
            }
            
            // To reuse existing robust logic without altering DAL too much:
            AnyStore.DAL.transactionsDAL tDAL = new AnyStore.DAL.transactionsDAL();
            bool allSuccess = true;
            
            foreach (DataRow row in dtCart.Rows)
            {
                AnyStore.BLL.transactionsBLL singleTBLL = new AnyStore.BLL.transactionsBLL();
                singleTBLL.type = row["Transacción"].ToString();
                singleTBLL.transaction_date = DateTime.Now;
                singleTBLL.added_by = udal.GetIDFromUsername(frmLogin.loggedIn).id;

                AnyStore.BLL.transactionDetailBLL singleTDBLL = new AnyStore.BLL.transactionDetailBLL();
                singleTDBLL.product_id = Convert.ToInt32(row["product_id"]);
                singleTDBLL.qty = Convert.ToDecimal(row["Cantidad"]);
                singleTDBLL.added_date = DateTime.Now;
                singleTDBLL.added_by = singleTBLL.added_by;
                
                if (!tDAL.Insert_Transaction(singleTBLL, singleTDBLL)) {
                    allSuccess = false;
                }
            }

            if(allSuccess)
            {
                MessageBox.Show("Lote guardado en inventario exitosamente.");
                dtCart.Rows.Clear();
                Clear();
                CalculateCategoryTotal();
            }
            else
            {
                MessageBox.Show("Ocurrió un error al guardar algunos pesajes.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cartSelectedRowIndex >= 0 && cartSelectedRowIndex < dtCart.Rows.Count)
            {
                string txType = cmbTransactionType.Text;
                decimal txQty = 0;
                if (!decimal.TryParse(txType == "ENTRADA" ? txtQty.Text : txtTransactionQty.Text, out txQty) || txQty <= 0)
                {
                    MessageBox.Show("Ingrese una cantidad valida.");
                    return;
                }
                
                dtCart.Rows[cartSelectedRowIndex]["Transacción"] = txType;
                dtCart.Rows[cartSelectedRowIndex]["Cantidad"] = txQty;
                
                MessageBox.Show("Fila actualizada en el carrito.");
                Clear();
                cartSelectedRowIndex = -1;
            }
            else
            {
                MessageBox.Show("Seleccione una fila de la tabla primero.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cartSelectedRowIndex >= 0 && cartSelectedRowIndex < dtCart.Rows.Count)
            {
                dtCart.Rows.RemoveAt(cartSelectedRowIndex);
                MessageBox.Show("Fila eliminada del carrito.");
                Clear();
                cartSelectedRowIndex = -1;
            }
            else
            {
                MessageBox.Show("Seleccione una fila de la tabla primero.");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Removed
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateCategoryTotal();
        }

        private void CalculateCategoryTotal()
        {
            if (lblCategoryTotal == null) return;
            string cat = cmbCategory.Text;
            if (string.IsNullOrEmpty(cat)) return;
            
            DataTable dt = pdal.DisplayProductsByCategory(cat);
            decimal total = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    decimal qty = 0;
                    if (row["qty"] != DBNull.Value && decimal.TryParse(row["qty"].ToString(), out qty))
                    {
                        total += qty;
                    }
                }
            }
            lblCategoryTotal.Text = "Total " + cat + ": " + total.ToString("0.00");
        }

        private void cmbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTransactionType.Text == "ENTRADA")
            {
                lblQty.Visible = true;
                txtQty.Visible = true;
                txtQty.Enabled = true;
                lblTransactionQty.Visible = false;
                txtTransactionQty.Visible = false;
            }
            else if (cmbTransactionType.Text == "SALIDA")
            {
                lblQty.Visible = false;
                txtQty.Visible = false;
                lblTransactionQty.Visible = true;
                txtTransactionQty.Visible = true;
            }
        }

        // PROCESAR = Añadir al Carrito temporal
        private void btnProcess_Click(object sender, EventArgs e)
        {
            string txType = cmbTransactionType.Text;
            decimal txQty = 0;
            if (!decimal.TryParse(txType == "ENTRADA" ? txtQty.Text : txtTransactionQty.Text, out txQty) || txQty <= 0)
            {
                MessageBox.Show("Ingrese una cantidad valida mayor a cero.");
                return;
            }
            if (string.IsNullOrEmpty(txType))
            {
                MessageBox.Show("Seleccione ENTRADA o SALIDA.");
                return;
            }

            string cat = cmbCategory.Text;
            if (string.IsNullOrEmpty(cat)) { MessageBox.Show("Seleccione una categoria."); return; }
            
            DataTable dtCat = pdal.DisplayProductsByCategory(cat);
            int resolvedProductId = -1;
            decimal currentQty = 0;
            
            if (dtCat != null && dtCat.Rows.Count > 0)
            {
                resolvedProductId = Convert.ToInt32(dtCat.Rows[0]["id"]);
                foreach (DataRow row in dtCat.Rows)
                {
                    decimal qty = 0;
                    if (row["qty"] != DBNull.Value && decimal.TryParse(row["qty"].ToString(), out qty))
                    {
                        currentQty += qty;
                    }
                }
            }
            else
            {
                AnyStore.BLL.productsBLL newProd = new AnyStore.BLL.productsBLL();
                newProd.name = "Base " + cat;
                newProd.category = cat;
                newProd.description = "Generado automaticamente";
                newProd.qty = 0;
                newProd.added_date = DateTime.Now;
                newProd.added_by = udal.GetIDFromUsername(frmLogin.loggedIn).id;
                
                pdal.Insert(newProd);
                dtCat = pdal.DisplayProductsByCategory(cat);
                resolvedProductId = Convert.ToInt32(dtCat.Rows[0]["id"]);
            }

            // Validar stock asumiendo también lo que ya está en el carrito!
            decimal cartPendingSalida = 0;
            decimal cartPendingEntrada = 0;
            foreach(DataRow r in dtCart.Rows) {
                if(r["Categoría"].ToString() == cat) {
                    if(r["Transacción"].ToString() == "SALIDA") cartPendingSalida += Convert.ToDecimal(r["Cantidad"]);
                    if(r["Transacción"].ToString() == "ENTRADA") cartPendingEntrada += Convert.ToDecimal(r["Cantidad"]);
                }
            }

            decimal projectedStock = currentQty + cartPendingEntrada - cartPendingSalida;

            if (txType == "SALIDA" && txQty > projectedStock)
            {
                MessageBox.Show("Error: Operacion rechazada. No se cuenta con stock suficiente considerando el carrito.", "Error de Stock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Añadir al Carrito
            dtCart.Rows.Add(resolvedProductId, cat, txType, txQty);
            Clear();
            MessageBox.Show("Pesaje añadido al lote temporal. No olvide Guardar Lote al finalizar.");
        }
    }
}
