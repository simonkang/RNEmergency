using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNEmergency.Model
{
    public class PetitionResult
    {
        public string email { get; set; }
        public string name { get; set; }
        public string phone_no { get; set; }
        public string daum_id { get; set; }
        public string work_place { get; set; }
        public string client_ip { get; set; }
        public string sign_image { get; set; }
        public DateTime insert_dt { get; set; }
        public string insert_dt_formatted { get { return insert_dt.ToString("yyyy-MM-dd HH:mm:ss"); } }
        public string err_msg { get; set; }
        public bool hasErr { get { return !string.IsNullOrWhiteSpace(err_msg); } }

        public byte[] sign_image_bytes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sign_image)) { return new byte[0]; }
                return Convert.FromBase64String(sign_image);
            }
            set
            {
                sign_image = Convert.ToBase64String(value);
            }
        }
    }
}