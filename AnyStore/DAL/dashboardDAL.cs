using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AnyStore.DAL
{
    public class dashboardDAL
    {
        static string myconnstrng = ConfigurationManager.ConnectionStrings["connstrng"].ConnectionString;

        public decimal GetTotalEntradas(DateTime start, DateTime end)
        {
            decimal total = 0;
            using (SqlConnection conn = new SqlConnection(myconnstrng))
            {
                string sql = "SELECT ISNULL(SUM(qty), 0) FROM tbl_transaction_detail td INNER JOIN tbl_transactions t ON t.added_by = td.added_by AND ABS(DATEDIFF(second, t.transaction_date, td.added_date)) <= 5 WHERE t.type = 'ENTRADA' AND t.transaction_date >= @start AND t.transaction_date <= @end";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start.Date);
                    cmd.Parameters.AddWithValue("@end", end.Date.AddDays(1).AddTicks(-1));
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        total = Convert.ToDecimal(result);
                    }
                }
            }
            return total;
        }

        public decimal GetTotalStock()
        {
            decimal total = 0;
            using (SqlConnection conn = new SqlConnection(myconnstrng))
            {
                string sql = "SELECT ISNULL(SUM(qty), 0) FROM tbl_products";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        total = Convert.ToDecimal(result);
                    }
                }
            }
            return total;
        }

        public DataTable GetRecentActivity(DateTime start, DateTime end)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(myconnstrng))
            {
                string sql = @"
                    SELECT TOP 5
                        t.transaction_date as [Fecha],
                        p.name as [Producto],
                        td.qty as [Cant. (Kg)],
                        t.type as [Tipo],
                        u.username as [Operario]
                    FROM tbl_transactions t
                    INNER JOIN tbl_transaction_detail td ON t.added_by = td.added_by AND ABS(DATEDIFF(second, t.transaction_date, td.added_date)) <= 5
                    INNER JOIN tbl_products p ON td.product_id = p.id
                    INNER JOIN tbl_users u ON t.added_by = u.id
                    WHERE t.transaction_date >= @start AND t.transaction_date <= @end
                    ORDER BY t.transaction_date DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start.Date);
                    cmd.Parameters.AddWithValue("@end", end.Date.AddDays(1).AddTicks(-1));
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable GetEntradasPorCategoria(DateTime start, DateTime end)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(myconnstrng))
            {
                string sql = @"
                    SELECT 
                        p.category as [category], 
                        SUM(td.qty) as [qty]
                    FROM tbl_transactions t
                    INNER JOIN tbl_transaction_detail td ON t.added_by = td.added_by AND ABS(DATEDIFF(second, t.transaction_date, td.added_date)) <= 5
                    INNER JOIN tbl_products p ON td.product_id = p.id
                    WHERE t.type = 'ENTRADA' AND t.transaction_date >= @start AND t.transaction_date <= @end
                    GROUP BY p.category";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start.Date);
                    cmd.Parameters.AddWithValue("@end", end.Date.AddDays(1).AddTicks(-1));
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable GetStockPorProducto()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(myconnstrng))
            {
                string sql = "SELECT name, qty FROM tbl_products ORDER BY qty DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}
