using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nexus.Logging;
using Nexus.Logging.Utils;

namespace Nexus.Prophecy.Controllers
{
    [Route("api/v1/logs")]
    public class LogsController : Controller
    {
        private readonly ILogService logService;
        private readonly ILog log;

        public LogsController(ILogService logService, ILog log)
        {
            this.logService = logService;
            this.log = log;
        }

        [HttpGet("{serviceName}")]
        public IActionResult GetLogs(
            [FromRoute] string serviceName,
            [FromQuery] LogLevel[]? levels,
            [FromQuery] string? substring,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] bool ignoreCase = true)
        {
            log.Info($"Received a request: \"{Request.Path}\" with query \"{Request.QueryString}\"");

            if (serviceName == null)
                return BadRequest("Service name must be specified");

            if (from == null || to == null)
                return BadRequest("\"from\" and \"to\" periods must be specified");
            if (from < to)
                return BadRequest("\"from\" must be greater than \"to\"");

            var filterParameters = new LogFilterParameters
            {
                IsIgnoreCase = ignoreCase,
                Substring = substring,
                LogLevels = levels
            };

            var result = logService.GetLogs(serviceName, from, to, filterParameters);

            if (result.IsFail)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
    }
}