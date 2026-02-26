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

## Testing

The project includes NUnit tests covering:

- Spread calculation correctness
- Best bid/ask price tracking
- Volume aggregation across orders
- Order book state management
- Multi-order priority handling

Run tests:
```bash
cd RealTimeOrderBook.Tests
dotnet test
```

Sample test output:
```
Test run for RealTimeOrderBook.Tests.dll (.NET 8.0)
Microsoft (R) Test Execution Command Line Tool

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    11, Skipped:     0, Total:    11
```

## Implementation Details

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

## Technical Highlights

The application uses several key patterns to ensure reliable real-time performance:

**Async Programming** - Background tasks run without blocking the UI using `async/await`, `Task`, and `CancellationToken` for proper cleanup.

**MVVM Architecture** - Clear separation between UI (XAML), presentation logic (ViewModels), and business logic (Models). The ViewModel implements `INotifyPropertyChanged` for automatic UI updates.

**Thread Safety** - Lock-based synchronization protects the order book from concurrent access. UI updates are marshaled to the main thread using `Dispatcher.Invoke()`.

**Data Binding** - `ObservableCollection` automatically updates the UI when new trades arrive. All UI elements bind directly to ViewModel properties.

**Event-Driven Design** - The market simulator raises events when orders are generated, keeping components loosely coupled.

## License

This project is licensed under the MIT License.
