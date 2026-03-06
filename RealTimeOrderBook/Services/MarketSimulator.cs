// Author: Harsh Jain
// Real-Time Order Book - Async Market Data Simulator

using System;
using System.Threading;
using System.Threading.Tasks;
using RealTimeOrderBook.Models;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Simulates real-time market data by generating random bid/ask orders.
    /// Uses async Task-based execution for non-blocking operation.
    /// </summary>
    public class MarketSimulator
    {
        private readonly Random _random = new Random();
        private decimal _currentPrice;

        public MarketSimulator()
        {
            _currentPrice = ConfigurationManager.Config.InitialPrice;
        }

        /// <summary>
        /// Event raised when a new order is generated.
        /// Subscribers should handle UI marshaling if updating UI elements.
        /// </summary>
        public event EventHandler<Order>? OrderGenerated;

        /// <summary>
        /// Starts the market simulation loop on a background task.
        /// Generates bid/ask orders at regular intervals until cancelled.
        /// </summary>
        /// <param name="symbol">Trading symbol to simulate</param>
        /// <param name="cancellationToken">Token to stop simulation</param>
        public async Task StartSimulationAsync(string symbol, CancellationToken cancellationToken)
        {
            try
            {
                ConfigurationManager.ValidateConfiguration();
                Logger.Info($"Market simulation started for symbol: {symbol}");
                int orderCount = 0;
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    GenerateRandomOrders(symbol);
                    orderCount += 2;
                    
                    if (orderCount % 100 == 0)
                    {
                        Logger.Debug($"Generated {orderCount} orders, current price: ${_currentPrice:F2}");
                    }
                    
                    await Task.Delay(ConfigurationManager.Config.UpdateIntervalMs, cancellationToken);
                }
                
                Logger.Info($"Market simulation stopped. Total orders generated: {orderCount}");
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("Simulation cancelled by user");
            }
            catch (Exception ex)
            {
                Logger.Error("Error in market simulation", ex);
            }
        }

        private void GenerateRandomOrders(string symbol)
        {
            var config = ConfigurationManager.Config;
            
            // Simulate price movement (random walk)
            var priceChange = (decimal)(_random.NextDouble() - 0.5) * config.PriceVolatility;
            _currentPrice += priceChange;

            // Keep price in reasonable range
            if (_currentPrice < config.MinPrice) _currentPrice = config.MinPrice;
            if (_currentPrice > config.MaxPrice) _currentPrice = config.MaxPrice;

            // Generate buy order (slightly below current price)
            var buyPrice = _currentPrice - (decimal)(_random.NextDouble() * 0.5);
            var buyQuantity = _random.Next(config.MinOrderQuantity, config.MaxOrderQuantity);
            var buyOrder = new Order(symbol, OrderSide.Buy, Math.Round(buyPrice, 2), buyQuantity);
            OrderGenerated?.Invoke(this, buyOrder);

            // Generate sell order (slightly above current price)
            var sellPrice = _currentPrice + (decimal)(_random.NextDouble() * 0.5);
            var sellQuantity = _random.Next(config.MinOrderQuantity, config.MaxOrderQuantity);
            var sellOrder = new Order(symbol, OrderSide.Sell, Math.Round(sellPrice, 2), sellQuantity);
            OrderGenerated?.Invoke(this, sellOrder);
        }
    }
}
