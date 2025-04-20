using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Security.Cryptography;
using System.Text;

namespace UrbanFoodWeb.Supplier
{
    public partial class SupplierProfile : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
        int supplierId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is logged in
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Customer/Login.aspx");
                return;
            }

            supplierId = Convert.ToInt32(Session["UserID"].ToString());

            if (!IsPostBack)
            {
                LoadSupplierProfile();
            }
        }

        private void LoadSupplierProfile()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GET_SUPPLIER_PROFILE", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierId;

                    // Output parameters
                    cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_phone_number", OracleDbType.Varchar2, 20).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_address", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_business_name", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_business_type", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    int result = Convert.ToInt32(cmd.Parameters["p_result"].Value.ToString());

                    if (result == 1)
                    {
                        txtFirstName.Text = cmd.Parameters["p_first_name"].Value.ToString();
                        txtLastName.Text = cmd.Parameters["p_last_name"].Value.ToString();
                        txtEmail.Text = cmd.Parameters["p_email"].Value.ToString();
                        txtPhoneNumber.Text = cmd.Parameters["p_phone_number"].Value.ToString();
                        txtAddress.Text = cmd.Parameters["p_address"].Value.ToString();
                        txtBusinessName.Text = cmd.Parameters["p_business_name"].Value.ToString();
                        ddlBusinessType.SelectedValue = cmd.Parameters["p_business_type"].Value.ToString();
                    }
                    else
                    {
                        ShowError("Error retrieving supplier profile.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading profile: " + ex.Message);
            }
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // Determine if we're updating the password too
                    bool updatePassword = !string.IsNullOrEmpty(txtCurrentPassword.Text) &&
                                         !string.IsNullOrEmpty(txtNewPassword.Text) &&
                                         !string.IsNullOrEmpty(txtConfirmPassword.Text);

                    if (updatePassword)
                    {
                        // First verify the current password
                        OracleCommand verifyCmd = new OracleCommand("VERIFY_SUPPLIER_PASSWORD", connection);
                        verifyCmd.CommandType = CommandType.StoredProcedure;

                        verifyCmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierId;
                        verifyCmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = txtCurrentPassword.Text;
                        verifyCmd.Parameters.Add("p_is_valid", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        verifyCmd.ExecuteNonQuery();

                        int isValid = Convert.ToInt32(verifyCmd.Parameters["p_is_valid"].Value.ToString());

                        if (isValid != 1)
                        {
                            ShowError("Current password is incorrect.");
                            return;
                        }
                    }

                    // Now update the profile
                    OracleCommand updateCmd = new OracleCommand("UPDATE_SUPPLIER_PROFILE", connection);
                    updateCmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    updateCmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierId;
                    updateCmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = txtFirstName.Text;
                    updateCmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = txtLastName.Text;
                    updateCmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = txtEmail.Text;
                    updateCmd.Parameters.Add("p_phone_number", OracleDbType.Varchar2).Value = txtPhoneNumber.Text;
                    updateCmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = txtAddress.Text;
                    updateCmd.Parameters.Add("p_business_name", OracleDbType.Varchar2).Value = txtBusinessName.Text;
                    updateCmd.Parameters.Add("p_business_type", OracleDbType.Varchar2).Value = ddlBusinessType.SelectedValue;

                    if (updatePassword)
                    {
                        updateCmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = txtNewPassword.Text;
                        updateCmd.Parameters.Add("p_update_password", OracleDbType.Int32).Value = 1;
                    }
                    else
                    {
                        updateCmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = DBNull.Value;
                        updateCmd.Parameters.Add("p_update_password", OracleDbType.Int32).Value = 0;
                    }

                    // Output parameter
                    updateCmd.Parameters.Add("p_result", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    updateCmd.ExecuteNonQuery();

                    int result = Convert.ToInt32(updateCmd.Parameters["p_result"].Value.ToString());

                    if (result == 1)
                    {
                        successAlert.Visible = true;
                        errorAlert.Visible = false;

                        // Clear password fields
                        txtCurrentPassword.Text = string.Empty;
                        txtNewPassword.Text = string.Empty;
                        txtConfirmPassword.Text = string.Empty;

                        // Update session value for supplier name if it changed
                        if (Session["SupplierName"] != null)
                        {
                            Session["SupplierName"] = txtFirstName.Text + " " + txtLastName.Text;
                        }
                    }
                    else
                    {
                        ShowError("Failed to update profile. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error updating profile: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        private void ShowError(string message)
        {
            litErrorMessage.Text = message;
            errorAlert.Visible = true;
            successAlert.Visible = false;
        }

       
    }
}