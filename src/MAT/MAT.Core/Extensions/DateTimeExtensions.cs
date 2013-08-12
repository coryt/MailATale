using System;

namespace MAT.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string NextMonth(this DateTime date)
        {
            return new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1).ToString("MMddyyyy");
        }

        public static string CurrentMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).ToString("MMddyyyy");
        }
    }
}