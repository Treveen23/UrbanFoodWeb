using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UrbanFoodWeb.Customer
{
    public partial class Orders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Customer/Login.aspx?returnUrl=Orders.aspx");
                    return;
                }

                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            int customerId = Convert.ToInt32(Session["UserID"].ToString());
            DataTable dtOrders = GetCustomerOrders(customerId);

            if (dtOrders.Rows.Count > 0)
            {
                rptOrders.DataSource = dtOrders;
                rptOrders.DataBind();
                pnlOrders.Visible = true;
                pnlNoOrders.Visible = false;
            }
            else
            {
                pnlOrders.Visible = false;
                pnlNoOrders.Visible = true;
            }
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int orderId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewDetails")
            {
                // Find the order details panel
                Panel pnlOrderDetails = (Panel)e.Item.FindControl("pnlOrderDetails");
                GridView gvOrderItems = (GridView)e.Item.FindControl("gvOrderItems");

                // Toggle visibility - if it's visible, hide it; if it's hidden, show it and load data
                if (pnlOrderDetails.Visible)
                {
                    pnlOrderDetails.Visible = false;
                }
                else
                {
                    // Load order items
                    DataTable dtOrderItems = GetOrderItems(orderId);
                    gvOrderItems.DataSource = dtOrderItems;
                    gvOrderItems.DataBind();
                    pnlOrderDetails.Visible = true;
                }
            }
            else if (e.CommandName == "CancelOrder")
            {
                int customerId = Convert.ToInt32(Session["UserID"].ToString());
                int result = CancelOrder(orderId, customerId);

                switch (result)
                {
                    case 1: // Success
                        ShowMessage("Your order has been cancelled successfully.", "success");
                        LoadOrders(); // Refresh the orders list
                        break;
                    case 0: // Order not found
                        ShowMessage("Order not found or does not belong to your account.", "danger");
                        break;
                    case 2: // Cannot cancel
                        ShowMessage("This order cannot be cancelled because it is already being processed.", "warning");
                        break;
                    default: // Error
                        ShowMessage("An error occurred while trying to cancel your order. Please try again later.", "danger");
                        break;
                }
            }
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                // Set the appropriate background color for delivery status
                string deliveryStatus = drv["DeliveryStatus"].ToString();
                Label lblDeliveryStatus = (Label)e.Item.FindControl("lblDeliveryStatus");

                if (lblDeliveryStatus != null)
                {
                    switch (deliveryStatus.ToLower())
                    {
                        case "delivered":
                            lblDeliveryStatus.CssClass = "badge bg-success";
                            break;
                        case "shipping":
                            lblDeliveryStatus.CssClass = "badge bg-primary";
                            break;
                        case "cancelled":
                            lblDeliveryStatus.CssClass = "badge bg-danger";
                            break;
                        default:
                            lblDeliveryStatus.CssClass = "badge bg-info";
                            break;
                    }
                }
            }
        }

        protected void btnBackToShop_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Customer/Shop.aspx");
        }

        private void ShowMessage(string message, string type)
        {
            litMessage.Text = message;
            pnlMessage.Visible = true;

            // Add the appropriate Bootstrap alert class based on the message type
            pnlMessage.CssClass = $"toast-container position-fixed bottom-0 end-0 p-3 alert alert-{type}";

            // Register a script to auto-hide the message after 5 seconds
            ScriptManager.RegisterStartupScript(this, GetType(), "HideMessage",
                "setTimeout(function() { document.getElementById('" + pnlMessage.ClientID + "').style.display='none'; }, 5000);", true);
        }

        #region Database Operations

        private DataTable GetCustomerOrders(int customerId)
        {
            DataTable dt = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("GetCustomerOrdersCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;

                        OracleParameter outputParameter = new OracleParameter("p_Cursor", OracleDbType.RefCursor);
                        outputParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParameter);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error in GetCustomerOrders: " + ex.Message);
            }

            return dt;
        }

        private DataTable GetOrderItems(int orderId)
        {
            DataTable dt = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("GetOrderItemsCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OrderID", OracleDbType.Int32).Value = orderId;

                        OracleParameter outputParameter = new OracleParameter("p_Cursor", OracleDbType.RefCursor);
                        outputParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParameter);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error in GetOrderItems: " + ex.Message);
            }

            return dt;
        }

        private int CancelOrder(int orderId, int customerId)
        {
            int result = -1;
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("CancelOrderCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OrderID", OracleDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("p_CustomerID", OracleDbType.Int32).Value = customerId;

                        OracleParameter outputParameter = new OracleParameter("p_Result", OracleDbType.Int32);
                        outputParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParameter);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        result = Convert.ToInt32(outputParameter.Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error in CancelOrder: " + ex.Message);
                result = -1;
            }

            return result;
        }

        #endregion
    }
}