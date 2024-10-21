using AggregationAndMapReduce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AggregationAndMapReduce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceComparisonController : ControllerBase
    {
        private readonly IPerformanceComparisonService _performanceComparisonService;

        public PerformanceComparisonController(IPerformanceComparisonService performanceComparisonService)
        {
            _performanceComparisonService = performanceComparisonService;
        }

        [HttpPost("run-comparison")]
        public async Task<IActionResult> RunComparison()
        {
            var dataSizes = new int[] { 10000, 100000, 1000000 };
            var result = await _performanceComparisonService.RunComparison(dataSizes);

            return Ok(result);
        }
    }
}
