using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeKit.DataStructure;
using TimeKit.Models;
using System.Linq;

namespace TimeKit.Scheduling
{
    public class TKResourceRequestRunner
    {
        public TkIRoleType RoleType { get; set; }
        public TkICapability RequiredCapability { get; set; }
        public TkIObjectType ObjectType { get; set; }

        public long NoOfObjects { get; set; }
        public TimeSpan TimeSpanPerObject { get; set; }
        public long TotalTicksNeeded => TimeSpanPerObject.Ticks * NoOfObjects;

        public List<long> WeekNumbers { get; set; }

        // The resource and its busy processes..
        public TkActor Actor { get; set; }
        public IEnumerable<TkIProcess> Busy { get; set; }

        public TKResourceRequestRunner(TkResourceRequest request, TkActor actor)
        {
            RoleType             = request.RoleType;
            RequiredCapability      = request.RequiredCapability;
            ObjectType              = request.ObjectType;
            NoOfObjects             = request.NoOfObjects;
            TimeSpanPerObject       = TimeSpan.FromMinutes(request.MinutesRequiredPerObject);
            WeekNumbers             = request.WeekNumbers;

            Actor = actor;
            Busy = request.AvailableProcesses;
        }
        
        public TKResourceResponse Run()
        {
            var busy = GetBusyTimeSet();
            var vacancy = GetVacancy();

            var totalTicksVacant = vacancy.Ticks();

            if (TotalTicksNeeded >= totalTicksVacant)
                return null;

            var schedule = TryScheduling(vacancy, NoOfObjects, TimeSpanPerObject);
            if (schedule.IsNull)
                return null;

            return new TKResourceResponse(Actor, busy, vacancy, schedule);
        }

        private TimeSet TryScheduling(TimeSet vacancy, long noOfObjects, TimeSpan spanPerObject)
        {
            var scheduledIntervals = new List<Interval>();
            var workSet = vacancy.Copy();

            for (var objectNo = 0; objectNo < noOfObjects; objectNo++)
            {
                var scheduledInterval = workSet.ExtractInterval(spanPerObject);
                if (scheduledInterval.isNull)
                    return TimeSet.Null();

                scheduledIntervals.Add(scheduledInterval);
            }

            var schedule = new TimeSet(scheduledIntervals.ToArray());
            return schedule;
        }

        private TimeSet GetVacancy(long weekNumber)
        {
            var workWeekTs = TimeSet.WorkWeek(weekNumber);
            var busyTs = GetBusyTimeSet();
            var vacancy = TimeSet.Difference(workWeekTs, busyTs);
            return vacancy;
        }

        private TimeSet GetVacancy()
        {
            TimeSet vacancy = new TimeSet();
            foreach (var wNo in WeekNumbers)
            {
                TimeSet.Union(vacancy, GetVacancy(wNo));
            }
            return vacancy;
        }

        private TimeSet GetBusyTimeSet()
        {
            var busyTs = new TimeSet(Busy.Select(o => new Interval(o.StartsAt.Value, o.EndsAt.Value)).ToArray());
            return busyTs;
        }
    }
}
