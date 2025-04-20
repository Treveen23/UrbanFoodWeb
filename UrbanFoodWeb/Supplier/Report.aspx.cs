using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Text;
using System.Web.UI.HtmlControls;

namespace UrbanFoodWeb.Supplier
{
    public partial class Report : System.Web.UI.Page
    {
        private OracleConnection con;
        private int supplierId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if supplier is logged in
            if (Session["UserID"] == null || Session["UserRole"] == null || Session["UserRole"].ToString() != "Supplier")
            {
                Response.Redirect("~/Customer/Login.aspx", true);
                return;
            }

            // Get the supplier ID from session
            supplierId = Convert.ToInt32(Session["UserID"].ToString());

            if (!IsPostBack)
            {
                // Set default date range (last 30 days)
                txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                // Load initial report data
                LoadReportData();
            }
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadReportData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            LoadReportData();
        }

        protected void btnDownloadReport_Click(object sender, EventArgs e)
        {
            string reportType = ddlReportType.SelectedValue;
            string reportFormat = ddlReportFormat.SelectedValue;

            try
            {
                // Generate the report based on selected type and format
                switch (reportType)
                {
                    case "SalesReport":
                        GenerateSalesReport(reportFormat);
                        break;
                    case "ProductSalesReport":
                        GenerateProductSalesReport(reportFormat);
                        break;
                    case "CategorySalesReport":
                        GenerateCategorySalesReport(reportFormat);
                        break;
                    case "OrderStatusReport":
                        GenerateOrderStatusReport(reportFormat);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Handle error
                string script = $"alert('Failed to generate report: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
            }
        }

        private void LoadReportData()
        {
            try
            {
                DateTime fromDate;
                DateTime toDate;

                // Make sure we can parse the dates
                if (!DateTime.TryParse(txtFromDate.Text, out fromDate))
                    fromDate = DateTime.Now.AddDays(-30);

                if (!DateTime.TryParse(txtToDate.Text, out toDate))
                    toDate = DateTime.Now;

                // Open database connection
                string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
                using (con = new OracleConnection(connectionString))
                {
                    con.Open();

                    // Load dashboard summary data
                    LoadDashboardSummary(fromDate, toDate);

                    // Load top products data
                    LoadTopProducts(fromDate, toDate);

                    // Load category sales data
                    LoadCategorySales(fromDate, toDate);

                    // Load recent orders data
                    LoadRecentOrders(fromDate, toDate);
                }
            }
            catch (Exception ex)
            {
                // Handle error
                string script = $"alert('Failed to load report data: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorAlert", script, true);
            }
        }

        private void LoadDashboardSummary(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand("GetSupplierMonthlySales", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                    cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;

                    OracleParameter totalSalesParam = new OracleParameter("p_TotalSales", OracleDbType.Decimal);
                    totalSalesParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(totalSalesParam);

                    OracleParameter totalOrdersParam = new OracleParameter("p_TotalOrders", OracleDbType.Int32);
                    totalOrdersParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(totalOrdersParam);

                    cmd.ExecuteNonQuery();

                    // Check if output parameters are DBNull or null
                    decimal totalSales = totalSalesParam.Value != DBNull.Value && totalSalesParam.Value != null
                        ? Convert.ToDecimal(totalSalesParam.Value.ToString()) : 0;

                    int totalOrders = totalOrdersParam.Value != DBNull.Value && totalOrdersParam.Value != null
                        ? Convert.ToInt32(totalOrdersParam.Value.ToString()) : 0;

                    lblMonthlySales.Text = totalSales.ToString("C");
                    lblMonthlyOrders.Text = totalOrders.ToString();
                }

                // Get active products count
                using (OracleCommand cmd = new OracleCommand("GetSupplierActiveProducts", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;

                    OracleParameter activeCountParam = new OracleParameter("p_ActiveCount", OracleDbType.Int32);
                    activeCountParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(activeCountParam);

                    cmd.ExecuteNonQuery();

                    int activeProducts = activeCountParam.Value != DBNull.Value && activeCountParam.Value != null
                        ? Convert.ToInt32(activeCountParam.Value.ToString()) : 0;

                    lblActiveProducts.Text = activeProducts.ToString();
                }

                // Get average rating
                using (OracleCommand cmd = new OracleCommand("GetSupplierAverageRating", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;

                    OracleParameter avgRatingParam = new OracleParameter("p_AverageRating", OracleDbType.Decimal);
                    avgRatingParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(avgRatingParam);

                    cmd.ExecuteNonQuery();

                    decimal averageRating = avgRatingParam.Value != DBNull.Value && avgRatingParam.Value != null
                        ? Convert.ToDecimal(avgRatingParam.Value.ToString()) : 0;

                    lblAverageRating.Text = averageRating.ToString("0.0");

                    // Set progress bar width based on rating (max 5.0)
                    int ratingPercentage = (int)((averageRating / 5m) * 100);
                    ratingProgressBar.Style["width"] = ratingPercentage + "%";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading dashboard summary: " + ex.Message);
            }
        }

        private void LoadTopProducts(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand("GetSupplierTopProducts", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                    cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            gvTopProducts.DataSource = dt;
                            gvTopProducts.DataBind();
                        }
                        else
                        {
                            // Handle empty data
                            dt = new DataTable();
                            dt.Columns.Add("ProductID", typeof(int));
                            dt.Columns.Add("ProductName", typeof(string));
                            dt.Columns.Add("TotalQuantity", typeof(int));
                            dt.Columns.Add("TotalSales", typeof(decimal));
                            dt.Rows.Add(0, "No products found in this date range", 0, 0);

                            gvTopProducts.DataSource = dt;
                            gvTopProducts.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading top products: " + ex.Message);
            }
        }

        private void LoadCategorySales(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand("GetSupplierCategorySales", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                    cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            gvCategorySales.DataSource = dt;
                            gvCategorySales.DataBind();
                        }
                        else
                        {
                            // Handle empty data
                            dt = new DataTable();
                            dt.Columns.Add("CategoryID", typeof(int));
                            dt.Columns.Add("CategoryName", typeof(string));
                            dt.Columns.Add("ProductCount", typeof(int));
                            dt.Columns.Add("TotalSales", typeof(decimal));
                            dt.Rows.Add(0, "No category sales found in this date range", 0, 0);

                            gvCategorySales.DataSource = dt;
                            gvCategorySales.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading category sales: " + ex.Message);
            }
        }

        private void LoadRecentOrders(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand("GetSupplierRecentOrders", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                    cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            gvRecentOrders.DataSource = dt;
                            gvRecentOrders.DataBind();
                        }
                        else
                        {
                            // Handle empty data
                            dt = new DataTable();
                            dt.Columns.Add("OrderID", typeof(int));
                            dt.Columns.Add("CustomerName", typeof(string));
                            dt.Columns.Add("OrderDate", typeof(DateTime));
                            dt.Columns.Add("TotalAmount", typeof(decimal));
                            dt.Columns.Add("Status", typeof(string));
                            dt.Rows.Add(0, "No orders found in this date range", DateTime.Now, 0, "N/A");

                            gvRecentOrders.DataSource = dt;
                            gvRecentOrders.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading recent orders: " + ex.Message);
            }
        }

        private void GenerateSalesReport(string format)
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);

            try
            {
                // Create a DataTable for the report
                DataTable dt = new DataTable();

                using (con = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString))
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("GetSupplierSalesTrend", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                // Export the data based on the selected format
                switch (format)
                {
                    case "Excel":
                        ExportToExcel(dt, "SalesReport");
                        break;
                    case "PDF":
                        ExportToPDF(dt, "SalesReport");
                        break;
                    case "CSV":
                        ExportToCSV(dt, "SalesReport");
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate Sales Report: " + ex.Message);
            }
        }

        private void GenerateProductSalesReport(string format)
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);

            try
            {
                // Create a DataTable for the report
                DataTable dt = new DataTable();

                using (con = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString))
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("GetSupplierTopProducts", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                // Export the data based on the selected format
                switch (format)
                {
                    case "Excel":
                        ExportToExcel(dt, "ProductSalesReport");
                        break;
                    case "PDF":
                        ExportToPDF(dt, "ProductSalesReport");
                        break;
                    case "CSV":
                        ExportToCSV(dt, "ProductSalesReport");
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate Product Sales Report: " + ex.Message);
            }
        }

        private void GenerateCategorySalesReport(string format)
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);

            try
            {
                // Create a DataTable for the report
                DataTable dt = new DataTable();

                using (con = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString))
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("GetSupplierCategorySales", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                // Export the data based on the selected format
                switch (format)
                {
                    case "Excel":
                        ExportToExcel(dt, "CategorySalesReport");
                        break;
                    case "PDF":
                        ExportToPDF(dt, "CategorySalesReport");
                        break;
                    case "CSV":
                        ExportToCSV(dt, "CategorySalesReport");
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate Category Sales Report: " + ex.Message);
            }
        }

        private void GenerateOrderStatusReport(string format)
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);

            try
            {
                // Create a DataTable for the report
                DataTable dt = new DataTable();

                using (con = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString))
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("GetSupplierRecentOrders", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_SupplierID", OracleDbType.Int32).Value = supplierId;
                        cmd.Parameters.Add("p_FromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add("p_ToDate", OracleDbType.Date).Value = toDate;
                        cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                // Export the data based on the selected format
                switch (format)
                {
                    case "Excel":
                        ExportToExcel(dt, "OrderStatusReport");
                        break;
                    case "PDF":
                        ExportToPDF(dt, "OrderStatusReport");
                        break;
                    case "CSV":
                        ExportToCSV(dt, "OrderStatusReport");
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate Order Status Report: " + ex.Message);
            }
        }

        private void ExportToExcel(DataTable dt, string fileName)
        {
            try
            {
                // Create a simple Excel file 
                StringBuilder sb = new StringBuilder();

                // Add column headers
                foreach (DataColumn col in dt.Columns)
                {
                    sb.Append(col.ColumnName + ",");
                }
                sb.Append("\r\n");

                // Add rows
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        // Handle nulls and replace commas
                        string value = row[col] == DBNull.Value ? "" : row[col].ToString().Replace(",", ";");
                        sb.Append(value + ",");
                    }
                    sb.Append("\r\n");
                }

                // Send the file to the browser
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".csv");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                throw new Exception("Export to Excel failed: " + ex.Message);
            }
        }

        private void ExportToPDF(DataTable dt, string fileName)
        {
            try
            {
                // For simplicity, we'll create a CSV instead of a true PDF
                // In production, you'd use a library like iTextSharp
                ExportToCSV(dt, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Export to PDF failed: " + ex.Message);
            }
        }

        private void ExportToCSV(DataTable dt, string fileName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // Add column headers
                foreach (DataColumn col in dt.Columns)
                {
                    sb.Append(col.ColumnName + ",");
                }
                sb.Append("\r\n");

                // Add rows
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        // Handle nulls and replace commas
                        string value = row[col] == DBNull.Value ? "" : row[col].ToString().Replace(",", ";");
                        sb.Append(value + ",");
                    }
                    sb.Append("\r\n");
                }

                // Send the file to the browser
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".csv");
                Response.Charset = "";
                Response.ContentType = "text/csv";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                throw new Exception("Export to CSV failed: " + ex.Message);
            }
        }
    }
}