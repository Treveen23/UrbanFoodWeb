using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Collections.Generic;

namespace UrbanFoodWeb.Supplier
{
    public partial class Dashboard : System.Web.UI.Page
    {
        // Connection string from web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
        private int supplierID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if supplier is logged in
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Customer/Login.aspx");
                return;
            }

            supplierID = Convert.ToInt32(Session["UserID"].ToString());

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // Load supplier name and current date
            ltSupplierName.Text = GetSupplierName(supplierID);
            ltCurrentDate.Text = DateTime.Now.ToString("MMMM dd, yyyy");

            // Load dashboard statistics
           

            // Load recent orders
            LoadRecentOrders();

            // Load low stock products
            LoadLowStockProducts();
        }

        private string GetSupplierName(int supplierId)
        {
            string supplierName = string.Empty;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GET_SUPPLIER_NAME_TO_DASHBOARD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierId;
                    cmd.Parameters.Add("p_supplier_name", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        supplierName = cmd.Parameters["p_supplier_name"].Value.ToString();
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        System.Diagnostics.Debug.WriteLine("Error getting supplier name: " + ex.Message);
                        supplierName = "Supplier";
                    }
                }
            }

            return supplierName;
        }

        
        

        private void LoadRecentOrders()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GET_SUPPLIER_RECENT_ORDERS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierID;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        gvRecentOrders.DataSource = dt;
                        gvRecentOrders.DataBind();
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        System.Diagnostics.Debug.WriteLine("Error loading recent orders: " + ex.Message);
                    }
                }
            }
        }

        private void LoadLowStockProducts()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GET_LOW_STOCK_PRODUCTS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierID;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        gvLowStock.DataSource = dt;
                        gvLowStock.DataBind();
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        System.Diagnostics.Debug.WriteLine("Error loading low stock products: " + ex.Message);
                    }
                }
            }
        }

        protected string GetStatusBadgeClass(string status)
        {
            switch (status.ToLower())
            {
                case "pending":
                    return "badge-status-pending";
                case "delivered":
                    return "badge-status-delivered";
                case "cancelled":
                    return "badge-status-cancelled";
                default:
                    return "badge-secondary";
            }
        }

        // Method to get sales chart data
        protected string GetSalesChartData()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<string> labels = new List<string>();
            List<decimal> values = new List<decimal>();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GET_SUPPLIER_SALES_CHART_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierID;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            labels.Add(reader["MONTH_NAME"].ToString());
                            values.Add(Convert.ToDecimal(reader["SALES_AMOUNT"]));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        System.Diagnostics.Debug.WriteLine("Error getting sales chart data: " + ex.Message);

                        // Provide sample data if error occurs
                        labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
                        values = new List<decimal> { 1200, 1900, 3000, 3500, 2500, 4000 };
                    }
                }
            }

            result["labels"] = labels;
            result["values"] = values;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(result);
        }

        // Method to get category chart data
        protected string GetCategoryChartData()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<string> labels = new List<string>();
            List<int> values = new List<int>();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GET_SUPPLIER_CATEGORY_CHART_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierID;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            labels.Add(reader["CATEGORY_NAME"].ToString());
                            values.Add(Convert.ToInt32(reader["PRODUCT_COUNT"]));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        System.Diagnostics.Debug.WriteLine("Error getting category chart data: " + ex.Message);

                        // Provide sample data if error occurs
                        labels = new List<string> { "Fruits", "Vegetables", "Dairy", "Meat" };
                        values = new List<int> { 15, 12, 8, 10 };
                    }
                }
            }

            result["labels"] = labels;
            result["values"] = values;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(result);
        }
    }
}