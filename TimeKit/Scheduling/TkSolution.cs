using System.Linq;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TkSolution
    {
        // When all of the actors in the responses array can work together
        public TkTimeSet MutualVacancy { get; set; }

        public TkRequest Request { get; set; }

        // Enumerates all of the actors and their respective vacancies
        public TkResourceResponse[] Responses { get; set; }

        public TkSolution(TkTimeSet mutualVacancy, TkRequest request, TkResourceResponse[] responses)
        {
            MutualVacancy = mutualVacancy;
            Request = request;
            Responses = responses;
        }

        public override string ToString()
        {
            var actors = string.Join(", ", Responses.Select(o => $"{o.Actor.DisplayName}"));
            return $"{actors} \n{MutualVacancy}";
        }
    }
}
