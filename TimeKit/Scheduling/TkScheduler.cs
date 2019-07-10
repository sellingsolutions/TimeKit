using System;
using System.Collections.Generic;
using TimeKit.DataStructure;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkScheduler
    {
        public static TkTimeSet TryScheduling(IEnumerable<TkTask> tasks, TkTimeSet vacancy)
        {
            var scheduledIntervals = new List<TkInterval>();
            var workSet = vacancy.Copy();

            foreach (var task in tasks)
            {
                var scheduledInterval = workSet.ExtractInterval(TimeSpan.FromTicks(task.PlannedDuration));
                if (scheduledInterval.isNull)
                    return TkTimeSet.Null();

                task.ScheduledInterval = scheduledInterval;
                scheduledIntervals.Add(scheduledInterval);
            }

            var schedule = new TkTimeSet(scheduledIntervals.ToArray());
            return schedule;
        }
    }
}