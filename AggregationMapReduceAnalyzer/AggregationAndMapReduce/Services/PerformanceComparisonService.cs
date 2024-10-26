using AggregationAndMapReduce.BusinessObject;
using Amazon.Runtime;
using MongoDB.Bson;
using MongoDB.Driver;
using ScottPlot;
using System.Diagnostics;
using ScottPlot;

namespace AggregationAndMapReduce.Services
{
    public class PerformanceComparisonService : IPerformanceComparisonService
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;
        private readonly IDataGenerationService _dataGenerator;
        private readonly IAggregationService _aggregationAnalyzer;
        private readonly IMapReduceService _mapReduceAnalyzer;
        private readonly List<string> _metrics = new List<string>();
        private readonly IList<PerformanceMetric> _performanceMetricList;

        public PerformanceComparisonService(IDataGenerationService dataGenerator,
            IAggregationService aggregationAnalyzer, IMapReduceService mapReduceService)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("ecommerce");
            _collectionName = "sales_test";
            _dataGenerator = dataGenerator;
            _aggregationAnalyzer = aggregationAnalyzer;
            _mapReduceAnalyzer = mapReduceService;
            _performanceMetricList = new List<PerformanceMetric>();
        }

        public async Task<List<string>> RunComparison(int[] dataSizes)
        {
            foreach (var size in dataSizes)
            {
                _metrics.Add($"Running comparison for {size} documents");

                await _dataGenerator.GenerateData(size);

                var aggregationResult = await MeasurePerformance(
                        () => _aggregationAnalyzer.AnalyzeSales
                            (new DateTime(2022, 1, 1), 
                                new DateTime(2024, 12, 31))
                );

                var mapReduceResult = await MeasurePerformance(
                       () => _mapReduceAnalyzer.AnalyzeSales
                           (new DateTime(2022, 1, 1), 
                                new DateTime(2024, 12, 31))
                );

                _metrics.Add($"Aggregation Pipeline: {aggregationResult}");
                _metrics.Add($"Map-Reduce: {mapReduceResult}");
                _metrics.Add($"-----------------------------");

                await CreateMetric(size, aggregationResult, mapReduceResult);

                await _database.DropCollectionAsync(_collectionName);
            }

            await CreateGraph();

            return _metrics;
        }

        private async Task<PerformanceResult> MeasurePerformance(
            Func<Task<List<BsonDocument>>> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            var processBefore = Process.GetCurrentProcess();
            var cpuTimeBefore = processBefore.TotalProcessorTime;
            var memoryBefore = GC.GetTotalMemory(true);

            var result = await operation();

            var processAfter = Process.GetCurrentProcess();
            var cpuTimeAfter = processAfter.TotalProcessorTime;
            var memoryAfter = GC.GetTotalMemory(false);
            stopwatch.Stop();

            return new PerformanceResult
            {
                ExecutionTime = stopwatch.Elapsed,
                MemoryUsed = (memoryAfter - memoryBefore) 
                    / (1024.0 * 1024.0),
                PeakWorkingSet = processBefore.PeakWorkingSet64 
                    / (1024.0 * 1024.0),
            };
        }

        public async Task CreateGraph()
        {
            var xValues = _performanceMetricList.Select(m => (double)m.Documents).ToArray();

            // Generate Execution Time Chart
            var timePlot = new ScottPlot.Plot();

            var aggTime = timePlot.Add.Scatter(xValues, _performanceMetricList.Select(m => m.AggregationTime).ToArray());
            aggTime.LegendText = "Aggregation Pipeline";

            var mrTime = timePlot.Add.Scatter(xValues, _performanceMetricList.Select(m => m.MapReduceTime).ToArray());
            mrTime.LegendText = "Map-Reduce";

            timePlot.Title("Execution Time Comparison");
            timePlot.XLabel("Number of Documents");
            timePlot.YLabel("Execution Time (seconds)");
            timePlot.ShowLegend();

            var memoryPlot = new ScottPlot.Plot();

            var aggMemory = memoryPlot.Add.Scatter(xValues, _performanceMetricList.Select(m => m.AggregationMemory).ToArray());
            aggMemory.LegendText = "Aggregation Pipeline";

            var mrMemory = memoryPlot.Add.Scatter(xValues, _performanceMetricList.Select(m => m.MapReduceMemory).ToArray());
            mrMemory.LegendText = "Map-Reduce";

            memoryPlot.Title("Memory Usage Comparison");
            memoryPlot.XLabel("Number of Documents");
            memoryPlot.YLabel("Memory Used (MB)");
            memoryPlot.ShowLegend();

            var pwsPlot = new ScottPlot.Plot();

            var aggPWS = pwsPlot.Add.Scatter(xValues, _performanceMetricList.Select(m => m.AggregationPWS).ToArray());
            aggPWS.LegendText = "Aggregation Pipeline";

            var mrPWS = pwsPlot.Add.Scatter(xValues, _performanceMetricList.Select(m => m.MapReducePWS).ToArray());
            mrPWS.LegendText = "Map-Reduce";

            pwsPlot.Title("Peak Working Set Comparison");
            pwsPlot.XLabel("Number of Documents");
            pwsPlot.YLabel("Peak Working Set (MB)");
            pwsPlot.ShowLegend();

            var timePath = Path.Combine("result", "time.png");
            timePlot.SavePng(timePath, 800, 600);

            var memoryPath = Path.Combine("result", "memory.png");
            memoryPlot.SavePng(memoryPath, 800, 600);

            var pwsPath = Path.Combine("result", "pws.png");
            pwsPlot.SavePng(pwsPath, 800, 600);
        }

        private async Task SaveGraph()
        {

        }

        private async Task CreateMetric(int size, PerformanceResult aggregationResult, PerformanceResult mapReduceResult)
        {
            _performanceMetricList.Add(
                new() { 
                    Documents = size, 
                    AggregationTime = aggregationResult.ExecutionTime.TotalSeconds,
                    MapReduceTime = mapReduceResult.ExecutionTime.TotalSeconds,
                    AggregationMemory = aggregationResult.MemoryUsed,
                    MapReduceMemory = mapReduceResult.MemoryUsed,
                    AggregationPWS = aggregationResult.PeakWorkingSet,
                    MapReducePWS = mapReduceResult.PeakWorkingSet
                });
        }
    }
}
