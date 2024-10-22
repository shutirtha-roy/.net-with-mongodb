namespace AggregationAndMapReduce.BusinessObject
{
    public class PerformanceResult
    {
        public TimeSpan ExecutionTime { get; set; }
        public double MemoryUsed { get; set; }
        public double PeakWorkingSet { get; set; }

        public override string ToString()
        {
            return $"Execution Time: {ExecutionTime.TotalSeconds:F2} seconds, " +
                   $"Memory Used: {MemoryUsed:F2} MB, " +
                   $"Peak Working Set: {PeakWorkingSet:F2} MB";
        }
    }
}
