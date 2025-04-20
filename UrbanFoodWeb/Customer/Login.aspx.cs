using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;

namespace UrbanFoodWeb.Customer
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlCustomerMessage.Visible = true;
                pnlSupplierMessage.Visible = false;
                btnCustomer.CssClass = "btn btn-toggle toggle-active me-2";
                btnSupplier.CssClass = "btn btn-toggle toggle-inactive";
                Session["UserType"] = "Customer";
            }
        }

        protected void btnCustomer_Click(object sender, EventArgs e)
        {
            // Update UI for Customer login
            pnlCustomerMessage.Visible = true;
            pnlSupplierMessage.Visible = false;

            // Update toggle button styles
            btnCustomer.CssClass = "btn btn-toggle toggle-active me-2";
            btnSupplier.CssClass = "btn btn-toggle toggle-inactive";

            // Update session
            Session["UserType"] = "Customer";

            // Clear error message if any
            pnlErrorMessage.Visible = false;
        }

        protected void btnSupplier_Click(object sender, EventArgs e)
        {
            // Update UI for Supplier login
            pnlCustomerMessage.Visible = false;
            pnlSupplierMessage.Visible = true;

            // Update toggle button styles
            btnCustomer.CssClass = "btn btn-toggle toggle-inactive me-2";
            btnSupplier.CssClass = "btn btn-toggle toggle-active";

            // Update session
            Session["UserType"] = "Supplier";

            // Clear error message if any
            pnlErrorMessage.Visible = false;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("LoginUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input Parameters
                        cmd.Parameters.Add("pEmail", OracleDbType.Varchar2).Value = email;
                        cmd.Parameters.Add("pPassword", OracleDbType.Varchar2).Value = password;

                        // Output Parameters
                        OracleParameter userIDParam = new OracleParameter("pUserID", OracleDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(userIDParam);

                        OracleParameter userRoleParam = new OracleParameter("pUserRole", OracleDbType.Varchar2, 50)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(userRoleParam);

                        cmd.ExecuteNonQuery();

                        // Get output values
                        object userID = userIDParam.Value;
                        object userRole = userRoleParam.Value;

                        if (userID != DBNull.Value && userRole != DBNull.Value)
                        {
                            Session["UserID"] = userID;
                            Session["UserRole"] = userRole.ToString();

                            if (userRole.ToString() == "Customer")
                                Response.Redirect("~/Customer/Home.aspx");
                            else if (userRole.ToString() == "Supplier")
                                Response.Redirect("~/Supplier/Dashboard.aspx");
                        }
                        else
                        {
                            pnlErrorMessage.Visible = true;
                            litErrorMessage.Text = "Invalid email or password.";
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Check for custom error from PL/SQL
                if (ex.Number == 20003)
                {
                    pnlErrorMessage.Visible = true;
                    litErrorMessage.Text = "Invalid email or password."; // Friendly error message
                }
                else
                {
                    // Log ex.Message if needed
                    pnlErrorMessage.Visible = true;
                    litErrorMessage.Text = "An unexpected database error occurred. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                // General exception handling
                pnlErrorMessage.Visible = true;
                litErrorMessage.Text = "An unexpected error occurred. Please try again later.";
            }
        }


    }
}