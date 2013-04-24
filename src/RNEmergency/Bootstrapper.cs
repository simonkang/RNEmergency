using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.TinyIoc;

namespace RNEmergency
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            pipelines.BeforeRequest.AddItemToEndOfPipeline(ctx =>
                {
                    if (!ctx.Request.Url.HostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                    {
                        var proto = ctx.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
                        if (proto == null)
                        {
                            if (!ctx.Request.Url.IsSecure)
                            {
                                return new RedirectResponse("https://" + ctx.Request.Url.HostName);
                            }
                        }
                        else if (!proto.Equals("https", StringComparison.OrdinalIgnoreCase))
                        {
                            return new RedirectResponse("https://" + ctx.Request.Url.HostName);
                        }
                    }
                    return null;
                });
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            //Conventions.StaticContentsConventions.Add(
            //    StaticContentConventionBuilder.AddDirectory("Scripts", @"Scripts", ".js"));
        }
    }
}