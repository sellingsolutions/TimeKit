using System.Collections.Generic;

namespace TimeKit.Models
{
    public interface TkIActor
    {
        ulong Id { get; set; }
        string Key { get; set; }
        string DisplayName { get; set; }

        IEnumerable<TkICapability> Capabilities { get; set; }
    }
}
