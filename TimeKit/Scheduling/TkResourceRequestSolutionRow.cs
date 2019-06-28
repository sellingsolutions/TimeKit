using System.Collections.Generic;
using System.Linq;

namespace TimeKit.Scheduling
{
    public class TkResourceRequestSolutionRow
    {
        public int Id { get; set; }

        public TkResourceRequestSolutionGroup Group { get; set; }

        public TkResourceRequest Request { get; set; }
        public List<TkResourceResponse> Responses { get; set; }

        public IEnumerable<TKResourceRequestRunner> RequestRunners { get; set; }

        public TkResourceRequestSolutionRow(
            TkResourceRequest request,
            IEnumerable<TKResourceRequestRunner> requestRunners,
            int rowId)
        {
            Id = rowId; 
            Request = request;
            RequestRunners = requestRunners;
        }

        public IEnumerable<TkResourceResponse> RunRequests()
        {
            var responses = new List<TkResourceResponse>();
            foreach (var requestRunner in RequestRunners)
            {
                var response = requestRunner.Run();
                if (response == null)
                    continue;

                response.id = Id;
                responses.Add(response);
            }

            Responses = responses;

            return responses;
        }


        public static TkResourceRequestSolutionRow CreateRow(
            TkResourceRequestSolutionGroup group,
            TkResourceRequest request)
        {
            if (request == null || !request.IsValid())
                return null;

            var actors = group.AvailableActors.Where(o => o.Capabilities.Contains(request.RequiredCapability));
            var runners = new List<TKResourceRequestRunner>();
            foreach (var actor in actors)
            {
                var processes = group.AvailableProcesses.Where(p => p.ParticipantId == actor.Key);
                // TODO: Why did I want to discard actors without processes?
                //if (!processes.Any())
                //    continue;

                var requestRunner = new TKResourceRequestRunner(group, request, actor, processes);
                runners.Add(requestRunner);
            }

            var row = new TkResourceRequestSolutionRow(
                request,
                runners,
                group.Rows.Count() + 1);

            return row;
        }
    }
}
