using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UrbanFoodWeb.Helpers;
using System.Threading.Tasks;
using UrbanFoodWeb.Models;
using UrbanFoodWeb.Repositories;
using UrbanFoodWeb.Supplier;

namespace UrbanFoodWeb.Customer
{
    public partial class ShopDetails : System.Web.UI.Page
    {
        private ReviewRepository _reviewRepository;
        private int _productId;

        protected void Page_Load(object sender, EventArgs e)
        {
            _reviewRepository = new ReviewRepository();

            if (!IsPostBack)
            {
                // 🔒 Ensure customer is logged in
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Customer/Login.aspx?returnUrl=ShopDetails.aspx?productId=" + Request.QueryString["productId"]);
                    return;
                }

                // ✅ Proceed to load product details
                if (Request.QueryString["productId"] != null)
                {
                    if (int.TryParse(Request.QueryString["productId"], out _productId))
                    {
                        hdnProductId.Value = _productId.ToString();
                        LoadProductDetails(_productId);
                        LoadReviews(_productId);
                    }
                    else
                    {
                        lblProductName.Text = "Invalid Product ID.";
                    }
                }
            }
            else
            {
                // For postbacks, ensure we have the product ID
                if (!string.IsNullOrEmpty(hdnProductId.Value))
                {
                    _productId = int.Parse(hdnProductId.Value);
                }
            }
        }

        private void LoadProductDetails(int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["OracleConString"].ConnectionString;
            using (OracleConnection con = new OracleConnection(connStr))
            {
                OracleCommand cmd = new OracleCommand("fn_GetProductByID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                // Define the return value (a ref cursor)
                OracleParameter retVal = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor);
                retVal.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(retVal);
                cmd.Parameters.Add("p_ProductID", OracleDbType.Int32).Value = productId;
                con.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Populate product details
                    lblProductName.Text = reader["ProductName"].ToString();
                    lblCategory.Text = reader["CategoryName"].ToString();

                    // Fix price display issue
                    decimal price = 0;
                    if (reader["Price"] != DBNull.Value)
                    {
                        // Try multiple approaches to get the price value
                        if (reader["Price"] is Oracle.ManagedDataAccess.Types.OracleDecimal oracleDecimal)
                        {
                            price = oracleDecimal.Value;
                        }
                        else if (Decimal.TryParse(reader["Price"].ToString(), out decimal parsedPrice))
                        {
                            price = parsedPrice;
                        }
                    }

                    lblPrice.Text = "$" + price.ToString("F2");
                    imgProduct.ImageUrl = reader["ImageURL"].ToString();
                    lblDescription.Text = reader["Description"].ToString();
                }
                reader.Close();
            }
        }

        private async void LoadReviews(int productId)
        {
            RegisterAsyncTask(new PageAsyncTask(async cancellationToken => {
                try
                {
                    // Load reviews from MongoDB
                    var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
                    rptReviews.DataSource = reviews;
                    rptReviews.DataBind();

                    // Get and display average rating
                    double avgRating = await _reviewRepository.GetAverageRatingAsync(productId);
                    lblAverageRating.Text = avgRating.ToString("F1");
                    lblReviewCount.Text = reviews.Count.ToString();

                    // Update average rating stars
                    ScriptManager.RegisterStartupScript(this, GetType(), "UpdateAverageRatingStars",
                        "$(document).ready(function() { updateAverageRatingStars(" + avgRating.ToString("F1") + "); });", true);
                }
                catch (Exception ex)
                {
                    // Log error or show message
                    ScriptManager.RegisterStartupScript(this, GetType(), "reviewLoadError",
                        $"console.error('Error loading reviews: {ex.Message}');", true);
                }
            }));
        }

        protected string GetStarRating(int rating)
        {
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                if (i <= rating)
                    stars += "<i class='fa fa-star checked'></i>";
                else
                    stars += "<i class='fa fa-star'></i>";
            }
            return stars;
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtReview.Text) ||
                hdnRating.Value == "0")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reviewError",
                    "alert('Please fill all required fields and select a rating.');", true);
                return;
            }

            // Create review object
            var review = new ProductReview
            {
                ProductId = _productId,
                CustomerName = txtName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                ReviewText = txtReview.Text.Trim(),
                Rating = Convert.ToInt32(hdnRating.Value),
                CreatedDate = DateTime.Now
            };

            // Handle different possible types of CustomerID in Session
            if (Session["UserID"] != null)
            {
                try
                {
                    // First try direct cast to int
                    if (Session["UserID"] is int intCustomerId)
                    {
                        review.CustomerId = intCustomerId;
                    }
                    // Then try converting from decimal
                    else if (Session["UserID"] is decimal decimalCustomerId)
                    {
                        review.CustomerId = Convert.ToInt32(decimalCustomerId);
                    }
                    // Finally, try generic conversion
                    else
                    {
                        review.CustomerId = Convert.ToInt32(Session["UserID"].ToString());
                    }
                }
                catch (Exception)
                {
                    // If all conversions fail, use a default value or throw error
                    review.CustomerId = -1; // Default/error value
                }
            }

            // Save the review to MongoDB
            SaveReviewAsync(review);
        }

        private async void SaveReviewAsync(ProductReview review)
        {
            RegisterAsyncTask(new PageAsyncTask(async cancellationToken => {
                try
                {
                    await _reviewRepository.AddReviewAsync(review);

                    // Clear form fields
                    txtName.Text = "";
                    txtEmail.Text = "";
                    txtReview.Text = "";
                    hdnRating.Value = "0";

                    // Reset star ratings in UI
                    ScriptManager.RegisterStartupScript(this, GetType(), "resetStars",
                        "$(document).ready(function() { $('.star-rating .fa').removeClass('checked'); });", true);

                    // Refresh reviews
                    LoadReviews(_productId);

                    // Show success message
                    ScriptManager.RegisterStartupScript(this, GetType(), "reviewSuccess",
                        "alert('Your review has been submitted successfully.');", true);
                }
                catch (Exception ex)
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, GetType(), "reviewError",
                        $"alert('Error submitting review: {ex.Message}');", true);
                }
            }));
        }

       
    }
}