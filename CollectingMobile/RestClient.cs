using System.Collections.Generic;
using System.Text;
using Android.Content;
using System.Json;
using Android.Net;
using System.Net;
using System.IO;
using System;
using Newtonsoft.Json;

namespace CollectingMobile
{
    class RestClient
    {
        public static string serverDomain;

        public static bool IsLoginOk(string username, string password)
        {
            string serverLoginURL = "https://" + serverDomain + "/LabTest/_invoke/Login";
            string requestJSON = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"createPersistentCookie\":true}";

            JsonValue responseJSON = JsonValue.Parse(PostWebContent(requestJSON, serverLoginURL));

            return responseJSON["d"];//{d:true} OR {d:false}                 
        }

        private static HttpWebRequest SetRequestWebJSON(string serverURL, byte[] dataJSON)
        {
            try
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
            catch (Exception)
            {
                Console.WriteLine("RestClient.SetRequestWebJSON: Exception");
                return null;
            }
        }

        public static List<Request> GetRequestsFromServer()
        {
            List<Request> requests = GetRequestsOnly();

            foreach (Request request in requests)
            {
                request.Specimens = GetSpecimensForRequest(request.ID);
            }
            return requests;
        }

        private static string PostWebContent(string requestJSON, string URL)
        {
            byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);

            HttpWebRequest requestWeb = SetRequestWebJSON(URL, dataJSON);
            if(requestWeb == null)
            {
                return "";
            }
            HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;
            string responseContent = new StreamReader(responseWeb.GetResponseStream()).ReadToEnd();

            return responseContent;
        }

        private static List<Request> GetRequestsOnly()
        {
            string getRequestsURL = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingrequest&username=";
            string requestsURLForUser = getRequestsURL + ActiveUser.User.Name;

            return RequestsFactory.GetRequestsFromJSON(PostWebContent("", requestsURLForUser));
        }

        private static List<Specimen> GetSpecimensForRequest(int requestID)
        {
            string getSpecimensURL = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingrequestitems&samplingrequestid=";
            string specimensURLForRequest = getSpecimensURL + requestID;

            return RequestsFactory.GetSpecimensFromJSON(PostWebContent("", specimensURLForRequest));
        }

        public static bool AmIOnline(ConnectivityManager cm)
        {
            NetworkInfo activeConnection = cm.ActiveNetworkInfo;
            if ((activeConnection != null) && activeConnection.IsConnected)
            {
                return true;
            }
            else
            {
                Console.WriteLine("RestClient.AmIOnline: NOT INTERNET CONNECTION");
                return false;
            }
        }

        public static bool IsServerReachable()
        {
            try
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create("https://jimsrv.no-ip.info/LabTest/");
                iNetRequest.Timeout = 3000;
                iNetRequest.Proxy = null;
                WebResponse iNetResponse = iNetRequest.GetResponse();
                iNetResponse.Close();
                return true;
            }
            catch (WebException)
            {
                Console.WriteLine("Server not reachable");
                return false;
            }
        }

        public static bool UploadSpecimens(Context context, List<Specimen> specimens)
        {
            string postSpecimensURL = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingresults";

            string specimensJSON = "[";
            foreach (Specimen s in specimens)
            {
                specimensJSON += CreateSpecimenJSON(context, s);
                specimensJSON += ",";
            }
            specimensJSON = specimensJSON.TrimEnd(',');
            specimensJSON += "]";

            JsonValue responseJSON = JsonValue.Parse(PostWebContent(specimensJSON, postSpecimensURL));

            return responseJSON["d"];
        }

        public static bool UploadSpecimen(Context context, Specimen specimen)
        {
            string postSpecimensURL = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingresults";

            string specimensJSON = "[" + CreateSpecimenJSON(context, specimen) + "]";

            JsonValue responseJSON = JsonValue.Parse(PostWebContent(specimensJSON, postSpecimensURL));

            return responseJSON["d"];
        }

        private static string CreateSpecimenJSON(Context context, Specimen specimen)
        {
            return JsonConvert.SerializeObject(specimen);
        }
    }
}



