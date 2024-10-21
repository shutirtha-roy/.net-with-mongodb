namespace AggregationAndMapReduce.Services
{
    public interface IPerformanceComparisonService
    {
        Task<List<string>> RunComparison(int[] dataSizes);
    }
}
