using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TkSolution
    {
        // When all of the actors in the responses array can work together
        public TkTimeSet MutualVacancy { get; set; }
        // Enumerates all of the actors and their respective vacancies
        public TkResourceResponse[] Responses { get; set; }

        public TkSolution(TkTimeSet mutualVacancy, TkResourceResponse[] responses)
        {
            MutualVacancy = mutualVacancy;
            Responses = responses;
        }

        public override string ToString()
        {
            var actors = string.Join(", ", Responses.Select(o => $"{o.Actor.DisplayName}"));
            return $"{actors} \n{MutualVacancy}";
        }
    }
}
