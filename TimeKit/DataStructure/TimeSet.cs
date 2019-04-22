using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKit.Extensions;

namespace TimeKit.DataStructure
{
    public struct TimeSet
    {
        public bool IsNull;
        public string Id;
        // 1. no directly adjacent intervals
        // 2. intervals sorted by ascending min
        // 3. intervals do not overlap
        private Interval[] _intervals;

        public static TimeSet Null()
        {
            var empty = new TimeSet(new Interval[0]);
            return empty;
        }
          
        public TimeSet(Interval[] intervals)
        {
            IsNull = intervals.Length == 0;
            Id = Guid.NewGuid().ToString();
            _intervals = intervals;
        }

        public Interval[]  GetIntervals()
        {
            return _intervals;
        }

        public TimeSet Copy()
        {
            var copy = new Interval[_intervals.Length];
            for (int i=0;i<_intervals.Length;i++)
            {
                var interval = _intervals[i];
                copy[i] = new Interval(interval.min, interval.max);
            }

            return new TimeSet(copy);
        }

        public int Count()
        {
            return _intervals.Count();
        }

        public long Ticks ()
        {
            var ticks = _intervals.Sum(interval => interval.Length().Ticks);
            return ticks;
        }

        public long Seconds()
        {
            return (long)(Ticks() / Math.Pow(10, 7));
        } 

        public long Minutes ()
        { 
            return Seconds() / 60;
        }
        
        public Interval ExtractInterval(TimeSpan timeSpan)
        {
            var orderedIntervals = _intervals.OrderBy(o => o.min);
            var intervalToExtractFrom = orderedIntervals.FirstOrDefault(o => o.Length() > timeSpan);

            if (intervalToExtractFrom.isNull)
            {
                return new Interval();
            }

            var index = indexOf(intervalToExtractFrom);
            Interval extractedInterval = new Interval();

            if (index != -1)
            {
                extractedInterval = intervalToExtractFrom.ExtractInterval(timeSpan);

                intervalToExtractFrom.SubtractTimeSpan(timeSpan);
                
                var updatedInterval = new Interval() {
                    id = intervalToExtractFrom.id,
                    min = intervalToExtractFrom.min,
                    max = intervalToExtractFrom.max
                };

                // If we have less than 5 minutes left, remove it
                if (updatedInterval.Length() < TimeSpan.FromMinutes(5))
                {
                    Remove(updatedInterval);
                }
                else
                {
                    _intervals[index] = updatedInterval;
                }
            }

            return extractedInterval;
        }

        public void Remove(Interval interval)
        {
            _intervals = _intervals.ToList().Where(o => o.id != interval.id).ToArray();
        }

        public int indexOf(Interval interval)
        {
            if (string.IsNullOrEmpty(interval.id))
            {
                return -1;
            }

            for (int i=0; i<_intervals.Count(); i++)
            {
                var _interval = _intervals[i];
                if (_interval.id == interval.id)
                {
                    return i;
                }
            }

            return -1;
        }

        public override string ToString()
        {
            return "{ " + string.Join("; ", _intervals.Select(o => $"[{o.min}, {o.max}]")) + " }";
        }

        public static TimeSet For(DateTime min, DateTime max)
        {
            return new TimeSet (new Interval[] { new Interval ( min, max ) });
        }

        public static TimeSet ForWeek(long weekNumber)
        {
            var min = DateTimeExtensions.GetFirstDayOfWeek(weekNumber);
            var max = min.AddDays(7);
            return For(min, max);
        }

        // Returns 45 hours since we're not excluding the lunch hour
        public static TimeSet WorkWeek(long weekNumber)
        {
            var set = new TimeSet ( new Interval[5] );
            var day = DateTimeExtensions.GetFirstDayOfWeek(weekNumber);
            for (int i = 0; i < 5; i++)
            {
                var startsAt = DateTime.SpecifyKind(new DateTime(day.Year, day.Month, day.Day, 6, 0, 0), DateTimeKind.Utc);
                var endsAt = DateTime.SpecifyKind(new DateTime(day.Year, day.Month, day.Day, 15, 0, 0), DateTimeKind.Utc);
                
                set._intervals[i] = new Interval(startsAt, endsAt);
                day = day.AddDays(1);
            }
            return set;
        }


        // assumes intervals not overlapping!
        public static TimeSet For(IEnumerable<Tuple<DateTime, DateTime>> intervals)
        {
            var intvals = intervals.OrderBy(o => o.Item1).Select(o => new Interval(o.Item1, o.Item2)).ToArray();
            return new TimeSet (intvals);
        }

        private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

        private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

        public static TimeSet Intersect(TimeSet first, TimeSet second)
        {
            IEnumerable<Interval> intersect(TimeSet a, TimeSet b)
            {
                int aIndex = 0, bIndex = 0;
                while (aIndex < a._intervals.Length &&
                       bIndex < b._intervals.Length)
                {
                    var aInterval = a._intervals[aIndex];
                    var bInterval = b._intervals[bIndex];
                    if (!aInterval.Overlaps(bInterval))
                    {
                        if (aInterval < bInterval) aIndex++;
                        else bIndex++;
                    }
                    else if (aInterval.Encloses(bInterval))
                    {
                        yield return bInterval;
                        bIndex++;
                    }
                    else if (bInterval.Encloses(aInterval))
                    {
                        yield return aInterval;
                        aIndex++;
                    }
                    else
                    {
                        yield return new Interval(Max(aInterval.min, bInterval.min), Min(aInterval.max, bInterval.max));
                        if (aInterval < bInterval) aIndex++;
                        else bIndex++;
                    }
                }
            }
            return new TimeSet ( intersect(first, second).ToArray() );
        }

        public static TimeSet Complement(TimeSet set)
        {
            IEnumerable<Interval> complement(TimeSet s)
            {
                DateTime t0 = DateTime.MinValue;
                foreach (var interval in s._intervals)
                {
                    var t1 = interval.min;
                    if ((t0 - t1).Ticks != 0)
                        yield return new Interval ( t0, t1 );
                    t0 = interval.max;
                }
                if ((DateTime.MaxValue - t0).Ticks != 0)
                {
                    yield return new Interval (t0, DateTime.MaxValue);
                }
            }
            return new TimeSet (complement(set).ToArray());
        }

        public static TimeSet Difference(TimeSet a, TimeSet b)
        {
            return Intersect(a, Complement(b));
        }

        // Does not join overlapping intervals
        // if an interval in A already contains the space made up by an interval in B, it'll be ignored
        public static TimeSet Union(TimeSet a, TimeSet b)
        {
            // The size of the union between a and b will at max be a.count + b.count
            var union = new Interval[a.Count() + b.Count()];
            var unionIndex = a.Count();

            // Set the union equal to A
            for (int i = 0; i < a._intervals.Length; i++)
            {
                union[i] = a._intervals[i];
            }
            
            // Only insert intervals from B into the union
            // if no interval in A claims that space 
            for (int i = 0; i < b._intervals.Length; i++)
            {
                var bInterval = b._intervals[i];
                var exists = false;

                for(int j=0; j < a._intervals.Length;j++)
                {
                    var aInterval = a._intervals[j];
                    if (aInterval.Contains(bInterval))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    union[unionIndex] = bInterval;
                    unionIndex++;
                }
            }

            return new TimeSet(union);
        }

        public static bool operator ==(TimeSet a, TimeSet b)
        {
            if (a._intervals.Length != b._intervals.Length)
                return false;
            for (int i = 0; i < a._intervals.Length; i++)
            {
                var aInterval = a._intervals[i];
                var bInterval = b._intervals[i];
                if (aInterval.min != bInterval.min ||
                    aInterval.max != bInterval.max)
                    return false;
            }
            return true;
        }

        public static bool operator !=(TimeSet a, TimeSet b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TimeSet)
            {
                var ts = (TimeSet)obj;
                return Id == ts.Id;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
