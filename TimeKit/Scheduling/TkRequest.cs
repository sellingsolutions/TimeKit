using System;
using System.Collections.Generic;
using System.Linq;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkRequest
    {
        public TkWorkWeekConfig WorkWeekConfig { get; set; } = TkWorkWeekConfig.Default;

        // All available actors
        public IEnumerable<TkActor> Actors { get; set; }

        // All available actor processes
        public IEnumerable<TkProcess> Busy { get; set; }

        // The schedule has to fit within this date interval
        public DateTime ScheduleStartsAt { get; set; }
        public DateTime ScheduleEndsAt { get; set; }

        // All the tasks that need to be scheduled
        public IEnumerable<TkTask> Tasks { get; set; }

        public long TotalTicksRequired { get; set; }

        public TkRequest()
        {

        }

        public TkRequest(
            DateTime scheduleStartsAt,
            DateTime scheduleEndsAt,
            IEnumerable<TkActor> actors, 
            IEnumerable<TkProcess> busy, 
            IEnumerable<TkTask> tasks)
        {
            ScheduleStartsAt = scheduleStartsAt;
            ScheduleEndsAt = scheduleEndsAt;
            Actors = actors;
            Busy = busy;
            Tasks = tasks;

            TotalTicksRequired = tasks.Sum(o => (o.PlannedDuration));
        }

        public bool IsValid()
        {
            if (ScheduleStartsAt != DateTime.MinValue &&
                ScheduleEndsAt != DateTime.MinValue &&
                Actors != null &&
                Busy != null &&
                Tasks != null)
                return Actors.Any() && Busy.Any() && Tasks.Any();

            return false;
        }

    }
}
