
using TimeKit.DataStructure;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TKResourceResponse
    {
        public TkActor Actor { get; set; }
        public TimeSet Busy { get; set; }
        public TimeSet Vacancy { get; set; }
        public TimeSet Schedule { get; set; }

        public bool IsValid ()
        {
            return Schedule.Count() > 0;
        }

        public TKResourceResponse(TkActor actor, TimeSet busy, TimeSet vacancy, TimeSet schedule)
        {
            Actor = actor;
            Busy = busy;
            Vacancy = vacancy;
            Schedule = schedule;
        }
    }
}
