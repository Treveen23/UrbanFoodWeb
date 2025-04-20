using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanFoodWeb.Helpers;
using UrbanFoodWeb.Models;

namespace UrbanFoodWeb.Models
{
    /// <summary>
    /// Product Review model class for MongoDB storage
    /// </summary>
    public class ProductReview
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("productId")]
        public int ProductId { get; set; }

        [BsonElement("customerId")]
        public int CustomerId { get; set; }  // This will store the CustomerId value

        [BsonElement("customerName")]
        public string CustomerName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("reviewText")]
        public string ReviewText { get; set; }

        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}

namespace UrbanFoodWeb.Repositories
{
    /// <summary>
    /// Repository for managing Product Reviews in MongoDB
    /// </summary>
    public class ReviewRepository
    {
        private readonly IMongoCollection<ProductReview> _reviews;
        private readonly MongoDBHelper _dbHelper;

        /// <summary>
        /// Constructor - initializes MongoDB collection
        /// </summary>
        public ReviewRepository()
        {
            _dbHelper = new Helpers.MongoDBHelper();
            _reviews = _dbHelper.GetCollection<ProductReview>("ProductReviews");
        }

        /// <summary>
        /// Get all reviews for a specific product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <returns>List of product reviews</returns>
        public async Task<List<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            try
            {
                var filter = Builders<ProductReview>.Filter.Eq("productId", productId);
                return await _reviews.Find(filter).SortByDescending(r => r.CreatedDate).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log error
                throw new Exception("Error retrieving reviews: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Calculate the average rating for a product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <returns>Average rating (0-5)</returns>
        public async Task<double> GetAverageRatingAsync(int productId)
        {
            try
            {
                var filter = Builders<ProductReview>.Filter.Eq("productId", productId);
                var reviews = await _reviews.Find(filter).ToListAsync();

                if (reviews.Count == 0)
                    return 0;

                double totalRating = 0;
                foreach (var review in reviews)
                {
                    totalRating += review.Rating;
                }

                return Math.Round(totalRating / reviews.Count, 1);
            }
            catch (Exception ex)
            {
                // Log error
                throw new Exception("Error calculating average rating: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Add a new review
        /// </summary>
        /// <param name="review">The review to add</param>
        public async Task AddReviewAsync(ProductReview review)
        {
            try
            {
                // Set creation date if not already set
                if (review.CreatedDate == DateTime.MinValue)
                {
                    review.CreatedDate = DateTime.Now;
                }

                await _reviews.InsertOneAsync(review);
            }
            catch (Exception ex)
            {
                // Log error
                throw new Exception("Error adding review: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Update an existing review
        /// </summary>
        /// <param name="review">The review to update</param>
        public async Task UpdateReviewAsync(ProductReview review)
        {
            try
            {
                var filter = Builders<ProductReview>.Filter.Eq("_id", ObjectId.Parse(review.Id));
                await _reviews.ReplaceOneAsync(filter, review);
            }
            catch (Exception ex)
            {
                // Log error
                throw new Exception("Error updating review: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Delete a review
        /// </summary>
        /// <param name="reviewId">The ID of the review to delete</param>
        public async Task DeleteReviewAsync(string reviewId)
        {
            try
            {
                var filter = Builders<ProductReview>.Filter.Eq("_id", ObjectId.Parse(reviewId));
                await _reviews.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                // Log error
                throw new Exception("Error deleting review: " + ex.Message, ex);
            }
        }
    }
}