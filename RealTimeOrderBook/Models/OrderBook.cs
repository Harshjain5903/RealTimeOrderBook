// Author: Harsh Jain
// Real-Time Order Book - Thread-Safe OrderBook Implementation

using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeOrderBook.Models
{
    /// <summary>
    /// Thread-safe order book implementation for managing bid/ask orders.
    /// Maintains best bid, best ask, spread calculation, and volume tracking.
    /// </summary>
    public class OrderBook
    {
        private const int MaxOrdersPerSide = 100;
        
        private readonly object _lock = new object();
        private readonly List<Order> _buyOrders = new List<Order>();
        private readonly List<Order> _sellOrders = new List<Order>();

        public string Symbol { get; }
        public decimal BestBid { get; private set; }
        public decimal BestAsk { get; private set; }
        public decimal Spread => BestAsk - BestBid;
        public long TotalVolume { get; private set; }

        /// <summary>
        /// Initializes a new OrderBook for the specified symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol (e.g., AAPL, MSFT)</param>
        /// <exception cref="ArgumentException">Thrown when symbol is null or empty</exception>
        public OrderBook(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
            
            Symbol = symbol.ToUpper();
            BestBid = 0;
            BestAsk = 0;
            TotalVolume = 0;
        }

        /// <summary>
        /// Adds an order to the book and updates best bid/ask prices.
        /// Thread-safe operation using lock-based synchronization.
        /// </summary>
        /// <param name="order">Order to add to the book</param>
        /// <exception cref="ArgumentNullException">Thrown when order is null</exception>
        public void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "Order cannot be null");

            lock (_lock)
            {
                try
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
                    if (_buyOrders.Count > MaxOrdersPerSide)
                    {
                        _buyOrders.RemoveRange(MaxOrdersPerSide, _buyOrders.Count - MaxOrdersPerSide);
                    }
                    if (_sellOrders.Count > MaxOrdersPerSide)
                    {
                        _sellOrders.RemoveRange(MaxOrdersPerSide, _sellOrders.Count - MaxOrdersPerSide);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error adding order to book: {order.Symbol}", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves current market data in a thread-safe manner.
        /// </summary>
        /// <returns>Tuple containing bid, ask, and spread values</returns>
        public (decimal bid, decimal ask, decimal spread) GetMarketData()
        {
            lock (_lock)
            {
                return (BestBid, BestAsk, Spread);
            }
        }

        /// <summary>
        /// Resets the order book to initial state, clearing all orders and prices.
        /// </summary>
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
