using System;
using System.Collections.Generic;
using System.Linq;
using TimeKit.DataStructure;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceRequestSolutionGroup
    {
        public int Id { get; set; }
        
        public IEnumerable<TkTask> Tasks { get; set; }
        public long TotalTicksRequired { get; set; }

        public List<TkResourceRequestSolutionRow> Rows { get; set; } = new List<TkResourceRequestSolutionRow>();

        public TkResourceRequestSolutionGroup(IEnumerable<TkTask> tasks)
        {
            Tasks = tasks;
            TotalTicksRequired = tasks.Sum(o => o.PlannedDuration);
        }

        public bool IsValid()
        {
            return Tasks != null;
        }

        public void AddRow(TkResourceRequestSolutionRow row)
        {
            row.Group = this;
            Rows.Add(row);            
        }

        public IEnumerable<TkSolution> GetSolution(TkSolutionType type)
        {
            if (type == TkSolutionType.MutualSchedule)
                return GetMutualSchedule();
            else
                return GetIndividualSchedule();
        }

        public IEnumerable<TkSolution> GetIndividualSchedule()
        {
            var solutions = new List<TkSolution>();
            foreach (var row in Rows)
            {
                foreach (var response in row.Responses)
                {
                    var solution = new TkSolution(response.Vacancy, this, new[] { response });
                    solutions.Add(solution);
                }
            }

            return solutions;
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
        public IEnumerable<TkSolution> GetMutualSchedule(int rowIndex = 0)
        {
            var solutions = new List<TkSolution>();

            var currentRow = Rows[rowIndex];

            // base case, exits when we hit the last index
            if (rowIndex == Rows.Count - 1)
            {
                foreach (var response in currentRow.Responses)
                {
                    var solution = new TkSolution(response.Vacancy, this, new[] { response });
                    solutions.Add(solution);
                }
            }

            // recursive case, iterates all of the rows and their responses
            else
            {
                foreach (var nextRowSolution in GetMutualSchedule(rowIndex + 1))
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
                        foreach (var task in Tasks)
                        {
                            var vacantSlot = mutualVacantIntervals
                                .FirstOrDefault(o => o.Length().Ticks >= task.Duration.Ticks);

                            if (vacantSlot != null)
                            {
                                vacantTicks += vacantSlot.Length().Ticks;
                                mutualVacantIntervals.Remove(vacantSlot);
                            }
                        }
                        if (vacantTicks <= TotalTicksRequired)
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

                        var solution = new TkSolution(mutualVacancies, this, responsesToPassOn);
                        solutions.Add(solution);
                    }
                }
            }

            return solutions;
        }
    }
}
