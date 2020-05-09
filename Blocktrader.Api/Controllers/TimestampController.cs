using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Service.Files;

namespace Nexus.Blocktrader.Api.Controllers
{
    [Route("api/v1/timestamps")]
    public class TimestampController : Controller
    {
        private readonly ITimestampManager timestampManager;

        public TimestampController(ITimestampManager timestampManager)
        {
            this.timestampManager = timestampManager;
        }

        [HttpGet("exchange/{exchange}/ticker/{ticker}/year/{year}/month/{month}/day/{day}")]
        public ActionResult GetTimestamp(
            [FromRoute] int year,
            [FromRoute] int month,
            [FromRoute] int day,
            [FromRoute] Ticker ticker,
            [FromRoute] ExchangeTitle exchange)
        {
            Console.WriteLine("Received a request");
            
            var selectedDate = new DateTime(year, month, day);

            var timestamp = timestampManager.ReadTimestampForDay(selectedDate, exchange, ticker);

            if (timestamp.IsFail)
                return NotFound("Нет такого файла");

            var byteData = timestamp.Value.Select(t => t.ToBytes()).SelectMany(t => t).ToArray();

            return File(byteData, "application/btd", "data.btd");
        }
    }
}