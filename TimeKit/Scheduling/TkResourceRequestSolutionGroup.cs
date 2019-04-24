using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKit.DataStructure;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceRequestSolutionGroup
    {
        public TkObjectType ObjectType { get; set; }
        public List<long> WeekNumbers { get; set; }

        public long NoOfObjects { get; set; }
        public long MinutesRequiredPerObject { get; set; }

        public TimeSpan TicksRequiredPerObject => TimeSpan.FromMinutes(MinutesRequiredPerObject);
        public TimeSpan TotalTicksRequired => TimeSpan.FromMinutes(MinutesRequiredPerObject * NoOfObjects);

        public List<TkActor> AvailableActors { get; set; }
        public List<TkProcess> AvailableProcesses { get; set; }

        public List<TkResourceRequestSolutionRow> Rows { get; set; } = new List<TkResourceRequestSolutionRow>();

        public TkResourceRequestSolutionGroup(
            TkObjectType objectType, 
            long noOfObjects, 
            long minutesRequiredPerObject, 
            List<long> weekNumbers, 
            List<TkActor> availableActors, 
            List<TkProcess> availableProcesses)
        {

            ObjectType = objectType;
            NoOfObjects = noOfObjects;
            MinutesRequiredPerObject = minutesRequiredPerObject;
            WeekNumbers = weekNumbers;
            AvailableActors = availableActors;
            AvailableProcesses = availableProcesses;
        }

        public bool IsValid()
        {
            if (AvailableActors == null ||
                AvailableProcesses == null ||
                NoOfObjects == 0 ||
                MinutesRequiredPerObject == 0 ||
                WeekNumbers == null)
            {
                return false;
            }

            return AvailableActors.Any() &&
                   AvailableProcesses.Any() &&
                   WeekNumbers.Any();
        }

        public void AddRow(TkResourceRequestSolutionRow row)
        {
            row.Group = this;
            Rows.Add(row);
            row.no = Rows.Count();
        }
        
        public IEnumerable<TkSolution> Solve(int rowIndex = 0)
        {
            var ticksPerObject = TimeSpan.FromMinutes(MinutesRequiredPerObject).Ticks;
            var totalTicksRequired = ticksPerObject * NoOfObjects;

            var row = Rows[rowIndex];

            // base case
            if (rowIndex == Rows.Count - 1)
            {
                foreach (var response in row.Responses)
                {
                    yield return new TkSolution(response.Vacancy, new[] { response });
                }
            }

            // recursive case
            else
            {
                foreach (var currentSolution in Solve(rowIndex + 1))
                {
                    var vacancyFromPreviousRow = currentSolution.MutualVacancy; 
                    var responsesFromPreviousRow = currentSolution.Responses;

                    foreach (var response in row.Responses)
                    {
                        // 1. Check that there's sufficient overlap in vacancies.
                        var vacancy = TimeSet.Intersect(vacancyFromPreviousRow, response.Vacancy);
                        if (vacancy.GetOrderedIntervals().Where(o => o.Length().Ticks >= ticksPerObject)
                                .Sum(o => o.Length().Ticks) < totalTicksRequired)
                            continue;

                        // 2. Check that we don't reuse the same actor twice.
                        if (responsesFromPreviousRow.Any(o => o.Actor == response.Actor))
                            continue;

                        var updatedResponses = new TkResourceResponse[responsesFromPreviousRow.Length + 1];
                        Array.Copy(responsesFromPreviousRow,
                            0,
                            updatedResponses,
                            1,
                            responsesFromPreviousRow.Length);

                        updatedResponses[0] = response;
                        yield return new TkSolution(vacancy, updatedResponses);
                    }
                }
            }
        }
    }
}
