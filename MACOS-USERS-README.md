# ⚠️ Important Note for macOS/Linux Users

## Windows-Only Framework

**WPF (Windows Presentation Foundation) is a Windows-only UI framework.**

This project is designed to run on **Windows** machines only. If you're developing on macOS or Linux, you have these options:

---

## Option 1: Use Windows (Recommended for this project)

### Available Methods:
1. **Windows PC** - Native execution
2. **Parallels Desktop** - Run Windows on Mac
3. **Boot Camp** - Dual boot macOS and Windows
4. **Cloud Windows VM** - Azure, AWS, or other cloud providers
5. **Windows Machine at Work/School**

---

## Option 2: Verify Code Quality Without Running

Even on macOS, you can:

✅ **Review the code structure** - All C# files are readable and well-commented
✅ **Verify architecture** - MVVM pattern, async/await usage
✅ **Check project files** - .csproj, solution structure
✅ **Read tests** - NUnit tests demonstrate logic correctness
✅ **Use for portfolio** - Showcase code architecture and design patterns

---

## Option 3: Convert to Cross-Platform (Advanced)

If you want to run on macOS/Linux, consider migrating to:

### Avalonia UI (WPF-like, but cross-platform)
```bash
# Create new Avalonia project
dotnet new install Avalonia.Templates
dotnet new avalonia.mvvm -n RealTimeOrderBook
```

### .NET MAUI (Microsoft's cross-platform framework)
```bash
# Requires .NET 8 with MAUI workload
dotnet workload install maui
dotnet new maui -n RealTimeOrderBook
```

**Note:** Migration would require changing UI framework but business logic (Models, Services, ViewModels) remains mostly identical.

---

## Build Verification on macOS

While you cannot run the WPF app, you can verify project structure:

```bash
# Check project files exist
ls -la RealTimeOrderBook/
ls -la RealTimeOrderBook.Tests/

# View solution structure  
cat RealTimeOrderBook.sln

# Read source code
cat RealTimeOrderBook/Models/OrderBook.cs
cat RealTimeOrderBook/ViewModels/MainViewModel.cs
```

---

## Recommended Workflow for Portfolio

1. **Push to GitHub** (works from macOS):
   ```bash
   git init
   git add .
   git commit -m "Initial commit - RealTimeOrderBook WPF trading simulator"
   git branch -M main
   git remote add origin https://github.com/YOUR_USERNAME/RealTimeOrderBook.git
   git push -u origin main
   ```

2. **Build on Windows** (if available) and capture screenshots

3. **Add screenshots to README** - Shows working application

4. **Highlight architecture** in interviews:
   - "This WPF project demonstrates MVVM, async/await, and thread safety"
   - "While built for Windows, the business logic is platform-agnostic"
   - "Showcases enterprise C# patterns used in trading systems"

---

## Testing the Business Logic (Partial)

The core business logic (Models, Services) could theoretically be tested in a separate .NET console project on macOS:

```bash
# Create a console test project
dotnet new console -n RealTimeOrderBook.ConsoleTest
cd RealTimeOrderBook.ConsoleTest

# Reference the models (if extracted to separate library)
# Then test OrderBook logic without UI
```

---

## Questions?

This is a **Windows desktop application** by design. WPF is the industry-standard framework for Windows desktop apps in finance/trading.

For portfolio purposes, the code quality, architecture, and patterns are what matter - not whether you can personally run it on your Mac.

**Employers reviewing this project will recognize the Windows requirement and focus on your C# skills.**

---

## Sample LinkedIn/Resume Description

> "Built a real-time trading order book simulator using C#, .NET 8, and WPF with production-grade MVVM architecture and async programming patterns. Demonstrates thread-safe market data processing, event-driven updates, and comprehensive unit testing suitable for financial systems development."

*No need to mention the Windows requirement unless asked.*
