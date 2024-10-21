﻿using Amazon.Runtime;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

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

        public PerformanceComparisonService(IDataGenerationService dataGenerator,
            IAggregationService aggregationAnalyzer, IMapReduceService mapReduceService)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("ecommerce");
            _collectionName = "sales_test";
            _dataGenerator = dataGenerator;
            _aggregationAnalyzer = aggregationAnalyzer;
            _mapReduceAnalyzer = mapReduceService;
        }

        public async Task<List<string>> RunComparison(int[] dataSizes)
        {
            foreach (var size in dataSizes)
            {
                Console.WriteLine($"Running comparison for {size} documents");
                _metrics.Add($"Running comparison for {size} documents");

                await _dataGenerator.GenerateData(size);

                var aggregationResult = await MeasurePerformance(() => _aggregationAnalyzer.AnalyzeSales(new DateTime(2020, 1, 1), new DateTime(2024, 1, 1)));
                var mapReduceResult = await MeasurePerformance(() => _mapReduceAnalyzer.AnalyzeSales(new DateTime(2020, 1, 1), new DateTime(2024, 1, 1)));

                Console.WriteLine($"Aggregation Pipeline: {aggregationResult.ExecutionTime.TotalSeconds} seconds, {aggregationResult.MemoryUsed} MB");
                Console.WriteLine($"Map-Reduce: {mapReduceResult.ExecutionTime.TotalSeconds} seconds, {mapReduceResult.MemoryUsed} MB");

                _metrics.Add($"Aggregation Pipeline: {aggregationResult.ExecutionTime.TotalSeconds} seconds, {aggregationResult.MemoryUsed} MB");
                _metrics.Add($"Map-Reduce: {mapReduceResult.ExecutionTime.TotalSeconds} seconds, {mapReduceResult.MemoryUsed} MB");

                await _database.DropCollectionAsync(_collectionName);
            }

            return _metrics;
        }

        private async Task<PerformanceResult> MeasurePerformance(Func<Task<List<BsonDocument>>> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            var memoryBefore = GC.GetTotalMemory(true);

            var result = await operation();

            var memoryAfter = GC.GetTotalMemory(false);
            stopwatch.Stop();

            return new PerformanceResult
            {
                ExecutionTime = stopwatch.Elapsed,
                MemoryUsed = (memoryAfter - memoryBefore) / (1024 * 1024), // Convert to MB
                ResultCount = result.Count
            };
        }
    }

    public class PerformanceResult
    {
        public TimeSpan ExecutionTime { get; set; }
        public long MemoryUsed { get; set; }
        public int ResultCount { get; set; }
    }
}