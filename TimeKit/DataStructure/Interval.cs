using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.DataStructure
{
    public struct Interval
    {
        public bool isNull => id == null || min.Ticks == 0 || max.Ticks == 0;
        public string id;
        public DateTime min, max; // [min,max]
        
        public Interval(DateTime start, DateTime end)
        {
            id = Guid.NewGuid().ToString();
            min = start;
            max = end;
        }

        public static Interval Null()
        {
            return new Interval();
        }

        public TimeSpan Length()
        {
            return max - min;
        } 

        public bool Contains(Interval other)
        {
            return Contains(other.min) && Contains(other.max);
        }

        public bool Contains(DateTime point)
        {
            return point >= min && point <= max;
        }

        // TODO: Why does other.Overlaps(this); lead to infinite recursion?
        public bool Overlaps(Interval other)
        {
            return (Contains(other.min) || Contains(other.max));// || other.Overlaps(this);
        }

        public bool Encloses(Interval other)
        {
            return Contains(other.min) && Contains(other.max);
        }

        public static bool operator <(Interval a, Interval b)
        {
            return a.min < b.min;
        }

        public static bool operator >(Interval a, Interval b)
        {
            throw new Exception();
        }

        public static bool operator ==(Interval a, Interval b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(Interval a, Interval b)
        {
            return !(a == b);
        }

        public static Interval operator -(Interval a, Interval b)
        {
            var newMax = a.max.AddTicks(-b.Length().Ticks);
            return new Interval( a.min, newMax );
        }
 
        public Interval ExtractInterval(TimeSpan timeSpan)
        {
            var newDatePosition = DateTime.MinValue.AddTicks(max.Ticks - timeSpan.Ticks);
            var extractedInterval = new Interval(newDatePosition, max);
            
            return extractedInterval;
        }

        public void SubtractTimeSpan(TimeSpan timeSpan)
        {
            var newDatePosition = DateTime.MinValue.AddTicks(max.Ticks - timeSpan.Ticks);
            max = newDatePosition;
        }
    }
}
