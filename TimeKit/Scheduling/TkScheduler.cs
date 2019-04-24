using System;
using System.Collections.Generic;
using System.Text;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TkScheduler
    {
        public static TimeSet Schedule(TkResourceRequestSolutionGroup group, TimeSet vacancy)
        {
            var scheduledIntervals = new List<Interval>();
            var workSet = vacancy.Copy();

            for (var objectNo = 0; objectNo < group.NoOfObjects; objectNo++)
            {
                var scheduledInterval = workSet.ExtractInterval(group.TicksRequiredPerObject);
                if (scheduledInterval.isNull)
                    return TimeSet.Null();

                scheduledIntervals.Add(scheduledInterval);
            }

            var schedule = new TimeSet(scheduledIntervals.ToArray());
            return schedule;
        }
    }
}
