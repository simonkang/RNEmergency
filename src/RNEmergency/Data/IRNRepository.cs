using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Npgsql;

namespace RNEmergency.Data
{
    public interface IRNRepository
    {
        bool VerifyDatabase();
    }

    public class RNRepository : IRNRepository
    {
        static string connStr;

        static RNRepository()
        {
            connStr = GetConnStr();
        }

        public static string GetConnStr()
        {
            var uriString = ConfigurationManager.AppSettings["ELEPHANTSQL_URL"] ??
                ConfigurationManager.AppSettings["LOCAL_URL"];
            var uri = new Uri(uriString);
            var db = uri.AbsolutePath.Trim('/');
            var user = uri.UserInfo.Split(':')[0];
            var passwd = uri.UserInfo.Split(':')[1];
            var port = uri.Port > 0 ? uri.Port : 5432;
            var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
                uri.Host, db, user, passwd, port);
            return connStr;
        }

        public bool VerifyDatabase()
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
            }
            return true;
        }
    }
}