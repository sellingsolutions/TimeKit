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
            IsNull      = intervals.Length == 0;
            Id          = Guid.NewGuid().ToString();
            _intervals  = intervals.OrderBy(o => o.min).ToArray();
        }

        public Interval[]  GetOrderedIntervals()
        {
            return _intervals.OrderBy(o=>o.min).ToArray();
        }

        public TimeSet Copy()
        {
            var copy = new Interval[_intervals.Length];
            for (var i=0;i<_intervals.Length;i++)
            {
                var interval = _intervals[i];
                copy[i] = new Interval(interval.min, interval.max);
            }

            return new TimeSet(copy);
        }

        public int Count()
        {
            if (IsEmpty())
                return 0;

            return _intervals.Count();
        }

        public bool IsEmpty()
        {
            return _intervals == null || !_intervals.Any();
        }

        public long Ticks ()
        {
            if (IsEmpty())
                return 0;

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

        public void Remove(Interval interval)
        {
            _intervals = _intervals.Where(o => o.id != interval.id).ToArray();
        }

        public Interval ElementAt(int index)
        {
            if (index >= Count())
                return Interval.Null();

            return _intervals[index];
        }

        public int indexOf(Interval interval)
        {
            if (string.IsNullOrEmpty(interval.id))
            {
                return -1;
            }

            for (var i = 0; i < Count(); i++)
            {
                var _interval = ElementAt(i);
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

        public Interval ExtractInterval(TimeSpan timeSpan)
        {
            var orderedIntervals = GetOrderedIntervals();
            var intervalToExtractFrom = orderedIntervals.FirstOrDefault(o => o.Length() > timeSpan);

            if (intervalToExtractFrom.isNull)
            {
                return Interval.Null();
            }

            var index = indexOf(intervalToExtractFrom);
            var extractedInterval = new Interval();

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
                    _intervals = GetOrderedIntervals();
                }
            }

            return extractedInterval;
        }
    
        public static TimeSet For(DateTime min, DateTime max)
        {
            return new TimeSet (new [] { new Interval ( min, max ) });
        }

        public static TimeSet ForWeek(long weekNumber)
        {
            var min = DateTimeExtensions.GetFirstDayOfWeek(weekNumber);
            var max = min.AddDays(7);
            return For(min, max);
        }

        private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

        private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

        // Returns 45 hours since we're not excluding the lunch hour
        public static TimeSet WorkWeek(long weekNumber)
        {
            var set = new TimeSet ( new Interval[5] );
            var day = DateTimeExtensions.GetFirstDayOfWeek(weekNumber);
            for (var i = 0; i < 5; i++)
            {
                var startsAt = DateTime.SpecifyKind(new DateTime(day.Year, day.Month, day.Day, 6, 0, 0), DateTimeKind.Utc);
                var endsAt = DateTime.SpecifyKind(new DateTime(day.Year, day.Month, day.Day, 15, 0, 0), DateTimeKind.Utc);
                
                set._intervals[i] = new Interval(startsAt, endsAt);
                day = day.AddDays(1);
            }
            return set;
        }

        #region Set Operations

        public static TimeSet Intersect(TimeSet first, TimeSet second)
        {
            IEnumerable<Interval> intersect(TimeSet a, TimeSet b)
            {
                int aIndex = 0, bIndex = 0;
                while (aIndex < a.Count() &&
                       bIndex < b.Count())
                {
                    var aInterval = a.ElementAt(aIndex);
                    var bInterval = b.ElementAt(bIndex);

                    // If A doesn't contain B
                    if (!aInterval.Overlaps(bInterval))
                    {
                        if (aInterval < bInterval) aIndex++;
                        else bIndex++;
                    }
                    // If A contains B, return B
                    else if (aInterval.Encloses(bInterval))
                    {
                        yield return bInterval;
                        bIndex++;
                    }
                    // If B contains A, return A
                    else if (bInterval.Encloses(aInterval))
                    {
                        yield return aInterval;
                        aIndex++;
                    }
                    // If A and B are separate intervals
                    else
                    {
                        var highestMin = Max(aInterval.min, bInterval.min);
                        var lowestMax = Min(aInterval.max, bInterval.max);

                        var interval = new Interval(highestMin, lowestMax);
                        if (interval.Length() > TimeSpan.Zero)
                            yield return interval;

                        if (aInterval < bInterval) aIndex++;
                        else bIndex++;
                    }
                }
            }

            var intersection = intersect(first, second).ToArray();
            return new TimeSet(intersection);
        }

        // The complement of B, ∁B. Is everything that's outside of B
        public static TimeSet Complement(TimeSet set)
        {
            IEnumerable<Interval> complement(TimeSet s)
            {
                var t0 = DateTime.MinValue;
                foreach (var interval in s.GetOrderedIntervals())
                {
                    var t1 = interval.min;
                    if ((t0 - t1).Ticks != 0)
                    {
                        var complementInterval = new Interval(t0, t1);
                        yield return complementInterval;
                    }
                    t0 = interval.max;
                }

                if ((DateTime.MaxValue - t0).Ticks != 0)
                {
                    var lastInterval = new Interval(t0, DateTime.MaxValue);
                    yield return lastInterval;
                }
            }

            var complementSet = complement(set).ToArray();
            return new TimeSet(complementSet);
        }

        // A - B equals the intersection between A and the complement of B
        public static TimeSet Difference(TimeSet a, TimeSet b)
        {
            // A - B = A ∩ ∁B
            var bComplement = Complement(b);
            var diff = Intersect(a, bComplement);
            return diff;
        }

        // Does not join overlapping intervals
        // if an interval in A already contains the space made up by an interval in B, it'll be ignored
        public static TimeSet Union(TimeSet a, TimeSet b)
        {
            if (b.IsNull || b.IsEmpty())
                return a;
            if (a.IsNull || a.IsEmpty())
                return b;

            // The size of the union between a and b will at max be a.count + b.count
            var union = new Interval[a.Count() + b.Count()];
            var unionIndex = a.Count();

            // Set the union equal to A
            for (var i = 0; i < a._intervals.Length; i++)
            {
                union[i] = a._intervals[i];
            }

            // Only insert intervals from B into the union
            // if no interval in A claims that space 
            for (var i = 0; i < b._intervals.Length; i++)
            {
                var bInterval = b._intervals[i];
                var exists = false;

                for (var j = 0; j < a._intervals.Length; j++)
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

        #endregion


        public static bool operator ==(TimeSet a, TimeSet b)
        {
            if (a._intervals.Length != b._intervals.Length)
                return false;
            for (var i = 0; i < a._intervals.Length; i++)
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
            if (obj is TimeSet ts)
            {
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
