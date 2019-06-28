using System;

namespace TimeKit.Models
{
    public class TkProcess: TkIProcess
    {
        public ulong Id { get; set; }
        public string Key { get; set; }
        public string ParticipantId { get; set; }

        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }

        public TkProcess(ulong id, string key, string participantId, DateTime? startsAt, DateTime? endsAt)
        {
            Id = id;
            Key = key;
            ParticipantId = participantId;
            StartsAt = startsAt;
            EndsAt = endsAt;
        }
    }
}
