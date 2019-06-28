using System;

namespace TimeKit.Models
{
    public interface TkIProcess
    {
        ulong Id { get; set; }
        string Key { get; set; }
        string ParticipantId { get; set; }

        DateTime? StartsAt { get; set; }
        DateTime? EndsAt { get; set; }
    }
}
