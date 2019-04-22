using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKit.Scheduling
{
    public class TKResourceRequestGroup
    {
        // R1.Vacancies = R1.Workweek - R1.Busy
        // R2.Vacancies = (R2.Workweek - R2.Busy) **intersection** R1.Vacancies
        // R3.Vacancies = (R3.Workweek - R3.Busy) **intersection** R1.Vacancies **intersection** R2.Vacancies

        // The intersection of all the vacancies is equal to when all of the resources are vacant

        // The brute force solution
        // If you've added 3 roles, you're looking for 3 people.
        // Bygg Week 15 -> returns 4 resources
        // Vent Week 15 -> returns 3 resources
        // El Week 15 -> returns 1 resource
        // 

        public List<TKResourceRequestRow> Rows { get; set; } = new List<TKResourceRequestRow>();

        public List<((TKResourceRequestRow row, TKResourceResponse res), 
            (TKResourceRequestRow row, TKResourceResponse res))> FindCompatibleRows ()
        {
            // Each 
            var compatible = new List<((TKResourceRequestRow row, TKResourceResponse res), (TKResourceRequestRow row, TKResourceResponse res))>();
            
            for (var i = 0; i < Rows.Count(); i++)
            {
                var row = Rows[i];
                for(var j=1;j<Rows.Count()-1;j++)
                {
                    var otherRow = Rows[j];

                    var compatibleResources = row.Compatible(otherRow);
                    compatible.AddRange(compatibleResources);

                    // If the current row is incompatible with any other row, we might as well abort immediately
                    if (!compatibleResources.Any())
                    {
                        // All the rows have to be compatible!
                        return null;
                    }
                }

            }

            return compatible;
        }
    }
}
