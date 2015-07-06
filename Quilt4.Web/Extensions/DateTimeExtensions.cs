using System;
using InfluxDB.Net;

namespace Quilt4.Web
{
    static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this long unixTime, TimeUnit timeUnit = TimeUnit.Seconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            switch (timeUnit)
            {
                case TimeUnit.Seconds:
                    return epoch.AddSeconds(unixTime);
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown timeUnit {0}.", timeUnit));
            }
        }

        public static long ToInfluxTime(this DateTime date, TimeUnit timeUnit = TimeUnit.Seconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = (date.ToUniversalTime() - epoch);
            switch (timeUnit)
            {
                case TimeUnit.Seconds:
                    return Convert.ToInt64(timeSpan.TotalSeconds);
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown timeUnit {0}.", timeUnit));
            }
        }
    }
}