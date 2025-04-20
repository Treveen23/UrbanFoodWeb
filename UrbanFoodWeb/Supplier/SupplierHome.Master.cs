using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UrbanFoodWeb.Supplier
{
    public partial class SupplierHome : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Customer/Login.aspx");
                    return;
                }

                LoadSupplierName();
            }
        }

        private void LoadSupplierName()
        {
            // Ensure session exists before accessing it
            if (Session["UserID"] != null)
            {
                int supplierId = Convert.ToInt32(Session["UserID"].ToString());
                string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        using (OracleCommand cmd = new OracleCommand("GetSupplierName", conn))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            // IN parameter
                            cmd.Parameters.Add("p_supplierId", OracleDbType.Varchar2).Value = supplierId;

                            // OUT parameter
                            cmd.Parameters.Add("p_supplierName", OracleDbType.Varchar2, 100).Direction = System.Data.ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            // Display supplier name in label
                            lblSupplierName.Text = cmd.Parameters["p_supplierName"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error or handle it gracefully
                        lblSupplierName.Text = "Unknown Supplier";
                        // Optional: log ex.Message for debugging purposes
                    }
                }
            }
            else
            {
                // Redirect to login page if session does not exist
                Response.Redirect("~/Customer/Login.aspx");
            }
        }
    }
}