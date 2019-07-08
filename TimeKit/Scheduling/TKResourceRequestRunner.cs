using System;
using System.Collections.Generic;
using System.Linq;
using TimeKit.DataStructure;
using TimeKit.Extensions;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TKResourceRequestRunner
    {

        public TkRequest Request { get; set; }

        public TKResourceRequestRunner(TkRequest request)
        {
            Request = request;
        }
        
        public IEnumerable<TkResourceResponse> Run()
        {
            var responses = new List<TkResourceResponse>();

            foreach (var actor in Request.Actors)
            {
                var busyProcesses = Request.Busy.Where(o => o.ParticipantId == actor.Key);

                var busy = GetBusy(busyProcesses, Request.ScheduleStartsAt, Request.ScheduleEndsAt);
                var vacancy = GetVacancy(Request.ScheduleStartsAt, Request.ScheduleEndsAt, busy, Request.WorkWeekConfig);

                var totalTicksVacant = vacancy.Ticks();

                if (Request.TotalTicksRequired >= totalTicksVacant)
                    return null;

                var schedule = TryScheduling(Request.Tasks, vacancy);
                if (schedule.IsNull)
                    return null;

               responses.Add(new TkResourceResponse(actor, busy, vacancy, schedule));
            }

            return responses;
        }

        private TkTimeSet TryScheduling(IEnumerable<TkTask> tasks, TkTimeSet vacancy)
        {
            var scheduledIntervals = new List<TkInterval>();
            var workSet = vacancy.Copy();

            foreach (var task in tasks)
            {
                var scheduledInterval = workSet.ExtractInterval(task.Duration);
                if (scheduledInterval.isNull)
                    return TkTimeSet.Null();

                task.ScheduledInterval = scheduledInterval;
                scheduledIntervals.Add(scheduledInterval);
            }

            var schedule = new TkTimeSet(scheduledIntervals.ToArray());
            return schedule;
        }

        private TkTimeSet GetVacancy(DateTime start, DateTime end, TkTimeSet busy, TkWorkWeekConfig config)
        {
            var workWeeks = TkTimeSet.WorkWeeks(start, end, config);
            var vacancy = TkTimeSet.Difference(workWeeks, busy);
            return vacancy;
        }

        private TkTimeSet GetVacancy(long weekNumber, TkTimeSet busy)
        {
            var workWeekTs = TkTimeSet.WorkWeek(weekNumber);
            var vacancy = TkTimeSet.Difference(workWeekTs, busy);
            return vacancy;
        }
        
        private TkTimeSet GetBusy(IEnumerable<TkProcess> busyProcesses, DateTime start, DateTime end)
        {
            var busyTs = new TkTimeSet(busyProcesses
                .Where(o=>o.StartsAt.HasValue && o.EndsAt.HasValue)
                .Where(o=>o.StartsAt.Value >= start && o.EndsAt.Value <= end)
                .Select(o=> new TkInterval(o.StartsAt.Value, o.EndsAt.Value))
                .ToArray());

            return busyTs;
        }

        private TkTimeSet GetBusy(IEnumerable<TkProcess> busyProcesses, long weekNumber)
        {
            var busyTs = new TkTimeSet(
                busyProcesses
                    .Where(o=>o.StartsAt.HasValue && o.EndsAt.HasValue)
                    .Where(o => DateTimeExtensions.WeekNumber(o.StartsAt.Value) == weekNumber &&
                                DateTimeExtensions.WeekNumber(o.EndsAt.Value) == weekNumber)
                    .Select(o => new TkInterval(o.StartsAt.Value, o.EndsAt.Value))
                    .ToArray());
            return busyTs;
        }
    }
}
