using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UrbanFoodWeb.Customer
{
    public partial class Shop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Customer/Login.aspx?returnUrl=Shop.aspx");
                    return;
                }

                LoadProducts();
                BindCategories();
            }

        }

        private void BindCategories()
        {
            string connStr = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                using (OracleCommand cmd = new OracleCommand("GetCategories", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Output parameter for the cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            rptCategories.DataSource = dt;
                            rptCategories.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions (e.g., log the error)
                        // For debugging purposes, you might display the error message
                        // Response.Write("Error: " + ex.Message);
                    }
                }
            }
        }


        private void LoadProducts()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GetProducts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        // Debug: Print column names to console
                        string columnNames = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                        System.Diagnostics.Debug.WriteLine("Available columns: " + columnNames);

                        rptProducts.DataSource = dt;
                        rptProducts.DataBind();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
                    }
                }
            }
        }

        // Helper method to resolve image paths correctly
        protected string GetImagePath(object imageUrlObj)
        {
            if (imageUrlObj == null || string.IsNullOrEmpty(imageUrlObj.ToString()))
                return "~/Images/Product/placeholder.jpg"; // Default image if null or empty

            string ImageUrl = imageUrlObj.ToString().Trim();


            // Check if the URL is a database-stored filename or a relative path
            if (!ImageUrl.StartsWith("~/") && !ImageUrl.StartsWith("/"))
            {
                // Assuming images are stored in an Images folder
                return "~/Images/Product/" + ImageUrl;
            }

            return ImageUrl; // Already a relative path with ~/ prefix
        }



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Implement search functionality
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            // Implement pagination functionality - previous page
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            // Implement pagination functionality - next page
        }

        protected void btnAddToCart_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                int productId = Convert.ToInt32(e.CommandArgument);

                // Check if user is logged in - CHANGED UserID to CustomerID
                // In btnAddToCart_Command method
                if (Session["UserID"] == null)  // Changed from CustomerID to UserID
                {
                    // Redirect to login page if not logged in
                    Response.Redirect("~/Customer/Login.aspx?returnUrl=Shop.aspx");
                    return;
                }

                int CustomerId = Convert.ToInt32(Session["UserID"].ToString()); // Changed from CustomerID to UserID

                try
                {
                    AddToCart(CustomerId, productId, 1);

                    // Redirect to cart page
                    Response.Redirect("~/Customer/Cart.aspx");
                }
                catch (Exception ex)
                {
                    // Handle exception
                    ClientScript.RegisterStartupScript(this.GetType(), "addError",
                        $"alert('Error adding product to cart: {ex.Message}');", true);
                }
            }


        }

        private void AddToCart(int CustomerId, int productId, int quantity)
        {
            // CHANGED OracleConnection to OracleConString to be consistent
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("AddToCart", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = CustomerId;
                    cmd.Parameters.Add("p_ProductID", OracleDbType.Int32).Value = productId;
                    cmd.Parameters.Add("p_Quantity", OracleDbType.Int32).Value = quantity;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}