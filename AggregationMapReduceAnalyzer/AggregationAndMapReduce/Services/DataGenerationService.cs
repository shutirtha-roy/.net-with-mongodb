using Bogus;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Transactions;

namespace AggregationAndMapReduce.Services
{
    public class DataGenerationService : IDataGenerationService
    {
        private readonly IMongoCollection<BsonDocument> _collection;
        private readonly Faker _faker;

        public DataGenerationService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ecommerce");
            _collection = database.GetCollection<BsonDocument>("sales_test");
            _faker = new Faker();
        }

        public async Task<bool> GenerateData(int size)
        {
            var documents = new List<BsonDocument>();

            for (int i = 0; i < size; i++)
            {
                var document = new BsonDocument
                {
                    { "date", _faker.Date.Between(new DateTime(2022, 1, 1), new DateTime(2024, 12, 31)) },
                    { "product", _faker.Commerce.ProductName() },
                    { "category", _faker.Commerce.Categories(1)[0] },
                    { "price", _faker.Random.Decimal(10, 1000) },
                    { "quantity", _faker.Random.Int(1, 10) },
                    { "customer", new BsonDocument
                        {
                            { "name", _faker.Name.FullName() },
                            { "email", _faker.Internet.Email() },
                            { "address", _faker.Address.FullAddress() }
                        }
                    }
                };

                documents.Add(document);

                if (documents.Count == 1000)
                {
                    await _collection.InsertManyAsync(documents);
                    documents.Clear();
                    Console.WriteLine($"Inserted {i + 1} documents");
                }
            }

            if (documents.Count > 0)
            {
                await _collection.InsertManyAsync(documents);
            }

            return true;
        }
    }
}
