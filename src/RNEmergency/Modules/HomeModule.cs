using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
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
                        var dbTest = repo.VerifyDatabase();
                        return "Good";
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                };
        }
    }
}