using MongoDB.Bson;
using MongoDBPerformance.API.BusinessObject;
using MongoDBPerformance.API.DTOs;

namespace MongoDBPerformance.API.Services
{
    public interface ITransactionService
    {
        Task<IList<Transaction>> GetAllData();
        List<BsonDocument> InsertData();
        Task<IList<MonthlySalesReportDTO>> GetMonthlySalesReportAsync();
    }
}
