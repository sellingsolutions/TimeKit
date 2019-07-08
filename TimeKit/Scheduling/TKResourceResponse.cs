
using TimeKit.DataStructure;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceResponse
    {
        public int id { get; set; }

        public TkActor Actor { get; set; }
        public TkTimeSet Busy { get; set; }
        public TkTimeSet Vacancy { get; set; }
        public TkTimeSet Schedule { get; set; }

        public bool IsValid ()
        {
            return Schedule.Count() > 0;
        }

        public TkResourceResponse(
            TkActor actor, 
            TkTimeSet busy, 
            TkTimeSet vacancy, 
            TkTimeSet schedule)
        {
            Actor = actor;
            Busy = busy;
            Vacancy = vacancy;
            Schedule = schedule;
        }
    }
}
