using System.Collections.Generic;
using System.Linq;
using System;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceRequest
    {
        public TkIRoleType TkIRoleType { get; set; }
        public TkICapability RequiredTkICapability { get; set; }
        public TkIObjectType TkIObjectType { get; set; }
        public long NoOfObjects { get; set; }
        public long MinutesRequiredPerObject { get; set; }
        public List<long> WeekNumbers { get; set; }

        public IEnumerable<TkIActor> AvailableActors { get; set; }
        public IEnumerable<TkIProcess> AvailableProcesses { get; set; }

        public TimeSpan TicksRequired => TimeSpan.FromMinutes(MinutesRequiredPerObject * NoOfObjects);

        public TkResourceRequest()
        {

        }

        public TkResourceRequest(
            TkIRoleType tkIRoleType, 
            TkICapability tkICapability, 
            TkIObjectType tkIObjectType, 
            long noOfObjects, 
            long minutesRequiredPerObject,
            List<long> weekNumbers,
            IEnumerable<TkIActor> availableActors,
            IEnumerable<TkIProcess> availableProcesses)
        {
            TkIRoleType = tkIRoleType;
            RequiredTkICapability = tkICapability;
            TkIObjectType = tkIObjectType;
            NoOfObjects = noOfObjects;
            MinutesRequiredPerObject = minutesRequiredPerObject;
            WeekNumbers = weekNumbers;

            AvailableActors = availableActors;
            AvailableProcesses = availableProcesses;
        }

        public bool IsValid ()
        {
            if (AvailableActors == null ||
                AvailableProcesses == null ||
                TkIRoleType == null ||
                RequiredTkICapability == null ||
                NoOfObjects == 0 || 
                MinutesRequiredPerObject == 0 || 
                WeekNumbers == null)
            {
                return false;
            }

            return WeekNumbers.Any();
        }

        public TKResourceResponse Run(TkIActor actor)
        {
            var runner = new TKResourceRequestRunner(this, actor);
            return runner.Run();
        }
    }
}
