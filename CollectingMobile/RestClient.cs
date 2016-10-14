using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Widget;
using System.Json;
using Android.Net;
using System.Net;

namespace CollectingMobile
{
    class RestClient
    {
        //TODO: use shared preferences instead of hardcoded strings
        public static string serverLoginURL = @"https://jimsrv.no-ip.info/LabTest/_invoke/Login";
        public static string requestsURL = @"http://jimsrv.no-ip.info/LabTest/ResourceService.ashx?type=samplingrequest&username=";
        public static string specimensURL = @"http://jimsrv.no-ip.info/LabTest/ResourceService.ashx?type=samplingrequestitems&samplingrequestid=";

        public static bool IsLoginOk(string username, string password)
        {
            string requestJSON = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"createPersistentCookie\":true}";
            byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);

            HttpWebRequest requestWeb = SetRequestWebJSON(serverLoginURL, dataJSON);
            HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;

            string responseContent = new System.IO.StreamReader(responseWeb.GetResponseStream()).ReadToEnd();
            JsonValue responseJSON = JsonValue.Parse(responseContent);

            return responseJSON["d"];//{d:true} OR {d:false}
        }

        private static HttpWebRequest SetRequestWebJSON(string serverURL, byte[] dataJSON)
        {
            HttpWebRequest requestWeb = WebRequest.Create(serverURL) as HttpWebRequest;
            requestWeb.Method = "POST";
            requestWeb.ContentType = "application/json";
            requestWeb.ContentLength = dataJSON.Length;
            requestWeb.Expect = "application/json";
            requestWeb.Proxy = null;
            requestWeb.GetRequestStream().Write(dataJSON, 0, dataJSON.Length);

            return requestWeb;
        }

        public static List<Request> GetDataFromServer()
        {
            List<Request> requests = GetRequests();

            foreach (Request request in requests)
            {
                request.Specimens = GetSpecimensForRequest(request.ID);
            }

            return requests;           
        }

        private static List<Request> GetRequests()
        {
            string requestsURLForUser = requestsURL + ActiveUser.Username;

            string requestJSON = "";
            byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);

            HttpWebRequest requestWeb = SetRequestWebJSON(requestsURLForUser, dataJSON);
            HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;

            string responseContent = new System.IO.StreamReader(responseWeb.GetResponseStream()).ReadToEnd();
            JsonValue responseJSON = JsonValue.Parse(responseContent);

            return RequestsFactory.GetRequestsFromJSON(responseJSON);
        }

        private static List<Specimen> GetSpecimensForRequest(string requestID)
        {
            string specimensURLForRequest = specimensURL + requestID;

            string requestJSON = "";
            byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);

            HttpWebRequest requestWeb = SetRequestWebJSON(specimensURLForRequest, dataJSON);
            HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;

            string responseContent = new System.IO.StreamReader(responseWeb.GetResponseStream()).ReadToEnd();
            JsonValue responseJSON = JsonValue.Parse(responseContent);

            return RequestsFactory.GetSpecimensFromJSON(responseJSON);
        }

        public static bool AmIOnline(Context context)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            if((activeConnection != null) && activeConnection.IsConnected)
            {
                return true;
            }
            else
            {
                Toast.MakeText(context, context.Resources.GetText(Resource.String.CheckNetwork), ToastLength.Short).Show();
                return false;
            }
        }

        public static bool IsServerReachable(Context context)
        {
            try
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(serverLoginURL);
                iNetRequest.Timeout = 5000;
                iNetRequest.Proxy = null;
                WebResponse iNetResponse = iNetRequest.GetResponse();
                iNetResponse.Close();
                return true;
            }
            catch (WebException)
            {
                Toast.MakeText(context, context.Resources.GetText(Resource.String.ServerProblem), ToastLength.Short).Show();
                return false;
            }
        }
    }
}



