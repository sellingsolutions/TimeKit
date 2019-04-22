using System.Collections.Generic;
using System.Linq;
using System;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TKResourceRequest
    {
        public IRole Role { get; set; }
        public ICapability RequiredCapability { get; set; }
        public IObjectType ObjectType { get; set; }
        public long NoOfObjects { get; set; }
        public long MinutesRequiredPerObject { get; set; }
        public List<long> WeekNumbers { get; set; }

        public List<IActor> AvailableActors { get; set; }
        public List<IProcess> AvailableProcesses { get; set; }

        public TimeSpan TicksRequired => TimeSpan.FromMinutes(MinutesRequiredPerObject * NoOfObjects);

        public TKResourceRequest(
            IRole role, 
            ICapability capability, 
            IObjectType objectType, 
            long noOfObjects, 
            long minutesRequiredPerObject,
            List<long> weekNumbers,
            List<IActor> availableActors,
            List<IProcess> availableProcesses)
        {
            Role = role;
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
                Role == null ||
                RequiredCapability == null ||
                NoOfObjects == 0 || 
                MinutesRequiredPerObject == 0 || 
                WeekNumbers == null)
            {
                return false;
            }
            if (!WeekNumbers.Any())
            {
                return false;
            }

            return true;
        }

        public TKResourceResponse Run(IActor actor)
        {
            var runner = new TKResourceRequestRunner(this, actor);
            return runner.Run();
        }
    }
}
