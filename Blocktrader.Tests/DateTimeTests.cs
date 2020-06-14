using System;
using FluentAssertions;
using Nexus.Core;
using NUnit.Framework;

namespace Blocktrader.Tests
{
    [TestFixture]
    public class DateTimeTests
    {
        [Test]
        public void Should_convert_unix_time_from_seconds()
        {
            var dateTime = new DateTime(1998, 3, 26, 2, 8, 30, DateTimeKind.Utc);

            dateTime.ToUnixTimeSeconds().ToDateTimeFromSeconds().Should().Be(dateTime);
        }
        
        [Test]
        public void Should_convert_unix_time_from_milliseconds()
        {
            var dateTime = new DateTime(1998, 3, 26, 2, 8, 30, DateTimeKind.Utc);

            dateTime.ToUnixTimeMilliseconds().ToDateTimeFromMilliseconds().Should().Be(dateTime);
        }
    }
}