using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public interface IRole
    {
        string Key { get; set; }
        string Name { get; set; }
    }
}
