using MongoDBPerformance.API.BusinessObject;
using MongoDBPerformance.API.Services;

namespace MongoDBPerformance.API.DTOs
{
    public class MongoDbDataListDTO
    {
        private readonly ITransactionService _transactionService;

        public MongoDbDataListDTO(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        internal async Task<object> GetAllData()
        {
            var dataList = await _transactionService.GetAllData();

            var response = new APIResponse();
            response.Result = dataList;

            return dataList;
        }
    }
}
