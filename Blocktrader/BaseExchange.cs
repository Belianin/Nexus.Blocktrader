using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blocktrader
{
    public abstract class BaseExchange
    {
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(10);

        private ICollection<Ticket> tickets;
        
        public string Name { get; }

        public abstract Timestamp GetTimestamp(Ticket ticket);

        public BaseExchange(string name, ICollection<Ticket> tickets)
        {
            Name = name;
            this.tickets = tickets;
            if (!Directory.Exists($"Data/{Name}"))
                Directory.CreateDirectory($"Data/{Name}");
            Task.Run(Update);
        }

        protected FileStream GetWriter(Ticket ticket)
        {
            var filename = $"Data/{Name}/{Name}_{ticket}_{DateTime.Now.ToString("MMM_yyyy", new CultureInfo("en_US"))}";

            return File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        private void Update()
        {
            while (true)
            {
                foreach (var ticket in tickets)
                {
                    var timestamp = GetTimestamp(ticket);
                    using var writer = GetWriter(ticket);
                    writer.Write(timestamp.ToBytes());
                }
                Thread.Sleep(updatePeriod);
            }
        }
    }
}