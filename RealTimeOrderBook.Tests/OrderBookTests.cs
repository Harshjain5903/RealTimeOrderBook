// Author: Harsh Jain
// Real-Time Order Book - NUnit Test Suite

using NUnit.Framework;
using RealTimeOrderBook.Models;

namespace RealTimeOrderBook.Tests
{
    [TestFixture]
    public class OrderBookTests
    {
        private OrderBook? _orderBook;

        [SetUp]
        public void Setup()
        {
            _orderBook = new OrderBook("AAPL");
        }

        [Test]
        public void OrderBook_InitialState_ShouldHaveZeroValues()
        {
            // Assert
            Assert.That(_orderBook!.Symbol, Is.EqualTo("AAPL"));
            Assert.That(_orderBook.BestBid, Is.EqualTo(0));
            Assert.That(_orderBook.BestAsk, Is.EqualTo(0));
            Assert.That(_orderBook.Spread, Is.EqualTo(0));
            Assert.That(_orderBook.TotalVolume, Is.EqualTo(0));
        }

        [Test]
        public void AddOrder_BuyOrder_ShouldUpdateBestBid()
        {
            // Arrange
            var buyOrder = new Order("AAPL", OrderSide.Buy, 150.50m, 100);

            // Act
            _orderBook!.AddOrder(buyOrder);

            // Assert
            Assert.That(_orderBook.BestBid, Is.EqualTo(150.50m));
            Assert.That(_orderBook.TotalVolume, Is.EqualTo(100));
        }

        [Test]
        public void AddOrder_SellOrder_ShouldUpdateBestAsk()
        {
            // Arrange
            var sellOrder = new Order("AAPL", OrderSide.Sell, 151.00m, 200);

            // Act
            _orderBook!.AddOrder(sellOrder);

            // Assert
            Assert.That(_orderBook.BestAsk, Is.EqualTo(151.00m));
            Assert.That(_orderBook.TotalVolume, Is.EqualTo(200));
        }

        [Test]
        public void AddOrder_MultipleBuyOrders_ShouldKeepHighestBid()
        {
            // Arrange
            var buyOrder1 = new Order("AAPL", OrderSide.Buy, 150.00m, 100);
            var buyOrder2 = new Order("AAPL", OrderSide.Buy, 151.00m, 150);
            var buyOrder3 = new Order("AAPL", OrderSide.Buy, 149.50m, 200);

            // Act
            _orderBook!.AddOrder(buyOrder1);
            _orderBook.AddOrder(buyOrder2);
            _orderBook.AddOrder(buyOrder3);

            // Assert
            Assert.That(_orderBook.BestBid, Is.EqualTo(151.00m));
            Assert.That(_orderBook.TotalVolume, Is.EqualTo(450));
        }

        [Test]
        public void AddOrder_MultipleSellOrders_ShouldKeepLowestAsk()
        {
            // Arrange
            var sellOrder1 = new Order("AAPL", OrderSide.Sell, 152.00m, 100);
            var sellOrder2 = new Order("AAPL", OrderSide.Sell, 151.50m, 150);
            var sellOrder3 = new Order("AAPL", OrderSide.Sell, 153.00m, 200);

            // Act
            _orderBook!.AddOrder(sellOrder1);
            _orderBook.AddOrder(sellOrder2);
            _orderBook.AddOrder(sellOrder3);

            // Assert
            Assert.That(_orderBook.BestAsk, Is.EqualTo(151.50m));
            Assert.That(_orderBook.TotalVolume, Is.EqualTo(450));
        }

        [Test]
        public void Spread_WithBothBidAndAsk_ShouldCalculateCorrectly()
        {
            // Arrange
            var buyOrder = new Order("AAPL", OrderSide.Buy, 150.00m, 100);
            var sellOrder = new Order("AAPL", OrderSide.Sell, 151.00m, 100);

            // Act
            _orderBook!.AddOrder(buyOrder);
            _orderBook.AddOrder(sellOrder);

            // Assert
            Assert.That(_orderBook.BestBid, Is.EqualTo(150.00m));
            Assert.That(_orderBook.BestAsk, Is.EqualTo(151.00m));
            Assert.That(_orderBook.Spread, Is.EqualTo(1.00m));
        }

        [Test]
        public void GetMarketData_ShouldReturnCorrectTuple()
        {
            // Arrange
            var buyOrder = new Order("AAPL", OrderSide.Buy, 150.25m, 100);
            var sellOrder = new Order("AAPL", OrderSide.Sell, 150.75m, 100);
            _orderBook!.AddOrder(buyOrder);
            _orderBook.AddOrder(sellOrder);

            // Act
            var (bid, ask, spread) = _orderBook.GetMarketData();

            // Assert
            Assert.That(bid, Is.EqualTo(150.25m));
            Assert.That(ask, Is.EqualTo(150.75m));
            Assert.That(spread, Is.EqualTo(0.50m));
        }

        [Test]
        public void Reset_ShouldClearAllData()
        {
            // Arrange
            _orderBook!.AddOrder(new Order("AAPL", OrderSide.Buy, 150.00m, 100));
            _orderBook.AddOrder(new Order("AAPL", OrderSide.Sell, 151.00m, 100));

            // Act
            _orderBook.Reset();

            // Assert
            Assert.That(_orderBook.BestBid, Is.EqualTo(0));
            Assert.That(_orderBook.BestAsk, Is.EqualTo(0));
            Assert.That(_orderBook.Spread, Is.EqualTo(0));
            Assert.That(_orderBook.TotalVolume, Is.EqualTo(0));
        }

        [Test]
        public void VolumeAggregation_MultipleOrders_ShouldSumCorrectly()
        {
            // Arrange & Act
            _orderBook!.AddOrder(new Order("AAPL", OrderSide.Buy, 150.00m, 100));
            _orderBook.AddOrder(new Order("AAPL", OrderSide.Buy, 149.00m, 250));
            _orderBook.AddOrder(new Order("AAPL", OrderSide.Sell, 151.00m, 300));
            _orderBook.AddOrder(new Order("AAPL", OrderSide.Sell, 152.00m, 150));

            // Assert
            Assert.That(_orderBook.TotalVolume, Is.EqualTo(800));
        }

        [Test]
        public void Order_Creation_ShouldSetPropertiesCorrectly()
        {
            // Arrange & Act
            var order = new Order("AAPL", OrderSide.Buy, 150.50m, 500);

            // Assert
            Assert.That(order.Symbol, Is.EqualTo("AAPL"));
            Assert.That(order.Side, Is.EqualTo(OrderSide.Buy));
            Assert.That(order.Price, Is.EqualTo(150.50m));
            Assert.That(order.Quantity, Is.EqualTo(500));
            Assert.That(order.Timestamp, Is.Not.EqualTo(default(DateTime)));
        }
    }
}
