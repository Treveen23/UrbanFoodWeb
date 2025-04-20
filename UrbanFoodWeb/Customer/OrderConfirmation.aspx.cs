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
    public partial class OrderConfirmation : System.Web.UI.Page
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

                // Check if order was placed
                if (Session["LastOrderID"] == null)
                {
                    Response.Redirect("~/Customer/Shop.aspx");
                    return;
                }

                int orderId = Convert.ToInt32(Session["LastOrderID"]);
                LoadOrderDetails(orderId);
            }
        }

        protected void LoadOrderDetails(int orderId)
        {
            int customerId = Convert.ToInt32(Session["UserID"].ToString());

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GetOrderDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_OrderID", OracleDbType.Int32).Value = orderId;
                    cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;
                    OracleParameter outputParameter = new OracleParameter("p_Cursor", OracleDbType.RefCursor);
                    outputParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParameter);

                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Display order details
                            lblOrderNumber.Text = orderId.ToString();
                            lblOrderDate.Text = Convert.ToDateTime(reader["OrderDate"]).ToString("MMMM dd, yyyy");
                            lblOrderTotal.Text = string.Format("${0:0.00}", Convert.ToDecimal(reader["TotalAmount"]));
                            lblPaymentMethod.Text = reader["PaymentMethod"].ToString();

                            // Fetch email directly from the procedure output
                            lblEmail.Text = reader["Email"].ToString();
                        }
                        else
                        {
                            // Order not found, redirect
                            Response.Redirect("~/Customer/Shop.aspx");
                        }
                    }
                }
            }
        }

        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            // Clear the order session variable
            Session.Remove("LastOrderID");
            Response.Redirect("~/Customer/Shop.aspx");
        }

        protected void btnViewOrders_Click(object sender, EventArgs e)
        {
            // Clear the order session variable
            Session.Remove("LastOrderID");
            Response.Redirect("~/Customer/Orders.aspx");
        }
    }
}