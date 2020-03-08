using System;

namespace Lockbase.CoreDomain.Extensions
{
    public static class DateTimeExtensions
    {

        private static DateTime DATE_1970 = new DateTime(1970, 1, 1);
        public static int UnixTime(this DateTime dateTime)
        => (int)(dateTime.Subtract(DATE_1970).TotalSeconds);
    }
}
