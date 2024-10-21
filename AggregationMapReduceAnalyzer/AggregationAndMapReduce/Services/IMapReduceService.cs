using MongoDB.Bson;

namespace AggregationAndMapReduce.Services
{
    public interface IMapReduceService
    {
        Task<List<BsonDocument>> AnalyzeSales(DateTime startDate, DateTime endDate);
    }
}
