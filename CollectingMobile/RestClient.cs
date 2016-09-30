using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.Json;

//{"username":"eugens1","password":"eugens1123%","createPersistentCookie":true}
//{"username":"eugens","password":"1R#EugenS","createPersistentCookie":true}

namespace CollectingMobile
{
    class RestClient
    {
        //TODO: 2 activitya 1. login  2. empty
        public static string sUrl = "https://jimsrv.no-ip.info/LabTest/_invoke/Login";
        public static string jsonLogin = "{\"username\":\"eugens1\",\"password\":\"eugens1123%\",\"createPersistentCookie\":true}";
        public static bool IsLoginOk(string username, string password)
        {
            //   string json = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"createPersistentCookie\":true}";
            string json = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"createPersistentCookie\":true}";
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] data = encoder.GetBytes(jsonLogin); // a json object, or xml, whatever...

            System.Net.HttpWebRequest request = System.Net.WebRequest.Create(sUrl) as System.Net.HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.Expect = "application/json";

            request.GetRequestStream().Write(data, 0, data.Length);

            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            var reader = new System.IO.StreamReader(response.GetResponseStream());
            string content = reader.ReadToEnd();
            var jsonIdk = JsonValue.Parse(content);
            return jsonIdk["d"];


        }


    }
}


