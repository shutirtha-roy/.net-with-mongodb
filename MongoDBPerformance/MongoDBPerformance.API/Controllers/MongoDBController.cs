using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDBPerformance.API.DTOs;
using MongoDBPerformance.API.Services;

namespace MongoDBPerformance.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MongoDBController : ControllerBase
    {
        private readonly ILogger<MongoDBController> _logger;
        private readonly MongoDbDataListDTO _dataDTO;
        private readonly MongoDbFeatureListDTO _featureListDTO;

        public MongoDBController(ILogger<MongoDBController> logger,
            MongoDbDataListDTO mongoDbDataListDto, MongoDbFeatureListDTO mongoDbFeatureListDto)
        {
            _logger = logger;
            _dataDTO = mongoDbDataListDto;
            _featureListDTO = mongoDbFeatureListDto;
        }

        [HttpGet(Name = "get-data-list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetAllData()
        {
            var dataGenerator = _dataDTO;
            var dataList = await dataGenerator.GetAllData();

            return Ok(dataList);
        }

        [HttpGet("monthly-report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetMonthlySalesReport()
        {
            var featureListDto = _featureListDTO;
            var response = await featureListDto.GetMonthlyReport();

            return Ok(response);
        }
    }
}
