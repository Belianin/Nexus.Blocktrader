using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Service;
using Nexus.Blocktrader.Service.Timestamps;
using Nexus.Logging;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Api.Controllers
{
    [Route("api/v1/timestamps")]
    public class TimestampController : Controller
    {
        private readonly ITimestampManager timestampManager;
        private readonly ILog log;

        public TimestampController(ITimestampManager timestampManager, ILog log)
        {
            this.timestampManager = timestampManager;
            this.log = log.ForContext("API");
        }

        [HttpGet("exchange/{exchange}/ticker/{ticker}/year/{year}/month/{month}/day/{day}")]
        public ActionResult GetTimestamp(
            [FromRoute] int year,
            [FromRoute] int month,
            [FromRoute] int day,
            [FromRoute] Ticker ticker,
            [FromRoute] ExchangeTitle exchange,
            [FromQuery] int precision = -1)
        {
            log.Info($"Received a request \"{Request.Path}\"");

            if (!DateTimeExtensions.TryParse(year, month, day, out var selectedDate))
            {
                log.Warn($"Invalid date {year}/{month}/{day}");
                return BadRequest($"Invalid date {year}/{month}/{day}");
            }
                

            var timestamp = timestampManager.ReadTimestampForDay(selectedDate, exchange, ticker);

            if (timestamp.IsFail)
            {
                log.Info($"No timestamps for {year}/{month}/{day}");
                return NotFound($"No timestamps for {year}/{month}/{day}");
            }

            // if precision == 0 skip
            var byteData = timestamp.Value
                .Select(t => new Timestamp(t.Date, new TickerInfo(
                    t.TickerInfo.AveragePrice,
                    new OrderBook(t.TickerInfo.OrderBook.Bids.Flat(precision, true).ToArray(),
                        t.TickerInfo.OrderBook.Asks.Flat(precision, false).ToArray()), 
                    t.TickerInfo.DateTime)))
                .Select(t => t.ToBytes2()).SelectMany(t => t).ToArray();
            
            log.Info($"Found timestamps for {year}/{month}/{day}: " +
                     $"Timestamps count \"{timestamp.Value.Length}\". Byte data length: {byteData.Length}");
            
            return File(byteData, "application/btd", "data.btd");
        }
    }
    
    public static class EnumerableExtensions
    {
        private static IEnumerable<Order> Flat(
            this IEnumerable<Order> orders,
            Func<Order, float> priceGetter,
            Func<Order, float, bool> predicate)
        {
            // Здесь происходят странные вещества
            var currentAmount = 0f;
            var firstOrder = orders.FirstOrDefault();
            var price = firstOrder != null ? priceGetter(firstOrder) : 0;
            
            foreach (var order in orders)
            {
                if (predicate(order, price))
                {
                    currentAmount += order.Amount;
                }
                else
                {
                    yield return new Order
                    {
                        Amount = currentAmount,
                        Price = price
                    };
                    price = priceGetter(order);
                    currentAmount = order.Amount;
                }
            }
        }
        public static IEnumerable<Order> Flat(this IEnumerable<Order> orders, int precision, bool isBid)
        {
            if (precision < 0 || orders == null) 
                return orders;
            
            var delta = (float) Math.Pow(10, precision);
            return isBid
                ? orders.Flat(o => (float) Math.Floor(o.Price / delta) * delta, (o, p) => p <= o.Price)
                : orders.Flat(o => (float) Math.Ceiling(o.Price / delta) * delta, (o, p) => p >= o.Price);
        }
    }
}