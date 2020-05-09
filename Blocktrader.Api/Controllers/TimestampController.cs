using Microsoft.AspNetCore.Mvc;
using Nexus.Blocktrader.Api.Models.Responses;

namespace Nexus.Blocktrader.Api.Controllers
{
    [Route("api/v1/timestamps")]
    public class TimestampController : Controller
    {
        [HttpGet("year/{year}/month/{month}")]
        public ActionResult GetTimestamp([FromRoute] int year, [FromRoute] int month)
        {
            return Ok(new TimestampResponse
            {
                Year = year,
                Month = month
            });
        }
    }
}