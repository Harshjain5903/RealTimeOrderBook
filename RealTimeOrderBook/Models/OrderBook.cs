// Author: Harsh Jain
// Real-Time Order Book - Thread-Safe OrderBook Implementation

using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeOrderBook.Models
{
    public class OrderBook
    {
        private readonly object _lock = new object();
        private readonly List<Order> _buyOrders = new List<Order>();
        private readonly List<Order> _sellOrders = new List<Order>();

        public string Symbol { get; }
        public decimal BestBid { get; private set; }
        public decimal BestAsk { get; private set; }
        public decimal Spread => BestAsk - BestBid;
        public long TotalVolume { get; private set; }

        public OrderBook(string symbol)
        {
            Symbol = symbol;
            BestBid = 0;
            BestAsk = 0;
            TotalVolume = 0;
        }

        public void AddOrder(Order order)
        {
            lock (_lock)
            {
                if (order.Side == OrderSide.Buy)
                {
                    _buyOrders.Add(order);
                    _buyOrders.Sort((a, b) => b.Price.CompareTo(a.Price)); // Descending
                    if (_buyOrders.Any())
                    {
                        BestBid = _buyOrders.First().Price;
                    }
                }
                else
                {
                    _sellOrders.Add(order);
                    _sellOrders.Sort((a, b) => a.Price.CompareTo(b.Price)); // Ascending
                    if (_sellOrders.Any())
                    {
                        BestAsk = _sellOrders.First().Price;
                    }
                }

                TotalVolume += order.Quantity;

                // Keep only recent orders (prevent memory growth)
                if (_buyOrders.Count > 100)
                {
                    _buyOrders.RemoveRange(100, _buyOrders.Count - 100);
                }
                if (_sellOrders.Count > 100)
                {
                    _sellOrders.RemoveRange(100, _sellOrders.Count - 100);
                }
            }
        }

        public (decimal bid, decimal ask, decimal spread) GetMarketData()
        {
            lock (_lock)
            {
                return (BestBid, BestAsk, Spread);
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _buyOrders.Clear();
                _sellOrders.Clear();
                BestBid = 0;
                BestAsk = 0;
                TotalVolume = 0;
            }
        }
    }
}
