# Running on Windows - Step-by-Step Guide

Since you're on macOS and this is a Windows-only WPF application, here are your options to run and screenshot the application:

## Option 1: Access a Windows Computer

### At School/University Lab
1. Use any Windows computer with .NET 8 SDK installed
2. Clone your repository: `git clone https://github.com/Harshjain5903/RealTimeOrderBook.git`
3. Follow the build instructions below

### At Work/Library
Many libraries and workspaces have Windows PCs available.

---

## Option 2: Use Windows on Mac (Virtualization)

### Parallels Desktop (Paid, $99/year)
1. Best performance for Mac
2. Install Parallels Desktop
3. Install Windows 11
4. Install .NET 8 SDK in Windows VM
5. Run the application

### UTM (Free)
1. Download UTM from Mac App Store (free)
2. Create Windows 11 VM
3. Install .NET 8 SDK
4. Run the application

### Boot Camp (Free, Intel Macs only)
1. Dual-boot into Windows
2. Install .NET 8 SDK
3. Run the application

---

## Option 3: Remote Windows Machine

### GitHub Codespaces (Free tier available)
While you can't run the GUI, you can build and test:
```bash
dotnet build
dotnet test
```

### Azure/AWS Windows VM (Trial credits available)
1. Create a Windows Server VM
2. Remote Desktop into it
3. Install .NET 8 SDK
4. Run the application

---

## Build Instructions (Once on Windows)

### 1. Install Prerequisites

**Download and install .NET 8 SDK:**
- Visit: https://dotnet.microsoft.com/download/dotnet/8.0
- Download "Windows x64" installer
- Run the installer

**Verify installation:**
```cmd
dotnet --version
```
Should show: `8.0.x`

### 2. Clone Repository

```cmd
git clone https://github.com/Harshjain5903/RealTimeOrderBook.git
cd RealTimeOrderBook
```

### 3. Build the Project

```cmd
dotnet restore
dotnet build
```

### 4. Run the Application

```cmd
dotnet run --project RealTimeOrderBook\RealTimeOrderBook.csproj
```

The WPF window should open.

### 5. Test the Application

1. Click **"Start Simulation"** button
2. Watch as:
   - Best Bid/Ask prices update every 200ms
   - Spread is calculated automatically
   - Volume increases continuously
   - Recent trades appear in the DataGrid
   - Buy orders show in green
   - Sell orders show in red
3. Click **"Stop Simulation"** to pause

### 6. Capture Screenshots

**Method 1: Windows Snipping Tool**
1. Press `Windows + Shift + S`
2. Select area to capture
3. Screenshot saves to clipboard
4. Open Paint and paste (`Ctrl + V`)
5. Save as PNG with these names:
   - `main-dashboard.png`
   - `order-feed.png`
   - `simulation-active.png`

**Method 2: Snip & Sketch**
1. Press `Windows + Shift + S`
2. Draw rectangle around application
3. Click save icon
4. Name and save to `screenshots/` folder

**Method 3: Print Screen**
1. Press `PrtScn` key (captures full screen)
2. Paste in Paint
3. Crop and save

### 7. Add Screenshots to Repository

```cmd
# Copy screenshots to the screenshots folder
copy main-dashboard.png screenshots\
copy order-feed.png screenshots\
copy simulation-active.png screenshots\

# Commit and push
git add screenshots\
git commit -m "Add application screenshots showing live simulation"
git push origin main
```

---

## What the Application Should Look Like

### Expected Behavior:

**Header Section:**
- Title: "Real-Time Order Book Simulator"
- Subtitle: "Professional Trading Dashboard"

**Market Data Panel (Center):**
- Symbol: AAPL
- Best Bid: $150.XX (green color)
- Best Ask: $151.XX (red color)
- Spread: $0.XX (yellow color)
- Volume: XXXXX (blue color)

**DataGrid (Bottom):**
- Columns: Timestamp | Symbol | Side | Price | Quantity
- Shows last 20 trades
- Auto-scrolls as new trades arrive
- Buy orders in green text
- Sell orders in red text

**Control Panel (Bottom):**
- "Start Simulation" button (green)
- "Stop Simulation" button (red)
- Status text showing current state

---

## Troubleshooting

### "dotnet command not found"
- .NET SDK not installed correctly
- Restart terminal after installing
- Add to PATH if needed

### "Cannot find project file"
- Make sure you're in the correct directory
- Check that RealTimeOrderBook.csproj exists

### Application doesn't start
- Check Windows version (needs Windows 10 or 11)
- Ensure .NET 8 SDK is installed, not just Runtime
- Try running with: `dotnet run -c Release --project RealTimeOrderBook\RealTimeOrderBook.csproj`

### No window appears
- WPF requires Desktop Windows (not Windows Server Core)
- Make sure you have a GUI session
- Check Windows Event Viewer for errors

---

## Alternative: Share with Someone on Windows

If you don't have immediate Windows access:

1. Ask a friend/classmate with Windows to:
   - Clone your repo
   - Run the application
   - Take screenshots
   - Send them to you

2. You can then add screenshots:
   ```bash
   git add screenshots/
   git commit -m "Add application screenshots"
   git push
   ```

---

## For Now (macOS)

You can still:
- ✅ Show the code quality
- ✅ Show the commit history
- ✅ Show the architecture
- ✅ Run tests (they work on macOS):
  ```bash
  cd RealTimeOrderBook.Tests
  dotnet test
  ```
- ✅ Show professional engineering practices

The GitHub repository itself demonstrates your skills, even without screenshots!

---

## Quick Checklist

- [ ] Access to Windows machine (physical, VM, or remote)
- [ ] .NET 8 SDK installed
- [ ] Repository cloned
- [ ] Application built successfully
- [ ] Application runs and window opens
- [ ] Simulation started and working
- [ ] Screenshots captured (3 images)
- [ ] Screenshots added to screenshots/ folder
- [ ] Changes committed and pushed to GitHub

---

Once you complete this, your README will show live proof of the working application!
