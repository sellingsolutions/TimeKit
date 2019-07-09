using System;
using System.Collections.Generic;
using System.Linq;
using TimeKit.Extensions;
using TimeKit.Models;

namespace TimeKit.DataStructure
{
    public struct TkTimeSet
    {
        public bool IsNull;
        public string Id;
        // 1. no directly adjacent tkIntervals
        // 2. tkIntervals sorted by ascending start
        // 3. tkIntervals do not overlap
        private TkInterval[] _tkIntervals;

        public static TkTimeSet Null()
        {
            var empty = new TkTimeSet(new TkInterval[0]);
            return empty;
        }
          
        public TkTimeSet(TkInterval[] tkIntervals)
        {
            IsNull      = tkIntervals.Length == 0;
            Id          = Guid.NewGuid().ToString();
            _tkIntervals  = tkIntervals.OrderBy(o => o.start).ToArray();
        }

        public TkInterval[]  GetOrderedIntervals()
        {
            return _tkIntervals.OrderBy(o=>o.start).ToArray();
        }

        public TkTimeSet Copy()
        {
            var copy = new TkInterval[_tkIntervals.Length];
            for (var i=0;i<_tkIntervals.Length;i++)
            {
                var interval = _tkIntervals[i];
                copy[i] = new TkInterval(interval.start, interval.end);
            }

            return new TkTimeSet(copy);
        }

        public IEnumerable<(DateTime, DateTime)> ToDateList()
        {
            return GetOrderedIntervals().Select(o => (min: o.start, max: o.end));
        }

        public int Count()
        {
            if (IsEmpty())
                return 0;

            return _tkIntervals.Count();
        }

        public bool IsEmpty()
        {
            return _tkIntervals == null || !_tkIntervals.Any();
        }

        public long Ticks ()
        {
            if (IsEmpty())
                return 0;

            var ticks = _tkIntervals.Sum(interval => interval.Length().Ticks);
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

        public void Remove(TkInterval tkInterval)
        {
            _tkIntervals = _tkIntervals.Where(o => o.id != tkInterval.id).ToArray();
        }

        public TkInterval ElementAt(int index)
        {
            if (index >= Count())
                return TkInterval.Null();

            return _tkIntervals[index];
        }

        public int indexOf(TkInterval tkInterval)
        {
            if (string.IsNullOrEmpty(tkInterval.id))
            {
                return -1;
            }

            for (var i = 0; i < Count(); i++)
            {
                var _interval = ElementAt(i);
                if (_interval.id == tkInterval.id)
                {
                    return i;
                }
            }

            return -1;
        }

        public override string ToString()
        {
            return "{ " + string.Join("; ", _tkIntervals.Select(o => $"[{o.start}, {o.end}] \n")) + " } ";
        }

        public TkInterval ExtractInterval(TimeSpan timeSpan)
        {
            var orderedIntervals = GetOrderedIntervals();
            var intervalToExtractFrom = orderedIntervals.FirstOrDefault(o => o.Length() > timeSpan);

            if (intervalToExtractFrom.isNull)
            {
                return TkInterval.Null();
            }

            var index = indexOf(intervalToExtractFrom);
            var extractedInterval = new TkInterval();

            if (index != -1)
            {
                extractedInterval = intervalToExtractFrom.ExtractInterval(timeSpan);

                intervalToExtractFrom.SubtractTimeSpan(timeSpan);
                
                var updatedInterval = new TkInterval() {
                    id = intervalToExtractFrom.id,
                    start = intervalToExtractFrom.start,
                    end = intervalToExtractFrom.end
                };

                // If we have less than 5 minutes left, remove it
                if (updatedInterval.Length() < TimeSpan.FromMinutes(5))
                {
                    Remove(updatedInterval);
                }
                else
                {
                    _tkIntervals[index] = updatedInterval;
                    _tkIntervals = GetOrderedIntervals();
                }
            }

            return extractedInterval;
        }
    
        public static TkTimeSet For(DateTime min, DateTime max)
        {
            return new TkTimeSet (new [] { new TkInterval ( min, max ) });
        }

        public static TkTimeSet ForWeek(long weekNumber)
        {
            var min = DateTimeExtensions.GetFirstDayOfWeek(weekNumber);
            var max = min.AddDays(7);
            return For(min, max);
        }

        private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

        private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

        // Returns 45 hours since we're not excluding the lunch hour
        public static TkTimeSet WorkWeek(long weekNumber)
        {
            var set = new TkTimeSet ( new TkInterval[5] );
            var day = DateTimeExtensions.GetFirstDayOfWeek(weekNumber);
            for (var i = 0; i < 5; i++)
            {
                var workDay = WorkDay(day, TkWorkWeekConfig.Default);
                set._tkIntervals[i] = workDay;
                day = day.AddDays(1);
            }
            return set;
        }

        public static TkTimeSet WorkWeeks(DateTime start, DateTime end, TkWorkWeekConfig config)
        {
            var intervals = new List<TkInterval>();
            var current = start;
            while (current <= end)
            {
                if (config.WorkDays.Contains(current.DayOfWeek))
                {
                    intervals.Add(WorkDay(current, config));
                }

                current = current.AddDays(1);
            }

            return new TkTimeSet(intervals.ToArray());
        }

        public static TkInterval WorkDay(DateTime day, TkWorkWeekConfig config)
        {
            var startsAt = DateTime.SpecifyKind(new DateTime(
                day.Year, 
                day.Month, 
                day.Day, 
                config.WorkDayStartHourUtc, 
                0, 
                0),
                DateTimeKind.Utc);

            var endsAt = DateTime.SpecifyKind(new DateTime(
                day.Year, 
                day.Month, 
                day.Day, 
                config.WorkDayStartHourUtc + config.WorkDayDuration, 
                0, 
                0), 
                DateTimeKind.Utc);

            return new TkInterval(startsAt, endsAt);
        }

        #region Set Operations

        public static TkTimeSet Intersect(TkTimeSet first, TkTimeSet second)
        {
            IEnumerable<TkInterval> intersect(TkTimeSet a, TkTimeSet b)
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
                    // If A and B are separate tkIntervals
                    else
                    {
                        var highestMin = Max(aInterval.start, bInterval.start);
                        var lowestMax = Min(aInterval.end, bInterval.end);

                        var interval = new TkInterval(highestMin, lowestMax);
                        if (interval.Length() > TimeSpan.Zero)
                            yield return interval;

                        if (aInterval < bInterval) aIndex++;
                        else bIndex++;
                    }
                }
            }

            var intersection = intersect(first, second).ToArray();
            return new TkTimeSet(intersection);
        }

        // The complement of B, ∁B. Is everything that's outside of B
        public static TkTimeSet Complement(TkTimeSet set)
        {
            IEnumerable<TkInterval> complement(TkTimeSet s)
            {
                var t0 = DateTime.MinValue;
                foreach (var interval in s.GetOrderedIntervals())
                {
                    var t1 = interval.start;
                    if ((t0 - t1).Ticks != 0)
                    {
                        var complementInterval = new TkInterval(t0, t1);
                        yield return complementInterval;
                    }
                    t0 = interval.end;
                }

                if ((DateTime.MaxValue - t0).Ticks != 0)
                {
                    var lastInterval = new TkInterval(t0, DateTime.MaxValue);
                    yield return lastInterval;
                }
            }

            var complementSet = complement(set).ToArray();
            return new TkTimeSet(complementSet);
        }

        // A - B equals the intersection between A and the complement of B
        public static TkTimeSet Difference(TkTimeSet a, TkTimeSet b)
        {
            // A - B = A ∩ ∁B
            var bComplement = Complement(b);
            var diff = Intersect(a, bComplement);
            return diff;
        }

        // Does not join overlapping tkIntervals
        // if an tkInterval in A already contains the space made up by an tkInterval in B, it'll be ignored
        public static TkTimeSet Union(TkTimeSet a, TkTimeSet b)
        {
            if (b.IsNull || b.IsEmpty())
                return a;
            if (a.IsNull || a.IsEmpty())
                return b;

            // The size of the union between a and b will at end be a.count + b.count
            var union = new TkInterval[a.Count() + b.Count()];
            var unionIndex = a.Count();

            // Set the union equal to A
            for (var i = 0; i < a._tkIntervals.Length; i++)
            {
                union[i] = a._tkIntervals[i];
            }

            // Only insert tkIntervals from B into the union
            // if no tkInterval in A claims that space 
            for (var i = 0; i < b._tkIntervals.Length; i++)
            {
                var bInterval = b._tkIntervals[i];
                var exists = false;

                for (var j = 0; j < a._tkIntervals.Length; j++)
                {
                    var aInterval = a._tkIntervals[j];
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

            return new TkTimeSet(union);
        }

        #endregion


        public static bool operator ==(TkTimeSet a, TkTimeSet b)
        {
            if (a._tkIntervals.Length != b._tkIntervals.Length)
                return false;
            for (var i = 0; i < a._tkIntervals.Length; i++)
            {
                var aInterval = a._tkIntervals[i];
                var bInterval = b._tkIntervals[i];
                if (aInterval.start != bInterval.start ||
                    aInterval.end != bInterval.end)
                    return false;
            }
            return true;
        }

        public static bool operator !=(TkTimeSet a, TkTimeSet b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TkTimeSet ts)
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
