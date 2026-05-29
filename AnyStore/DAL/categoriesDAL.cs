using AnyStore.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyStore.DAL
{
    class categoriesDAL
    {
        //Static String Method for Database Connection String
        static string myconnstrng = ConfigurationManager.ConnectionStrings["connstrng"].ConnectionString;

        #region Select Method
        public DataTable Select()
        {
            //Creating Database Connection
            SqlConnection conn = new SqlConnection(myconnstrng);

            DataTable dt = new DataTable();

            try
            {
                //Wrting SQL Query to get all the data from DAtabase
                string sql = "SELECT c.id, c.title, c.description, c.added_date, u.username AS added_by FROM tbl_categories c LEFT JOIN tbl_users u ON c.added_by = u.id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //Open DAtabase Connection
                conn.Open();
                //Adding the value from adapter to Data TAble dt
                adapter.Fill(dt);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }
        #endregion
        #region Insert New CAtegory
        public bool Insert(categoriesBLL c)
        {
            bool isSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(myconnstrng))
                {
                    string sql = "INSERT INTO tbl_categories (title, description, added_date, added_by) VALUES (@title, @description, @added_date, @added_by)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", c.title);
                        cmd.Parameters.AddWithValue("@description", c.description);
                        cmd.Parameters.AddWithValue("@added_date", c.added_date);
                        cmd.Parameters.AddWithValue("@added_by", c.added_by);
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = rows > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
            return isSuccess;
        }
        #endregion
        #region Update Method
        public bool Update(categoriesBLL c)
        {
            bool isSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(myconnstrng))
                {
                    string sql = "UPDATE tbl_categories SET title=@title, description=@description, added_date=@added_date, added_by=@added_by WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", c.title);
                        cmd.Parameters.AddWithValue("@description", c.description);
                        cmd.Parameters.AddWithValue("@added_date", c.added_date);
                        cmd.Parameters.AddWithValue("@added_by", c.added_by);
                        cmd.Parameters.AddWithValue("@id", c.id);
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = rows > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
            return isSuccess;
        }
        #endregion
        #region Delete Category Method
        public bool Delete(categoriesBLL c)
        {
            bool isSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(myconnstrng))
                {
                    string sql = "DELETE FROM tbl_categories WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", c.id);
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = rows > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
            return isSuccess;
        }
        #endregion
        #region Method for Searh Funtionality
        public DataTable Search(string keywords)
        {
            //SQL Connection For Database Connection
            SqlConnection conn = new SqlConnection(myconnstrng);

            //Creating Data TAble to hold the data from database temporarily
            DataTable dt = new DataTable();

            try
            {
                //SQL Query To Search Categories from DAtabase
                String sql = "SELECT c.id, c.title, c.description, c.added_date, u.username AS added_by FROM tbl_categories c LEFT JOIN tbl_users u ON c.added_by = u.id WHERE c.id LIKE '%"+keywords+"%' OR c.title LIKE '%"+keywords+"%' OR c.description LIKE '%"+keywords+"%'";
                //Creating SQL Command to Execute the Query
                SqlCommand cmd = new SqlCommand(sql, conn);

                //Getting DAta From DAtabase
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //Open DatabaseConnection
                conn.Open();
                //Passing values from adapter to Data Table dt
                adapter.Fill(dt);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }
        #endregion
    }
}

