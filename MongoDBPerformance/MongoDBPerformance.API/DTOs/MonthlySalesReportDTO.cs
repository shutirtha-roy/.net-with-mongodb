using System.Text.Json.Serialization;

namespace MongoDBPerformance.API.DTOs
{
    public class MonthlySalesReportDTO
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("month")]
        public int Month { get; set; }

        [JsonPropertyName("totalSales")]
        public decimal TotalSales { get; set; }

        [JsonPropertyName("averageOrderValue")]
        public decimal AverageOrderValue { get; set; }

        [JsonPropertyName("orderCount")]
        public int OrderCount { get; set; }
    }
}
