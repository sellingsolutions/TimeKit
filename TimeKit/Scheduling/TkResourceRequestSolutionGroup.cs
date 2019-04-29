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
        public int id { get; set; }

        public TkObjectType ObjectType { get; set; }
        public List<long> WeekNumbers { get; set; }

        public long NoOfObjects { get; set; }
        public long MinutesRequiredPerObject { get; set; }

        public TimeSpan TimeSpanRequiredPerObject => TimeSpan.FromMinutes(MinutesRequiredPerObject);
        public long TicksRequiredPerObject => TimeSpanRequiredPerObject.Ticks;

        public long TotalTicksRequired => TimeSpan.FromMinutes(MinutesRequiredPerObject * NoOfObjects).Ticks;

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
            
        }

        public void ReplaceRow(TkResourceRequestSolutionRow row1, TkResourceRequestSolutionRow row2)
        {
            var index = Rows.IndexOf(row1);
            Rows[index] = row2;
        }
        
        /// <summary>
        /// Returns a list of solutions where each solution contains a list of actors and their mutual vacancies
        ///
        /// 
        /// The recursive algorithm below simply iterates each row(role) and its responses
        /// 1. Checks if the response vacancy can hold the TicksRequired, if not skip that response
        /// 2. Makes sure that we're not assigning the same actor to different roles (Will be changed!)
        /// 3. Passes the vacancy on to the next row
        ///
        /// Each pass runs an intersection on the previous row's vacancy and the current row's vacancy
        /// Once we've passed through all of the rows, we will have the intersection between all of the row vacancies
        /// Which means that we'll have a TimeSet that holds the mutual vacancies for the resources that can perform the required roles 
        /// 
        /// </summary>
        /// <returns> A list of solutions </returns>
        public IEnumerable<TkSolution> Solve(int rowIndex = 0)
        {
            var currentRow = Rows[rowIndex];

            // base case, exits when we hit the last index
            if (rowIndex == Rows.Count - 1)
            {
                foreach (var response in currentRow.Responses)
                {
                    yield return new TkSolution(response.Vacancy, new[] { response });
                }
            }

            // recursive case, iterates all of the rows and their responses
            else
            {
                foreach (var nextRowSolution in Solve(rowIndex + 1))
                {
                    var vacancyFromNextRow = nextRowSolution.MutualVacancy; 
                    var responsesFromNextRow = nextRowSolution.Responses;

                    foreach (var responseFromCurrentRow in currentRow.Responses)
                    {
                        // 1. Check that there's sufficient overlap in vacancies.
                        // The current row and the next row must have mutual vacancies
                        var mutualVacancies = TimeSet.Intersect(vacancyFromNextRow, responseFromCurrentRow.Vacancy);
                        if (mutualVacancies.GetOrderedIntervals().Where(o => o.Length().Ticks >= TicksRequiredPerObject)
                                .Sum(o => o.Length().Ticks) < TotalTicksRequired)
                            continue;

                        // 2. Check that we don't reuse the same actor twice.
                        // if (responsesFromNextRow.Any(o => o.Actor == responseFromCurrentRow.Actor))
                        //    continue;

                        var responsesToPassOn = new TkResourceResponse[responsesFromNextRow.Length + 1];

                        // Move the previous responses to index 1 and forward, leaving index 0 empty
                        Array.Copy(responsesFromNextRow,
                            0,
                            responsesToPassOn,
                            1,
                            responsesFromNextRow.Length);

                        // Add the response and pass it on to the next row
                        responsesToPassOn[0] = responseFromCurrentRow;

                        yield return new TkSolution(mutualVacancies, responsesToPassOn);
                    }
                }
            }
        }
    }
}
