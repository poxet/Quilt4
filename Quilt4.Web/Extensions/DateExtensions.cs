using System;

namespace Quilt4.Web
{
    public static class DateExtensions
    {
        public static string ToDateTimeString(this DateTime date)
        {
            return date.ToShortDateString() + " " + date.ToLongTimeString();
        }

        public static string ToTimeAgo(this DateTime date)
        {
            var dateTime = DateTime.Now;


            var seconds = (dateTime - date).TotalSeconds;
            if (seconds < 60)
            {
                if (seconds < 2)
                    return "1 second";

                return Math.Round(seconds) + " seconds";
            }

            var minutes = (dateTime - date).TotalMinutes;
            if (minutes < 60)
            {
                if (minutes < 2)
                    return "1 minute";
                return Math.Round(minutes) + " minutes";
            }

            var hours = (dateTime - date).TotalHours;
            if (hours < 24)
            {
                if (hours < 2)
                    return "1 hour";
                return Math.Round(hours) + " hours";
            }

            var days = (dateTime - date).TotalDays;
            if (days < DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
            {
                if (days < 2)
                    return "1 day";
                
                return Math.Round(days) + " days";
            }

            var months = days / 30.4;
            var years = days / 365.25;
            if (months > 1 && years < 1)
            {
                if (months < 2)
                    return "1 month";

                return Math.Round(months) + " months";
            }

            if (years < 2)
                return "1 year";

            return Math.Round(years) + " years";
            
        }
    }
}