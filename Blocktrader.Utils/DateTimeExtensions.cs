using System;

namespace Nexus.Blocktrader.Utils
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
            catch (ArgumentOutOfRangeException e)
            {
                dateTime = DateTime.Now;
                return false;
            }
        }

        public static long ToUnixTime(this DateTime dateTime)
        { 
            var unixTimestamp = (long) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; 
            return unixTimestamp * 1000;
        }
    }
}