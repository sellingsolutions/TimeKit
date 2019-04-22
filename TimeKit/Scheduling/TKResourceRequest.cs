using System.Collections.Generic;
using System.Linq;
using System;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceRequest
    {
        public TkIRoleType RoleType { get; set; }
        public TkICapability RequiredCapability { get; set; }
        public TkIObjectType ObjectType { get; set; }
        public long NoOfObjects { get; set; }
        public long MinutesRequiredPerObject { get; set; }
        public List<long> WeekNumbers { get; set; }

        public List<TkActor> AvailableActors { get; set; }
        public List<TkProcess> AvailableProcesses { get; set; }

        public TimeSpan TicksRequired => TimeSpan.FromMinutes(MinutesRequiredPerObject * NoOfObjects);

        public TkResourceRequest()
        {

        }

        public TkResourceRequest(
            TkIRoleType roleType, 
            TkICapability capability, 
            TkIObjectType objectType, 
            long noOfObjects, 
            long minutesRequiredPerObject,
            List<long> weekNumbers,
            List<TkActor> availableActors,
            List<TkProcess> availableProcesses)
        {
            RoleType = roleType;
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
                RoleType == null ||
                RequiredCapability == null ||
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

        public TKResourceResponse Run(TkActor actor)
        {
            var runner = new TKResourceRequestRunner(this, actor);
            var response = runner.Run();
            return response;
        }
    }
}
