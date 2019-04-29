using System;
using System.Collections.Generic;
using TimeKit.DataStructure;
using System.Linq;
using TimeKit.Models;

namespace TimeKit.Scheduling
{
    public class TkResourceRequestSolutionRow
    {
        public int Id { get; set; }

        public TkResourceRequestSolutionGroup Group { get; set; }

        public TkResourceRequest Request { get; set; }
        public List<TkResourceResponse> Responses { get; set; }

        public TkResourceRequestSolutionRow(int id, TkResourceRequest request, List<TkResourceResponse> responses)
        {
            Id = id; 
            Request = request;
            Responses = responses;

            foreach (var response in Responses)
            {
                response.id = id;
            }
        }

        public static TkResourceRequestSolutionRow NewRow(
            TkResourceRequestSolutionGroup group, 
            TkResourceRequest request)
        {
            if (!group.IsValid() || !request.IsValid())
                return null;

            var actors = group.AvailableActors
                            .Where(o => o.Capabilities
                            .FirstOrDefault(c => c.Key == request.RequiredCapability.Key) != null);

            var responses = new List<TkResourceResponse>();
            foreach (var actor in actors)
            {
                var processes = group.AvailableProcesses
                    .Where(p => p.ParticipantId == actor.Key);

                if (!processes.Any())
                    continue;

                var requestRunner = new TKResourceRequestRunner(group, request, actor, processes);
                var response = requestRunner.Run();

                if (response == null)
                    continue;
                
                responses.Add(response);
            }

            var row = new TkResourceRequestSolutionRow(group.Rows.Count() +1, request, responses);
            return row;
        }
    }
}
