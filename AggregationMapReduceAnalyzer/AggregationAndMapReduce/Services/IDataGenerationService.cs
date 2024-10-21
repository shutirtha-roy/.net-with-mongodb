namespace AggregationAndMapReduce.Services
{
    public interface IDataGenerationService
    {
        Task<bool> GenerateData(int size);
    }
}
