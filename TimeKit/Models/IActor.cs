using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public interface IActor
    {
        string Key { get; set; }
        IEnumerable<ICapability> Capabilities { get; set; }
    }
}
