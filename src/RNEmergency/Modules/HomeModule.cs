﻿using System;
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
        static TimeZoneInfo koreaTZI = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");

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
            Post["/Petition"] = p =>
                {
                    var pr = this.Bind<PetitionResult>();
                    pr.client_ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Context.Request.UserHostAddress;
                    pr.insert_dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, koreaTZI);
                    if (string.IsNullOrWhiteSpace(pr.email))
                    {
                        pr.err_msg = "email required";
                    }
                    else if (string.IsNullOrWhiteSpace(pr.name))
                    {
                        pr.err_msg = "name required";
                    }
                    else if (string.IsNullOrWhiteSpace(pr.work_place))
                    {
                        pr.err_msg = "work place required";
                    }
                    else if (string.IsNullOrWhiteSpace(pr.phone_no))
                    {
                        pr.err_msg = "phone number required";
                    }
                    else if (string.IsNullOrWhiteSpace(pr.sign_image1) && string.IsNullOrWhiteSpace(pr.sign_image2))
                    {
                        pr.err_msg = "1 서명 required";
                    }
                    if (string.IsNullOrWhiteSpace(pr.err_msg))
                    {
                        pr.err_msg = repo.InsertPetitionResult(pr);
                    }
                    return View["result.sshtml", pr];
                };
        }
    }
}