using System;
using System.Collections.Generic;
using System.Linq;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TkResourceRequestSolutionGroup
    {
        public int Id { get; set; }
        
        public TkRequest Request { get; set; }

        public List<TkResourceRequestSolutionRow> Rows { get; set; } = new List<TkResourceRequestSolutionRow>();

        public bool IsValid()
        {
    

            return true;
        }

        public void AddRow(TkResourceRequestSolutionRow row)
        {
            row.Group = this;
            Rows.Add(row);            
        }

        
        /// <summary>
        /// Returns a list of solutions where each solution contains a list of actors and their mutual vacancies
        ///
        /// THIS ALGORITHM ONLY RETURNS SOLUTIONS WHERE THE ACTORS CAN BE SCHEDULED AT THE EXACT SAME TIME
        /// 
        /// The recursive algorithm below simply iterates each row(role) and its responses
        /// 1. Checks if the response vacancy can hold the TicksRequired, if not skip that response
        /// 2. Passes the vacancy on to the next row
        ///
        /// Each pass runs an intersection on the previous row's vacancy and the current row's vacancy
        /// Once we've passed through all of the rows, we will have the intersection between all of the row vacancies
        /// Which means that we'll have a TkTimeSet that holds the mutual vacancies for the resources that can perform the required roles 
        /// 
        /// </summary>
        /// <returns> A list of solutions </returns>
        public IEnumerable<TkSolution> Solve(int rowIndex = 0)
        {
            var solutions = new List<TkSolution>();

            var currentRow = Rows[rowIndex];

            // base case, exits when we hit the last index
            if (rowIndex == Rows.Count - 1)
            {
                foreach (var response in currentRow.Responses)
                {
                    solutions.Add(new TkSolution(response.Vacancy, currentRow.Request, new[] { response }));
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
                        var vacancyFromCurrentRow = responseFromCurrentRow.Vacancy;

                        // The current row and the next row must have mutual vacancies
                        var mutualVacancies = TkTimeSet.Intersect(vacancyFromNextRow, vacancyFromCurrentRow);
                        var mutualVacantIntervals = mutualVacancies.GetOrderedIntervals().ToList();

                        // Check that there's sufficient overlap in vacancies
                        long vacantTicks = 0;
                        foreach (var task in Request.Tasks)
                        {
                            var vacantSlot = mutualVacantIntervals
                                .FirstOrDefault(o => o.Length().Ticks >= task.Duration.Ticks);

                            if (vacantSlot != null)
                            {
                                vacantTicks += vacantSlot.Length().Ticks;
                                mutualVacantIntervals.Remove(vacantSlot);
                            }
                        }
                        if (vacantTicks <= Request.TotalTicksRequired)
                            continue;

                        var responsesToPassOn = new TkResourceResponse[responsesFromNextRow.Length + 1];

                        // Move the previous responses to index 1 and forward, leaving index 0 empty
                        Array.Copy(responsesFromNextRow,
                            0,
                            responsesToPassOn,
                            1,
                            responsesFromNextRow.Length);

                        // Add the response and pass it on to the next row
                        responsesToPassOn[0] = responseFromCurrentRow;

                        solutions.Add(new TkSolution(mutualVacancies, currentRow.Request, responsesToPassOn));
                    }
                }
            }

            return solutions;
        }
    }
}
