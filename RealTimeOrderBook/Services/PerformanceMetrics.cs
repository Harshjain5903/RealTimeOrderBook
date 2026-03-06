// Author: Harsh Jain
// Real-Time Order Book - Performance metrics tracking service

using System;
using System.Diagnostics;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Tracks performance metrics for the order book simulation including
    /// timing, throughput, and resource utilization statistics.
    /// </summary>
    public class PerformanceMetrics
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private long _ordersProcessed;
        private long _totalProcessingTimeMs;
        private decimal _minLatencyMs = decimal.MaxValue;
        private decimal _maxLatencyMs = 0;

        public long OrdersProcessed => _ordersProcessed;
        public long TotalProcessingTimeMs => _totalProcessingTimeMs;
        public long ElapsedTimeMs => _stopwatch.ElapsedMilliseconds;
        
        public decimal AvgLatencyMs => _ordersProcessed > 0 
            ? (decimal)_totalProcessingTimeMs / _ordersProcessed 
            : 0;
            
        public decimal OrdersPerSecond => ElapsedTimeMs > 0 
            ? (_ordersProcessed * 1000m) / ElapsedTimeMs 
            : 0;
            
        public decimal MinLatencyMs => _minLatencyMs == decimal.MaxValue ? 0 : _minLatencyMs;
        public decimal MaxLatencyMs => _maxLatencyMs;

        public void Start()
        {
            _stopwatch.Start();
            Logger.Info("Performance metrics tracking started");
        }

        public void Stop()
        {
            _stopwatch.Stop();
            Logger.Info($"Performance metrics tracking stopped. Total elapsed: {ElapsedTimeMs}ms");
        }

        public void RecordOrderProcessing(long latencyMs)
        {
            _ordersProcessed++;
            _totalProcessingTimeMs += latencyMs;
            
            if (latencyMs < (long)_minLatencyMs)
                _minLatencyMs = latencyMs;
                
            if (latencyMs > (long)_maxLatencyMs)
                _maxLatencyMs = latencyMs;
        }

        public void Reset()
        {
            _stopwatch.Restart();
            _ordersProcessed = 0;
            _totalProcessingTimeMs = 0;
            _minLatencyMs = decimal.MaxValue;
            _maxLatencyMs = 0;
        }

        public string GetSummary()
        {
            return $@"=== Performance Metrics Summary ===
Orders Processed: {OrdersProcessed}
Total Time: {ElapsedTimeMs}ms ({ElapsedTimeMs / 1000.0:F2}s)
Throughput: {OrdersPerSecond:F2} orders/sec
Avg Latency: {AvgLatencyMs:F3}ms
Min Latency: {MinLatencyMs:F3}ms
Max Latency: {MaxLatencyMs:F3}ms
====================================";
        }
    }
}
