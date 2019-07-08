using System;
using TimeKit.DataStructure;

namespace TimeKit.Models
{
    /// <summary>
    /// For instance an apartment that we want to schedule an inspection for
    /// </summary>
    public class TkTask
    {
        // How much time do we need to schedule for this task?
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }

        public TimeSpan Duration => EndsAt - StartsAt;

        // An external Id that this task points to, e.g. the inspection instance (Besiktningstillfälle)
        public ulong Id { get; set; }

        public TkInterval ScheduledInterval { get; set; }
    }
}