using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nexus.Blocktrader.Api.Models.Responses;
using Nexus.Blocktrader.Models;
using Nexus.Blocktrader.Trades;
using Nexus.Core;
using Nexus.Logging;

namespace Nexus.Blocktrader.Api.Controllers
{
    [Route("api/v1/trades")]
    public class TradeController : Controller
    {
        private readonly ILog log;
        private readonly ITradesManager manager;

        public TradeController(ITradesManager manager, ILog log)
        {
            this.log = log;
            this.manager = manager;
        }
        
        [HttpGet("ticker/{ticker}/year/{year}/month/{month}/statistics")]
        public async Task<IActionResult> GetMonthStatistics(
            [FromRoute] int year,
            [FromRoute] int month,
            [FromRoute] Ticker ticker)
        {
            log.Info($"Received a request \"{Request.Path}\"");

            if (!DateTimeExtensions.TryParse(year, month, 1, out var selectedDate))
            {
                log.Warn($"Invalid date {year}/{month}");
                return BadRequest($"Invalid date {year}/{month}");
            }

            var months = new List<Trade[]>();
            foreach (var exchange in new[] {ExchangeTitle.Binance, ExchangeTitle.Bitfinex, ExchangeTitle.Bitstamp})
            {
                var result = await manager.ReadForMonthAsync(exchange, ticker, selectedDate).ConfigureAwait(false);
                if (result.IsFail)
                    log.Error($"Failed to read trades from {exchange.ToString()} for {year}/{month}: {result.Error}");
                else
                    months.Add(result);
            }
            

            return Ok(new TradesMonthStatisticsResponse
            {
                AsksAmount = months.SelectMany(t => t).Where(t => t.IsSale).Sum(t => t.Amount),
                BidsAmount = months.SelectMany(t => t).Where(t => t.IsBuy).Sum(t => t.Amount)
            });
        }

        [HttpGet("exchange/{exchange}/ticker/{ticker}/year/{year}/month/{month}/statistics")]
        public async Task<IActionResult> GetMonthStatistics(
            [FromRoute] int year,
            [FromRoute] int month,
            [FromRoute] Ticker ticker,
            [FromRoute] ExchangeTitle exchange)
        {
            log.Info($"Received a request \"{Request.Path}\"");

            if (!DateTimeExtensions.TryParse(year, month, 1, out var selectedDate))
            {
                log.Warn($"Invalid date {year}/{month}");
                return BadRequest($"Invalid date {year}/{month}");
            }

            var result = await manager.ReadForMonthAsync(exchange, ticker, selectedDate).ConfigureAwait(false);
            if (result.IsFail)
                return NotFound(result.Error);

            return Ok(new TradesMonthStatisticsResponse
            {
                AsksAmount = result.Value.Where(t => t.IsSale).Sum(t => t.Amount),
                BidsAmount = result.Value.Where(t => t.IsBuy).Sum(t => t.Amount)
            });
        }
        
        [HttpGet("exchange/{exchange}/ticker/{ticker}/year/{year}/month/{month}")]
        public async Task<IActionResult> GetTrades(
            [FromRoute] int year,
            [FromRoute] int month,
            [FromRoute] Ticker ticker,
            [FromRoute] ExchangeTitle exchange)
        {
            log.Info($"Received a request \"{Request.Path}\"");

            if (!DateTimeExtensions.TryParse(year, month, 1, out var selectedDate))
            {
                log.Warn($"Invalid date {year}/{month}");
                return BadRequest($"Invalid date {year}/{month}");
            }

            var result = await manager.ReadForMonthAsync(exchange, ticker, selectedDate).ConfigureAwait(false);
            if (result.IsFail)
                return NotFound(result.Error);

            return Ok(result.Value.Select(TradeResponse.FromTrade));
        }

        [HttpGet("exchange/{exchange}/ticker/{ticker}/year/{year}/month/{month}/day/{day}")]
        public async Task<IActionResult> GetTrades(
            [FromRoute] int year,
            [FromRoute] int month,
            [FromRoute] int day,
            [FromRoute] Ticker ticker,
            [FromRoute] ExchangeTitle exchange)
        {
            log.Info($"Received a request \"{Request.Path}\"");

            if (!DateTimeExtensions.TryParse(year, month, day, out var selectedDate))
            {
                log.Warn($"Invalid date {year}/{month}/{day}");
                return BadRequest($"Invalid date {year}/{month}/{day}");
            }

            var result = await manager.ReadForDayAsync(exchange, ticker, selectedDate).ConfigureAwait(false);
            if (result.IsFail)
                return NotFound(result.Error);

            return Ok(result.Value.Select(TradeResponse.FromTrade));
        }
    }
}