using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public interface TkIActor
    {
        string Key { get; set; }
        string DisplayName { get; set; }

        IEnumerable<TkICapability> Capabilities { get; set; }
    }
}
