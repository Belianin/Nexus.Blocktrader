using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Core;

namespace Nexus.Prophecy
{
    public interface ILogService
    {
        Result<IEnumerable<string>> GetLogs(string service, DateTime from, DateTime to);
    }
}