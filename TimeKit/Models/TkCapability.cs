using System;
using System.Collections;

namespace TimeKit.Models
{
    public class TkCapability : TkICapability
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public TkCapability(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return Name == ((TkCapability)obj).Name;
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
            return ((TkCapability)x).Name == ((TkCapability)y).Name;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return Convert.ToInt32(((TkCapability)obj).Key);
        }
    }
}
