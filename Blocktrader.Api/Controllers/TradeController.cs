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