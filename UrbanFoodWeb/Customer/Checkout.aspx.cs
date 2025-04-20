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
    public partial class Checkout : System.Web.UI.Page
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

                // Load user's cart items for order summary
                LoadCartItems();
                LoadCustomerProfile();

            }
        }

        protected void LoadCartItems()
        {
            int customerId = Convert.ToInt32(Session["UserID"].ToString());

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GetCartItems", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;
                    OracleParameter outputParameter = new OracleParameter("p_Cursor", OracleDbType.RefCursor);
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Check if cart has items
                        if (dt.Rows.Count > 0)
                        {
                            rptOrderSummary.DataSource = dt;
                            rptOrderSummary.DataBind();

                            // Calculate totals
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

                            // Store order details in session for order processing
                            Session["OrderSubtotal"] = subtotal;
                            Session["OrderShipping"] = shipping;
                            Session["OrderTotal"] = total;
                        }
                        else
                        {
                            // No items in cart, redirect to cart page
                            Response.Redirect("~/Customer/Cart.aspx");
                        }
                    }
                }
            }
        }

        protected void LoadCustomerProfile()
        {
            int customerId = Convert.ToInt32(Session["UserID"].ToString());

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GetCustomerProfile", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;
                    OracleParameter outputParameter = new OracleParameter("p_Cursor", OracleDbType.RefCursor);
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);

                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];

                            // Populate form with user data
                            txtFirstName.Text = row["FirstName"].ToString();
                            txtLastName.Text = row["LastName"].ToString();
                            txtEmail.Text = row["Email"].ToString();
                            txtPhone.Text = row["PhoneNumber"].ToString();
                            txtAddress.Text = row["Address"].ToString();
                            
                        }
                    }
                }
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            int customerId = Convert.ToInt32(Session["UserID"].ToString());
            decimal subtotal = Convert.ToDecimal(Session["OrderSubtotal"]);
            decimal shipping = Convert.ToDecimal(Session["OrderShipping"]);
            decimal total = Convert.ToDecimal(Session["OrderTotal"]);

            // Get payment method from selected RadioButton
            string paymentMethod = "PayPal"; // Default

            if (rdoCashOnDelivery.Checked)
            {
                paymentMethod = "Cash On Delivery";
            }
            else if (rdoBankTransfer.Checked)
            {
                paymentMethod = "Bank Transfer";
            }
            // If PayPal is still selected (or default), it's already set

            // Create order
            int orderId = CreateOrder(customerId, subtotal, shipping, total, paymentMethod);

            if (orderId > 0)
            {
                // Move cart items to order items
                if (MoveCartItemsToOrder(customerId, orderId))
                {
                    // Clear cart
                    ClearCart(customerId);

                    // Store order ID in session
                    Session["LastOrderID"] = orderId;

                    // Redirect to order confirmation page
                    Response.Redirect("~/Customer/OrderConfirmation.aspx");
                }
                else
                {
                    // Error handling
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "orderError",
                        "alert('There was an error processing your order. Please try again.');", true);
                }
            }
            else
            {
                // Error handling
                ScriptManager.RegisterStartupScript(this, this.GetType(), "orderError",
                    "alert('There was an error creating your order. Please try again.');", true);
            }
        }

        private int CreateOrder(int customerId, decimal subtotal, decimal shipping, decimal total, string paymentMethod)
        {
            int orderId = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("CreateOrder", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;
                    cmd.Parameters.Add("p_ShippingAddress", OracleDbType.Varchar2).Value =
                        txtAddress.Text + ", " + txtCity.Text + ", " + txtState.Text + " " + txtZip.Text;
                    cmd.Parameters.Add("p_BillingAddress", OracleDbType.Varchar2).Value =
                        txtAddress.Text + ", " + txtCity.Text + ", " + txtState.Text + " " + txtZip.Text;
                    cmd.Parameters.Add("p_Subtotal", OracleDbType.Decimal).Value = subtotal;
                    cmd.Parameters.Add("p_ShippingCost", OracleDbType.Decimal).Value = shipping;
                    cmd.Parameters.Add("p_TotalAmount", OracleDbType.Decimal).Value = total;
                    cmd.Parameters.Add("p_PaymentMethod", OracleDbType.Varchar2).Value = paymentMethod;

                    // Output parameter for order ID
                    OracleParameter outputParameter = new OracleParameter("p_OrderID", OracleDbType.Int32);
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    // Get the order ID
                    if (outputParameter.Value != null && outputParameter.Value != DBNull.Value)
                    {
                        var oracleDecimalValue = (Oracle.ManagedDataAccess.Types.OracleDecimal)outputParameter.Value;
                        orderId = oracleDecimalValue.ToInt32(); // Convert safely to int
                    }
                    else
                    {
                        Console.WriteLine("Output parameter is null or DBNull.");
                    }

                }
            }

            return orderId;
        }

        private bool MoveCartItemsToOrder(int customerId, int orderId)
        {
            bool success = false;

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("MoveCartItemsToOrder", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;
                    cmd.Parameters.Add("p_OrderID", OracleDbType.Int32).Value = orderId;

                    // Output parameter for success
                    OracleParameter outputParameter = new OracleParameter("p_Success", OracleDbType.Int32);
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Check if successful
                    if (outputParameter.Value != null && outputParameter.Value != DBNull.Value)
                    {
                        // Safely convert OracleDecimal to int
                        var oracleDecimalValue = (Oracle.ManagedDataAccess.Types.OracleDecimal)outputParameter.Value;
                        success = oracleDecimalValue.ToInt32() == 1; // Compare value safely
                    }
                    else
                    {
                        success = false; // Handle null or DBNull case
                    }
                }
            }

            return success;
        }

        private void ClearCart(int userId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("ClearCart", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = userId;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}