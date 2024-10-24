﻿using Bogus;
using Bogus.DataSets;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBPerformance.API.BusinessObject;
using MongoDBPerformance.API.DTOs;
using System.Net;
using System.Text.Json;

namespace MongoDBPerformance.API.Services
{    

    public class TransactionService : ITransactionService
    {
        private readonly IMongoCollection<Transaction> _collection;

        public TransactionService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ecommerce");
            _collection = database.GetCollection<Transaction>("transactions");
        }

        public async Task<IList<Transaction>> GetAllData()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ecommerce");
            var collection = database.GetCollection<BsonDocument>("transactions");

            var transactionDocuments = await collection.Find(new BsonDocument())
                .Limit(10)
                .ToListAsync();

            var transactions = new List<Transaction>();

            foreach (var doc in transactionDocuments)
            {
                var transaction = new Transaction
                {
                    TransactionId = doc.GetValue("TransactionId").AsObjectId.ToString(),
                    UserId = doc.GetValue("UserId").AsObjectId.ToString(),
                    Timestamp = doc.GetValue("Timestamp").ToUniversalTime(),
                    TotalAmount = doc.GetValue("TotalAmount").ToDecimal(),
                    PaymentMethod = doc.GetValue("PaymentMethod").AsString,
                    ShippingAddress = new ShippingAddress
                    {
                        Street = doc.GetValue("ShippingAddress")["Street"].AsString,
                        City = doc.GetValue("ShippingAddress")["City"].AsString,
                        State = doc.GetValue("ShippingAddress")["State"].AsString,
                        Country = doc.GetValue("ShippingAddress")["Country"].AsString,
                        ZipCode = doc.GetValue("ShippingAddress")["ZipCode"].AsString
                    },
                    Items = new List<Item>()
                };

                // Assuming Items are in a subdocument "Items" in BsonDocument format
                foreach (var itemDoc in doc.GetValue("Items").AsBsonArray)
                {
                    var item = new Item
                    {
                        ItemId = itemDoc["ProductId"].AsObjectId.ToString(),
                        Description = itemDoc["ProductName"].AsString,
                        Quantity = itemDoc["Quantity"].AsInt32,
                        Price = itemDoc["Price"].ToDecimal()
                    };
                    transaction.Items.Add(item);
                }

                transactions.Add(transaction);
            }

            return transactions;
        }

        public List<BsonDocument> InsertData()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ecommerce");
            var collection = database.GetCollection<BsonDocument>("transactions");

            var faker = new Faker();
            var transactions = new List<BsonDocument>();

            Console.WriteLine("Generating data...");

            for (int i = 0; i < 1000000; i++) // Generate 1 million records
            {
                var transaction = new BsonDocument
            {
                { "TransactionId", ObjectId.GenerateNewId() },
                { "UserId", ObjectId.GenerateNewId() },
                { "Timestamp", faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now) },
                { "TotalAmount", faker.Finance.Amount(10, 1000, 2) },
                { "PaymentMethod", faker.PickRandom(new[] { "Credit Card", "PayPal", "Bank Transfer" }) },
                { "ShippingAddress", new BsonDocument
                    {
                        { "Street", faker.Address.StreetAddress() },
                        { "City", faker.Address.City() },
                        { "State", faker.Address.State() },
                        { "Country", faker.Address.Country() },
                        { "ZipCode", faker.Address.ZipCode() }
                    }
                },
                { "Items", new BsonArray(GenerateItems(faker)) }
            };

                transactions.Add(transaction);

                if (transactions.Count == 10000) // Insert in batches of 10,000
                {
                    collection.InsertMany(transactions);
                    transactions.Clear();
                    Console.WriteLine($"Inserted {i + 1} records");
                }
            }

            if (transactions.Count > 0)
            {
                collection.InsertMany(transactions);
            }

            Console.WriteLine("Data generation complete.");

            return transactions;
        }

        private List<BsonDocument> GenerateItems(Faker faker)
        {
            var items = new List<BsonDocument>();
            var itemCount = faker.Random.Int(1, 5);

            for (int i = 0; i < itemCount; i++)
            {
                items.Add(new BsonDocument
            {
                { "ProductId", ObjectId.GenerateNewId() },
                { "ProductName", faker.Commerce.ProductName() },
                { "Category", faker.Commerce.Categories(1)[0] },
                { "Price", faker.Finance.Amount(5, 500, 2) },
                { "Quantity", faker.Random.Int(1, 10) }
            });
            }

            return items;
        }

        public async Task<IList<MonthlySalesReportDTO>> GetMonthlySalesReportAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument
                        {
                            { "year", new BsonDocument("$year", "$Timestamp") },
                            { "month", new BsonDocument("$month", "$Timestamp") }
                        }
                    },
                    { "totalSales", new BsonDocument("$sum", "$TotalAmount") },
                    { "averageOrderValue", new BsonDocument("$avg", "$TotalAmount") },
                    { "orderCount", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var monthlySalesReports = new List<MonthlySalesReportDTO>();

            foreach (var doc in result)
            {
                var idDoc = doc["_id"].AsBsonDocument;
                monthlySalesReports.Add(new MonthlySalesReportDTO
                {
                    Year = idDoc["year"].AsInt32,
                    Month = idDoc["month"].AsInt32,
                    TotalSales = doc["totalSales"].AsDecimal,
                    AverageOrderValue = doc["averageOrderValue"].AsDecimal,
                    OrderCount = doc["orderCount"].AsInt32
                });
            }

            return monthlySalesReports;
        }
    }
}
