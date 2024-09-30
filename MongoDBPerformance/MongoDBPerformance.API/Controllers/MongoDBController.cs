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

        public MongoDBController(ILogger<MongoDBController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetAllData()
        {
            var dataGenerator = new MongoDbDataListDTO();
            var dataList = await dataGenerator.GetAllData();

            return Ok(dataList);
        }
    }
}
