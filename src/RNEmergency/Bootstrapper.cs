using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses;
using Nancy.TinyIoc;

namespace RNEmergency
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
#if !DEBUG
            pipelines.BeforeRequest.AddItemToEndOfPipeline(ctx => ctx.Request.Url.IsSecure ? null : new RedirectResponse("https://" + ctx.Request.Url.HostName + GetPort(ctx.Request.Url.Port)));
#endif
        }

        private static string GetPort(int? port)
        {
            return (!port.HasValue) ?
                string.Empty :
                string.Concat(":", port.Value);
        }
    }
}