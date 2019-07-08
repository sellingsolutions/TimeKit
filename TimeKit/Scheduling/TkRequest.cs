using System.Collections.Generic;
using System.Linq;
using TimeKit.DataStructure;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkRequest
    {
        public TkWorkWeekConfig WorkWeekConfig { get; set; }

        // All available actors
        public IEnumerable<TkActor> Actors { get; set; }

        // All available actor processes
        public IEnumerable<TkProcess> Busy { get; set; }

        // The schedule has to fit within these date intervals
        public IEnumerable<TkInterval> RequiredIntervals { get; set; }

        // All the tasks that need to be scheduled
        public IEnumerable<TkTask> Tasks { get; set; }

        public long TotalTicksRequired { get; set; }

        public TkRequest()
        {

        }

        public TkRequest(
            IEnumerable<TkActor> actors, 
            IEnumerable<TkProcess> busy, 
            IEnumerable<TkInterval> requiredIntervals,
            IEnumerable<TkTask> tasks)
        {
            Actors = actors;
            Busy = busy;
            RequiredIntervals = requiredIntervals;
            Tasks = tasks;

            TotalTicksRequired = tasks.Sum(o => (o.EndsAt - o.StartsAt).Ticks);
        }

        public bool IsValid()
        {
            return Actors != null && 
                   Busy != null &&
                   RequiredIntervals != null &&
                   Tasks != null;
        }

    }
}
