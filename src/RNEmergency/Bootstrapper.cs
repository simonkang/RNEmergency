using System;
using System.Collections.Generic;
using System.IO;
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
        const string offlineHtml = 
@"<!DOCTYPE html>
<html lang=""en""><head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<style type=""text/css"">
/* Main marketing message and sign up button */
.jumbotron {
	margin: 20px 0;
	text-align: center;
}
.jumbotron h1 {
	font-size: 80px;
	line-height: 1;
}
.jumbotron .lead {
	font-size: 24px;
	line-height: 1.25;
}
</style><title>RN Emergency - Offline</title></head><body><div class=""jumbotron""><h1>RN Emergency - Offline</h1><p class=""lead"">Offline for Maintenance</p><hr></div></body></html>";

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
                {
                    if (!ctx.Request.Url.HostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                    {
                        return new HtmlResponse(HttpStatusCode.OK, stream =>
                            {
                                var sw = new StreamWriter(stream, System.Text.Encoding.UTF8) { AutoFlush = true };
                                sw.Write(offlineHtml);
                            });
                    }
                    return null;
                });
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