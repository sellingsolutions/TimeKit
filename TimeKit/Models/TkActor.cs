using System.Collections.Generic;

namespace TimeKit.Models
{
    public class TkActor : TkIActor
    {
        public ulong Id { get; set; }
        public string Key { get; set; }
        public string DisplayName { get; set; }

        public IEnumerable<TkICapability> Capabilities { get; set; } = new List<TkICapability>();

        public TkActor(ulong id, string key, string displayName, IEnumerable<TkICapability> capabilities)
        {
            Id = id;
            Key = key;
            DisplayName = displayName;
            Capabilities = capabilities ?? new List<TkICapability>();
        }

        public TkActor(ulong id, string key, string displayName)
        {
            Id = id;
            Key = key;
            DisplayName = displayName;
        }
    }
}
