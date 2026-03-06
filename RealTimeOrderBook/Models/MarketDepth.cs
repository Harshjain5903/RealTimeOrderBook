// Author: Harsh Jain
// Real-Time Order Book - Price level aggregation for market depth analysis

using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeOrderBook.Models
{
    /// <summary>
    /// Represents aggregated order data at a specific price level.
    /// Used for market depth visualization and analysis.
    /// </summary>
    public class PriceLevel
    {
        public decimal Price { get; set; }
        public long AggregatedVolume { get; set; }
        public int OrderCount { get; set; }

        public PriceLevel(decimal price, long volume, int orderCount)
        {
            Price = price;
            AggregatedVolume = volume;
            OrderCount = orderCount;
        }

        public override string ToString()
        {
            return $"Price: ${Price:F2}, Volume: {AggregatedVolume}, Orders: {OrderCount}";
        }
    }

    /// <summary>
    /// Market depth aggregator that groups orders by price level.
    /// Provides efficient depth-of-market (DOM) analysis.
    /// Thread-safe implementation for concurrent access.
    /// </summary>
    public class MarketDepth
    {
        private readonly object _lock = new object();
        private readonly Dictionary<decimal, (long volume, int count)> _bidLevels = new();
        private readonly Dictionary<decimal, (long volume, int count)> _askLevels = new();

        public string Symbol { get; }
        public int BidLevels => _bidLevels.Count;
        public int AskLevels => _askLevels.Count;

        public MarketDepth(string symbol)
        {
            Symbol = symbol;
        }

        public void UpdateFromOrderBook(OrderBook orderBook)
        {
            var (bid, ask, _) = orderBook.GetMarketData();
            
            // Note: In a production system, you would have access to the internal order lists
            // For now, this demonstrates the architecture
            Logger.Debug($"Market depth updated - Bid: ${bid:F2}, Ask: ${ask:F2}");
        }

        public List<PriceLevel> GetBidDepth(int maxLevels = 10)
        {
            lock (_lock)
            {
                return _bidLevels
                    .OrderByDescending(x => x.Key)
                    .Take(maxLevels)
                    .Select(x => new PriceLevel(x.Key, x.Value.volume, x.Value.count))
                    .ToList();
            }
        }

        public List<PriceLevel> GetAskDepth(int maxLevels = 10)
        {
            lock (_lock)
            {
                return _askLevels
                    .OrderBy(x => x.Key)
                    .Take(maxLevels)
                    .Select(x => new PriceLevel(x.Key, x.Value.volume, x.Value.count))
                    .ToList();
            }
        }

        public void AddOrder(Order order)
        {
            lock (_lock)
            {
                if (order.Side == OrderSide.Buy)
                {
                    if (_bidLevels.ContainsKey(order.Price))
                    {
                        var (volume, count) = _bidLevels[order.Price];
                        _bidLevels[order.Price] = (volume + order.Quantity, count + 1);
                    }
                    else
                    {
                        _bidLevels[order.Price] = (order.Quantity, 1);
                    }
                }
                else
                {
                    if (_askLevels.ContainsKey(order.Price))
                    {
                        var (volume, count) = _askLevels[order.Price];
                        _askLevels[order.Price] = (volume + order.Quantity, count + 1);
                    }
                    else
                    {
                        _askLevels[order.Price] = (order.Quantity, 1);
                    }
                }
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _bidLevels.Clear();
                _askLevels.Clear();
            }
        }
    }
}
