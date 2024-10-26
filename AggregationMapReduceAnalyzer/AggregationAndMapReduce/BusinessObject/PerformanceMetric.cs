namespace AggregationAndMapReduce.BusinessObject
{
    public class PerformanceMetric
    {
        public int Documents { get; set; }
        public double AggregationTime { get; set; }
        public double MapReduceTime { get; set; }
        public double AggregationMemory { get; set; }
        public double MapReduceMemory { get; set; }
        public double AggregationPWS { get; set; }
        public double MapReducePWS { get; set; }
    }
}
