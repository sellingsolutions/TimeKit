using System.Collections.Generic;

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
        
        public static TkResourceRequestSolutionRow CreateRow(TkRequest request)
        {
            if (request == null || !request.IsValid())
                return null;

            var requestRunner = new TKResourceRequestRunner(request);
            var responses = requestRunner.Run();

            var row = new TkResourceRequestSolutionRow(
                request,
                responses,
                -1);

            return row;
        }
    }
}
