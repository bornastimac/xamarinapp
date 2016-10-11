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
using System.Threading;
using System.Threading.Tasks;
using Android.App.Usage;
using Android.Net;
using System.Net;
//{"username":"eugens1","password":"eugens1123%","createPersistentCookie":true} 

namespace CollectingMobile
{
    class RestClient
    {
        public static string serverLoginUrl = "https://jimsrv.no-ip.info/LabTest/_invoke/Login";

        public static bool IsLoginOk(string username, string password)
        {

            string json = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"createPersistentCookie\":true}";
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] data = encoder.GetBytes(json);

            System.Net.HttpWebRequest request = System.Net.WebRequest.Create(serverLoginUrl) as System.Net.HttpWebRequest;
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

        public static List<Request> GetDataFromServer()
        {
            return RequestsFactory.GetMockSpecimensRequestsForUser(ActiveUser.Username, new Random().Next(3, 10));
        }

        public static bool AmIOnline(Context context)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            return (activeConnection != null) && activeConnection.IsConnected;

        }

        public static bool IsServerReachable(Context context)
        {
            try
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(serverLoginUrl);
                iNetRequest.Timeout = 5000;
                WebResponse iNetResponse = iNetRequest.GetResponse();
                iNetResponse.Close();
                return true;
            }
            catch (WebException)
            {
                if (!AmIOnline(context))
                    Toast.MakeText(context, "Check your internet connection", ToastLength.Short).Show();
                else
                    Toast.MakeText(context, "Server currently unavailable", ToastLength.Short).Show();
                return false;
            }
        }
    }
}



