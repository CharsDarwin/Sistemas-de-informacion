using AnyStore.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AnyStore.DAL;
using System.Windows.Forms.DataVisualization.Charting;

namespace AnyStore
{
    public partial class frmAdminDashboard : Form
    {
        private Button btnHome;
        private Panel pnlDashboardCharts;
        private Chart chartStock;
        private Chart chartCategory;
        private ComboBox cmbTimeFilter;
        private Label lblTotalEntradasVal;
        private Label lblTotalStockVal;
        private DataGridView dgvRecentActivity;
        
        private dashboardDAL dbDAL = new dashboardDAL();

        public frmAdminDashboard()
        {
            InitializeComponent();
            InitializeDashboardControls();
        }

        private void InitializeDashboardControls()
        {
            // Ocultar el título principal en el Dashboard para que no estorbe con los KPIs
            this.lblSHead.Visible = false;

            // Configurar btnHome (Inicio)
            this.btnHome = new Button();
            this.btnHome.Dock = DockStyle.Top;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatStyle = FlatStyle.Flat;
            this.btnHome.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.btnHome.ForeColor = Color.White;
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new Size(220, 50);
            this.btnHome.Text = "Inicio / Dashboard";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new EventHandler(this.btnHome_Click);
            
            this.pnlSidebar.Controls.Add(this.btnHome);
            this.pnlSidebar.Controls.SetChildIndex(this.btnHome, this.pnlSidebar.Controls.Count - 2);

            // Configurar panel principal de gráficos transparente
            this.pnlDashboardCharts = new Panel();
            this.pnlDashboardCharts.BackColor = Color.Transparent;
            this.pnlDashboardCharts.Size = new Size(800, 500); // Default size to prevent 0 width
            this.pnlDashboardCharts.Dock = DockStyle.Fill;
            this.pnlDashboardCharts.Padding = new Padding(20, 20, 20, 20);

            // ================= CABECERA (Filtros y KPIs) =================
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 80;
            pnlHeader.BackColor = Color.Transparent;

            Label lblFilter = new Label();
            lblFilter.Text = "Filtro de Tiempo:";
            lblFilter.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblFilter.ForeColor = Color.DarkGreen;
            lblFilter.Location = new Point(10, 25);
            lblFilter.AutoSize = true;

            this.cmbTimeFilter = new ComboBox();
            this.cmbTimeFilter.Items.AddRange(new object[] { "Hoy", "Esta Semana", "Este Mes", "Este Año", "General" });
            this.cmbTimeFilter.Font = new Font("Segoe UI", 12F);
            this.cmbTimeFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTimeFilter.Location = new Point(160, 22);
            this.cmbTimeFilter.Width = 150;
            this.cmbTimeFilter.SelectedIndex = 2; // "Este Mes" por defecto
            this.cmbTimeFilter.SelectedIndexChanged += new EventHandler(this.cmbTimeFilter_SelectedIndexChanged);

            // KPI 1: Ingresos
            Panel pnlKPI1 = new Panel();
            pnlKPI1.BackColor = Color.FromArgb(230, 255, 255, 255);
            pnlKPI1.Size = new Size(220, 70);
            pnlKPI1.Location = new Point(350, 5);
            Label lblTotalEntradasTitle = new Label();
            lblTotalEntradasTitle.Text = "Ingresos del Periodo (Kg)";
            lblTotalEntradasTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotalEntradasTitle.ForeColor = Color.Gray;
            lblTotalEntradasTitle.Dock = DockStyle.Top;
            lblTotalEntradasTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTotalEntradasVal = new Label();
            this.lblTotalEntradasVal.Text = "0.00";
            this.lblTotalEntradasVal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTotalEntradasVal.ForeColor = Color.DarkGoldenrod;
            this.lblTotalEntradasVal.Dock = DockStyle.Fill;
            this.lblTotalEntradasVal.TextAlign = ContentAlignment.MiddleCenter;
            pnlKPI1.Controls.Add(this.lblTotalEntradasVal);
            pnlKPI1.Controls.Add(lblTotalEntradasTitle);

            // KPI 2: Stock
            Panel pnlKPI2 = new Panel();
            pnlKPI2.BackColor = Color.FromArgb(230, 255, 255, 255);
            pnlKPI2.Size = new Size(220, 70);
            pnlKPI2.Location = new Point(600, 5);
            Label lblTotalStockTitle = new Label();
            lblTotalStockTitle.Text = "Stock Físico Total (Kg)";
            lblTotalStockTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotalStockTitle.ForeColor = Color.Gray;
            lblTotalStockTitle.Dock = DockStyle.Top;
            lblTotalStockTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTotalStockVal = new Label();
            this.lblTotalStockVal.Text = "0.00";
            this.lblTotalStockVal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTotalStockVal.ForeColor = Color.DarkGreen;
            this.lblTotalStockVal.Dock = DockStyle.Fill;
            this.lblTotalStockVal.TextAlign = ContentAlignment.MiddleCenter;
            pnlKPI2.Controls.Add(this.lblTotalStockVal);
            pnlKPI2.Controls.Add(lblTotalStockTitle);

            pnlHeader.Controls.Add(lblFilter);
            pnlHeader.Controls.Add(this.cmbTimeFilter);
            pnlHeader.Controls.Add(pnlKPI1);
            pnlHeader.Controls.Add(pnlKPI2);

            // ================= GRÁFICOS (Franja Media) =================
            Panel pnlMiddle = new Panel();
            pnlMiddle.Size = new Size(800, 300); // Default size
            pnlMiddle.Dock = DockStyle.Fill;
            pnlMiddle.BackColor = Color.Transparent;

            // Gráfico de Barras (Stock Actual)
            this.chartStock = new Chart();
            ChartArea caStock = new ChartArea();
            caStock.BackColor = Color.Transparent;
            caStock.AxisX.MajorGrid.LineWidth = 0; 
            caStock.AxisY.MajorGrid.LineWidth = 0;
            caStock.AxisX.LabelStyle.ForeColor = Color.DarkGreen;
            caStock.AxisY.LabelStyle.ForeColor = Color.DarkGreen;
            caStock.AxisX.LabelStyle.Angle = -45;
            this.chartStock.ChartAreas.Add(caStock);
            this.chartStock.Size = new Size(400, 300); // Default size
            this.chartStock.Dock = DockStyle.Left;
            this.chartStock.BackColor = Color.Transparent;
            
            Title tStock = new Title("Stock Físico por Producto");
            tStock.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            tStock.ForeColor = Color.DarkGreen;
            this.chartStock.Titles.Add(tStock);
            
            // Gráfico de Pastel (Distribución por Categorías)
            this.chartCategory = new Chart();
            this.chartCategory.Palette = ChartColorPalette.EarthTones; 
            ChartArea caCat = new ChartArea();
            caCat.BackColor = Color.Transparent;
            this.chartCategory.ChartAreas.Add(caCat);

            Legend leg = new Legend();
            leg.BackColor = Color.Transparent;
            leg.ForeColor = Color.DarkGreen;
            leg.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.chartCategory.Legends.Add(leg);

            this.chartCategory.Size = new Size(400, 300); // Default size
            this.chartCategory.Dock = DockStyle.Fill;
            this.chartCategory.BackColor = Color.Transparent;
            
            Title tCat = new Title("Ingresos por Categoría (En Periodo)");
            tCat.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            tCat.ForeColor = Color.DarkGreen;
            this.chartCategory.Titles.Add(tCat);

            pnlMiddle.Controls.Add(this.chartCategory);
            pnlMiddle.Controls.Add(this.chartStock);

            // ================= ACTIVIDAD RECIENTE (Abajo) =================
            Panel pnlBottom = new Panel();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 150;
            pnlBottom.BackColor = Color.Transparent;
            pnlBottom.Padding = new Padding(10);

            Label lblRecent = new Label();
            lblRecent.Text = "Últimos 5 Movimientos";
            lblRecent.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblRecent.ForeColor = Color.DarkGreen;
            lblRecent.Dock = DockStyle.Top;

            this.dgvRecentActivity = new DataGridView();
            this.dgvRecentActivity.Dock = DockStyle.Fill;
            this.dgvRecentActivity.BackgroundColor = Color.WhiteSmoke;
            this.dgvRecentActivity.BorderStyle = BorderStyle.None;
            this.dgvRecentActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecentActivity.ReadOnly = true;
            this.dgvRecentActivity.AllowUserToAddRows = false;
            this.dgvRecentActivity.RowHeadersVisible = false;
            this.dgvRecentActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecentActivity.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            this.dgvRecentActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            pnlBottom.Controls.Add(this.dgvRecentActivity);
            pnlBottom.Controls.Add(lblRecent);

            // Agregar a la vista
            this.pnlDashboardCharts.Controls.Add(pnlMiddle);
            this.pnlDashboardCharts.Controls.Add(pnlHeader);
            this.pnlDashboardCharts.Controls.Add(pnlBottom);
            
            this.pnlContent.Controls.Add(this.pnlDashboardCharts);
        }

        private void GetDateRange(out DateTime start, out DateTime end)
        {
            DateTime today = DateTime.Now;
            start = today;
            end = today;

            string filter = cmbTimeFilter.SelectedItem.ToString();
            switch (filter)
            {
                case "Hoy":
                    start = today.Date;
                    end = today.Date;
                    break;
                case "Esta Semana":
                    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    start = today.AddDays(-1 * diff).Date;
                    end = today.Date;
                    break;
                case "Este Mes":
                    start = new DateTime(today.Year, today.Month, 1);
                    end = today.Date;
                    break;
                case "Este Año":
                    start = new DateTime(today.Year, 1, 1);
                    end = today.Date;
                    break;
                case "General":
                    start = new DateTime(2000, 1, 1);
                    end = today.Date;
                    break;
            }
        }

        private void LoadDashboardData()
        {
            DateTime start, end;
            GetDateRange(out start, out end);

            // 1. Cargar KPIs
            decimal totalEntradas = dbDAL.GetTotalEntradas(start, end);
            decimal totalStock = dbDAL.GetTotalStock();
            
            this.lblTotalEntradasVal.Text = totalEntradas.ToString("0.00");
            this.lblTotalStockVal.Text = totalStock.ToString("0.00");

            // 2. Cargar Gráfico Stock Físico
            this.chartStock.Series.Clear();
            Series sStock = new Series("Stock");
            sStock.ChartType = SeriesChartType.Column;
            sStock.Color = Color.DarkGoldenrod;
            sStock.IsValueShownAsLabel = true;
            sStock.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            sStock.LabelForeColor = Color.SaddleBrown;

            DataTable dtStock = dbDAL.GetStockPorProducto();
            foreach(DataRow dr in dtStock.Rows)
            {
                string name = dr["name"].ToString();
                decimal qty = Convert.ToDecimal(dr["qty"]);
                sStock.Points.AddXY(name, qty);
            }
            this.chartStock.Series.Add(sStock);

            // 3. Cargar Gráfico Dona (Entradas por Categoría en Periodo)
            this.chartCategory.Series.Clear();
            Series sCat = new Series("Categorias");
            sCat.ChartType = SeriesChartType.Doughnut;
            sCat.IsValueShownAsLabel = true;
            sCat.Label = "#VALX\n#VAL Kg"; // Muestra "Categoría" y "Cantidad Kg"
            sCat.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            sCat.LabelForeColor = Color.White;
            sCat.ToolTip = "#VALX: #VAL Kg (#PERCENT)";
            
            DataTable dtCat = dbDAL.GetEntradasPorCategoria(start, end);
            foreach(DataRow dr in dtCat.Rows)
            {
                string cat = dr["category"].ToString();
                decimal qty = Convert.ToDecimal(dr["qty"]);
                sCat.Points.AddXY(cat, qty);
            }
            this.chartCategory.Series.Add(sCat);

            // 4. Cargar Actividad Reciente
            DataTable dtActivity = dbDAL.GetRecentActivity(start, end);
            this.dgvRecentActivity.DataSource = dtActivity;
        }

        private void cmbTimeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void frmAdminDashboard_Load(object sender, EventArgs e)
        {
            lblLoggedInUser.Text = frmLogin.loggedIn;
            LoadDashboardData();
        }

        private void frmAdminDashboard_FormClosed(object sender, FormClosedEventArgs e)
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
            }
            // Ocultar Dashboard
            this.pnlDashboardCharts.Visible = false;
            this.lblSHead.Visible = false;
            
            // Cargar Form
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            this.pnlContent.Controls.Add(frm);
            this.pnlContent.Tag = frm;
            frm.Show();
            frm.BringToFront();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (this.pnlContent.Controls.Count > 0)
            {
                var currentForm = this.pnlContent.Tag as Form;
                if (currentForm != null)
                {
                    currentForm.Close();
                }
            }
            // Mostrar Dashboard y recalcular en vivo
            this.pnlDashboardCharts.Visible = true;
            this.lblSHead.Visible = false; // Oculto en Dashboard
            this.pnlDashboardCharts.BringToFront();
            LoadDashboardData();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new frmUsers());
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new frmCategories());
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
