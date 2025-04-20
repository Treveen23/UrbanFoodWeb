using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client; // For modern Oracle access

namespace UrbanFoodWeb.Supplier
{
    public partial class Category : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategories();
                ResetForm();
            }
        }

        private void BindCategories()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("BEGIN GET_ALL_CATEGORIES(:cursor); END;", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // Create parameter for cursor
                    OracleParameter param = cmd.Parameters.Add("cursor", OracleDbType.RefCursor);
                    param.Direction = ParameterDirection.Output;

                    conn.Open();
                    OracleDataReader reader = cmd.ExecuteReader();

                    // Create a DataTable to hold the results
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    gvCategories.DataSource = dt;
                    gvCategories.DataBind();

                    reader.Close();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string imagePath = UploadImage();
                    if (string.IsNullOrEmpty(imagePath) && fileImage.HasFile)
                    {
                        ShowMessage("Failed to upload image. Please try again.", true);
                        return;
                    }

                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        using (OracleCommand cmd = new OracleCommand("BEGIN INSERT_CATEGORY(:CategoryName, :Description, :ImagePath, :IsActive, :Result); END;", conn))
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.Add(":CategoryName", OracleDbType.Varchar2).Value = txtCategoryName.Text.Trim();
                            cmd.Parameters.Add(":Description", OracleDbType.Varchar2).Value = txtDescription.Text.Trim();
                            cmd.Parameters.Add(":ImagePath", OracleDbType.Varchar2).Value = imagePath;
                            cmd.Parameters.Add(":IsActive", OracleDbType.Int32).Value = chkIsActive.Checked ? 1 : 0;

                            OracleParameter resultParam = cmd.Parameters.Add(":Result", OracleDbType.Int32);
                            resultParam.Direction = ParameterDirection.Output;

                            conn.Open();
                            cmd.ExecuteNonQuery();

                            int result = Convert.ToInt32(resultParam.Value.ToString());
                            if (result > 0)
                            {
                                ShowMessage("Category added successfully!", false);
                                ResetForm();
                                BindCategories();
                            }
                            else
                            {
                                ShowMessage("Failed to add category. Please try again.", true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, true);
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int categoryId = Convert.ToInt32(hdnCategoryId.Value);
                    string imagePath = imgPreview.Visible ? imgCategory.ImageUrl : string.Empty;

                    // Upload new image if provided
                    if (fileImage.HasFile)
                    {
                        string newImagePath = UploadImage();
                        if (!string.IsNullOrEmpty(newImagePath))
                        {
                            imagePath = newImagePath;
                        }
                        else
                        {
                            ShowMessage("Failed to upload new image. Please try again.", true);
                            return;
                        }
                    }

                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        using (OracleCommand cmd = new OracleCommand("BEGIN UPDATE_CATEGORY(:CategoryId, :CategoryName, :Description, :ImagePath, :IsActive, :Result); END;", conn))
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.Add(":CategoryId", OracleDbType.Int32).Value = categoryId;
                            cmd.Parameters.Add(":CategoryName", OracleDbType.Varchar2).Value = txtCategoryName.Text.Trim();
                            cmd.Parameters.Add(":Description", OracleDbType.Varchar2).Value = txtDescription.Text.Trim();
                            cmd.Parameters.Add(":ImagePath", OracleDbType.Varchar2).Value = imagePath;
                            cmd.Parameters.Add(":IsActive", OracleDbType.Int32).Value = chkIsActive.Checked ? 1 : 0;

                            OracleParameter resultParam = cmd.Parameters.Add(":Result", OracleDbType.Int32);
                            resultParam.Direction = ParameterDirection.Output;

                            conn.Open();
                            cmd.ExecuteNonQuery();

                            int result = Convert.ToInt32(resultParam.Value.ToString());
                            if (result > 0)
                            {
                                ShowMessage("Category updated successfully!", false);
                                ResetForm();
                                BindCategories();
                            }
                            else
                            {
                                ShowMessage("Failed to update category. Please try again.", true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, true);
                }
            }
        }

        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditCategory")
            {
                int categoryId = Convert.ToInt32(e.CommandArgument);
                LoadCategoryForEdit(categoryId);
            }
        }

        protected void gvCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int categoryId = Convert.ToInt32(gvCategories.DataKeys[e.RowIndex].Value);

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("BEGIN DELETE_CATEGORY(:CategoryId, :Result); END;", conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add(":CategoryId", OracleDbType.Int32).Value = categoryId;

                        OracleParameter resultParam = cmd.Parameters.Add(":Result", OracleDbType.Int32);
                        resultParam.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        int result = Convert.ToInt32(resultParam.Value.ToString());
                        if (result > 0)
                        {
                            ShowMessage("Category deleted successfully!", false);
                            BindCategories();
                        }
                        else
                        {
                            ShowMessage("Failed to delete category. It may be in use.", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, true);
            }
        }

        private void LoadCategoryForEdit(int categoryId)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("BEGIN GET_CATEGORY_BY_ID(:CategoryId, :cursor); END;", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(":CategoryId", OracleDbType.Int32).Value = categoryId;

                    OracleParameter cursorParam = cmd.Parameters.Add(":cursor", OracleDbType.RefCursor);
                    cursorParam.Direction = ParameterDirection.Output;

                    conn.Open();
                    OracleDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        hdnCategoryId.Value = reader["CategoryId"].ToString();
                        txtCategoryName.Text = reader["CategoryName"].ToString();
                        txtDescription.Text = reader["Description"].ToString();
                        chkIsActive.Checked = Convert.ToBoolean(Convert.ToInt32(reader["IsActive"]));

                        string imagePath = reader["ImagePath"].ToString();
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            imgCategory.ImageUrl = imagePath;
                            imgPreview.Visible = true;
                        }

                        // Toggle buttons
                        btnSave.Visible = false;
                        btnUpdate.Visible = true;
                        rfvImage.Enabled = false; // Don't require image during edit
                    }

                    reader.Close();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            hdnCategoryId.Value = "0";
            txtCategoryName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            chkIsActive.Checked = true;
            imgCategory.ImageUrl = string.Empty;
            imgPreview.Visible = false;
            btnSave.Visible = true;
            btnUpdate.Visible = false;
            rfvImage.Enabled = true;
            lblMessage.Visible = false;
        }

        private string UploadImage()
        {
            if (!fileImage.HasFile)
                return string.Empty;

            try
            {
                // Verify file is an image and size is acceptable
                if (!fileImage.PostedFile.ContentType.StartsWith("image/"))
                {
                    ShowMessage("Please upload only image files.", true);
                    return string.Empty;
                }

                // Check file size (max 2MB)
                if (fileImage.PostedFile.ContentLength > 2 * 1024 * 1024)
                {
                    ShowMessage("Image size should not exceed 2MB.", true);
                    return string.Empty;
                }

                // Create upload directory if it doesn't exist
                string uploadDir = Server.MapPath("~/Images/Category/");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Generate unique filename
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileImage.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                // Save file
                fileImage.SaveAs(filePath);

                // Return relative path for database storage
                return "~/Images/Category/" + fileName;
            }
            catch (Exception ex)
            {
                ShowMessage("Error uploading image: " + ex.Message, true);
                return string.Empty;
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
            lblMessage.Visible = true;
        }
    }
}