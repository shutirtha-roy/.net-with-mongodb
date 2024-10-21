using MongoDB.Bson;
using MongoDB.Driver;

namespace AggregationAndMapReduce.Services
{
    public class MapReduceService : IMapReduceService
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MapReduceService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ecommerce");
            _collection = database.GetCollection<BsonDocument>("sales_test");
        }

        public async Task<List<BsonDocument>> AnalyzeSales(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Gte("date", startDate),
                Builders<BsonDocument>.Filter.Lt("date", endDate)
            );

            var documents = await _collection.Find(filter).ToListAsync();

            var results = documents
                .Select(Map)
                .Where(x => x.HasValue)
                .GroupBy(x => x.Value.Key)
                .Select(g => Reduce(g.Key, g.Select(x => x.Value.Value)))
                .Select(Finalize)
                .OrderBy(x => x["_id"]["year"])
                .ThenBy(x => x["_id"]["month"])
                .ThenBy(x => x["_id"]["category"])
                .ToList();

            return results;
        }

        private static KeyValuePair<BsonDocument, BsonDocument>? Map(BsonDocument document)
        {
            if (document.TryGetValue("date", out BsonValue dateValue) &&
                document.TryGetValue("price", out BsonValue priceValue) &&
                document.TryGetValue("quantity", out BsonValue quantityValue) &&
                document.TryGetValue("category", out BsonValue categoryValue))
            {
                if (dateValue.IsValidDateTime && priceValue.IsNumeric && quantityValue.IsNumeric)
                {
                    var date = dateValue.ToUniversalTime();
                    var year = date.Year;
                    var month = date.Month;
                    var sales = priceValue.ToDouble() * quantityValue.ToInt32();

                    var key = new BsonDocument
                    {
                        { "year", year },
                        { "month", month },
                        { "category", categoryValue.AsString }
                    };

                    var value = new BsonDocument
                    {
                        { "totalSales", sales },
                        { "count", 1 }
                    };

                    return new KeyValuePair<BsonDocument, BsonDocument>(key, value);
                }
            }
            return null;
        }

        private static BsonDocument Reduce(BsonDocument key, IEnumerable<BsonDocument> values)
        {
            double totalSales = values.Sum(v => v["totalSales"].AsDouble);
            int count = values.Sum(v => v["count"].AsInt32);

            return new BsonDocument
            {
                { "_id", key },
                { "value", new BsonDocument
                    {
                        { "totalSales", totalSales },
                        { "count", count }
                    }
                }
            };
        }

        private static BsonDocument Finalize(BsonDocument result)
        {
            var value = result["value"].AsBsonDocument;
            double totalSales = value["totalSales"].AsDouble;
            int count = value["count"].AsInt32;

            value.Add("averageSale", count > 0 ? totalSales / count : 0);

            return result;
        }
    }
}
