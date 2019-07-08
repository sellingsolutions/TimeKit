using System;
using System.Collections.Generic;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TkScheduler
    {
        public static TkTimeSet Schedule(TkSolution solution)
        {
            var scheduledIntervals = new List<TkInterval>();
            var workSet = solution.MutualVacancy.Copy();

            foreach (var task in solution.Request.Tasks)
            {
                var scheduledInterval = workSet.ExtractInterval(task.Duration);
                if (scheduledInterval.isNull)
                    return TkTimeSet.Null();

                scheduledIntervals.Add(scheduledInterval);
            }

            var schedule = new TkTimeSet(scheduledIntervals.ToArray());
            return schedule;
        }
    }
}
