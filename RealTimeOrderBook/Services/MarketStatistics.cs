// Author: Harsh Jain
// Real-Time Order Book - Market Statistics tracking and analysis

using System;
using System.Collections.Generic;

namespace RealTimeOrderBook.Services
{
    /// <summary>
    /// Tracks market statistics including VWAP, volatility, and trade patterns.
    /// Used for market analysis and trading strategy development.
    /// </summary>
    public class MarketStatistics
    {
        private readonly List<decimal> _prices = new List<decimal>();
        private readonly List<long> _volumes = new List<long>();
        private decimal _totalValue = 0;
        private decimal _priceHigh = decimal.MinValue;
        private decimal _priceLow = decimal.MaxValue;
        private decimal _priceOpen = 0;
        private decimal _priceClose = 0;

        public string Symbol { get; }
        public int TradeCount { get; private set; }
        public long TotalVolume { get; private set; }
        public decimal VolumeWeightedAveragePrice { get; private set; }
        public decimal PriceHigh => _priceHigh;
        public decimal PriceLow => _priceLow;
        public decimal PriceOpen => _priceOpen;
        public decimal PriceClose => _priceClose;
        public decimal Volatility { get; private set; }

        public MarketStatistics(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
            
            Symbol = symbol.ToUpper();
        }

        public void UpdateTrade(decimal price, long volume)
        {
            if (price <= 0)
                throw new ArgumentException("Price must be positive", nameof(price));
            
            if (volume <= 0)
                throw new ArgumentException("Volume must be positive", nameof(volume));

            if (TradeCount == 0)
                _priceOpen = price;

            _prices.Add(price);
            _volumes.Add(volume);
            _totalValue += price * volume;
            TotalVolume += volume;
            TradeCount++;
            _priceClose = price;

            // Update high/low
            if (price > _priceHigh) _priceHigh = price;
            if (price < _priceLow) _priceLow = price;

            // Calculate VWAP
            if (TotalVolume > 0)
                VolumeWeightedAveragePrice = _totalValue / TotalVolume;

            // Calculate volatility
            CalculateVolatility();
        }

        private void CalculateVolatility()
        {
            if (_prices.Count < 2)
            {
                Volatility = 0;
                return;
            }

            decimal mean = VolumeWeightedAveragePrice;
            decimal sumSquaredDev = 0;

            foreach (var price in _prices)
            {
                var dev = price - mean;
                sumSquaredDev += dev * dev;
            }

            decimal variance = sumSquaredDev / _prices.Count;
            Volatility = (decimal)Math.Sqrt((double)variance);
        }

        public decimal GetPriceRange() => _priceHigh - _priceLow;

        public decimal GetPriceChangePercent()
        {
            if (_priceOpen == 0) return 0;
            return ((_priceClose - _priceOpen) / _priceOpen) * 100;
        }

        public string GetSummary()
        {
            return $@"=== Market Statistics ({Symbol}) ===
Trades: {TradeCount}
Volume: {TotalVolume}
VWAP: ${VolumeWeightedAveragePrice:F2}
High: ${PriceHigh:F2}
Low: ${PriceLow:F2}
Range: ${GetPriceRange():F2}
Open: ${PriceOpen:F2}
Close: ${PriceClose:F2}
Change: {GetPriceChangePercent():F2}%
Volatility: {Volatility:F4}
====================================";
        }

        public void Reset()
        {
            _prices.Clear();
            _volumes.Clear();
            _totalValue = 0;
            _priceHigh = decimal.MinValue;
            _priceLow = decimal.MaxValue;
            _priceOpen = 0;
            _priceClose = 0;
            TradeCount = 0;
            TotalVolume = 0;
            VolumeWeightedAveragePrice = 0;
            Volatility = 0;
        }
    }
}
