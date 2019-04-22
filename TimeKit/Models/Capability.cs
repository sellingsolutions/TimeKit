using System;
using System.Collections;

namespace TimeKit.Models
{
    public class Capability : ICapability
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return Name == ((ICapability)obj).Name;
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(Key);
        }

        public override string ToString()
        {
            return $"{Key} {Name}";
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            return ((ICapability)x).Name == ((ICapability)y).Name;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return Convert.ToInt32(((ICapability)obj).Key);
        }
    }
}
