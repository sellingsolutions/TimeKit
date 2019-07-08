using System;
using System.Collections.Generic;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TkScheduler
    {
        public static TkTimeSet Schedule(long noOfObjects, TimeSpan timeSpanPerObject, TkTimeSet vacancy)
        {
            var scheduledIntervals = new List<TkInterval>();
            var workSet = vacancy.Copy();

            for (var objectNo = 0; objectNo < noOfObjects; objectNo++)
            {
                var scheduledInterval = workSet.ExtractInterval(timeSpanPerObject);
                if (scheduledInterval.isNull)
                    return TkTimeSet.Null();

                scheduledIntervals.Add(scheduledInterval);
            }

            var schedule = new TkTimeSet(scheduledIntervals.ToArray());
            return schedule;
        }
    }
}
