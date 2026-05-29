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
    class productsDAL
    {
        //Creating STATI String Method for DB Connection
        static string myconnstrng = ConfigurationManager.ConnectionStrings["connstrng"].ConnectionString;

        #region Select method for Product Module
        public DataTable Select()
        {
            //Create Sql Connection to connect Databaes
            SqlConnection conn = new SqlConnection(myconnstrng);

            //DAtaTable to hold the data from database
            DataTable dt = new DataTable();

            try
            {
                //Writing the Query to Select all the products from database
                String sql = "SELECT p.id, p.name, p.category, p.description, p.qty, p.added_date, u.username AS added_by FROM tbl_products p LEFT JOIN tbl_users u ON p.added_by = u.id";

                //Creating SQL Command to Execute Query
                SqlCommand cmd = new SqlCommand(sql, conn);

                //SQL Data Adapter to hold the value from database temporarily
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //Open DAtabase Connection
                conn.Open();

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
        #region Method to Insert Product in database
                public bool Insert(productsBLL p)
        {
            bool isSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(myconnstrng))
                {
                    String sql = "INSERT INTO tbl_products (name, category, description, qty, added_date, added_by) VALUES (@name, @category, @description, @qty, @added_date, @added_by)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", p.name);
                        cmd.Parameters.AddWithValue("@category", p.category);
                        cmd.Parameters.AddWithValue("@description", p.description);
                        cmd.Parameters.AddWithValue("@qty", p.qty);
                        cmd.Parameters.AddWithValue("@added_date", p.added_date);
                        cmd.Parameters.AddWithValue("@added_by", p.added_by);
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = rows > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al agregar producto: " + ex.Message);
            }
            return isSuccess;
        }
        #endregion
        #region Method to Update Product in Database
                public bool Update(productsBLL p)
        {
            bool isSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(myconnstrng))
                {
                    String sql = "UPDATE tbl_products SET name=@name, category=@category, description=@description, added_date=@added_date, added_by=@added_by WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", p.name);
                        cmd.Parameters.AddWithValue("@category", p.category);
                        cmd.Parameters.AddWithValue("@description", p.description);
                        cmd.Parameters.AddWithValue("@added_date", p.added_date);
                        cmd.Parameters.AddWithValue("@added_by", p.added_by);
                        cmd.Parameters.AddWithValue("@id", p.id);
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = rows > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al actualizar producto: " + ex.Message);
            }
            return isSuccess;
        }
        #endregion
        #region Method to Delete Product from Database
                public bool Delete(productsBLL p)
        {
            bool isSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(myconnstrng))
                {
                    String sql = "DELETE FROM tbl_products WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", p.id);
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = rows > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al eliminar producto: " + ex.Message);
            }
            return isSuccess;
        }
        #endregion
        #region SEARCH Method for Product Module
        public DataTable Search (string keywords)
        {
            //SQL Connection fro DB Connection
            SqlConnection conn = new SqlConnection(myconnstrng);
            //Creating DAtaTable to hold value from dAtabase
            DataTable dt = new DataTable();

            try
            {
                //SQL query to search product
                string sql = "SELECT p.id, p.name, p.category, p.description, p.qty, p.added_date, u.username AS added_by FROM tbl_products p LEFT JOIN tbl_users u ON p.added_by = u.id WHERE p.id LIKE '%"+keywords+"%' OR p.name LIKE '%"+keywords+"%' OR p.category LIKE '%"+keywords+"%'";
                //Sql Command to execute Query
                SqlCommand cmd = new SqlCommand(sql, conn);

                //SQL Data Adapter to hold the data from database temporarily
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //Open Database Connection
                conn.Open();

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
        #region METHOD TO SEARCH PRODUCT IN TRANSACTION MODULE
        public productsBLL GetProductsForTransaction(string keyword)
        {
            //Create an object of productsBLL and return it
            productsBLL p = new productsBLL();
            //SqlConnection
            SqlConnection conn = new SqlConnection(myconnstrng);
            //Datatable to store data temporarily
            DataTable dt = new DataTable();

            try
            {
                //Write the Query to Get the detaisl
                string sql = "SELECT name, qty FROM tbl_products WHERE id LIKE '%"+keyword+"%' OR name LIKE '%"+keyword+"%'";
                //Create Sql Data Adapter to Execute the query
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);

                //Open DAtabase Connection
                conn.Open();

                //Pass the value from adapter to dt
                adapter.Fill(dt);

                //If we have any values on dt then set the values to productsBLL
                if(dt.Rows.Count>0)
                {
                    p.name = dt.Rows[0]["name"].ToString();
                    p.qty = decimal.Parse(dt.Rows[0]["qty"].ToString());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Close Database Connection
                conn.Close();
            }

            return p;
        }
        #endregion
        #region METHOD TO GET PRODUCT ID BASED ON PRODUCT NAME
        public productsBLL GetProductIDFromName(string ProductName)
        {
            //First Create an Object of DeaCust BLL and REturn it
            productsBLL p = new productsBLL();

            //SQL Conection here
            SqlConnection conn = new SqlConnection(myconnstrng);
            //Data TAble to Holdthe data temporarily
            DataTable dt = new DataTable();

            try
            {
                //SQL Query to Get id based on Name
                string sql = "SELECT id FROM tbl_products WHERE name='" + ProductName + "'";
                //Create the SQL Data Adapter to Execute the Query
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);

                conn.Open();

                //Passing the CAlue from Adapter to DAtatable
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //Pass the value from dt to DeaCustBLL dc
                    p.id = int.Parse(dt.Rows[0]["id"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return p;
        }
        #endregion
        #region METHOD TO GET CURRENT QUantity from the Database based on Product ID
        public decimal GetProductQty(int ProductID)
        {
            //SQl Connection First
            SqlConnection conn = new SqlConnection(myconnstrng);
            //Create a Decimal Variable and set its default value to 0
            decimal qty = 0;

            //Create Data Table to save the data from database temporarily
            DataTable dt = new DataTable();

            try
            {
                //Write WQL Query to Get Quantity from Database
                string sql = "SELECT qty FROM tbl_products WHERE id = "+ProductID;

                //Cerate A SqlCommand
                SqlCommand cmd = new SqlCommand(sql, conn);

                //Create a SQL Data Adapter to Execute the query
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //open DAtabase Connection
                conn.Open();

                //PAss the calue from Data Adapter to DataTable
                adapter.Fill(dt);

                //Lets check if the datatable has value or not
                if(dt.Rows.Count>0)
                {
                    qty = decimal.Parse(dt.Rows[0]["qty"].ToString());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Close Database Connection
                conn.Close();
            }

            return qty;
        }
        #endregion
        #region METHOD TO UPDATE QUANTITY
        public bool UpdateQuantity(int ProductID, decimal Qty)
        {
            //Create a Boolean Variable and Set its value to false
            bool success = false;

            //SQl Connection to Connect Database
            SqlConnection conn = new SqlConnection(myconnstrng);

            try
            {
                //Write the SQL Query to Update Qty
                string sql = "UPDATE tbl_products SET qty=@qty WHERE id=@id";

                //Create SQL Command to Pass the calue into Queyr
                SqlCommand cmd = new SqlCommand(sql, conn);
                //Passing the VAlue trhough parameters
                cmd.Parameters.AddWithValue("@qty", Qty);
                cmd.Parameters.AddWithValue("@id", ProductID);

                //Open Database Connection
                conn.Open();

                //Create Int Variable and Check whether the query is executed Successfully or not
                int rows = cmd.ExecuteNonQuery();
                //Lets check if the query is executed Successfully or not
                if(rows>0)
                {
                    //Query Executed Successfully
                    success = true;
                }
                else
                {
                    //Failed to Execute Query
                    success = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return success;
        }
        #endregion
        #region METHOD TO INCREASE PRODUCT
        public bool IncreaseProduct(int ProductID, decimal IncreaseQty)
        {
            //Create a Boolean Variable and SEt its value to False
            bool success = false;

            //Create SQL Connection To Connect DAtabase
            SqlConnection conn = new SqlConnection(myconnstrng);

            try
            {
                //Get the Current Qty From dAtabase based on id
                decimal currentQty = GetProductQty(ProductID);

                //Increase the Current Quantity by the qty purchased from Dealer
                decimal NewQty = currentQty + IncreaseQty;

                //Update the Prudcty Quantity Now
                success = UpdateQuantity(ProductID, NewQty);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return success;
        }
        #endregion
        #region METHOD TO DECREASE PRODUCT
        public bool DecreaseProduct(int ProductID, decimal Qty)
        {
            //Create Boolean Variable and SEt its Value to false
            bool success = false;

            SqlConnection conn = new SqlConnection(myconnstrng);

            try
            {
                //Get the Current product Quantity
                decimal currentQty = GetProductQty(ProductID);

                //Decrease the Product Quantity based on product sales
                decimal NewQty = currentQty - Qty;

                //Update Product in Database
                success = UpdateQuantity(ProductID, NewQty);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return success;
        }
        #endregion
        #region DESPLAY PRODUCTS BASED ON CATEGORIES
        public DataTable DisplayProductsByCategory(string category)
        {
            //Sql Connection First
            SqlConnection conn = new SqlConnection(myconnstrng);

            DataTable dt = new DataTable();

            try
            {
                //SQL Query to Display Product Based on CAtegory
                string sql = "SELECT p.id, p.name, p.category, p.description, p.qty, p.added_date, u.username AS added_by FROM tbl_products p LEFT JOIN tbl_users u ON p.added_by = u.id WHERE p.category='"+category+"'";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //Open Database Connection Here
                conn.Open();

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


