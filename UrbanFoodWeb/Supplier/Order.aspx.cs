using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UrbanFoodWeb.Supplier
{
    public partial class Order : System.Web.UI.Page
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
        private int supplierId;

        protected void Page_Load(object sender, EventArgs e)
        {

            // Check if supplier is logged in
            if (Session["UserID"] == null || Session["UserRole"] == null || Session["UserRole"].ToString() != "Supplier")
            {

                return;
            }

            // Get the supplier ID from session

            supplierId = Convert.ToInt32(Session["UserID"].ToString());


            if (!IsPostBack)
            {
                // Set default dates for filtering (last 30 days)
                txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                // Load orders
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            try
            {

                string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("GetSupplierOrders", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.Add("p_SupplierId", OracleDbType.Int32).Value = supplierId;

                        // Add filter parameters
                        cmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = ddlOrderStatus.SelectedValue;

                        // Handle date parameters
                        if (DateTime.TryParse(txtFromDate.Text, out DateTime fromDate))
                        {
                            cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                        }
                        else
                        {
                            cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = DBNull.Value;
                        }

                        if (DateTime.TryParse(txtToDate.Text, out DateTime toDate))
                        {
                            cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                        }
                        else
                        {
                            cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = DBNull.Value;
                        }

                        // Output cursor parameter
                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Execute command and get data
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Bind data to GridView
                        gvOrders.DataSource = dt;
                        gvOrders.DataBind();

                        // Show message if no orders found
                        if (dt.Rows.Count == 0)
                        {
                            // You can add a label for "No orders found" message if desired
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or show error message
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                    $"alert('Error loading orders: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }
        protected string GetStatusBadgeClass(string status)
        {
            switch (status.ToLower())
            {
                case "new":
                    return "badge badge-info";
                case "confirmed":
                    return "badge badge-primary";
                case "processing":
                    return "badge badge-warning";
                case "shipped":
                    return "badge badge-secondary";
                case "delivered":
                    return "badge badge-success";
                case "cancelled":
                    return "badge badge-danger";
                default:
                    return "badge badge-light";
            }
        }

        protected void ddlOrderStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlOrderStatus.SelectedValue = "";
            txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            LoadOrders();
        }

        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "ViewOrder")
            {
                // TEST: Simple alert to see if button fires correctly
                ScriptManager.RegisterStartupScript(this, GetType(), "testViewBtn",
                    $"alert('View button clicked. OrderID: {e.CommandArgument}');", true);

                // Your original code
                int orderId = Convert.ToInt32(e.CommandArgument);
                LoadOrderDetails(orderId);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "showOrderModal();", true);

            }

            else if (e.CommandName == "ConfirmOrder")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                hdnOrderIdForConfirmation.Value = orderId.ToString();

                // Set default estimated delivery date (7 days from now)
                txtEstimatedDelivery.Text = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowConfirmModal", "showConfirmOrderModal();", true);
            }
            else if (e.CommandName == "RejectOrder")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                hdnOrderIdForRejection.Value = orderId.ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowRejectModal", "showRejectOrderModal();", true);

            }


            else if (e.CommandName == "UpdateStatus")
            {
                string commandArg = e.CommandArgument.ToString();

                if (commandArg.Contains("|"))
                {
                    string[] args = commandArg.Split('|');

                    int orderIdFromCommand;
                    if (int.TryParse(args[0], out orderIdFromCommand))
                    {
                        string status = args[1];

                        hdnOrderIdForStatus.Value = orderIdFromCommand.ToString();

                        if (ddlUpdateStatus.Items.FindByValue(status) != null)
                        {
                            ddlUpdateStatus.SelectedValue = status;
                        }

                        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                            "setTimeout(function(){ $('#updateStatusModal').modal('show'); }, 500);", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                            "alert('Invalid Order ID format.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                        "alert('Invalid command argument format.');", true);
                }
            }






        }








        private void LoadOrderDetails(int orderId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    // 1. Load order header information
                    using (OracleCommand cmd = new OracleCommand("GetSupplierOrderDetails", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OrderId", OracleDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("p_SupplierId", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblOrderId.Text = reader["OrderId"].ToString();
                                lblOrderDate.Text = Convert.ToDateTime(reader["OrderDate"]).ToString("dd MMM yyyy, hh:mm tt");
                                lblOrderStatus.Text = reader["OrderStatus"].ToString();
                                lblPaymentMethod.Text = reader["PaymentMethod"].ToString();
                                lblTotalAmount.Text = string.Format(CultureInfo.CurrentCulture, "{0:C}", Convert.ToDecimal(reader["TotalAmount"]));
                                lblCustomerName.Text = reader["CustomerName"].ToString();
                                lblCustomerEmail.Text = reader["CustomerEmail"].ToString();
                                lblCustomerPhone.Text = reader["CustomerPhone"].ToString();
                                lblCustomerAddress.Text = reader["ShippingAddress"].ToString();

                                // Show confirmation panel only for "New" orders
                                pnlConfirmPanel.Visible = reader["OrderStatus"].ToString() == "New";
                            }
                        }
                    }



                    // 2. Load order items
                    using (OracleCommand cmd = new OracleCommand("GetSupplierOrderItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OrderId", OracleDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("p_SupplierId", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        gvOrderItems.DataSource = dt;
                        gvOrderItems.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                    $"alert('Error loading order details: {ex.Message.Replace("'", "\\'")}');", true);
            }



        }

        protected void btnQuickConfirm_Click(object sender, EventArgs e)
        {
            // Set order ID from currently viewed order details
            int orderId = Convert.ToInt32(lblOrderId.Text);
            hdnOrderIdForConfirmation.Value = orderId.ToString();

            // Set default estimated delivery date (7 days from now)
            txtEstimatedDelivery.Text = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowConfirmModal", "showConfirmOrderModal();", true);
        }

        protected void btnQuickReject_Click(object sender, EventArgs e)
        {
            // Set order ID from currently viewed order details
            int orderId = Convert.ToInt32(lblOrderId.Text);
            hdnOrderIdForRejection.Value = orderId.ToString();

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowRejectModal", "showRejectOrderModal();", true);

        }

        protected void btnUpdateOrderStatus_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(hdnOrderIdForStatus.Value);
                string status = ddlUpdateStatus.SelectedValue;
                string notes = txtStatusNotes.Text;


                string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("UpdateOrderStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OrderId", OracleDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("p_SupplierId", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = status;
                        cmd.Parameters.Add("p_Notes", OracleDbType.Varchar2).Value = notes;
                        cmd.Parameters.Add("p_Success", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        int success = Convert.ToInt32(cmd.Parameters["p_Success"].Value.ToString());
                        if (success == 1)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert",
                                "alert('Order status updated successfully!'); $('#updateStatusModal').modal('hide');", true);
                            LoadOrders();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                                "alert('Failed to update order status. Please try again.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                    $"alert('Error updating order status: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        protected void btnConfirmReject_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(hdnOrderIdForRejection.Value);
                string reason = txtRejectionReason.Text;
                bool notifyCustomer = chkNotifyCustomer.Checked;


                string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("RejectOrder", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OrderId", OracleDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("p_SupplierId", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_Reason", OracleDbType.Varchar2).Value = reason;
                        cmd.Parameters.Add("p_NotifyCustomer", OracleDbType.Int32).Value = notifyCustomer ? 1 : 0;
                        cmd.Parameters.Add("p_Success", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        int success = Convert.ToInt32(cmd.Parameters["p_Success"].Value.ToString());
                        if (success == 1)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert",
                                "alert('Order rejected successfully!'); $('#rejectOrderModal').modal('hide');", true);
                            LoadOrders();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                                "alert('Failed to reject order. Please try again.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                    $"alert('Error rejecting order: {ex.Message.Replace("'", "\\'")}');", true);
            }

        }

        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(hdnOrderIdForConfirmation.Value);
                DateTime estimatedDelivery = DateTime.Parse(txtEstimatedDelivery.Text);
                string notes = txtConfirmationNotes.Text;
                bool sendConfirmation = chkSendConfirmation.Checked;


                string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("ConfirmOrder", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OrderId", OracleDbType.Int32).Value = orderId;
                        cmd.Parameters.Add("p_SupplierId", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_EstimatedDelivery", OracleDbType.Date).Value = estimatedDelivery;
                        cmd.Parameters.Add("p_Notes", OracleDbType.Varchar2).Value = notes;
                        cmd.Parameters.Add("p_SendConfirmation", OracleDbType.Int32).Value = sendConfirmation ? 1 : 0;
                        cmd.Parameters.Add("p_Success", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        int success = Convert.ToInt32(cmd.Parameters["p_Success"].Value.ToString());
                        if (success == 1)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "SuccessAlert",
                                "alert('Order confirmed successfully!'); $('#confirmOrderModal').modal('hide');", true);
                            LoadOrders();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                                "alert('Failed to confirm order. Please try again.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert",
                    $"alert('Error confirming order: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }


    }
}