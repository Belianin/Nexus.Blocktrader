using System;
using System.Globalization;
using System.IO;

namespace Blocktrader
{
    public abstract class BaseExchange
    {
        public BaseExchange(string name)
        {
            Name = name;
        }

        public string Name { get; }
        
        protected FileStream GetWriter(Ticket ticket)
        {
            if (!Directory.Exists($"Data/{Name}"))
                Directory.CreateDirectory($"Data/{Name}");
            var filename = $"Data/{Name}/{Name}_{ticket}_{DateTime.Now.ToString("MMM_yyyy", new CultureInfo("en_US"))}";

            return File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        protected abstract Timestamp GetTimestamp(Ticket ticket);
    }
}