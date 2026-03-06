// Author: Harsh Jain
// Real-Time Order Book - Configuration management system

using System;
using System.IO;
using System.Text.Json;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Configuration settings for the order book simulator.
    /// Supports JSON-based configuration file persistence.
    /// </summary>
    public class SimulationConfig
    {
        public int UpdateIntervalMs { get; set; } = 200;
        public decimal InitialPrice { get; set; } = 150.00m;
        public decimal PriceVolatility { get; set; } = 0.50m;
        public decimal MinPrice { get; set; } = 100.00m;
        public decimal MaxPrice { get; set; } = 200.00m;
        public int MinOrderQuantity { get; set; } = 100;
        public int MaxOrderQuantity { get; set; } = 1000;
        public int MaxOrdersPerSide { get; set; } = 100;
        public bool EnableLogging { get; set; } = true;
        public bool EnableFileLogging { get; set; } = true;
    }

    /// <summary>
    /// Manages application configuration with file persistence and validation.
    /// Provides singleton access to configuration throughout the application.
    /// </summary>
    public static class ConfigurationManager
    {
        private static SimulationConfig? _config;
        private static readonly object _lockObject = new object();
        private static readonly string _configDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RealTimeOrderBook"
        );
        private static readonly string _configFilePath = Path.Combine(_configDirectory, "config.json");

        public static SimulationConfig Config
        {
            get
            {
                if (_config == null)
                {
                    lock (_lockObject)
                    {
                        if (_config == null)
                        {
                            _config = LoadConfiguration();
                        }
                    }
                }
                return _config;
            }
        }

        private static SimulationConfig LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    var jsonContent = File.ReadAllText(_configFilePath);
                    var config = JsonSerializer.Deserialize<SimulationConfig>(jsonContent);
                    Logger.Info("Configuration loaded from file");
                    return config ?? new SimulationConfig();
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to load configuration: {ex.Message}. Using defaults.");
            }

            // Return and save default configuration
            var defaultConfig = new SimulationConfig();
            SaveConfiguration(defaultConfig);
            return defaultConfig;
        }

        public static void SaveConfiguration(SimulationConfig config)
        {
            try
            {
                if (!Directory.Exists(_configDirectory))
                {
                    Directory.CreateDirectory(_configDirectory);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonContent = JsonSerializer.Serialize(config, options);
                File.WriteAllText(_configFilePath, jsonContent);
                Logger.Info("Configuration saved to file");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save configuration", ex);
            }
        }

        public static void ResetToDefaults()
        {
            lock (_lockObject)
            {
                _config = new SimulationConfig();
                SaveConfiguration(_config);
                Logger.Info("Configuration reset to defaults");
            }
        }

        public static void ValidateConfiguration()
        {
            var config = Config;
            
            if (config.UpdateIntervalMs <= 0)
                throw new InvalidOperationException("UpdateIntervalMs must be greater than 0");
            
            if (config.MinPrice >= config.MaxPrice)
                throw new InvalidOperationException("MinPrice must be less than MaxPrice");
            
            if (config.MinOrderQuantity > config.MaxOrderQuantity)
                throw new InvalidOperationException("MinOrderQuantity must be less than MaxOrderQuantity");
            
            if (config.PriceVolatility < 0)
                throw new InvalidOperationException("PriceVolatility must be non-negative");

            Logger.Info("Configuration validation passed");
        }
    }
}
