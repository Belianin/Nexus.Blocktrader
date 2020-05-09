﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Service;
using Nexus.Blocktrader.Service.Files;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimeSpan updateInterval = TimeSpan.FromMinutes(10);
        private readonly BlocktraderService service;
        private readonly ITimestampManager timestampManager;

        private FilterSettings filterSettings = new FilterSettings();

        private Ticker currentTicker = Ticker.BtcUsd;
        private DateTime selectedDate = DateTime.Now;
        private int selectedTick = 0;
        private MonthTimestamp selectedTimestamp;
        private int precision = -1;
        
        private bool isUpdating = false;

        private bool needToCache = true;
        
        public MainWindow()
        {
            InitializeComponent();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            
            var logger = loggerFactory.CreateLogger<MainWindow>();
            service = new BlocktraderService(logger);
            timestampManager = new FileTimestampManager(logger);

            TicketPicker.ItemsSource = (Ticker[]) Enum.GetValues(typeof(Ticker));
            DatePicker.SelectedDate = DateTime.Now;
            PrecPicker.Value = 0;

            var timer = new Timer(updateInterval.TotalMilliseconds) {AutoReset = true};
            timer.Elapsed += (s, e) => DownloadAsync().Wait();
            timer.Start();
            logger.LogInformation($"Blocktader initializated");
        }

        private async Task DownloadAsync()
        {
            isUpdating = true;
            var timestamp = await service.GetCurrentTimestampAsync().ConfigureAwait(false);
            await timestampManager.WriteAsync(timestamp);
            needToCache = true;
            isUpdating = false;
        }

        private void Filter(object sender, RoutedEventArgs routedEventArgs)
        {
            if (float.TryParse(OrderSizeInput.Text, out var value))
            {
                filterSettings.MinSize = value;
            }

            OrderSizeInput.Text = filterSettings.MinSize.ToString(CultureInfo.CurrentCulture);
            Update();
        }
        
        private bool IsOk(Order order)
        {
            return order.Amount >= filterSettings.MinSize;
        }

        private void TicketPicker_OnSelected(object sender, RoutedEventArgs e)
        {
            currentTicker = (Ticker) TicketPicker.SelectedItem;

        }
        
        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePicker.SelectedDate != null)
                selectedDate = DatePicker.SelectedDate.Value;
        }

        private void DataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            var grid = (DataGrid)sender;
            foreach (var item in grid.Columns)
            {
                if (item.Header.ToString() == "Count")
                {
                    item.DisplayIndex = grid.Columns.Count - 1;
                    break;
                }
            }
        }
        private void Update()
        {
            if (isUpdating)
                return;

            if (needToCache && (selectedDate.Month == DateTime.Now.Month || selectedTimestamp == null))
            {
                selectedTimestamp = timestampManager.ReadTimestampsFromMonth(selectedDate, currentTicker);
                needToCache = false;
            }
            
            var tickTimestampResult = GetTickInfos();
            if (tickTimestampResult.IsFail)
                return;
            var tickTimestamp = tickTimestampResult.Value;
            
            try
            {
                BitstampBidsGrid.ItemsSource = tickTimestamp[ExchangeTitle.Binance].OrderBook.Bids.Where(IsOk).OrderByDescending(b => b.Price).Flat(precision, true);
                BitstampAsksGrid.ItemsSource = tickTimestamp[ExchangeTitle.Binance].OrderBook.Asks.Where(IsOk).OrderBy(p => p.Price).Flat(precision, false);
                BitfinexBidsGrid.ItemsSource = tickTimestamp[ExchangeTitle.Bitfinex].OrderBook.Bids.Where(IsOk).OrderByDescending(b => b.Price).Flat(precision, true);
                BitfinexAsksGrid.ItemsSource = tickTimestamp[ExchangeTitle.Bitfinex].OrderBook.Asks.Where(IsOk).OrderBy(p => p.Price).Flat(precision, false);
                BinanceBidsGrid.ItemsSource = tickTimestamp[ExchangeTitle.Bitstamp].OrderBook.Bids.Where(IsOk).OrderByDescending(b => b.Price).Flat(precision, true);
                BinanceAsksGrid.ItemsSource = tickTimestamp[ExchangeTitle.Bitstamp].OrderBook.Asks.Where(IsOk).OrderBy(p => p.Price).Flat(precision, false);

                var tickDateTime = tickTimestamp[ExchangeTitle.Binance].DateTime;
                TimeTextBlock.Text = "Time: " + tickDateTime.ToString("F", CultureInfo.CurrentCulture);
                PriceTextBlock.Text = Math.Floor(tickTimestamp[ExchangeTitle.Binance].AveragePrice).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(e.Message);
            }
                 
            InvalidateVisual();
        }

        private Result<Dictionary<ExchangeTitle, TickerInfo>> GetTickInfos()
        {
            var currentDayTimestamp = selectedTimestamp.Info
                .Where(i => i.Key.Day == selectedDate.Day)
                .Select(v => v.Value)
                .ToArray();

            if (currentDayTimestamp.Length == 0)
                return "No timestamps";
            
            // можно проверку на 0
            if (selectedTick >= currentDayTimestamp.Length)
                selectedTick = currentDayTimestamp.Length - 1;  
            
            // Что это тут вообще делает :/
            TimePicker.Maximum = currentDayTimestamp.Length;
            TimePicker.TickFrequency = 1;
            TimePicker.TickPlacement = TickPlacement.BottomRight;

            return currentDayTimestamp[selectedTick];
        }

        private void PrecPickerChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            precision = (int) PrecPicker.Value - 1;
            Update();
        }

        private void TimePickerChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedTick = (int) TimePicker.Value;
            Update();

        }

        private void PickerLeft6_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ShiftCount.Text, out var value))
            {
                TimePicker.Value -= value;
            }
            
        }

        private void PickerRight6_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ShiftCount.Text, out var value))
            {
                TimePicker.Value += value;
            }
        }
    }
}