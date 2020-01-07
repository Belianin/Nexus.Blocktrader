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
        private BinanceExchange binance;

        private BitfinexExchange bitfinex;

        private BitstampExchange bitstamp;
        
        private TimeSpan updateFrequency = TimeSpan.FromSeconds(5);

        private FilterSettings filterSettings = new FilterSettings();
        
        public MainWindow()
        {
            InitializeComponent();
            binance = new BinanceExchange();
            bitfinex = new BitfinexExchange();
            bitstamp = new BitstampExchange();

            Task.Run(Update);
        }

        public void Filter(object sender, RoutedEventArgs routedEventArgs)
        {
            if (decimal.TryParse(OrderSizeInput.Text, out var value))
            {
                filterSettings.MinSize = value;
                BinanceBidsGrid.ItemsSource = GetBinanceBinds();
                BinanceAsksGrid.ItemsSource = GetBinanceBinds();
            }

            OrderSizeInput.Text = filterSettings.MinSize.ToString(CultureInfo.CurrentCulture);
        }

        private IEnumerable<BinanceOrder> GetBinanceBinds()
        {
            return binance.Bids.Where(b => b.Size >= filterSettings.MinSize);
        }

        private IEnumerable<BinanceOrder> GetBinanceAsks()
        {
            return binance.Asks.Where(b => b.Size >= filterSettings.MinSize);
        }
        
        
        public void Update()
        {
            while (true)
            {
                try
                {
                    BinanceBidsGrid.Dispatcher?.Invoke(() => BinanceBidsGrid.ItemsSource = GetBinanceBinds());
                    BinanceAsksGrid.Dispatcher?.Invoke(() => BinanceAsksGrid.ItemsSource = GetBinanceAsks());

                    BitfinexBidsGrid.Dispatcher?.Invoke(() =>
                        BitfinexBidsGrid.ItemsSource = bitfinex.Orders.Where(o => o.Amount > 0));
                    BitfinexAsksGrid.Dispatcher?.Invoke(() =>
                        BitfinexAsksGrid.ItemsSource = bitfinex.Orders.Where(o => o.Amount < 0));

                    BitstampBidsGrid.Dispatcher?.Invoke(() =>
                        BitstampBidsGrid.ItemsSource = bitstamp.Bids);
                    BitstampAsksGrid.Dispatcher?.Invoke(() =>
                        BitstampAsksGrid.ItemsSource = bitstamp.Asks);
                    
                    Task.Delay(updateFrequency).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }

    internal class FilterSettings
    {
        public decimal MinSize { get; set; } = 0;
    }
}