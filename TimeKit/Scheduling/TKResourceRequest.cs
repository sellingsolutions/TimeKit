using System.Collections.Generic;
using System.Linq;
using System;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceRequest
    {
        public TkIRoleType TkIRoleType { get; set; }
        public ICapability RequiredCapability { get; set; }
        public IObjectType ObjectType { get; set; }
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
            ICapability capability, 
            IObjectType objectType, 
            long noOfObjects, 
            long minutesRequiredPerObject,
            List<long> weekNumbers,
            IEnumerable<TkIActor> availableActors,
            IEnumerable<TkIProcess> availableProcesses)
        {
            TkIRoleType = tkIRoleType;
            RequiredCapability = capability;
            ObjectType = objectType;
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
                RequiredCapability == null ||
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
