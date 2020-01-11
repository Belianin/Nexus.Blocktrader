using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Blocktrader
{
    public partial class Picker : Window
    {
        private Timestamp[] timestamps;
        
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

            BitstampBids.ItemsSource = timestamp.Bids;
            BitstampAsks.ItemsSource = timestamp.Asks;
        }
    }
}