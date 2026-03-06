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
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
            
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero", nameof(price));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Symbol = symbol.ToUpper();
            Side = side;
            Price = price;
            Quantity = quantity;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss.fff}] {Side} {Quantity} {Symbol} @ ${Price:F2}";
        }
    }
}
