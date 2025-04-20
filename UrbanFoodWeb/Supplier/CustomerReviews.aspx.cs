using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using MongoDB.Bson;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace UrbanFoodWeb.Supplier
{
    public partial class CustomerReviews : System.Web.UI.Page
    {
        // Connection strings - you should store these in web.config in production
        private readonly string mongoConnectionString = "mongodb://localhost:27017";
        private readonly string oracleConnectionString = "User Id=system;Password=SYS321;Data Source=localhost:1521/xe";

        // MongoDB collection and database names
        private readonly string databaseName = "UrbanFoodDB";
        private readonly string reviewsCollectionName = "ProductReviews";

        private int supplierId;
        private int pageSize = 10; // Number of reviews per page

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserRole"] == null || Session["UserRole"].ToString() != "Supplier")
            {

                return;
            }


            // Save the SupplierID from session
            supplierId = Convert.ToInt32(Session["UserID"].ToString());

            if (!IsPostBack)
            {
                ViewState["CurrentPage"] = 1;

                LoadProductsDropdown();  // Load products in a dropdown list
                LoadReviews();           // Load existing reviews for selected product or all
                UpdateReviewStats();     // Calculate average rating or review count
            }
        }

        private void LoadProductsDropdown()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(oracleConnectionString))
                {
                    conn.Open();

                    // Using fn_GetAllProducts function instead of direct SQL
                    using (OracleCommand cmd = new OracleCommand("BEGIN :result := fn_GetAllProducts(:supplierId); END;", conn))
                    {
                        cmd.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("supplierId", OracleDbType.Int32).Value = supplierId;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            ddlProductFilter.Items.Clear();
                            ddlProductFilter.Items.Add(new ListItem("All Products", "0"));

                            while (reader.Read())
                            {
                                string productId = reader["ProductID"].ToString();
                                string productName = reader["ProductName"].ToString();
                                ddlProductFilter.Items.Add(new ListItem(productName, productId));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error loading products: " + ex.Message);
            }
        }

        protected async void LoadReviews()
        {
            try
            {
                // Get filter values
                int selectedProductId = Convert.ToInt32(ddlProductFilter.SelectedValue);
                int selectedRating = Convert.ToInt32(ddlRatingFilter.SelectedValue);
                int currentPage = Convert.ToInt32(ViewState["CurrentPage"]);

                // Get supplier's product IDs using the stored function
                List<int> supplierProductIds = await GetSupplierProductIds();

                if (supplierProductIds.Count == 0)
                {
                    // No products found for this supplier
                    pnlNoReviews.Visible = true;
                    rptReviews.Visible = false;
                    return;
                }

                // Connect to MongoDB
                var client = new MongoClient(mongoConnectionString);
                var database = client.GetDatabase(databaseName);
                var reviewsCollection = database.GetCollection<BsonDocument>(reviewsCollectionName);

                // Build the filter
                FilterDefinition<BsonDocument> filter;

                if (selectedProductId > 0)
                {
                    // Filter by specific product
                    filter = Builders<BsonDocument>.Filter.Eq("productId", selectedProductId);
                }
                else
                {
                    // Filter by all products of this supplier
                    filter = Builders<BsonDocument>.Filter.In("productId", supplierProductIds);
                }

                // Add rating filter if selected
                if (selectedRating > 0)
                {
                    filter = Builders<BsonDocument>.Filter.And(
                        filter,
                        Builders<BsonDocument>.Filter.Eq("rating", selectedRating)
                    );
                }

                // Calculate skip for pagination
                int skip = (currentPage - 1) * pageSize;

                // Get total count for pagination
                long totalCount = await reviewsCollection.CountDocumentsAsync(filter);

                // Get the reviews with sorting and pagination
                var reviews = await reviewsCollection
                    .Find(filter)
                    .Sort(Builders<BsonDocument>.Sort.Descending("createdDate"))
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();

                // Process the reviews to join with product information
                var reviewData = await ProcessReviewsData(reviews);

                // Bind data to repeater
                rptReviews.DataSource = reviewData;
                rptReviews.DataBind();

                pnlNoReviews.Visible = reviewData.Count == 0;
                rptReviews.Visible = reviewData.Count > 0;

                // Setup pagination
                SetupPagination(totalCount);
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error loading reviews: " + ex.Message);
                pnlNoReviews.Visible = true;
                rptReviews.Visible = false;
            }
        }

        private async Task<List<int>> GetSupplierProductIds()
        {
            List<int> productIds = new List<int>();

            try
            {
                using (OracleConnection conn = new OracleConnection(oracleConnectionString))
                {
                    await conn.OpenAsync();

                    // Using fn_GetAllProducts function to get products for this supplier
                    using (OracleCommand cmd = new OracleCommand("BEGIN :result := fn_GetAllProducts(:supplierId); END;", conn))
                    {
                        cmd.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("supplierId", OracleDbType.Int32).Value = supplierId;

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                productIds.Add(Convert.ToInt32(reader["ProductID"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error getting supplier product IDs: " + ex.Message);
            }

            return productIds;
        }

        private async Task<List<dynamic>> ProcessReviewsData(List<BsonDocument> reviews)
        {
            var result = new List<dynamic>();
            Dictionary<int, string> productNames = await GetProductNames();

            foreach (var review in reviews)
            {
                int productId = review["productId"].AsInt32;

                // Create a dynamic object with all the needed properties
                result.Add(new
                {
                    ReviewId = review["_id"].ToString(),
                    ProductId = productId,
                    ProductName = productNames.ContainsKey(productId) ? productNames[productId] : "Unknown Product",
                    CustomerName = review["customerName"].AsString,
                    Rating = review["rating"].AsInt32,
                    ReviewText = review["reviewText"].AsString,
                    ReviewDate = review["createdDate"].ToUniversalTime()
                });
            }

            return result;
        }

        private async Task<Dictionary<int, string>> GetProductNames()
        {
            Dictionary<int, string> productNames = new Dictionary<int, string>();

            try
            {
                using (OracleConnection conn = new OracleConnection(oracleConnectionString))
                {
                    await conn.OpenAsync();

                    // Using fn_GetAllProducts function instead of direct SQL
                    using (OracleCommand cmd = new OracleCommand("BEGIN :result := fn_GetAllProducts(:supplierId); END;", conn))
                    {
                        cmd.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("supplierId", OracleDbType.Int32).Value = supplierId;

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int productId = Convert.ToInt32(reader["ProductID"]);
                                string productName = reader["ProductName"].ToString();
                                productNames[productId] = productName;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error getting product names: " + ex.Message);
            }

            return productNames;
        }

        private void SetupPagination(long totalCount)
        {
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (totalPages > 1)
            {
                List<int> pages = new List<int>();
                for (int i = 1; i <= totalPages; i++)
                {
                    pages.Add(i);
                }

                rptPaging.DataSource = pages;
                rptPaging.DataBind();
                rptPaging.Visible = true;
            }
            else
            {
                rptPaging.Visible = false;
            }
        }

        protected async void UpdateReviewStats()
        {
            try
            {
                // Get supplier's product IDs 
                List<int> supplierProductIds = await GetSupplierProductIds();

                if (supplierProductIds.Count == 0)
                {
                    return;
                }

                // Connect to MongoDB
                var client = new MongoClient(mongoConnectionString);
                var database = client.GetDatabase(databaseName);
                var reviewsCollection = database.GetCollection<BsonDocument>(reviewsCollectionName);

                // Base filter for supplier's products only
                var filter = Builders<BsonDocument>.Filter.In("productId", supplierProductIds);

                // Total reviews
                long totalReviews = await reviewsCollection.CountDocumentsAsync(filter);
                litTotalReviews.Text = totalReviews.ToString();

                // Calculate average rating
                if (totalReviews > 0)
                {
                    var pipeline = new BsonDocument[]
                    {
                        new BsonDocument("$match", new BsonDocument("productId", new BsonDocument("$in", new BsonArray(supplierProductIds)))),
                        new BsonDocument("$group", new BsonDocument
                        {
                            { "_id", null },
                            { "averageRating", new BsonDocument("$avg", "$rating") }
                        })
                    };

                    var result = await reviewsCollection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                    double avgRating = result != null ? result["averageRating"].AsDouble : 0;
                    litAvgRating.Text = avgRating.ToString("F1");
                }
                else
                {
                    litAvgRating.Text = "0.0";
                }

                // Recent reviews (last 7 days)
                var lastWeekDate = DateTime.UtcNow.AddDays(-7);
                var recentFilter = Builders<BsonDocument>.Filter.And(
                    filter,
                    Builders<BsonDocument>.Filter.Gte("createdDate", lastWeekDate)
                );

                long recentReviews = await reviewsCollection.CountDocumentsAsync(recentFilter);
                litRecentReviews.Text = recentReviews.ToString();

                // Negative reviews (rating <= 2)
                var negativeFilter = Builders<BsonDocument>.Filter.And(
                    filter,
                    Builders<BsonDocument>.Filter.Lte("rating", 2)
                );

                long negativeReviews = await reviewsCollection.CountDocumentsAsync(negativeFilter);
                litNegativeReviews.Text = negativeReviews.ToString();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Error updating stats: " + ex.Message);
            }
        }

        protected string GetStarRating(int rating)
        {
            string stars = string.Empty;

            for (int i = 1; i <= 5; i++)
            {
                if (i <= rating)
                {
                    stars += "★"; // Filled star
                }
                else
                {
                    stars += "☆"; // Empty star
                }
            }

            return stars;
        }

        protected void ddlProductFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["CurrentPage"] = 1;
            LoadReviews();
            UpdateReviewStats();
        }

        protected void ddlRatingFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["CurrentPage"] = 1;
            LoadReviews();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            ViewState["CurrentPage"] = 1;
            LoadReviews();
            UpdateReviewStats();
        }

        protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                ViewState["CurrentPage"] = Convert.ToInt32(e.CommandArgument);
                LoadReviews();
            }
        }
    }
}