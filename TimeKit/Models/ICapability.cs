using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Models
{
    public interface ICapability: IEqualityComparer
    {
        string Key { get; set; }
        string Name { get; set; }
    }
}
