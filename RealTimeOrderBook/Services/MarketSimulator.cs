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
        private const decimal InitialPrice = 150.00m;
        private const decimal PriceVolatility = 0.50m;
        private const int UpdateIntervalMs = 200;
        private const decimal MinPrice = 100.00m;
        private const decimal MaxPrice = 200.00m;
        private const int MinOrderQuantity = 100;
        private const int MaxOrderQuantity = 1000;
        
        private readonly Random _random = new Random();
        private decimal _currentPrice = InitialPrice;

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
                while (!cancellationToken.IsCancellationRequested)
                {
                    GenerateRandomOrders(symbol);
                    await Task.Delay(UpdateIntervalMs, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        private void GenerateRandomOrders(string symbol)
        {
            // Simulate price movement (random walk)
            var priceChange = (decimal)(_random.NextDouble() - 0.5) * PriceVolatility;
            _currentPrice += priceChange;

            // Keep price in reasonable range
            if (_currentPrice < MinPrice) _currentPrice = MinPrice;
            if (_currentPrice > MaxPrice) _currentPrice = MaxPrice;

            // Generate buy order (slightly below current price)
            var buyPrice = _currentPrice - (decimal)(_random.NextDouble() * 0.5);
            var buyQuantity = _random.Next(MinOrderQuantity, MaxOrderQuantity);
            var buyOrder = new Order(symbol, OrderSide.Buy, Math.Round(buyPrice, 2), buyQuantity);
            OrderGenerated?.Invoke(this, buyOrder);

            // Generate sell order (slightly above current price)
            var sellPrice = _currentPrice + (decimal)(_random.NextDouble() * 0.5);
            var sellQuantity = _random.Next(MinOrderQuantity, MaxOrderQuantity);
            var sellOrder = new Order(symbol, OrderSide.Sell, Math.Round(sellPrice, 2), sellQuantity);
            OrderGenerated?.Invoke(this, sellOrder);
        }
    }
}
