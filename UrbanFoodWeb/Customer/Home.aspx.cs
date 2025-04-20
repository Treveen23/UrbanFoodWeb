using MongoDB.Driver.Core.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UrbanFoodWeb.Supplier;

namespace UrbanFoodWeb.Customer
{
    public partial class Home : System.Web.UI.Page
    {
        private int currentCategoryId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Customer/Login.aspx?returnUrl=Cart.aspx");
                return;
            }
            if (!IsPostBack)
            {
                int customerId = Convert.ToInt32(Session["UserID"].ToString());
                string firstName = GetFirstName(customerId);
                lblWelcome.Text = "Welcome! " + firstName.ToUpper();
            }
            LoadCategories();
            LoadProducts(0);
        }
       

        private string GetFirstName(int custId)
        {
            string firstName = "";
            string connStr = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connStr))
            {
                using (OracleCommand cmd = new OracleCommand("get_customer_firstname_by_id", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_cust_id", OracleDbType.Int32).Value = custId;

                    // Output parameter
                    cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();

                    firstName = cmd.Parameters["p_first_name"].Value.ToString();
                }
            }

            return firstName;
        }

        private void LoadCategories()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "GET_ACTIVE_CATEGORIES";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add output parameter
                    OracleParameter outCursor = new OracleParameter();
                    outCursor.ParameterName = "p_result";
                    outCursor.Direction = ParameterDirection.Output;
                    outCursor.OracleDbType = OracleDbType.RefCursor;
                    cmd.Parameters.Add(outCursor);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Cast the OUT cursor as OracleRefCursor
                    OracleRefCursor refCursor = (OracleRefCursor)outCursor.Value;

                    // Get the result set using ExecuteReader
                    OracleDataReader reader = refCursor.GetDataReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    // Bind to repeater
                    rptCategories.DataSource = dt;
                    rptCategories.DataBind();
                }
            }
        }
    

        protected void FilterCategory_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "FilterCategory")
            {
                // Get the selected category ID
                int categoryId = Convert.ToInt32(e.CommandArgument);

                // Store current category ID
                currentCategoryId = categoryId;

                // Load products filtered by the selected category
                LoadProducts(categoryId);

                // Update the active state of the filter buttons
                foreach (RepeaterItem item in rptCategories.Items)
                {
                    LinkButton btn = (LinkButton)item.FindControl("btnCategory");
                    if (btn != null)
                    {
                        int btnCategoryId = Convert.ToInt32(btn.CommandArgument);
                        if (btnCategoryId == categoryId)
                        {
                            btn.CssClass = "btn btn-secondary rounded-pill px-4 me-2 mb-3 active";
                        }
                        else
                        {
                            btn.CssClass = "btn btn-outline-secondary rounded-pill px-4 me-2 mb-3";
                        }
                    }
                }

                // Update "All Products" button state
                if (categoryId == 0)
                {
                    btnAllCategories.CssClass = "btn btn-secondary rounded-pill px-4 me-2 mb-3 active";
                }
                else
                {
                    btnAllCategories.CssClass = "btn btn-outline-secondary rounded-pill px-4 me-2 mb-3";
                }
            }
        }


        private void LoadProducts(int categoryId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "GET_ACTIVE_PRODUCTS";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add category filter parameter (pass NULL if categoryId is 0)
                    OracleParameter paramCategoryId = new OracleParameter();
                    paramCategoryId.ParameterName = "p_category_id";
                    paramCategoryId.OracleDbType = OracleDbType.Int32;
                    paramCategoryId.Direction = ParameterDirection.Input;
                    paramCategoryId.Value = (categoryId == 0) ? DBNull.Value : (object)categoryId;
                    cmd.Parameters.Add(paramCategoryId);

                    // Add output parameter
                    OracleParameter outCursor = new OracleParameter();
                    outCursor.ParameterName = "p_result";
                    outCursor.Direction = ParameterDirection.Output;
                    outCursor.OracleDbType = OracleDbType.RefCursor;
                    cmd.Parameters.Add(outCursor);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Get the result set
                    OracleRefCursor refCursor = (OracleRefCursor)outCursor.Value;

                    // Get the result set using ExecuteReader
                    OracleDataReader reader = refCursor.GetDataReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    // Bind to repeater
                    rptProducts.DataSource = dt;
                    rptProducts.DataBind();
                }
            }
        }

        //Helper method to resolve image paths correctly
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

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Customer/Shop.aspx");
        }
    }
}