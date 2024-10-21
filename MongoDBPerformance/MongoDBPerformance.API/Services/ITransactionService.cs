using MongoDB.Bson;
using MongoDBPerformance.API.BusinessObject;

namespace MongoDBPerformance.API.Services
{
    public interface ITransactionService
    {
        Task<IList<Transaction>> GetAllData();
        List<BsonDocument> InsertData();
    }
}
