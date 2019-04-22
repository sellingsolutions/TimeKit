using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeKit.Scheduling;
using TimeKit.Models;
using System.Linq;

namespace TimeKit
{
    public class TimeKitClient
    {
        public TimeKitClient()
        {
        }


        public TKResourceRequestRow CreateRow(TkResourceRequest request)
        {
            if (!request.IsValid())
                return null;

            var actors = request.AvailableActors.Where(o => o.Capabilities.Contains(request.RequiredCapability));
            var responses = actors.Select(actor => request.Run(actor)).Where(r=>r != null).ToList();
            var row = new TKResourceRequestRow(request, responses);

            return row;
        }
    }
}
