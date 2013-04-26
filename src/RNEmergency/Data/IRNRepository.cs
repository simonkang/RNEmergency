using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Npgsql;
using RNEmergency.Model;

namespace RNEmergency.Data
{
    public interface IRNRepository
    {
        bool VerifyDatabase();
        string InsertPetitionResult(PetitionResult pr);
    }

    public class RNRepository : IRNRepository
    {
        static Lazy<string> connStr = new Lazy<string>(() => GetConnStr());
        static int curDBVersion = 8;

        public static string GetConnStr()
        {
            return "";
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
                if (tblExists)
                {
                    using (var cmd2 = new NpgsqlCommand("select ver from rn_version", conn))
                    {
                        using (var rdr = cmd2.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                var curVersion = rdr.GetInt32(0);
                                if (curVersion != curDBVersion)
                                {
                                    tblExists = false;
                                }
                            }
                        }
                    }
                }
                if (!tblExists) { CreateTables(conn); }
            }
        }

        public static void CreateTables(NpgsqlConnection conn)
        {
            using (var cmdRename1 = new NpgsqlCommand("select table_name from information_schema.tables where table_name = 'rn_results'", conn))
            {
                var tblExists = false;
                using (var rdr1 = cmdRename1.ExecuteReader()) { tblExists = rdr1.Read(); }
                cmdRename1.Dispose();
                if (tblExists)
                {
                    var curCount = 0;
                    using (var cmdRename2 = new NpgsqlCommand("select count(*) from rn_results", conn))
                    {
                        curCount = Convert.ToInt32(cmdRename2.ExecuteScalar());
                    }
                    if (curCount == 0)
                    {
                        using (var cmdRename3 = new NpgsqlCommand("drop table rn_results", conn)) { cmdRename3.ExecuteNonQuery(); }
                    }
                    else
                    {
                        using (var cmdRename2 = new NpgsqlCommand("alter table rn_results rename to rn_results_old" + (curDBVersion - 1).ToString(), conn)) { cmdRename2.ExecuteNonQuery(); }
                    }
                }
            }
            using (var cmd1 = new NpgsqlCommand("drop table rn_version;", conn)) { cmd1.ExecuteNonQuery(); }
            using (var cmd2 = new NpgsqlCommand("create table rn_version (ver integer);", conn)) { cmd2.ExecuteNonQuery(); }
            using (var cmd3 = new NpgsqlCommand("insert into rn_version (ver) values (" + curDBVersion.ToString() + ")", conn)) { cmd3.ExecuteScalar(); }
            using (var cmd4 = new NpgsqlCommand("create table rn_results (email varchar(100), name varchar(100), phone_no varchar(100), daum_id varchar(100), work_place varchar(200), client_ip varchar(50), sign_image1 text, sign_image2 text, insert_dt timestamp, PRIMARY KEY(email));", conn))
            {
                cmd4.ExecuteNonQuery();
            }
            using (var cmd5 = new NpgsqlCommand("create index rn_results_idx_insert_dt on rn_results (insert_dt)")) { cmd5.ExecuteNonQuery(); }
        }

        public bool VerifyDatabase()
        {
            using (var conn = new NpgsqlConnection(connStr.Value))
            {
                conn.Open();
            }
            return true;
        }

        public string InsertPetitionResult(PetitionResult pr)
        {
            var ret = "";
            using (var conn = new NpgsqlConnection(connStr.Value))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("insert into rn_results (email, name, phone_no, daum_id, work_place, client_ip, sign_image1, sign_image2, insert_dt) values (:email, :name, :phone_no, :daum_id, :work_place, :client_ip, :sign_image1, :sign_image2, :insert_dt)", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("email", pr.email));
                    cmd.Parameters.Add(new NpgsqlParameter("name", pr.name));
                    cmd.Parameters.Add(new NpgsqlParameter("phone_no", pr.phone_no));
                    cmd.Parameters.Add(new NpgsqlParameter("daum_id", ""));
                    cmd.Parameters.Add(new NpgsqlParameter("work_place", pr.work_place));
                    cmd.Parameters.Add(new NpgsqlParameter("client_ip", pr.client_ip));
                    cmd.Parameters.Add(new NpgsqlParameter("sign_image1", pr.sign_image1));
                    cmd.Parameters.Add(new NpgsqlParameter("sign_image2", pr.sign_image2));
                    cmd.Parameters.Add(new NpgsqlParameter("insert_dt", pr.insert_dt));
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (NpgsqlException ex)
                    {
                        if (ex.Code == "23505")
                        {
                            ret = "email already exists";
                        }
                        else { throw; }
                    }
                }
            }
            return ret;
        }
    }
}