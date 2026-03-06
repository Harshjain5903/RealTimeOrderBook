// Author: Harsh Jain
// Real-Time Order Book - Data export service for analytics and reporting

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Service for exporting order data to CSV format for external analysis.
    /// Includes trade data, statistics, and performance metrics.
    /// </summary>
    public class DataExportService
    {
        private readonly string _exportDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RealTimeOrderBook",
            "Exports"
        );

        public DataExportService()
        {
            try
            {
                if (!Directory.Exists(_exportDirectory))
                {
                    Directory.CreateDirectory(_exportDirectory);
                    Logger.Info($"Created export directory: {_exportDirectory}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to initialize export directory", ex);
            }
        }

        /// <summary>
        /// Exports order data to CSV file.
        /// </summary>
        public string ExportOrdersToCsv(IEnumerable<Models.Order> orders, string? filename = null)
        {
            try
            {
                filename ??= $"Orders_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
                var filepath = Path.Combine(_exportDirectory, filename);

                using (var writer = new StreamWriter(filepath, false, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine("Symbol,Side,Price,Quantity,Timestamp");

                    // Write orders
                    foreach (var order in orders)
                    {
                        var line = $"{order.Symbol},{order.Side},{order.Price:F2},{order.Quantity},{order.Timestamp:yyyy-MM-dd HH:mm:ss.fff}";
                        writer.WriteLine(line);
                    }
                }

                Logger.Info($"Exported {orders.Count()} orders to {filepath}");
                return filepath;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to export orders to CSV", ex);
                throw;
            }
        }

        /// <summary>
        /// Exports statistics to CSV format.
        /// </summary>
        public string ExportStatisticsToCsv(string symbol, decimal bestBid, decimal bestAsk, 
            long totalVolume, PerformanceMetrics metrics, string? filename = null)
        {
            try
            {
                filename ??= $"Statistics_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
                var filepath = Path.Combine(_exportDirectory, filename);

                using (var writer = new StreamWriter(filepath, false, Encoding.UTF8))
                {
                    writer.WriteLine("Metric,Value");
                    writer.WriteLine($"Symbol,{symbol}");
                    writer.WriteLine($"Best Bid,${bestBid:F2}");
                    writer.WriteLine($"Best Ask,${bestAsk:F2}");
                    writer.WriteLine($"Spread,${bestAsk - bestBid:F2}");
                    writer.WriteLine($"Total Volume,{totalVolume}");
                    writer.WriteLine($"Orders Processed,{metrics.OrdersProcessed}");
                    writer.WriteLine($"Total Time (ms),{metrics.ElapsedTimeMs}");
                    writer.WriteLine($"Throughput (orders/sec),{metrics.OrdersPerSecond:F2}");
                    writer.WriteLine($"Avg Latency (ms),{metrics.AvgLatencyMs:F3}");
                    writer.WriteLine($"Min Latency (ms),{metrics.MinLatencyMs:F3}");
                    writer.WriteLine($"Max Latency (ms),{metrics.MaxLatencyMs:F3}");
                }

                Logger.Info($"Exported statistics to {filepath}");
                return filepath;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to export statistics to CSV", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the export directory path.
        /// </summary>
        public string GetExportDirectory() => _exportDirectory;

        /// <summary>
        /// Gets all exported files.
        /// </summary>
        public List<FileInfo> GetExportedFiles()
        {
            try
            {
                var directory = new DirectoryInfo(_exportDirectory);
                return directory.GetFiles("*.csv").OrderByDescending(f => f.CreationTime).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to retrieve exported files", ex);
                return new List<FileInfo>();
            }
        }

        /// <summary>
        /// Clears old export files (older than specified days).
        /// </summary>
        public void CleanupOldExports(int daysToKeep = 7)
        {
            try
            {
                var directory = new DirectoryInfo(_exportDirectory);
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var file in directory.GetFiles("*.csv"))
                {
                    if (file.CreationTime < cutoffDate)
                    {
                        file.Delete();
                        Logger.Info($"Deleted old export file: {file.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to cleanup old exports", ex);
            }
        }
    }
}
