using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Blocktrader.Binance;
using Blocktrader.Bitfinex;
using Blocktrader.Bitstamp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blocktrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BinanceExchange binance;

        private readonly BitfinexExchange bitfinex;

        private readonly BitstampExchange bitstamp;

        private FilterSettings filterSettings = new FilterSettings();
        
        public MainWindow()
        {
            InitializeComponent();
            binance = new BinanceExchange();
            bitfinex = new BitfinexExchange();
            bitstamp = new BitstampExchange();

            binance.OnUpdate += (s, e) => UpdateBinance();
            bitfinex.OnUpdate += (s, e) => UpdateBitfinex();
            bitstamp.OnUpdate += (s, e) => UpdateBitstamp();

            Task.Run(Update);
        }

        public void Filter(object sender, RoutedEventArgs routedEventArgs)
        {
            if (decimal.TryParse(OrderSizeInput.Text, out var value))
            {
                filterSettings.MinSize = value;
                Update();
            }

            OrderSizeInput.Text = filterSettings.MinSize.ToString(CultureInfo.CurrentCulture);
        }

        private void UpdateBinance()
        {
            BinanceBidsGrid.Dispatcher?.Invoke(() => 
                BinanceBidsGrid.ItemsSource = binance.Bids.Where(b => b.Size >= filterSettings.MinSize));
            BinanceAsksGrid.Dispatcher?.Invoke(() =>
                BinanceAsksGrid.ItemsSource = binance.Asks.Where(b => b.Size >= filterSettings.MinSize));
        }


        private void UpdateBitfinex()
        {
            var orders = bitfinex.Orders.Where(b => Math.Abs(b.Amount) >= filterSettings.MinSize);
            BitfinexBidsGrid.Dispatcher?.Invoke(() =>
                BitfinexBidsGrid.ItemsSource = orders.Where(o => o.Amount > 0));
            BitfinexAsksGrid.Dispatcher?.Invoke(() =>
                BitfinexAsksGrid.ItemsSource = orders.Where(o => o.Amount < 0));
        }
        
        private void UpdateBitstamp()
        {
            BitstampBidsGrid.Dispatcher?.Invoke(() =>
                BitstampBidsGrid.ItemsSource = bitstamp.Bids.Where(b => b.Size >= filterSettings.MinSize));
            BitstampAsksGrid.Dispatcher?.Invoke(() =>
                BitstampAsksGrid.ItemsSource = bitstamp.Asks.Where(a => a.Size >= filterSettings.MinSize));
        }
        
        public void Update()
        {
            UpdateBinance();
            UpdateBitfinex();
            UpdateBitstamp();
        }
    }

    internal class FilterSettings
    {
        public decimal MinSize { get; set; } = 0;
    }
}