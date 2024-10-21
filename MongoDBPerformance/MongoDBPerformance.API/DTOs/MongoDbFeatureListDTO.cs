using MongoDBPerformance.API.Services;

namespace MongoDBPerformance.API.DTOs
{
    public class MongoDbFeatureListDTO
    {
        private readonly ITransactionService _transactionService;

        public MongoDbFeatureListDTO(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        internal async Task<object> GetMonthlyReport()
        {
            var monthlyRepost = await _transactionService.GetMonthlySalesReportAsync();

            var response = new APIResponse();
            response.Result = monthlyRepost;

            return monthlyRepost;
        }
    }
}
