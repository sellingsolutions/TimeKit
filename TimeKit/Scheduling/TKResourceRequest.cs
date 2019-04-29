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
        

        public TkResourceRequest()
        {

        }

        public TkResourceRequest(TkIRoleType roleType, TkICapability capability)
        {
            RoleType = roleType;
            RequiredCapability = capability;
        }

        public bool IsValid()
        {
            return RoleType != null && 
                   RoleType.Key != null &&
                   RequiredCapability != null &&
                   RequiredCapability.Key != null;
        }

    }
}
