using System.Collections.Generic;
using System.Linq;

namespace TimeKit.Scheduling
{
    public class TkResourceRequestSolutionRow
    {
        public int Id { get; set; }

        public TkResourceRequestSolutionGroup Group { get; set; }

        public TkRequest Request { get; set; }
        public IEnumerable<TkResourceResponse> Responses { get; set; }

        public TkResourceRequestSolutionRow(
            TkRequest request,
            IEnumerable<TkResourceResponse> responses,
            int rowId)
        {
            Id = rowId; 
            Request = request;
            Responses = responses;
        }

       

        public static TkResourceRequestSolutionRow CreateRow(
            TkResourceRequestSolutionGroup group,
            TkRequest request)
        {
            if (request == null || !request.IsValid())
                return null;

            var requestRunner = new TKResourceRequestRunner(request);
            

            // TODO: Do we really need rows and groups in TimeKit?
            // Can we just run the request, get the responses, check which ones can be scheduled together and return that?

            var row = new TkResourceRequestSolutionRow(
                request,
                runners,
                group.Rows.Count() + 1);

            return row;
        }
    }
}
