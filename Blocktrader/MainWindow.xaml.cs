using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        
        private Timestamp[] timestamps;

        private FilterSettings filterSettings = new FilterSettings();
        
        public MainWindow()
        {
            InitializeComponent();
            binance = new BinanceExchange();
            bitfinex = new BitfinexExchange();
            bitstamp = new BitstampExchange();
        }

        private bool IsOk(Order order)
        {
            return order.Amount >= filterSettings.MinSize;
        }

        private IEnumerable<Order> Flat(IEnumerable<Order> orders, float delta)
        {
            //var sorted = orders.ToList()
            //.Sort();
            var currentAmount = 0f;
            var currentPrice = orders.First().Price;
            foreach (var order in orders)
            {
                if (currentPrice - order.Price <= delta)
                {
                    currentAmount += order.Amount;
                }
                else
                {
                    yield return new Order
                    {
                        Amount = currentAmount,
                        Price = currentPrice
                    };
                    currentPrice = order.Price;
                    currentAmount = order.Amount;
                }
            }
        }
        
        private string GetFileName(string exchange, Ticket ticket, DateTime dateTime)   
        {
            return $"Data/{exchange}/{exchange}_{ticket}_{dateTime.ToString("MMM_yyyy", new CultureInfo("en_US"))}";
        }

        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var dateTime = DatePicker.SelectedDate;
            if (dateTime == null)
                return;

            var filename = GetFileName("Bitstamp", Ticket.BtcUsd, dateTime.Value);
            var rawData = File.ReadAllBytes(filename);
            timestamps = Timestamp.FromBytes(rawData)
                .Where(d => d.Date.Day == dateTime.Value.Day)
                .ToArray();
            TimePicker.Maximum = timestamps.Count();
            TimePicker.TickFrequency = 1;
            TimePicker.TickPlacement = TickPlacement.BottomRight;
        }

        private void TimePicker_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timestamps == null)
                return;
            var index = (int) e.NewValue;
            if (index < 0 || index >= timestamps.Length)
                return;
            var timestamp = timestamps[(int) e.NewValue];

            BitstampBids.ItemsSource = timestamp.Bids.Where(IsOk).OrderByDescending(b => b.Price).Flat(50);
            BitstampAsks.ItemsSource = timestamp.Asks.Where(IsOk).OrderBy(p => p.Price).Flat(50);
        }
    }
}