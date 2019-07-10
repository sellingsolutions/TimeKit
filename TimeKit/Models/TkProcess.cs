using System;
using TimeKit.Scheduling;

namespace TimeKit.Models
{
    public class TkProcess: TkIProcess
    {
        public ulong Id { get; set; }
        public string Key { get; set; }
        public string ParticipantId { get; set; }

        // Is this is process still just a placeholder?
        public bool IsTentative { get; set; }
        public TkSolution Solution { get; set; }

        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }

        public TkProcess(
            ulong id,
            string key, 
            string participantId, 
            DateTime? startsAt, 
            DateTime? endsAt, 
            bool isTentative = false,
            TkSolution solution = null)
        {
            Id = id;
            Key = key;
            ParticipantId = participantId;
            StartsAt = startsAt;
            EndsAt = endsAt;
            IsTentative = isTentative;
            Solution = solution;
        }
    }
}
