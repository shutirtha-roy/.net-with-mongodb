using MongoDB.Bson;

namespace AggregationAndMapReduce.Services
{
    public interface IAggregationService
    {
        Task<List<BsonDocument>> AnalyzeSales(DateTime startDate, DateTime endDate);
    }
}
