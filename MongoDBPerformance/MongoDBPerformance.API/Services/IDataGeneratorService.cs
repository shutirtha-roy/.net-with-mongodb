using MongoDB.Bson;
using MongoDBPerformance.API.BusinessObject;

namespace MongoDBPerformance.API.Services
{
    public interface IDataGeneratorService
    {
        Task<IList<Transaction>> GetAllData();
        List<BsonDocument> InsertData();
    }
}
