using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public class TkProcess: TkIProcess
    {
        public string Key { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }

        public TkProcess(string key, DateTime? startsAt, DateTime? endsAt)
        {
            Key = key;
            StartsAt = startsAt;
            EndsAt = endsAt;
        }
    }
}
