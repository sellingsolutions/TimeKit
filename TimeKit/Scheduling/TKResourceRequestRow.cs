using System.Collections.Generic;
using TimeKit.DataStructure;

namespace TimeKit.Scheduling
{
    public class TKResourceRequestRow
    {
        public TKResourceRequest Request { get; set; }
        public List<TKResourceResponse> Responses { get; set; }

        public TKResourceRequestRow(TKResourceRequest request, List<TKResourceResponse> responses)
        {
            Request = request;
            Responses = responses;
        }

        // We probably need a depth first search where we find compatible nodes and switch branch on failure
        // We have an array of responses for each row
        // For each response in the first row, we need to check every single response in all the other rows..

        public List<((TKResourceRequestRow row, TKResourceResponse res), (TKResourceRequestRow row, TKResourceResponse res))> Compatible(TKResourceRequestRow other)
        {
            var compatible = new List<((TKResourceRequestRow row, TKResourceResponse res), (TKResourceRequestRow row, TKResourceResponse res))>();
            foreach (var response in Responses)
            {
                foreach (var otherResponse in other.Responses)
                {
                    if (response.Actor.Key == otherResponse.Actor.Key)
                        continue;

                    var mutualSlots = TimeSet.Intersect(response.Vacancy, otherResponse.Vacancy);

                    // Given the intersection of our two vacancies
                    // is there enough ticks between the two of us to work together?
                    if (mutualSlots.Ticks() <= Request.TicksRequired.Ticks)
                    {
                        compatible.Add(((this, response), (other, otherResponse)));
                    }
                }
            }

            return compatible;
        }
    }
}
