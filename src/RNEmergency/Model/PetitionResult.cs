using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RNEmergency.Model
{
    public class PetitionResult
    {
        private string _sign_image1;
        private string _sign_image_dataurl1;
        private string _sign_image2;
        private string _sign_image_dataurl2;

        public string email { get; set; }
        public string name { get; set; }
        public string phone_no { get; set; }
        public string daum_id { get; set; }
        public string work_place { get; set; }
        public string client_ip { get; set; }
        public string sign_image1
        {
            get { return _sign_image1; }
            set
            {
                _sign_image1 = value;
                _sign_image_dataurl1 = CreateDataUrl(_sign_image1);
            }
        }
        public string sign_image2
        {
            get { return _sign_image2; }
            set
            {
                _sign_image2 = value;
                _sign_image_dataurl2 = CreateDataUrl(_sign_image2);
            }
        }
        public string CreateDataUrl(string url)
        {
            return "data:image/png;base64," + url;
        }

        public DateTime insert_dt { get; set; }

        public string insert_dt_formatted { get { return insert_dt.ToString("yyyy-MM-dd HH:mm:ss"); } }
        public string err_msg { get; set; }
        public bool hasErr { get { return !string.IsNullOrWhiteSpace(err_msg); } }

        public byte[] sign_image_bytes1
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sign_image1)) { return new byte[0]; }
                return Convert.FromBase64String(sign_image1);
            }
            set
            {
                sign_image1 = Convert.ToBase64String(value);
            }
        }
        public byte[] sign_image_bytes2
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sign_image2)) { return new byte[0]; }
                return Convert.FromBase64String(sign_image2);
            }
            set
            {
                sign_image2 = Convert.ToBase64String(value);
            }
        }

        public string sign_image_dataurl1
        {
            get { return _sign_image_dataurl1; }
            set
            {
                _sign_image1 = CreateSignImage(value);
                _sign_image_dataurl1 = value;
            }
        }
        public string sign_image_dataurl2
        {
            get { return _sign_image_dataurl2; }
            set
            {
                _sign_image2 = CreateSignImage(value);
                _sign_image_dataurl2 = value;
            }
        }
        public string CreateSignImage(string url)
        {
            if (url.StartsWith("data", StringComparison.OrdinalIgnoreCase))
            {
                return  url.Substring(url.IndexOf(',') + 1);
            }
            else
            {
                return url;
            }
        }
    }
}