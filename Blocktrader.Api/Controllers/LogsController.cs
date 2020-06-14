using System;
using Microsoft.AspNetCore.Mvc;
using Nexus.Logging.File;

namespace Nexus.Blocktrader.Api.Controllers
{
    [Route("api/v1/logs")]
    public class LogsController : Controller
    {
        [HttpGet("last")]
        public IActionResult GetLastLogs()
        {
            var filename = FileLog.GetFileName(DateTime.Now);

            if (!System.IO.File.Exists(filename))
                return NotFound();
            
            var logs = System.IO.File.ReadAllText(filename);

            return Ok(logs);
        }
    }
}