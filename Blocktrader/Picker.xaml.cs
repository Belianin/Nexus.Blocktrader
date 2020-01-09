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
            
            var file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var rawData = new byte[int.MaxValue / 4];
            file.Read(rawData, 0, int.MaxValue / 4);
            var timestamps = Timestamp.FromBytes(rawData);

            var timestamp = timestamps.FirstOrDefault();//(d => d.Date.Day == dateTime.Value.Day);
            if (timestamp == null)
                return;

            BitstampBids.ItemsSource = timestamp.Bids;
            BitstampAsks.ItemsSource = timestamp.Asks;
        }
    }
}