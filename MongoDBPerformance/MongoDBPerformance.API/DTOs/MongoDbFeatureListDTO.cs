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
    }
}
