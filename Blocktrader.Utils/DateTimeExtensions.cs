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
    }
}