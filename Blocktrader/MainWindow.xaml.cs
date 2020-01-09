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
//
//            binance.OnUpdate += (s, e) => UpdateBinance();
//            bitfinex.OnUpdate += (s, e) => UpdateBitfinex();
//            bitstamp.OnUpdate += (s, e) => UpdateBitstamp();

            TicketPicker.ItemsSource = new[]
            {
                Ticket.BtcUsd,
                Ticket.EthUsd,
                Ticket.EthBtc,
                Ticket.XrpUsd,
                Ticket.XrpBtc
            };

            var picker = new Picker();
            picker.Show();
        }

        public void Filter(object sender, RoutedEventArgs routedEventArgs)
        {
            if (float.TryParse(OrderSizeInput.Text, out var value))
            {
                filterSettings.MinSize = value;
                ForceUpdate();
            }

            OrderSizeInput.Text = filterSettings.MinSize.ToString(CultureInfo.CurrentCulture);
        }

//        private void UpdateBinance()
//        {
//            var info = binance.GetInfo();
//            BinanceBidsGrid.Dispatcher?.Invoke(() => 
//                BinanceBidsGrid.ItemsSource = info.Bids.Where(IsOk));
//            BinanceAsksGrid.Dispatcher?.Invoke(() =>
//                BinanceAsksGrid.ItemsSource = info.Asks.Where(IsOk));
//        }
//
//
//        private void UpdateBitfinex()
//        {
//            var info = bitfinex.GetInfo();
//            BitfinexBidsGrid.Dispatcher?.Invoke(() =>
//                BitfinexBidsGrid.ItemsSource = info.Bids.Where(IsOk));
//            BitfinexAsksGrid.Dispatcher?.Invoke(() =>
//                BitfinexAsksGrid.ItemsSource = info.Asks.Where(IsOk));
//        }
//        
//        private void UpdateBitstamp()
//        {
//            var info = bitstamp.GetInfo();
//            BitstampBidsGrid.Dispatcher?.Invoke(() =>
//                BitstampBidsGrid.ItemsSource = info.Bids.Where(IsOk));
//            BitstampAsksGrid.Dispatcher?.Invoke(() =>
//                BitstampAsksGrid.ItemsSource = info.Asks.Where(IsOk));
//        }

        private void ForceUpdate()
        {
//            binance.ForceUpdate();
//            bitfinex.ForceUpdate();
//            bitstamp.ForceUpdate();
        }

        private bool IsOk(Order order)
        {
            return order.Amount >= filterSettings.MinSize;
        }
        
        
        private void TicketPicker_OnSelected(object sender, RoutedEventArgs e)
        {
            var ticket = (Ticket) TicketPicker.SelectedItem;
            binance.Ticket = ticket;
            bitfinex.Ticket = ticket;
            //bitstamp.Ticket = ticket;
            
            //ForceUpdate(); 
        }
    }

    internal class FilterSettings
    {
        public float MinSize { get; set; } = 0;
    }
}