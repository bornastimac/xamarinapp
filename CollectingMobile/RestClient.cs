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

namespace CollectingMobile
{
    class RestClient         
    {
        public static string sUrl = "https://jimsrv.no-ip.info/LabTest/_invoke/Login";
        public static string json = "{\"username\":\"eugens\",\"password\":\"1R#EugenS\",\"createPersistentCookie\":true}";

        public static void GetResponse() {

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] data = encoder.GetBytes(json); // a json object, or xml, whatever...

            System.Net.HttpWebRequest request = System.Net.WebRequest.Create(sUrl) as System.Net.HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.Expect = "application/json";

            request.GetRequestStream().Write(data, 0, data.Length);

            System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse;
            var reader = new System.IO.StreamReader(response.GetResponseStream());
            string content = reader.ReadToEnd();
            Toast.MakeText(Application.Context, content, ToastLength.Long).Show();

        }
    }
}