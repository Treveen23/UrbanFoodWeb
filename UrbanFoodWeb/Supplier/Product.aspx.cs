using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace UrbanFoodWeb.Supplier
{
    public partial class Product : System.Web.UI.Page
    {
        // Get connection string from web.config
        string connectionString = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if supplier is logged in
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Customer/Login.aspx");
                    return;
                }

                // Load categories
                LoadCategories();

                // Load product dropdown for update
                LoadProductDropdown();

                // Load products grid
                LoadProducts();

                // Check if in edit mode
                if (Request.QueryString["id"] != null)
                {
                    int productId;
                    if (int.TryParse(Request.QueryString["id"], out productId))
                    {
                        LoadProductForEdit(productId);
                    }
                }
            }
        }

        private void LoadProductDropdown()
        {
            int supplierID = Convert.ToInt32(Session["UserID"].ToString());

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "GET_SUPPLIER_PRODUCTS";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierID;

                    // Output cursor parameter
                    OracleParameter cursorParam = new OracleParameter();
                    cursorParam.ParameterName = "p_cursor";
                    cursorParam.OracleDbType = OracleDbType.RefCursor;
                    cursorParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(cursorParam);

                    try
                    {
                        con.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            ddlProductsToUpdate.Items.Clear();
                            ddlProductsToUpdate.Items.Add(new ListItem("-- Select Product to Update --", "0"));

                            while (reader.Read())
                            {
                                ddlProductsToUpdate.Items.Add(new ListItem(
                                    reader["ProductName"].ToString(),
                                    reader["ProductID"].ToString()
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error loading product dropdown: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        private void LoadCategories()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "GET_CATEGORIES_TO_PRODUCT";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Output cursor parameter
                    OracleParameter cursorParam = new OracleParameter();
                    cursorParam.ParameterName = "p_cursor";
                    cursorParam.OracleDbType = OracleDbType.RefCursor;
                    cursorParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(cursorParam);

                    try
                    {
                        con.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            ddlCategory.Items.Clear();
                            ddlCategory.Items.Add(new ListItem("-- Select Category --", "0"));

                            while (reader.Read())
                            {
                                ddlCategory.Items.Add(new ListItem(
                                    reader["CategoryName"].ToString(),
                                    reader["CategoryID"].ToString()
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error loading categories: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        private void LoadProducts()
        {
            int supplierID = Convert.ToInt32(Session["UserID"].ToString());

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "GET_SUPPLIER_PRODUCTS";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierID;

                    // Output cursor parameter
                    OracleParameter cursorParam = new OracleParameter();
                    cursorParam.ParameterName = "p_cursor";
                    cursorParam.OracleDbType = OracleDbType.RefCursor;
                    cursorParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(cursorParam);

                    try
                    {
                        con.Open();
                        DataTable dt = new DataTable();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }

                        gvProducts.DataSource = dt;
                        gvProducts.DataBind();
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error loading products: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        protected void ddlProductsToUpdate_SelectedIndexChanged(object sender, EventArgs e)
        {
            int productId;
            if (int.TryParse(ddlProductsToUpdate.SelectedValue, out productId) && productId > 0)
            {
                LoadProductForEdit(productId);

                // Switch to update mode
                btnSave.Visible = false;
                btnUpdate.Visible = true;

                // Scroll to form to make it visible
                ScriptManager.RegisterStartupScript(this, GetType(), "scrollToForm",
                    "window.scrollTo(0, document.getElementById('productForm').offsetTop);", true);
            }
            else
            {
                ResetForm();
                btnSave.Visible = true;
                btnUpdate.Visible = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int supplierID = Convert.ToInt32(Session["UserID"].ToString()); // Changed from SupplierID to UserID for consistency
            int categoryID = Convert.ToInt32(ddlCategory.SelectedValue);
            string productName = txtProductName.Text.Trim();
            string description = txtDescription.Text.Trim();
            decimal price = Convert.ToDecimal(txtPrice.Text.Trim());
            int quantity = Convert.ToInt32(txtQuantity.Text.Trim());
            int isActive = chkIsActive.Checked ? 1 : 0;
            int isFeatured = chkIsFeatured.Checked ? 1 : 0;
            string imageUrl = string.Empty;

            // Check if we're in edit mode
            bool isEditMode = !string.IsNullOrEmpty(hdnProductId.Value) && hdnProductId.Value != "0";
            int productID = isEditMode ? Convert.ToInt32(hdnProductId.Value) : 0;

            // Handle file upload
            bool hasNewImage = fileImage.HasFile;

            if (hasNewImage)
            {
                // Check file size (max 2MB)
                if (fileImage.PostedFile.ContentLength > 2097152)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                        "alert('File size exceeds the 2MB limit.');", true);
                    return;
                }

                // Check file type
                string fileExtension = Path.GetExtension(fileImage.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                        "alert('Only .jpg, .jpeg, .png and .gif files are allowed.');", true);
                    return;
                }

                // Generate a unique filename
                string fileName = Guid.NewGuid().ToString() + fileExtension;
                string uploadPath = Server.MapPath("~/Images/Product/");

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Save the file
                fileImage.SaveAs(Path.Combine(uploadPath, fileName));


                // Set imageUrl for database
                imageUrl = "~/Images/Product/" + fileName;
            }
            else if (!isEditMode)
            {
                // Only require image for new products
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Please select an image file.');", true);
                return;
            }
            else
            {
                // Use existing image for updates
                imageUrl = hfCurrentImageUrl.Value;
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;

                    try
                    {
                        con.Open();

                        if (isEditMode)
                        {
                            // Update existing product
                            if (hasNewImage)
                            {
                                cmd.CommandText = "UPDATE_PRODUCT_WITH_IMAGE";
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = productID;
                                cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = categoryID;
                                cmd.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = productName;
                                cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = description;
                                cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = price;
                                cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = quantity;
                                cmd.Parameters.Add("p_is_active", OracleDbType.Int32).Value = isActive;
                                cmd.Parameters.Add("p_is_featured", OracleDbType.Int32).Value = isFeatured;
                                cmd.Parameters.Add("p_image_url", OracleDbType.Varchar2).Value = imageUrl;
                            }
                            else
                            {
                                cmd.CommandText = "UPDATE_PRODUCT_NO_IMAGE";
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = productID;
                                cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = categoryID;
                                cmd.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = productName;
                                cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = description;
                                cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = price;
                                cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = quantity;
                                cmd.Parameters.Add("p_is_active", OracleDbType.Int32).Value = isActive;
                                cmd.Parameters.Add("p_is_featured", OracleDbType.Int32).Value = isFeatured;
                            }

                            cmd.ExecuteNonQuery();
                            ShowSuccessMessage("Product updated successfully!");
                        }
                        else
                        {
                            // Insert new product
                            cmd.CommandText = "INSERT_PRODUCT";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("p_supplier_id", OracleDbType.Int32).Value = supplierID;
                            cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = categoryID;
                            cmd.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = productName;
                            cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = description;
                            cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = price;
                            cmd.Parameters.Add("p_quantity", OracleDbType.Int32).Value = quantity;
                            cmd.Parameters.Add("p_is_active", OracleDbType.Int32).Value = isActive;
                            cmd.Parameters.Add("p_is_featured", OracleDbType.Int32).Value = isFeatured;
                            cmd.Parameters.Add("p_image_url", OracleDbType.Varchar2).Value = imageUrl;

                            // Output parameter for returning the new product ID
                            OracleParameter productIdParam = new OracleParameter();
                            productIdParam.ParameterName = "p_product_id";
                            productIdParam.OracleDbType = OracleDbType.Int32;
                            productIdParam.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(productIdParam);

                            cmd.ExecuteNonQuery();
                            ShowSuccessMessage("Product added successfully!");
                        }

                        // Reset form
                        ResetForm();

                        // Reload products grid and dropdown
                        LoadProducts();
                        LoadProductDropdown();
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error saving product: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        private void ResetForm()
        {
            txtProductName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            ddlCategory.SelectedValue = "0";
            chkIsActive.Checked = true;
            chkIsFeatured.Checked = false;
            hdnProductId.Value = "0";
            btnSave.Visible = true;
            btnUpdate.Visible = false;
            pnlCurrentImage.Visible = false;
            hfCurrentImageUrl.Value = string.Empty;
            rfvImage.Enabled = true;  // Re-enable image validation for new products
            ddlProductsToUpdate.SelectedValue = "0"; // Reset product dropdown
        }

        private void LoadProductForEdit(int productId)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "GET_PRODUCT_DETAILS";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = productId;

                    // Output cursor parameter
                    OracleParameter cursorParam = new OracleParameter();
                    cursorParam.ParameterName = "p_cursor";
                    cursorParam.OracleDbType = OracleDbType.RefCursor;
                    cursorParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(cursorParam);

                    try
                    {
                        con.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string categoryId = reader["CategoryID"].ToString();
                                if (ddlCategory.Items.FindByValue(categoryId) != null)
                                {
                                    ddlCategory.SelectedValue = categoryId;
                                }

                                txtProductName.Text = reader["ProductName"].ToString();
                                txtDescription.Text = reader["Description"].ToString();
                                txtPrice.Text = reader["Price"].ToString();
                                txtQuantity.Text = reader["QuantityAvailable"].ToString();

                                // Convert integer values to boolean for checkboxes
                                chkIsActive.Checked = reader["IsActive"] != DBNull.Value && Convert.ToInt32(reader["IsActive"]) == 1;
                                chkIsFeatured.Checked = reader["IsFeatured"] != DBNull.Value && Convert.ToInt32(reader["IsFeatured"]) == 1;

                                string imageUrl = reader["ImageURL"] != DBNull.Value ? reader["ImageURL"].ToString() : string.Empty;

                                // Store image URL in hidden field
                                hfCurrentImageUrl.Value = imageUrl;

                                // Show current image
                                imgCurrent.ImageUrl = imageUrl;
                                pnlCurrentImage.Visible = !string.IsNullOrEmpty(imageUrl);

                                // Set product ID and update button visibility
                                hdnProductId.Value = productId.ToString();
                                btnSave.Visible = false;
                                btnUpdate.Visible = true;

                                // Disable image validation for edit mode
                                rfvImage.Enabled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error loading product details: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null)
                return;

            int productId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditProduct")
            {
                // Set the dropdown value to match the product being edited
                ListItem item = ddlProductsToUpdate.Items.FindByValue(productId.ToString());
                if (item != null)
                {
                    ddlProductsToUpdate.SelectedValue = productId.ToString();
                }

                LoadProductForEdit(productId);

                // Scroll to top of the form
                ScriptManager.RegisterStartupScript(this, GetType(), "scrollToTop",
                    "window.scrollTo(0, 0);", true);
            }
            else if (e.CommandName == "DeleteProduct")
            {
                DeleteProduct(productId);
            }
        }

        private void DeleteProduct(int productId)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "DELETE_PRODUCT";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_product_id", OracleDbType.Int32).Value = productId;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        // Reload products grid and dropdown
                        LoadProducts();
                        LoadProductDropdown();

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Product deleted successfully.');", true);
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error deleting product: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            btnSave_Click(sender, e); // Reuse the save logic since it already handles updates
        }

        private void ShowSuccessMessage(string message)
        {
            string script = @"
            var messageDiv = document.createElement('div');
            messageDiv.style.cssText = 'position:fixed;top:20px;right:20px;background-color:#4CAF50;color:white;padding:15px;border-radius:5px;box-shadow:0 4px 8px rgba(0,0,0,0.2);z-index:1000;opacity:0;transition:opacity 0.5s;';
            messageDiv.innerHTML = '" + message.Replace("'", "\\'") + @"';
            document.body.appendChild(messageDiv);
            setTimeout(function() { messageDiv.style.opacity = '1'; }, 100);
            setTimeout(function() { 
                messageDiv.style.opacity = '0'; 
                setTimeout(function() { document.body.removeChild(messageDiv); }, 500);
            }, 3000);";

            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessMessage", script, true);
        }
    }
}