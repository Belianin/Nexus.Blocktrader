using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Blocktrader
{
    public abstract class BaseExchange
    {
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(10);

        private ICollection<Ticket> tickets;
        
        public string Name { get; }
        
        public BaseExchange(string name, ICollection<Ticket> tickets)
        {
            Name = name;
            if (!Directory.Exists($"Data/{Name}"))
                Directory.CreateDirectory($"Data/{Name}");
        }
        
        protected FileStream GetWriter(Ticket ticket)
        {
            var filename = $"Data/{Name}/{Name}_{ticket}_{DateTime.Now.ToString("MMM_yyyy", new CultureInfo("en_US"))}";

            return File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        protected abstract Timestamp GetTimestamp(Ticket ticket);

        private void Update()
        {
            while (true)
            {
                foreach (var ticket in tickets)
                {
                    using var writer = GetWriter(ticket);
                    var timestamp = GetTimestamp(ticket);
                    writer.Write(timestamp.ToBytes());
                }
                Thread.Sleep(updatePeriod);
            }
        }
    }
}