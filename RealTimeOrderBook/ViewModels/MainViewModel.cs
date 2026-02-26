using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RealTimeOrderBook.Models;
using RealTimeOrderBook.Services;

namespace RealTimeOrderBook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MarketSimulator _simulator;
        private readonly OrderBook _orderBook;
        private CancellationTokenSource? _cancellationTokenSource;

        private string _symbol = "AAPL";
        private decimal _bestBid;
        private decimal _bestAsk;
        private decimal _spread;
        private long _volume;
        private bool _isRunning;

        public string Symbol
        {
            get => _symbol;
            set { _symbol = value; OnPropertyChanged(); }
        }

        public decimal BestBid
        {
            get => _bestBid;
            set { _bestBid = value; OnPropertyChanged(); }
        }

        public decimal BestAsk
        {
            get => _bestAsk;
            set { _bestAsk = value; OnPropertyChanged(); }
        }

        public decimal Spread
        {
            get => _spread;
            set { _spread = value; OnPropertyChanged(); }
        }

        public long Volume
        {
            get => _volume;
            set { _volume = value; OnPropertyChanged(); }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set { _isRunning = value; OnPropertyChanged(); OnPropertyChanged(nameof(StatusText)); }
        }

        public string StatusText => IsRunning ? "Simulation Running" : "Simulation Stopped";

        public ObservableCollection<Order> RecentTrades { get; }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public MainViewModel()
        {
            _simulator = new MarketSimulator();
            _orderBook = new OrderBook(_symbol);
            RecentTrades = new ObservableCollection<Order>();

            StartCommand = new RelayCommand(async _ => await StartSimulation(), _ => !IsRunning);
            StopCommand = new RelayCommand(_ => StopSimulation(), _ => IsRunning);

            _simulator.OrderGenerated += OnOrderGenerated;
        }

        private async Task StartSimulation()
        {
            IsRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() => _simulator.StartSimulationAsync(_symbol, _cancellationTokenSource.Token));
        }

        private void StopSimulation()
        {
            _cancellationTokenSource?.Cancel();
            IsRunning = false;
        }

        private void OnOrderGenerated(object? sender, Order order)
        {
            // Update order book
            _orderBook.AddOrder(order);

            // Update UI on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Update market data
                BestBid = _orderBook.BestBid;
                BestAsk = _orderBook.BestAsk;
                Spread = _orderBook.Spread;
                Volume = _orderBook.TotalVolume;

                // Add to recent trades
                RecentTrades.Insert(0, order);

                // Keep only last 20 trades
                while (RecentTrades.Count > 20)
                {
                    RecentTrades.RemoveAt(RecentTrades.Count - 1);
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple RelayCommand implementation for MVVM
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
