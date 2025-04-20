using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UrbanFoodWeb.Customer
{
    public partial class Cart : System.Web.UI.Page

    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Customer/Login.aspx?returnUrl=Cart.aspx");
                    return;
                }

                LoadCartItems();
            }

        }
        protected void LoadCartItems()
        {
            int CustomerID = Convert.ToInt32(Session["UserID"].ToString());

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GetCartItems", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = CustomerID;
                    OracleParameter outputParameter = new OracleParameter("p_Cursor", OracleDbType.RefCursor);
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        rptCartItems.DataSource = dt;
                        rptCartItems.DataBind();

                        // Calculate and display totals
                        if (dt.Rows.Count > 0)
                        {
                            decimal subtotal = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                subtotal += Convert.ToDecimal(row["TotalPrice"]);
                            }

                            lblSubtotal.Text = string.Format("${0:0.00}", subtotal);

                            // Fixed shipping cost
                            decimal shipping = 3.00m;
                            lblShipping.Text = string.Format("${0:0.00}", shipping);

                            // Total
                            decimal total = subtotal + shipping;
                            lblTotal.Text = string.Format("${0:0.00}", total);

                            // Store total in session for checkout
                            Session["CartTotal"] = total;
                        }
                        else
                        {
                            // No items in cart
                            pnlEmptyCart.Visible = true;
                            pnlCartContent.Visible = false;
                        }
                    }
                }
            }
        }

        protected void btnUpdateQuantity_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "UpdateQuantity")
            {
                string[] args = e.CommandArgument.ToString().Split(',');
                int productId = Convert.ToInt32(args[0]);
                TextBox txtQuantity = (TextBox)rptCartItems.Items[Convert.ToInt32(args[1])].FindControl("txtQuantity");
                int quantity = Convert.ToInt32(txtQuantity.Text);
                int userId = Convert.ToInt32(Session["UserID"].ToString());

                UpdateCartQuantity(userId, productId, quantity);

                // Reload cart items
                LoadCartItems();
            }
        }
        private void UpdateCartQuantity(int customerId, int productId, int quantity)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("update_cart_quantity", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_customer_id", OracleDbType.Int32).Value = customerId;
                    cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = productId;
                    cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = quantity;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnProceedCheckout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Customer/Checkout.aspx");
        }

        protected void btnRemoveItem_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "RemoveItem")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserID"].ToString());

                RemoveFromCart(userId, productId);

                // Reload cart items
                LoadCartItems();
            }
        }
        private void RemoveFromCart(int customerId, int productId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("RemoveFromCart", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;
                    cmd.Parameters.Add("p_ProductID", OracleDbType.Int32).Value = productId;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
