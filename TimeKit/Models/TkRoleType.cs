using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public class TkRoleType: TkIRoleType
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public TkRoleType(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
