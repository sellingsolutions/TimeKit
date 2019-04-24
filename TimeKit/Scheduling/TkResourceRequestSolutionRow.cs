using System;
using System.Collections.Generic;
using TimeKit.DataStructure;
using System.Linq;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceRequestSolutionRow
    {
        public int no { get; set; }

        public TkResourceRequestSolutionGroup Group { get; set; }

        public TkResourceRequest Request { get; set; }
        public List<TkResourceResponse> Responses { get; set; }

        public TkResourceRequestSolutionRow(TkResourceRequest request, List<TkResourceResponse> responses)
        {
            Request = request;
            Responses = responses;
        }

        public static TkResourceRequestSolutionRow NewRow(TkResourceRequestSolutionGroup group, TkResourceRequest request)
        {
            if (!request.IsValid())
                return null;

            var actors = group.AvailableActors.Where(o => o.Capabilities.Contains(request.RequiredCapability));
            var responses = new List<TkResourceResponse>();
            foreach (var actor in actors)
            {
                var processes = group.AvailableProcesses.Where(p => p.ParticipantId == actor.Key);
                if (!processes.Any())
                    continue;

                var requestRunner = new TKResourceRequestRunner(group, request, actor, processes);
                var response = requestRunner.Run();

                if (response == null)
                    continue;
                
                responses.Add(response);
            }

            var row = new TkResourceRequestSolutionRow(request, responses);
            return row;
        }
    }
}
