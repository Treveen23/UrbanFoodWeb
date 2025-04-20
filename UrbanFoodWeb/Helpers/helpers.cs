using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace UrbanFoodWeb.Helpers
{
    /// <summary>
    /// Helper class for MongoDB operations
    /// </summary>
    public class MongoDBHelper
    {
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Constructor - initializes MongoDB connection using Web.config settings
        /// </summary>
        public MongoDBHelper()
        {
            try
            {
                // Get connection string from web.config
                string connectionString = ConfigurationManager.AppSettings["MongoDBConnectionString"];
                string databaseName = ConfigurationManager.AppSettings["MongoDBDatabaseName"];

                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
                {
                    throw new ConfigurationErrorsException("MongoDB connection string or database name is missing in configuration.");
                }

                // Create MongoDB client and get database
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                // Log error - you might want to implement proper logging here
                throw new Exception("Failed to initialize MongoDB connection: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Get a MongoDB collection of specified type
        /// </summary>
        /// <typeparam name="T">The type of documents in the collection</typeparam>
        /// <param name="collectionName">Name of the collection</param>
        /// <returns>MongoDB collection</returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Check if connection to MongoDB is active
        /// </summary>
        /// <returns>True if connection is valid</returns>
        public bool IsConnectionValid()
        {
            try
            {
                // Try to ping the server
                _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}