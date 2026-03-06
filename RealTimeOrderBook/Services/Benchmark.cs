// Author: Harsh Jain
// Real-Time Order Book - Performance benchmarking utilities

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Represents a single benchmark result.
    /// </summary>
    public class BenchmarkResult
    {
        public string Name { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public long ElapsedTicks { get; set; }
        public int Iterations { get; set; }
        public double AvgTimeMs => (double)ElapsedMilliseconds / Iterations;
        public double OpsPerSecond => (Iterations * 1000.0) / ElapsedMilliseconds;

        public BenchmarkResult(string name, long elapsedMs, long elapsedTicks, int iterations)
        {
            Name = name;
            ElapsedMilliseconds = elapsedMs;
            ElapsedTicks = elapsedTicks;
            Iterations = iterations;
        }

        public override string ToString()
        {
            return $"{Name}: {ElapsedMilliseconds}ms for {Iterations} iterations ({AvgTimeMs:F4}ms/op, {OpsPerSecond:F2} ops/sec)";
        }
    }

    /// <summary>
    /// Benchmarking utility for performance testing and optimization.
    /// </summary>
    public class Benchmark
    {
        private readonly List<BenchmarkResult> _results = new List<BenchmarkResult>();

        /// <summary>
        /// Runs a benchmark on the given action.
        /// </summary>
        public BenchmarkResult Measure(string name, Action action, int iterations = 1000)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (iterations <= 0)
                throw new ArgumentException("Iterations must be positive", nameof(iterations));

            // Warmup
            for (int i = 0; i < 10; i++)
            {
                action();
            }

            // Actual benchmark
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                action();
            }
            sw.Stop();

            var result = new BenchmarkResult(name, sw.ElapsedMilliseconds, sw.ElapsedTicks, iterations);
            _results.Add(result);
            Logger.Debug(result.ToString());
            
            return result;
        }

        /// <summary>
        /// Runs a benchmark on the given function and returns a value.
        /// </summary>
        public BenchmarkResult<T> Measure<T>(string name, Func<T> func, int iterations = 1000)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (iterations <= 0)
                throw new ArgumentException("Iterations must be positive", nameof(iterations));

            // Warmup
            for (int i = 0; i < 10; i++)
            {
                func();
            }

            // Actual benchmark
            var sw = Stopwatch.StartNew();
            T result = default!;
            for (int i = 0; i < iterations; i++)
            {
                result = func();
            }
            sw.Stop();

            var benchResult = new BenchmarkResult<T>(name, sw.ElapsedMilliseconds, sw.ElapsedTicks, iterations, result);
            _results.Add(benchResult);
            Logger.Debug(benchResult.ToString());
            
            return benchResult;
        }

        /// <summary>
        /// Gets all benchmark results.
        /// </summary>
        public List<BenchmarkResult> GetResults() => new List<BenchmarkResult>(_results);

        /// <summary>
        /// Gets summary of all benchmarks.
        /// </summary>
        public string GetSummary()
        {
            if (_results.Count == 0)
                return "No benchmark results";

            var summary = "=== Benchmark Summary ===\n";
            foreach (var result in _results)
            {
                summary += result.ToString() + "\n";
            }
            summary += "========================";
            return summary;
        }

        /// <summary>
        /// Clears all results.
        /// </summary>
        public void Clear()
        {
            _results.Clear();
        }
    }

    /// <summary>
    /// Generic benchmark result with return value.
    /// </summary>
    public class BenchmarkResult<T> : BenchmarkResult
    {
        public T? Value { get; set; }

        public BenchmarkResult(string name, long elapsedMs, long elapsedTicks, int iterations, T? value)
            : base(name, elapsedMs, elapsedTicks, iterations)
        {
            Value = value;
        }
    }
}
