# Screenshots Placeholder

This folder will contain application screenshots once the project is run on Windows.

## Required Screenshots:

1. **main-dashboard.png** - Overall application view showing:
   - Symbol (AAPL)
   - Best Bid price
   - Best Ask price
   - Spread calculation
   - Total Volume
   - Application header

2. **order-feed.png** - DataGrid view showing:
   - Recent 20 trades
   - Timestamp column
   - Symbol column
   - Side column (Buy/Sell)
   - Price column
   - Quantity column
   - Color coding (green for buy, red for sell)

3. **simulation-active.png** - Application during active simulation:
   - Prices updating in real-time
   - Trade feed scrolling
   - Volume increasing
   - Status showing 'Simulation Running'

## How to Capture Screenshots on Windows:

1. Run the application: `dotnet run --project RealTimeOrderBook/RealTimeOrderBook.csproj`
2. Click 'Start Simulation'
3. Wait 5-10 seconds for data to populate
4. Press `Windows + Shift + S` to capture screenshots
5. Save screenshots with the names listed above
6. Place them in this directory

## Alternative Screenshot Tools:

- **Snipping Tool** (Windows built-in)
- **ShareX** (Free, open-source)
- **Greenshot** (Free screenshot tool)
