// Author: Harsh Jain
// Real-Time Order Book - Logging Service for diagnostics and monitoring

using System;
using System.IO;

namespace RealTimeOrderBook.Services
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Singleton logger for application-wide diagnostics and event tracking.
    /// Writes logs to both console and file for persistent records.
    /// Thread-safe implementation for concurrent logging from multiple threads.
    /// </summary>
    public static class Logger
    {
        private static readonly object _lockObject = new object();
        private static readonly string _logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RealTimeOrderBook",
            "Logs"
        );
        private static readonly string _logFilePath = Path.Combine(_logDirectory, $"app_{DateTime.Now:yyyy-MM-dd_HHmm}.log");
        private static readonly bool _enableFileLogging = true;

        static Logger()
        {
            try
            {
                if (_enableFileLogging && !Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create log directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a message at the specified level with timestamp.
        /// </summary>
        public static void Log(LogLevel level, string message, Exception? exception = null)
        {
            lock (_lockObject)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logMessage = $"[{timestamp}] [{level.ToString().ToUpper()}] {message}";

                if (exception != null)
                {
                    logMessage += Environment.NewLine + $"Exception: {exception.GetType().Name} - {exception.Message}" +
                        Environment.NewLine + $"Stack Trace: {exception.StackTrace}";
                }

                // Console output
                Console.WriteLine(logMessage);

                // File output
                if (_enableFileLogging)
                {
                    try
                    {
                        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
                    }
                    catch
                    {
                        // Silently fail if file logging fails
                    }
                }
            }
        }

        public static void Debug(string message) => Log(LogLevel.Debug, message);
        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Warning(string message) => Log(LogLevel.Warning, message);
        public static void Error(string message, Exception? ex = null) => Log(LogLevel.Error, message, ex);
        public static void Critical(string message, Exception? ex = null) => Log(LogLevel.Critical, message, ex);
    }
}
