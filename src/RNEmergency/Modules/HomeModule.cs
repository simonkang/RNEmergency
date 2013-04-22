using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace RNEmergency.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => View["index.sshtml"];
        }
    }
}