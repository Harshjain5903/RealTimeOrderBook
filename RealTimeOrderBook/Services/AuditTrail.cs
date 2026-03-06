// Author: Harsh Jain
// Real-Time Order Book - Event auditing for compliance and debugging

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RealTimeOrderBook.Services
{
    public enum AuditEventType
    {
        OrderReceived,
        OrderProcessed,
        PriceUpdate,
        SimulationStarted,
        SimulationStopped,
        ConfigurationChanged,
        DataExported,
        ErrorOccurred
    }

    /// <summary>
    /// Represents a single audit event.
    /// </summary>
    public class AuditEvent
    {
        public DateTime Timestamp { get; set; }
        public AuditEventType EventType { get; set; }
        public string Source { get; set; }
        public string Details { get; set; }
        public string? ErrorMessage { get; set; }

        public AuditEvent(AuditEventType type, string source, string details)
        {
            Timestamp = DateTime.Now;
            EventType = type;
            Source = source;
            Details = details;
        }

        public override string ToString()
        {
            var msg = $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {EventType} | {Source} | {Details}";
            if (!string.IsNullOrEmpty(ErrorMessage))
                msg += $" | Error: {ErrorMessage}";
            return msg;
        }
    }

    /// <summary>
    /// Audit trail system for compliance, debugging, and forensic analysis.
    /// Thread-safe with optional file persistence.
    /// </summary>
    public class AuditTrail
    {
        private readonly object _lock = new object();
        private readonly List<AuditEvent> _events = new List<AuditEvent>();
        private readonly string _auditLogPath;
        private readonly bool _persistToFile;
        private readonly int _maxEventsInMemory;

        public int EventCount => _events.Count;

        public AuditTrail(bool persistToFile = true, int maxEventsInMemory = 5000)
        {
            _persistToFile = persistToFile;
            _maxEventsInMemory = maxEventsInMemory;
            
            if (_persistToFile)
            {
                var auditDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "RealTimeOrderBook",
                    "Audit"
                );
                
                try
                {
                    if (!Directory.Exists(auditDir))
                        Directory.CreateDirectory(auditDir);
                    
                    _auditLogPath = Path.Combine(auditDir, $"audit_{DateTime.Now:yyyy-MM-dd_HHmm}.log");
                    Logger.Info($"Audit trail initialized: {_auditLogPath}");
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to initialize audit trail file", ex);
                    _persistToFile = false;
                }
            }
        }

        /// <summary>
        /// Records an audit event.
        /// </summary>
        public void RecordEvent(AuditEventType type, string source, string details, string? errorMsg = null)
        {
            var evt = new AuditEvent(type, source, details) { ErrorMessage = errorMsg };
            
            lock (_lock)
            {
                _events.Add(evt);

                // Trim old events if exceeding max
                if (_events.Count > _maxEventsInMemory)
                {
                    _events.RemoveRange(0, _events.Count - _maxEventsInMemory);
                }
            }

            // Persist to file
            if (_persistToFile)
            {
                try
                {
                    File.AppendAllText(_auditLogPath, evt.ToString() + Environment.NewLine);
                }
                catch { /* Silently fail */ }
            }
        }

        /// <summary>
        /// Gets events of specific type.
        /// </summary>
        public List<AuditEvent> GetEventsByType(AuditEventType type)
        {
            lock (_lock)
            {
                return _events.Where(e => e.EventType == type).ToList();
            }
        }

        /// <summary>
        /// Gets recent events.
        /// </summary>
        public List<AuditEvent> GetRecentEvents(int count = 100)
        {
            lock (_lock)
            {
                return _events.TakeLast(count).ToList();
            }
        }

        /// <summary>
        /// Gets events in time range.
        /// </summary>
        public List<AuditEvent> GetEventsByTimeRange(DateTime startTime, DateTime endTime)
        {
            lock (_lock)
            {
                return _events
                    .Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets audit summary.
        /// </summary>
        public string GetSummary()
        {
            lock (_lock)
            {
                if (_events.Count == 0)
                    return "No audit events recorded";

                var grouped = _events.GroupBy(e => e.EventType);
                var sb = new StringBuilder();
                sb.AppendLine("=== Audit Trail Summary ===");
                sb.AppendLine($"Total Events: {_events.Count}");
                
                foreach (var group in grouped)
                {
                    sb.AppendLine($"{group.Key}: {group.Count()}");
                }
                
                sb.AppendLine($"Time Span: {_events.First().Timestamp:yyyy-MM-dd HH:mm:ss} to {_events.Last().Timestamp:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("===========================");
                
                return sb.ToString();
            }
        }

        /// <summary>
        /// Exports audit trail to CSV.
        /// </summary>
        public string ExportToCsv(string? filename = null)
        {
            lock (_lock)
            {
                filename ??= $"audit_export_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
                var exportPath = Path.Combine(
                    Path.GetDirectoryName(_auditLogPath) ?? "",
                    filename
                );

                try
                {
                    using (var writer = new StreamWriter(exportPath, false, Encoding.UTF8))
                    {
                        writer.WriteLine("Timestamp,EventType,Source,Details,ErrorMessage");
                        
                        foreach (var evt in _events)
                        {
                            var line = $"\"{evt.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\",{evt.EventType},{evt.Source},\"{evt.Details}\",\"{evt.ErrorMessage ?? ""}\"";
                            writer.WriteLine(line);
                        }
                    }
                    
                    Logger.Info($"Audit trail exported to {exportPath}");
                    return exportPath;
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to export audit trail", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Clears all audit events.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _events.Clear();
                Logger.Info("Audit trail cleared");
            }
        }
    }
}
