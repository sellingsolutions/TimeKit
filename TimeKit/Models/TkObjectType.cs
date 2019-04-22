using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public class TkObjectType: TkIObjectType
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public TkObjectType(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
