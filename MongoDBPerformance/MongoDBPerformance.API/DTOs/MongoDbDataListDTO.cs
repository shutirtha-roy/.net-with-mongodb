using MongoDBPerformance.API.BusinessObject;
using MongoDBPerformance.API.Services;

namespace MongoDBPerformance.API.DTOs
{
    public class MongoDbDataListDTO
    {
        private readonly IDataGeneratorService _dataGeneratorService;

        public MongoDbDataListDTO()
        {
            _dataGeneratorService = new DataGeneratorService();
        }

        internal async Task<object> GetAllData()
        {
            var dataList = await _dataGeneratorService.GetAllData();

            var response = new APIResponse();
            response.Result = dataList;

            return dataList;
        }
    }
}
