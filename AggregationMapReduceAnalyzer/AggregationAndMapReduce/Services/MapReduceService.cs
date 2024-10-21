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
            var map = new BsonJavaScript(@"
                function() {
                    var year = this.date.getFullYear();
                    var month = this.date.getMonth() + 1;
                    var sales = this.price * this.quantity;
                    emit({ year: year, month: month, category: this.category }, { totalSales: sales, count: 1 });
                }");

            var reduce = new BsonJavaScript(@"
                function(key, values) {
                    var result = { totalSales: 0, count: 0 };
                    values.forEach(function(value) {
                        result.totalSales += value.totalSales;
                        result.count += value.count;
                    });
                    return result;
                }");

            var finalize = new BsonJavaScript(@"
                function(key, reducedValue) {
                    reducedValue.averageSale = reducedValue.totalSales / reducedValue.count;
                    return reducedValue;
                }");

            var options = new MapReduceOptions<BsonDocument, BsonDocument>
            {
                OutputOptions = MapReduceOutputOptions.Inline,
                Filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Gte("date", startDate),
                    Builders<BsonDocument>.Filter.Lt("date", endDate)
                ),
                Finalize = finalize,
                Sort = Builders<BsonDocument>.Sort.Ascending("date")
            };

            var results = await _collection.MapReduceAsync(map, reduce, options);
            return results.ToList();
        }
    }
}
