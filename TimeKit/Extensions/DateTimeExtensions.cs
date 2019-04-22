using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Extensions
{

    public static class DateTimeExtensions
    {
        public static int WeekNumber(this System.DateTime? value)
        {
            if (value == null)
            {
                return -1;
            }

            var calendar = CultureInfo.CurrentCulture.Calendar;
            calendar.GetDayOfWeek(value.Value);
            return calendar.GetWeekOfYear(value.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime GetFirstDayOfWeek(long weekNumber)
        {
            var date = GetFirstDayOfTheYear();

            var dateWeekNo = WeekNumber(date);
            while (dateWeekNo != weekNumber)
            {
                date = date.AddDays(1);
                dateWeekNo = WeekNumber(date);
            }
            return date;
        }

        public static DateTime GetFirstDayOfTheYear()
        {
            var date = DateTime.UtcNow;
            var noOfDays = CultureInfo.CurrentCulture.Calendar.GetDayOfYear(date);
            date = date.AddDays(-(noOfDays - 1));
            date = date.AddHours(-(date.Hour - 1));

            return date;
        }

        /// <summary>
        /// Returns the first day of the week that the specified
        /// date is in using the current culture. 
        /// </summary>
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        /// <summary>
        /// Returns the first day of the week that the specified date 
        /// is in. 
        /// </summary>
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }
    }
}
