// Author: Harsh Jain
// Real-Time Order Book - Order history management with filtering capabilities

using System;
using System.Collections.Generic;
using System.Linq;
using RealTimeOrderBook.Models;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Filter criteria for order history queries.
    /// </summary>
    public class OrderHistoryFilter
    {
        public OrderSide? Side { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinQuantity { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// Manages complete order history with filtering, search, and analytics capabilities.
    /// Thread-safe implementation for concurrent access and updates.
    /// </summary>
    public class OrderHistory
    {
        private readonly object _lock = new object();
        private readonly List<Order> _allOrders = new List<Order>();
        private readonly int _maxHistorySize;

        public string Symbol { get; }
        public int TotalOrdersProcessed => _allOrders.Count;

        public OrderHistory(string symbol, int maxHistorySize = 10000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
            
            if (maxHistorySize <= 0)
                throw new ArgumentException("Max history size must be positive", nameof(maxHistorySize));

            Symbol = symbol.ToUpper();
            _maxHistorySize = maxHistorySize;
        }

        /// <summary>
        /// Adds an order to history and maintains size limit.
        /// </summary>
        public void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "Order cannot be null");

            lock (_lock)
            {
                _allOrders.Add(order);

                // Maintain history size limit - remove oldest orders
                if (_allOrders.Count > _maxHistorySize)
                {
                    _allOrders.RemoveRange(0, _allOrders.Count - _maxHistorySize);
                    Logger.Debug($"Order history trimmed to {_maxHistorySize} entries");
                }
            }
        }

        /// <summary>
        /// Filters orders based on provided criteria.
        /// </summary>
        public List<Order> FilterOrders(OrderHistoryFilter? filter = null)
        {
            lock (_lock)
            {
                var query = _allOrders.AsEnumerable();

                if (filter != null)
                {
                    if (filter.Side.HasValue)
                        query = query.Where(o => o.Side == filter.Side.Value);

                    if (filter.MinPrice.HasValue)
                        query = query.Where(o => o.Price >= filter.MinPrice.Value);

                    if (filter.MaxPrice.HasValue)
                        query = query.Where(o => o.Price <= filter.MaxPrice.Value);

                    if (filter.MinQuantity.HasValue)
                        query = query.Where(o => o.Quantity >= filter.MinQuantity.Value);

                    if (filter.StartTime.HasValue)
                        query = query.Where(o => o.Timestamp >= filter.StartTime.Value);

                    if (filter.EndTime.HasValue)
                        query = query.Where(o => o.Timestamp <= filter.EndTime.Value);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets recent orders up to specified count.
        /// </summary>
        public List<Order> GetRecentOrders(int count = 100)
        {
            lock (_lock)
            {
                return _allOrders.TakeLast(count).ToList();
            }
        }

        /// <summary>
        /// Gets buy orders by price level.
        /// </summary>
        public Dictionary<decimal, int> GetBuyOrdersByPrice()
        {
            lock (_lock)
            {
                return _allOrders
                    .Where(o => o.Side == OrderSide.Buy)
                    .GroupBy(o => o.Price)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
        }

        /// <summary>
        /// Gets sell orders by price level.
        /// </summary>
        public Dictionary<decimal, int> GetSellOrdersByPrice()
        {
            lock (_lock)
            {
                return _allOrders
                    .Where(o => o.Side == OrderSide.Sell)
                    .GroupBy(o => o.Price)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
        }

        /// <summary>
        /// Gets statistics about order history.
        /// </summary>
        public string GetStatistics()
        {
            lock (_lock)
            {
                if (_allOrders.Count == 0)
                    return "No orders in history";

                var buyOrders = _allOrders.Count(o => o.Side == OrderSide.Buy);
                var sellOrders = _allOrders.Count(o => o.Side == OrderSide.Sell);
                var avgPrice = _allOrders.Average(o => o.Price);
                var avgQuantity = _allOrders.Average(o => o.Quantity);

                return $@"=== Order History Statistics ({Symbol}) ===
Total Orders: {_allOrders.Count}
Buy Orders: {buyOrders}
Sell Orders: {sellOrders}
Avg Price: ${avgPrice:F2}
Avg Quantity: {avgQuantity:F0}
Price Range: ${_allOrders.Min(o => o.Price):F2} - ${_allOrders.Max(o => o.Price):F2}
Oldest: {_allOrders.First().Timestamp:yyyy-MM-dd HH:mm:ss}
Newest: {_allOrders.Last().Timestamp:yyyy-MM-dd HH:mm:ss}
=========================================";
            }
        }

        /// <summary>
        /// Clears all history.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _allOrders.Clear();
                Logger.Info("Order history cleared");
            }
        }
    }
}
