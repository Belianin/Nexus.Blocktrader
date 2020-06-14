using System;

namespace Nexus.Core
{
    public static class DateTimeExtensions
    {
        public static bool TryParse(int year, int month, int day, out DateTime dateTime)
        {
            try
            {
                dateTime = new DateTime(year, month, day);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                dateTime = DateTime.Now;
                return false;
            }
        }

        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        { 
            return (long) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static long ToUnixTimeSeconds(this DateTime dateTime)
        { 
            return (int) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime ToDateTimeFromSeconds(this long seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(seconds);
        }

        public static DateTime ToDateTimeFromSeconds(this int seconds)
        {
            return ((long) seconds).ToDateTimeFromSeconds();
        }


        public static DateTime ToDateTimeFromMilliseconds(this long milliseconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(milliseconds);
        }
        
        public static DateTime ToDateTimeFromMilliseconds(this int milliseconds)
        {
            return ((long) milliseconds).ToDateTimeFromMilliseconds();
        }
    }
}