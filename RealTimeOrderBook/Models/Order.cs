// Author: Harsh Jain
// Real-Time Order Book - Order Model

using System;

namespace RealTimeOrderBook.Models
{
    public enum OrderSide
    {
        Buy,
        Sell
    }

    public class Order
    {
        public string Symbol { get; set; }
        public OrderSide Side { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime Timestamp { get; set; }

        public Order(string symbol, OrderSide side, decimal price, int quantity)
        {
            Symbol = symbol;
            Side = side;
            Price = price;
            Quantity = quantity;
            Timestamp = DateTime.Now;
        }
    }
}
