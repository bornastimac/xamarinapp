using System.Collections.Generic;
using System.Text;
using Android.Content;
using System.Json;
using Android.Net;
using System.Net;
using System.IO;
using System;
using Newtonsoft.Json;
using Android.App;
using System.Threading.Tasks;

namespace CollectingMobile
{
    class RestClient
    {
        public static string serverDomain;

        public static bool IsLoginOk(string username, string password)
        {
            string serverLoginURL = "https://" + serverDomain + "/LabTest/_invoke/Login";
            string requestJSON = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"createPersistentCookie\":true}";
            try
            {
                JsonValue responseJSON = JsonValue.Parse(PostAndGetResponseWebContent(requestJSON, serverLoginURL));
                return responseJSON["d"];//{d:true} OR {d:false}
            }
            catch (WebException)
            {
                throw;
            }
            catch (UriFormatException)
            {
                throw;
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

        private static string PostAndGetResponseWebContent(string requestJSON, string URL)
        {
            byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);

            try
            {
                HttpWebRequest requestWeb = WebRequest.Create(URL) as HttpWebRequest;
                requestWeb.Method = "POST";
                requestWeb.ContentType = "application/json";
                requestWeb.ContentLength = dataJSON.Length;
                requestWeb.Expect = "application/json";
                requestWeb.Proxy = null;
                requestWeb.GetRequestStream().Write(dataJSON, 0, dataJSON.Length);
                HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;
                string responseContent = new StreamReader(responseWeb.GetResponseStream()).ReadToEnd();

                return responseContent;
            }
            catch (WebException)
            {
                throw;
            }
            catch (UriFormatException)
            {
                throw;
            }
        }

        private static List<Request> GetRequestsOnly()
        {
            string requestsURLForUser = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingrequest&username=" + ActiveUser.User.Name;
            try
            {
                JsonValue responseJSON = JsonValue.Parse(PostAndGetResponseWebContent("", requestsURLForUser));
                return RequestsFactory.GetRequestsFromJSON(responseJSON);
            }
            catch (WebException)
            {
                throw;
            }
            catch (UriFormatException)
            {
                throw;
            }
        }

        private static List<Specimen> GetSpecimensForRequest(int requestID)
        {
            string specimensURLForRequest = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingrequestitems&samplingrequestid=" + requestID;
            try
            {
                return RequestsFactory.GetSpecimensFromJSON(PostAndGetResponseWebContent("", specimensURLForRequest));
            }
            catch (WebException)
            {
                throw;
            }
            catch (UriFormatException)
            {
                throw;
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
            
            try
            {
                JsonValue responseJSON = JsonValue.Parse(PostAndGetResponseWebContent(specimensJSON, postSpecimensURL));
                return responseJSON["d"];
            }
            catch (WebException)
            {
                throw;
            }
            catch (UriFormatException)
            {
                throw;
            }
        }

        public static bool UploadSpecimen(Context context, Specimen specimen)
        {
            string postSpecimensURL = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingresults";
            string specimensJSON = "[" + CreateSpecimenJSON(context, specimen) + "]";
            try
            {
                JsonValue responseJSON = JsonValue.Parse(PostAndGetResponseWebContent(specimensJSON, postSpecimensURL));
                return responseJSON["d"];
            }
            catch (WebException)
            {
                throw;
            }
            catch (UriFormatException)
            {
                throw;
            }
        }

        private static string CreateSpecimenJSON(Context context, Specimen specimen)
        {
            return JsonConvert.SerializeObject(specimen);
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
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create("https://" + serverDomain + "/LabTest/");
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


    }
}



