using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Blocktrader
{
    public partial class Picker : Window
    {
        public Picker()
        {
            InitializeComponent();

            var file = File.Open(GetFileName("bitstamp", Ticket.BtcUsd, DateTime.Now), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var rawData = new byte[int.MaxValue / 8];
            file.Read(rawData, 0, int.MaxValue / 8);
            var timestamps = Timestamp.FromBytes(rawData);

            var orders = timestamps.First().Orders;
//            var min = orders.Min(o => o.Price);
//            var max = orders.Max(o => o.Price) - min;
//            BitstampBids.ItemsSource = orders.Select(o => new DataGridRow
//            {
//                Item = o,
//                Background = new SolidColorBrush(Color.FromRgb(50, (byte) (int) (255 * (o.Price / min)), 50))
//            });
            BitstampBids.ItemsSource = orders;
        }

        private string GetFileName(string exchange, Ticket ticket, DateTime dateTime)
        {
            return $"{exchange}_{ticket}_{dateTime.ToString("MMM-yyyy", new CultureInfo("en_US"))}";
        }
    }
}