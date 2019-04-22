using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public interface IObjectType
    {
        string Key { get; set; }
        string Name { get; set; }
    }
}
