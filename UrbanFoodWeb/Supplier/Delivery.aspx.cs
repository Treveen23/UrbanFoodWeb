using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UrbanFoodWeb.Supplier
{

    public partial class Delivery : System.Web.UI.Page
    {
        protected string connString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
        protected int supplierId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserRole"] == null || Session["UserRole"].ToString() != "Supplier")
            {
                Response.Redirect("~/Customer/Login.aspx");
                return;
            }


            supplierId = Convert.ToInt32(Session["UserID"].ToString());

            if (!IsPostBack)
            {
                LoadDeliveryRequests();
            }
        }
        private void LoadDeliveryRequests()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                using (OracleCommand cmd = new OracleCommand("GetSupplierDeliveries", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                    cmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = DBNull.Value;

                    OracleParameter pCursor = cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor);
                    pCursor.Direction = ParameterDirection.Output;
                    conn.Open();
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        gvDeliveryRequests.DataSource = dt;
                        gvDeliveryRequests.DataBind();
                    }
                    else
                    {
                        // Handle no data case
                        dt.Rows.Add(dt.NewRow());
                        gvDeliveryRequests.DataSource = dt;
                        gvDeliveryRequests.DataBind();

                        int totalColumns = gvDeliveryRequests.Rows[0].Cells.Count;
                        gvDeliveryRequests.Rows[0].Cells.Clear();
                        gvDeliveryRequests.Rows[0].Cells.Add(new TableCell());
                        gvDeliveryRequests.Rows[0].Cells[0].ColumnSpan = totalColumns;
                        gvDeliveryRequests.Rows[0].Cells[0].Text = "No delivery requests found.";
                        gvDeliveryRequests.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
        }


        protected void gvDeliveryRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int deliveryId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewDetails")
            {
                ViewDeliveryDetails(deliveryId);
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowDetailsModal", "showDeliveryDetailsModal();", true);
            }
            else if (e.CommandName == "UpdateStatus")
            {
                hdnDeliveryID.Value = deliveryId.ToString();
                LoadDeliveryForUpdate(deliveryId);
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowUpdateModal", "showUpdateStatusModal();", true);
            }
        }
        private void ViewDeliveryDetails(int deliveryId)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                // Get delivery details
                using (OracleCommand cmd = new OracleCommand("GetDeliveryDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_DeliveryID", OracleDbType.Int32).Value = deliveryId;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;

                    OracleParameter pCursor = cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor);
                    pCursor.Direction = ParameterDirection.Output;

                    conn.Open();
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblDeliveryID.Text = reader["DeliveryID"].ToString();
                        lblOrderID.Text = reader["OrderID"].ToString();
                        lblCustomerName.Text = reader["CustomerName"].ToString();
                        lblContact.Text = reader["Contact"].ToString();
                        lblAddress.Text = reader["Address"].ToString();
                        lblRequestDate.Text = Convert.ToDateTime(reader["RequestDate"]).ToString("dd/MM/yyyy");

                        if (reader["DeliveryDate"] != DBNull.Value)
                            lblDeliveryDate.Text = Convert.ToDateTime(reader["DeliveryDate"]).ToString("dd/MM/yyyy");
                        else
                            lblDeliveryDate.Text = "Not scheduled";

                        lblStatus.Text = reader["Status"].ToString();
                        lblNotes.Text = reader["Notes"].ToString();
                    }
                    reader.Close();

                }
                // Get order items
                using (OracleCommand cmd = new OracleCommand("GetDeliveryItems", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_DeliveryID", OracleDbType.Int32).Value = deliveryId;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;

                    OracleParameter pCursor = cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor);
                    pCursor.Direction = ParameterDirection.Output;

                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvOrderItems.DataSource = dt;
                    gvOrderItems.DataBind();
                }
            }
        }

        private void LoadDeliveryForUpdate(int deliveryId)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                using (OracleCommand cmd = new OracleCommand("GetDeliveryDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_DeliveryID", OracleDbType.Int32).Value = deliveryId;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;

                    OracleParameter pCursor = cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor);
                    pCursor.Direction = ParameterDirection.Output;

                    conn.Open();
                    OracleDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        ddlStatus.SelectedValue = reader["Status"].ToString();

                        if (reader["DeliveryDate"] != DBNull.Value)
                        {
                            DateTime deliveryDate = Convert.ToDateTime(reader["DeliveryDate"]);
                            txtDeliveryDate.Text = deliveryDate.ToString("yyyy-MM-dd");
                        }

                        txtNotes.Text = reader["Notes"].ToString();
                    }
                }
            }
        }

        protected void gvDeliveryRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                LinkButton btnUpdate = (LinkButton)e.Row.FindControl("btnUpdate");

                // Customize row based on status
                switch (status)
                {
                    case "Delivered":
                        e.Row.CssClass = "table-success";
                        btnUpdate.Visible = false;
                        break;
                    case "Cancelled":
                        e.Row.CssClass = "table-danger";
                        btnUpdate.Visible = false;
                        break;
                    case "Out for Delivery":
                        e.Row.CssClass = "table-info";
                        break;
                    case "Processing":
                        e.Row.CssClass = "table-warning";
                        break;
                }
            }
        }




        protected void btnSaveStatus_Click(object sender, EventArgs e)
        {
            int deliveryId = Convert.ToInt32(hdnDeliveryID.Value);
            string status = ddlStatus.SelectedValue;
            DateTime? deliveryDate = null;

            if (!string.IsNullOrEmpty(txtDeliveryDate.Text))
                deliveryDate = Convert.ToDateTime(txtDeliveryDate.Text);

            using (OracleConnection conn = new OracleConnection(connString))
            {
                using (OracleCommand cmd = new OracleCommand("UpdateDeliveryStatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_DeliveryID", OracleDbType.Int32).Value = deliveryId;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                    cmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = status;

                    if (deliveryDate.HasValue)
                        cmd.Parameters.Add("p_DeliveryDate", OracleDbType.Date).Value = deliveryDate.Value;
                    else
                        cmd.Parameters.Add("p_DeliveryDate", OracleDbType.Date).Value = DBNull.Value;

                    cmd.Parameters.Add("p_Notes", OracleDbType.Varchar2).Value = txtNotes.Text;

                    OracleParameter pSuccess = cmd.Parameters.Add("p_Success", OracleDbType.Int32);
                    pSuccess.Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    if (Convert.ToInt32(pSuccess.Value.ToString()) == 1)
                    {
                        // Success
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess",
                            "alert('Delivery status updated successfully!'); $('#updateStatusModal').modal('hide');", true);
                        LoadDeliveryRequests();
                    }
                    else
                    {
                        // Error
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowError",
                            "alert('Failed to update delivery status. Please try again.');", true);
                    }
                }
            }
        }

    }
}