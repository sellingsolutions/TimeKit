using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public interface TkIProcess
    {
        string Key { get; set; }
        string ParticipantId { get; set; }

        DateTime? StartsAt { get; set; }
        DateTime? EndsAt { get; set; }
    }
}
