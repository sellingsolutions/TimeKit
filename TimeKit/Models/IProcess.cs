using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public interface IProcess
    {
        string Key { get; set; }
        DateTime? StartsAt { get; set; }
        DateTime? EndsAt { get; set; }
    }
}
