# Quick Start Guide

## Prerequisites

Ensure you have .NET 8 SDK installed:
```bash
dotnet --version
# Should show 8.0.x or higher
```

If not installed, download from: https://dotnet.microsoft.com/download/dotnet/8.0

## Build & Run Instructions

### 1. Navigate to Project Directory
```bash
cd "/Users/harshjain/Downloads/Real-Time Order Book Desktop Application"
```

### 2. Restore NuGet Packages
```bash
dotnet restore
```

### 3. Build the Solution
```bash
# Debug build
dotnet build

# Or Release build (optimized)
dotnet build -c Release
```

### 4. Run the Application
```bash
# Run in Debug mode
dotnet run --project RealTimeOrderBook/RealTimeOrderBook.csproj

# Run in Release mode
dotnet run --project RealTimeOrderBook/RealTimeOrderBook.csproj -c Release
```

### 5. Run Unit Tests
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Development Commands

### Clean Build Artifacts
```bash
dotnet clean
```

### Rebuild from Scratch
```bash
dotnet clean && dotnet restore && dotnet build
```

### Open in Visual Studio (Windows)
```bash
start RealTimeOrderBook.sln
```

### Open in VS Code
```bash
code .
```

## Git Initialization & GitHub Push

### 1. Initialize Git Repository
```bash
git init
```

### 2. Stage All Files
```bash
git add .
```

### 3. Create Initial Commit
```bash
git commit -m "Initial commit - RealTimeOrderBook WPF trading simulator"
```

### 4. Rename Branch to 'main'
```bash
git branch -M main
```

### 5. Add Remote Repository
**Replace `YOUR_USERNAME` with your actual GitHub username:**
```bash
git remote add origin https://github.com/YOUR_USERNAME/RealTimeOrderBook.git
```

### 6. Push to GitHub
```bash
git push -u origin main
```

## Creating GitHub Repository

### Option 1: Using GitHub CLI (gh)
```bash
# Install GitHub CLI first: https://cli.github.com/

# Authenticate
gh auth login

# Create repository
gh repo create RealTimeOrderBook --public --source=. --remote=origin --push
```

### Option 2: Using GitHub Web Interface

1. Go to https://github.com/new
2. Repository name: `RealTimeOrderBook`
3. Description: "Real-time stock order book simulator using C# WPF and async programming"
4. Choose Public
5. **DO NOT** initialize with README (we already have one)
6. Click "Create repository"
7. Follow the "push an existing repository" instructions shown

---

## 📊 Verify Everything Works

### Test Checklist:
- [ ] Project builds without errors: `dotnet build`
- [ ] All 11 unit tests pass: `dotnet test`
- [ ] Application launches: `dotnet run --project RealTimeOrderBook/RealTimeOrderBook.csproj`
- [ ] Click "Start Simulation" - trades appear in real-time
- [ ] Bid/Ask prices update every ~200ms
- [ ] Volume increases continuously
- [ ] "Stop Simulation" button works
- [ ] Git repository initialized successfully
- [ ] Code pushed to GitHub

## Troubleshooting

### Issue: "SDK not found"
**Solution:** Install .NET 8 SDK from https://dotnet.microsoft.com/download

### Issue: "Project file does not exist"
**Solution:** Ensure you're in the correct directory and paths are correct

### Issue: "git command not found"
**Solution:** Install Git from https://git-scm.com/downloads

### Issue: NuGet restore fails
**Solution:** 
```bash
dotnet nuget locals all --clear
dotnet restore --force
```

### Issue: Application doesn't start on macOS/Linux
**Note:** WPF is Windows-only. For cross-platform, consider Avalonia UI or .NET MAUI
