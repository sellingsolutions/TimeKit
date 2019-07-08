using System;

namespace TimeKit.DataStructure
{
    public struct TkInterval
    {
        public bool isNull => id == null || start.Ticks == 0 || end.Ticks == 0;
        public string id;
        public DateTime start, end; // [start,end]
        
        public TkInterval(DateTime start, DateTime end)
        {
            id = Guid.NewGuid().ToString();
            this.start = start;
            this.end = end;
        }

        public static TkInterval Null()
        {
            return new TkInterval();
        }

        public TimeSpan Length()
        {
            return end - start;
        } 

        public bool Contains(TkInterval other)
        {
            return Contains(other.start) && Contains(other.end);
        }

        public bool Contains(DateTime point)
        {
            return point >= start && point <= end;
        }

        public bool Overlaps(TkInterval other)
        {
            // Does this interval contain the start or end of the other interval?
            return (Contains(other.start) || Contains(other.end) ||
                    // Does the other interval contain the start or end of this interval?
                    other.Contains(start) || other.Contains(end));
        }

        public bool Encloses(TkInterval other)
        {
            return Contains(other.start) && Contains(other.end);
        }

        public static bool operator <(TkInterval a, TkInterval b)
        {
            return a.start < b.start;
        }

        public static bool operator >(TkInterval a, TkInterval b)
        {
            throw new Exception();
        }

        public static bool operator ==(TkInterval a, TkInterval b)
        {
            return a.start == b.start && a.end == b.end;
        }

        public static bool operator !=(TkInterval a, TkInterval b)
        {
            return !(a == b);
        }

        public static TkInterval operator -(TkInterval a, TkInterval b)
        {
            var newMax = a.end.AddTicks(-b.Length().Ticks);
            return new TkInterval( a.start, newMax );
        }
 
        public TkInterval ExtractInterval(TimeSpan timeSpan)
        {
            var newDatePosition = DateTime.MinValue.AddTicks(end.Ticks - timeSpan.Ticks);
            var extractedInterval = new TkInterval(newDatePosition, end);
            
            return extractedInterval;
        }

        public void SubtractTimeSpan(TimeSpan timeSpan)
        {
            var newDatePosition = DateTime.MinValue.AddTicks(end.Ticks - timeSpan.Ticks);
            end = newDatePosition;
        }
    }
}
