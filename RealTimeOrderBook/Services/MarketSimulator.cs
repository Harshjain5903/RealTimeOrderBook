using System;
using System.Threading;
using System.Threading.Tasks;
using RealTimeOrderBook.Models;

namespace RealTimeOrderBook.Services
{
    public class MarketSimulator
    {
        private readonly Random _random = new Random();
        private decimal _currentPrice = 150.00m;
        private const decimal PriceVolatility = 0.50m;
        private const int UpdateIntervalMs = 200;

        public event EventHandler<Order>? OrderGenerated;

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
            if (_currentPrice < 100) _currentPrice = 100;
            if (_currentPrice > 200) _currentPrice = 200;

            // Generate buy order (slightly below current price)
            var buyPrice = _currentPrice - (decimal)(_random.NextDouble() * 0.5);
            var buyQuantity = _random.Next(100, 1000);
            var buyOrder = new Order(symbol, OrderSide.Buy, Math.Round(buyPrice, 2), buyQuantity);
            OrderGenerated?.Invoke(this, buyOrder);

            // Generate sell order (slightly above current price)
            var sellPrice = _currentPrice + (decimal)(_random.NextDouble() * 0.5);
            var sellQuantity = _random.Next(100, 1000);
            var sellOrder = new Order(symbol, OrderSide.Sell, Math.Round(sellPrice, 2), sellQuantity);
            OrderGenerated?.Invoke(this, sellOrder);
        }
    }
}
