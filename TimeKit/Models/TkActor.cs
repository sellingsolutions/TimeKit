using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public class TkActor : TkIActor
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }

        public IEnumerable<TkICapability> Capabilities { get; set; }

        public TkActor(string key, string displayName, IEnumerable<TkICapability> capabilities)
        {
            Key = key;
            DisplayName = displayName;
            Capabilities = capabilities;
        }
        public TkActor(string key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }
    }
}
