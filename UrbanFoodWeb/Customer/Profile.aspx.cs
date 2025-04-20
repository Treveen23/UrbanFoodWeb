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
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Customer/Login.aspx?returnUrl=Cart.aspx");
                return;
            }
            LoadCustomerData();

        }

        private void LoadCustomerData()
        {
            int customerId = Convert.ToInt32(Session["UserID"].ToString());

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            
            using (OracleCommand cmd = new OracleCommand("GET_CUSTOMER_PROFILE", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_customer_id", OracleDbType.Int32).Value = customerId;
                cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                
                conn.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtFirstName.Text = reader["FirstName"].ToString();
                        txtLastName.Text = reader["LastName"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                        txtPassword.Attributes["value"] = reader["Password"].ToString();
                        txtConfirmPassword.Attributes["value"] = reader["Password"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        ddlDietary.SelectedValue = reader["DietaryPreference"].ToString();
                    }
                }
            }
        }




        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblMessage.Text = "Passwords do not match.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            int customerId = Convert.ToInt32(Session["UserID"].ToString());

            string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(connectionString))
            using (OracleCommand cmd = new OracleCommand("UPDATE_CUSTOMER_PROFILE", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_customer_id", OracleDbType.Int32).Value = customerId;
                cmd.Parameters.Add("p_firstname", OracleDbType.Varchar2).Value = txtFirstName.Text;
                cmd.Parameters.Add("p_lastname", OracleDbType.Varchar2).Value = txtLastName.Text;
                cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = txtPhoneNumber.Text;
                cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = txtPassword.Text;
                cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = txtAddress.Text;
                cmd.Parameters.Add("p_diet", OracleDbType.Varchar2).Value = ddlDietary.SelectedValue;

                conn.Open();
                cmd.ExecuteNonQuery();

                lblMessage.Text = "Profile updated successfully!";
                lblMessage.CssClass = "text-success";
            }
        }
    }
}