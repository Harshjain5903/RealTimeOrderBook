# Real-Time Order Book Desktop Application

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=c-sharp)
![WPF](https://img.shields.io/badge/WPF-Desktop-0078D4?style=flat-square)
![License](https://img.shields.io/badge/license-MIT-blue?style=flat-square)

A WPF desktop application that simulates a real-time stock order book with continuous bid/ask price updates. Built using modern C# async programming patterns and MVVM architecture.

## Overview

This application simulates a live trading order book similar to what you'd see in professional trading platforms. It continuously updates bid/ask prices, processes orders, and maintains accurate volume tracking in real-time.

**Key features:**
- Market data simulation with 200ms update intervals
- Thread-safe order book management
- Non-blocking UI using async/await patterns
- MVVM architecture with minimal code-behind
- Unit test coverage with NUnit

## Architecture

### Design Patterns & Technologies

**MVVM Pattern:**
- **Models:** `Order`, `OrderBook` - Business logic and data structures
- **ViewModels:** `MainViewModel` - Presentation logic with `INotifyPropertyChanged`
- **Views:** `MainWindow.xaml` - Pure XAML UI with data binding

**Async Programming:**
- Background `Task` execution for market simulation
- `CancellationToken` for graceful shutdown
- `Dispatcher` for thread-safe UI updates
- Non-blocking event-driven architecture

**Thread Safety:**
- Lock-based synchronization in OrderBook
- Proper UI thread marshaling
- Safe collection access patterns

### Project Structure

```
RealTimeOrderBook/
├── Models/
│   ├── Order.cs              # Order entity with buy/sell sides
│   └── OrderBook.cs          # Thread-safe order book with bid/ask tracking
├── ViewModels/
│   └── MainViewModel.cs      # MVVM presentation layer with INotifyPropertyChanged
├── Services/
│   └── MarketSimulator.cs    # Background task-based market data generator
├── Views/
│   └── MainWindow.xaml       # WPF UI with data bindings
├── App.xaml                  # Application entry point
└── RealTimeOrderBook.csproj

RealTimeOrderBook.Tests/
└── OrderBookTests.cs         # NUnit test suite for pricing logic
```

## Tech Stack

| Technology | Purpose |
|-----------|---------|
| **.NET 8** | Modern .NET platform with latest C# features |
| **WPF** | Rich desktop UI framework |
| **MVVM** | Separation of concerns architecture |
| **ObservableCollection** | Automatic UI updates via data binding |
| **async/await** | Non-blocking asynchronous operations |
| **Task Parallel Library** | Background market simulation |
| **NUnit** | Unit testing framework |
| **Dispatcher** | UI thread synchronization |

## Features

### Real-Time Market Data
- Continuous bid/ask price updates every 200ms
- Realistic price movement simulation (random walk)
- Best bid/ask tracking with spread calculation

### Order Book Management
- Thread-safe order insertion
- Automatic bid/ask sorting (descending bids, ascending asks)
- Volume aggregation across all trades

### Live Trade Feed
- DataGrid showing last 20 trades
- Color-coded buy (green) and sell (red) orders
- Millisecond timestamp precision

### UI/UX
- Responsive controls (Start/Stop simulation)
- Real-time status indicator
- Clean professional dashboard layout
- No UI freezing during intensive updates

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows OS (WPF requirement)
- IDE: Visual Studio 2022 or Rider (optional)

### Installation & Running

1. Clone the repository:
   ```bash
   git clone https://github.com/Harshjain5903/RealTimeOrderBook.git
   cd RealTimeOrderBook
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run --project RealTimeOrderBook/RealTimeOrderBook.csproj
   ```

5. Run tests:
   ```bash
   dotnet test
   ```

### Quick Start Commands

```bash
# Build in Release mode
dotnet build -c Release

# Run with optimizations
dotnet run --project RealTimeOrderBook/RealTimeOrderBook.csproj -c Release

# Run tests with detailed output
dotnet test --verbosity normal

# Clean build artifacts
dotnet clean
```

## 🧪 Testing

Comprehensive NUnit test suite covering:

- ✅ Spread calculation correctness
- ✅ Best bid/ask price tracking
- ✅ Volume aggregation across orders
- ✅ Order book state management
- ✅ Multi-order priority handling

**Run tests:**
```bash
cd RealTimeOrderBook.Tests
dotnet test
```

**Sample Test Output:**
```
Test run for RealTimeOrderBook.Tests.dll (.NET 8.0)
Microsoft (R) Test Execution Command Line Tool

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    11, Skipped:     0, Total:    11
```

## 📸 Screenshots

### Main Trading Dashboard
*Professional order book interface with real-time updates*

[Screenshot placeholder - Run the application and capture the UI]

### Live Order Feed
*Color-coded buy/sell orders with millisecond timestamps*

[Screenshot placeholder - Show DataGrid with active trades]

## 🔧 How It Works

### Market Simulation Flow

1. **Initialization:** `MarketSimulator` starts a background `Task`
2. **Price Generation:** Every 200ms, generates random bid/ask prices using a random walk algorithm
3. **Order Creation:** Creates buy orders (below market) and sell orders (above market)
4. **Event Propagation:** Fires `OrderGenerated` event
5. **ViewModel Update:** `MainViewModel` receives events and updates `OrderBook`
6. **UI Synchronization:** `Dispatcher.Invoke` marshals updates to UI thread
7. **Data Binding:** WPF automatically refreshes bound controls

### Thread Safety Strategy

```csharp
// Background simulation (worker thread)
Task.Run(() => _simulator.StartSimulationAsync(symbol, token));

// Thread-safe order book update
lock (_lock) { /* Update bid/ask */ }

// UI thread marshaling
Dispatcher.Invoke(() => { 
    BestBid = orderBook.BestBid;  // Triggers INotifyPropertyChanged
});
```

## 🎯 Key Learning Outcomes

This project demonstrates:

- **Async Programming:** Proper use of `async/await`, `Task`, `CancellationToken`
- **MVVM Mastery:** Clean separation of UI, logic, and data
- **Thread Safety:** Lock-based synchronization and thread marshaling
- **Data Binding:** `ObservableCollection`, `INotifyPropertyChanged`
- **Event-Driven Design:** Decoupled components via events
- **Unit Testing:** TDD practices with NUnit
- **Professional Code:** Interview-ready, readable, maintainable

## 📄 Technical Interview Talking Points

**Q: How does your application handle UI thread safety?**
> "I use `Dispatcher.Invoke()` to marshal background thread updates to the UI thread. The market simulator runs on a background Task, but all ObservableCollection modifications happen on the UI thread to prevent cross-thread exceptions."

**Q: Why MVVM for this application?**
> "MVVM provides clear separation of concerns. The View is pure XAML with no code-behind, ViewModels handle presentation logic with INotifyPropertyChanged, and Models contain business logic. This makes the code testable, maintainable, and follows WPF best practices."

**Q: How do you prevent memory leaks with continuous updates?**
> "The OrderBook limits stored orders to 100 per side, and RecentTrades collection caps at 20 items. I also use CancellationToken for proper async cleanup and dispose patterns."

## 🔐 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Contact

**Your Name**  
📧 your.email@example.com  
🔗 [LinkedIn](https://linkedin.com/in/yourprofile)  
💼 [Portfolio](https://yourportfolio.com)

---

⭐ **Built with C# and WPF - Showcasing enterprise-grade desktop application development skills**
