using System;

namespace TimeKit.Models
{
    public class TkWorkWeekConfig
    {
        private static TkWorkWeekConfig _instance;

        public static TkWorkWeekConfig Default {
            get
            {
                if (_instance == null)
                    _instance = new TkWorkWeekConfig();
                return _instance;
            }
        }

        public DayOfWeek[] WorkDays { get; set; } = new[]
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
        };

        public int WorkDayStartHourUtc { get; set; } = 6;
        public int WorkDayDuration { get; set; } = 9;
    }
}