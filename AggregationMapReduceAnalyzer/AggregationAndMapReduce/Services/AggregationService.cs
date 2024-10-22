using Bogus;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Transactions;

namespace AggregationAndMapReduce.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public AggregationService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ecommerce");
            _collection = database.GetCollection<BsonDocument>("sales_test");
        }

        public async Task<List<BsonDocument>> AnalyzeSales(DateTime startDate, DateTime endDate)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "date", new BsonDocument
                        {
                            { "$gte", startDate },
                            { "$lt", endDate }
                        }
                    }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument
                        {
                            { "year", new BsonDocument("$year", "$date") },
                            { "month", new BsonDocument("$month", "$date") },
                            { "category", "$category" }
                        }
                    },
                    { "totalSales", new BsonDocument("$sum", new BsonDocument("$multiply", new BsonArray { "$price", "$quantity" })) },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "year", "$_id.year" },
                    { "month", "$_id.month" },
                    { "category", "$_id.category" },
                    { "totalSales", 1 },
                    { "count", 1 },
                    { "averageSale", new BsonDocument("$divide", new BsonArray { "$totalSales", "$count" }) }
                }),
                new BsonDocument("$sort", new BsonDocument
                {
                    { "year", 1 },
                    { "month", 1 },
                    { "category", 1 }
                })
            };

            var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results;
        }
    }
}
