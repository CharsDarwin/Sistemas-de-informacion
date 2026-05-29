using System.Data;
using AnyStore.BLL;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AnyStore.DAL
{
    public class transactionsDAL
    {
        static string myconnstrng = ConfigurationManager.ConnectionStrings["connstrng"].ConnectionString;

        public bool Insert_Transaction(transactionsBLL t, transactionDetailBLL td)
        {
            bool isSuccess = false;
            int transactionID = -1;

            using (SqlConnection conn = new SqlConnection(myconnstrng))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Master Transaction
                        string sqlMaster = "INSERT INTO tbl_transactions (type, dea_cust_id, grandTotal, transaction_date, tax, discount, added_by) VALUES (@type, @dea_cust_id, @grandTotal, @transaction_date, @tax, @discount, @added_by); SELECT SCOPE_IDENTITY();";
                        using (SqlCommand cmd = new SqlCommand(sqlMaster, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@type", t.type);
                            cmd.Parameters.AddWithValue("@dea_cust_id", t.dea_cust_id);
                            cmd.Parameters.AddWithValue("@grandTotal", t.grandTotal);
                            cmd.Parameters.AddWithValue("@transaction_date", t.transaction_date);
                            cmd.Parameters.AddWithValue("@tax", t.tax);
                            cmd.Parameters.AddWithValue("@discount", t.discount);
                            cmd.Parameters.AddWithValue("@added_by", t.added_by);

                            object obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                transactionID = Convert.ToInt32(obj);
                            }
                        }

                        // 2. Insert Transaction Detail
                        string sqlDetail = "INSERT INTO tbl_transaction_detail (product_id, rate, qty, total, dea_cust_id, added_date, added_by) VALUES (@product_id, @rate, @qty, @total, @dea_cust_id, @added_date, @added_by)";
                        using (SqlCommand cmdDetail = new SqlCommand(sqlDetail, conn, transaction))
                        {
                            cmdDetail.Parameters.AddWithValue("@product_id", td.product_id);
                            cmdDetail.Parameters.AddWithValue("@rate", td.rate);
                            cmdDetail.Parameters.AddWithValue("@qty", td.qty);
                            cmdDetail.Parameters.AddWithValue("@total", td.total);
                            cmdDetail.Parameters.AddWithValue("@dea_cust_id", td.dea_cust_id);
                            cmdDetail.Parameters.AddWithValue("@added_date", td.added_date);
                            cmdDetail.Parameters.AddWithValue("@added_by", td.added_by);

                            cmdDetail.ExecuteNonQuery();
                        }

                        // 3. Update Product Quantity
                        string sqlUpdate = "";
                        if (t.type == "ENTRADA")
                        {
                            sqlUpdate = "UPDATE tbl_products SET qty = qty + @qty WHERE id = @id";
                        }
                        else if (t.type == "SALIDA")
                        {
                            sqlUpdate = "UPDATE tbl_products SET qty = qty - @qty WHERE id = @id";
                        }

                        using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn, transaction))
                        {
                            cmdUpdate.Parameters.AddWithValue("@qty", td.qty);
                            cmdUpdate.Parameters.AddWithValue("@id", td.product_id);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return isSuccess;
        }

        public DataTable Display_Audit(DateTime start, DateTime end)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(myconnstrng))
                {
                    // JOIN tbl_transactions, tbl_transaction_detail, tbl_products, tbl_users
                    string sql = @"
                        SELECT 
                            t.transaction_date as [Fecha],
                            p.category as [Categoría],
                            t.type as [Tipo Operación],
                            td.qty as [Cantidad (Kg)],
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dt;
        }
    }
}


