﻿using System;
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
        static Lazy<string> connStr = new Lazy<string>(() => GetConnStr());

        public static string GetConnStr()
        {
            try
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

                VerifySchema(connStr);
                return connStr;
            }
            catch { return ""; }
        }

        public static void VerifySchema(string connStr)
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                var tblExists = false;
                conn.Open();
                using (var cmd1 = new NpgsqlCommand("select * from information_schema.tables where table_name = 'rn_version'", conn))
                {
                    using (var rdr = cmd1.ExecuteReader())
                    {
                        tblExists = rdr.Read();
                    }
                }
                if (!tblExists)
                {
                    using (var cmd2 = new NpgsqlCommand("create table rn_version (ver integer);", conn))
                    {
                        cmd2.ExecuteNonQuery();
                    }
                    using (var cmd3 = new NpgsqlCommand("insert into rn_version (ver) values (1)", conn))
                    {
                        cmd3.ExecuteScalar();
                    }
                    using (var cmd4 = new NpgsqlCommand("create table rn_results (email varchar(100), daum_id varchar(100), hospital varchar(200), aff varchar(200));", conn))
                    {
                        cmd4.ExecuteNonQuery();
                    }
                }
                else
                {
                    var updateTable = true;
                    using (var cmd2 = new NpgsqlCommand("select ver from rn_version", conn))
                    {
                        using (var rdr = cmd2.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                var curVersion = rdr.GetInt32(0);
                                if (curVersion == 1) { updateTable = false; }
                            }
                        }
                    }
                }
            }
        }

        public bool VerifyDatabase()
        {
            using (var conn = new NpgsqlConnection(connStr.Value))
            {
                conn.Open();
            }
            return true;
        }
    }
}