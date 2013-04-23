using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.ModelBinding;
using RNEmergency.Data;
using RNEmergency.Model;

namespace RNEmergency.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(IRNRepository repo)
        {
            Get["/"] = _ => View["index.sshtml"];
            Get["/Test"] = _ => 
                {
                    try
                    {
                        var clientIP =  System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? "";
                        var dbTest = repo.VerifyDatabase();
                        return "Good : " + Context.Request.UserHostAddress + " : " + clientIP;
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                };
            Post["/"] = p =>
                {
                    var pr = this.Bind<PetitionResult>();
                    pr.client_ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Context.Request.UserHostAddress;
                    pr.err_msg = repo.InsertPetitionResult(pr);
                    return View["result.sshtml", pr];
                };
        }
    }
}