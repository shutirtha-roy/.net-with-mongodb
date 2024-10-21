using AggregationAndMapReduce.Services;
using Microsoft.AspNetCore.Mvc;

namespace AggregationAndMapReduce.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDataGenerationService _dataGenerationService;

        public DataController(IDataGenerationService dataGenerationService)
        {
            _dataGenerationService = dataGenerationService;
        }

        [HttpGet("create-dataset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetAllData()
        {
            var response = await _dataGenerationService.GenerateData(20000000);

            return Ok(response);
        }


    }
}
