using System;
using System.Collections.Generic;
using System.Linq;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TkSolution
    {
        // When all of the actors in the responses array can work together
        public TkTimeSet MutualVacancy { get; set; }

        public TkTimeSet MutualSchedule { get; set; }

        public TkResourceRequestSolutionGroup Group { get; set; }

        // Enumerates all of the actors and their respective vacancies
        public TkResourceResponse[] Responses { get; set; }

        public TkSolution(
            TkTimeSet mutualVacancy,
            TkResourceRequestSolutionGroup group, 
            TkResourceResponse[] responses)
        {
            MutualVacancy = mutualVacancy;
            MutualSchedule = TkScheduler.TryScheduling(group.Tasks, mutualVacancy);
            Group = group;
            Responses = responses;
        }
    }
}
