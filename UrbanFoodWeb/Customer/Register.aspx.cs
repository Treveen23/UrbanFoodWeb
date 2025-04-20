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
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)  // Prevents form reset on postback
            {
                pnlCustomerFields.Visible = true;
                pnlSupplierFields.Visible = false;
                btnCustomer.CssClass = "btn btn-toggle toggle-active me-2";
                btnSupplier.CssClass = "btn btn-toggle toggle-inactive";
            }
        }
        protected void btnCustomer_Click(object sender, EventArgs e)
        {
            pnlCustomerFields.Visible = true;
            pnlSupplierFields.Visible = false;

            btnCustomer.CssClass = "btn btn-toggle toggle-active me-2";
            btnSupplier.CssClass = "btn btn-toggle toggle-inactive";

            hfUserType.Value = "Customer";  // Set hidden field
        }

        protected void btnSupplier_Click(object sender, EventArgs e)
        {
            pnlCustomerFields.Visible = false;
            pnlSupplierFields.Visible = true;

            btnCustomer.CssClass = "btn btn-toggle toggle-inactive me-2";
            btnSupplier.CssClass = "btn btn-toggle toggle-active";

            hfUserType.Value = "Supplier";  // Set hidden field
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!chkTerms.Checked)
            {
                cvTerms.IsValid = false;
                return;
            }

            string userType = hfUserType.Value;  // Retrieve hidden field value

            if (string.IsNullOrEmpty(userType))
            {

            }

            if (userType == "Customer")
            {
                SaveCustomerToDatabase();
            }
            else if (userType == "Supplier")
            {
                SaveSupplierToDatabase();
            }
        }

        private void SaveCustomerToDatabase()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                int customerId = 0;
                try
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("SignupCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters for customer registration
                        cmd.Parameters.Add("pFirstName", OracleDbType.Varchar2).Value = txtFirstName.Text;
                        cmd.Parameters.Add("pLastName", OracleDbType.Varchar2).Value = txtLastName.Text;
                        cmd.Parameters.Add("pEmail", OracleDbType.Varchar2).Value = txtEmail.Text;
                        cmd.Parameters.Add("pPhoneNumber", OracleDbType.Varchar2).Value = txtPhone.Text;
                        cmd.Parameters.Add("pPassword", OracleDbType.Varchar2).Value = txtPassword.Text;
                        cmd.Parameters.Add("pAddress", OracleDbType.Varchar2).Value = txtAddress.Text;
                        cmd.Parameters.Add("pDietaryPreference", OracleDbType.Varchar2).Value = ddlPreferences.SelectedValue;
                        OracleParameter outputParam = new OracleParameter("pCustomerID", OracleDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        cmd.ExecuteNonQuery();

                        customerId = Convert.ToInt32(outputParam.Value.ToString());

                        ClearForm();
                    }

                    lblCustomerStatus.Text = "Customer registered successfully.";
                    lblCustomerStatus.ForeColor = System.Drawing.Color.Green;


                }

                catch (OracleException ex)
                {
                    // Handle the OracleException and get the specific error message
                    lblCustomerStatus.Text = "Oracle Error: " + ex.Message;
                    lblCustomerStatus.ForeColor = System.Drawing.Color.Red;
                    // Logs full error details
                }

            }

        }


        private void SaveSupplierToDatabase()
        {
            int supplierId = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("SignupSupplier", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters matching the procedure
                        cmd.Parameters.Add("pFirstName", OracleDbType.Varchar2).Value = txtFirstName.Text;
                        cmd.Parameters.Add("pLastName", OracleDbType.Varchar2).Value = txtLastName.Text;
                        cmd.Parameters.Add("pEmail", OracleDbType.Varchar2).Value = txtEmail.Text.ToLower();
                        cmd.Parameters.Add("pPhoneNumber", OracleDbType.Varchar2).Value = txtPhone.Text;
                        cmd.Parameters.Add("pPassword", OracleDbType.Varchar2).Value = txtPassword.Text;
                        cmd.Parameters.Add("pAddress", OracleDbType.Varchar2).Value = txtAddress.Text;
                        cmd.Parameters.Add("pBusinessName", OracleDbType.Varchar2).Value = txtBusinessName.Text;
                        cmd.Parameters.Add("pBusinessType", OracleDbType.Varchar2).Value = ddlBusinessType.SelectedValue;

                        OracleParameter outputParam = new OracleParameter("pSupplierID", OracleDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        cmd.ExecuteNonQuery();

                        supplierId = Convert.ToInt32(outputParam.Value.ToString());

                        ClearForm();
                    }

                    lblSupplierStatus.Text = "Supplier registered successfully.";
                    lblSupplierStatus.ForeColor = System.Drawing.Color.Green;

                }
                catch (OracleException ex)
                {
                    lblSupplierStatus.Text = "Oracle Error: " + ex.Message;
                    lblSupplierStatus.ForeColor = System.Drawing.Color.Red;

                }
                finally
                {
                    con.Close();
                }

            }
        }

        private void ClearForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtPassword.Text = "";
            txtAddress.Text = "";
            ddlPreferences.SelectedIndex = 0;  // Reset dropdown to first option
            txtBusinessName.Text = "";
            ddlBusinessType.SelectedIndex = 0;  // Reset business type dropdown
            chkTerms.Checked = false;  // Uncheck terms & conditions
            lblCustomerStatus.Text = "";
            lblSupplierStatus.Text = "";

        }


    }
}