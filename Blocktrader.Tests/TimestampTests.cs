using System;
using System.Linq;
using FluentAssertions;
using Nexus.Blocktrader.Models;
using Nexus.Blocktrader.Timestamps;
using NUnit.Framework;

namespace Blocktrader.Tests
{
    [TestFixture]
    public class TimestampTests
    {
        [Test]
        public void Should_parse_timestamp_from_byte_to_bytes()
        {
            var datetime = DateTime.Now;
            var timestamp = new Timestamp(
                datetime, new TickerInfo(7654.3f, new OrderBook(
                    new []{new Order(123.3f, 10), new Order(4452.344f, 1234.1f), },
                    new []{new Order(9593, 0.1f), }), 
                    datetime));

            var timestamps = new[] {timestamp};

            Timestamp.FromBytes(timestamp.ToBytes()).ToArray().Should().BeEquivalentTo(timestamps);
        }
    }
}