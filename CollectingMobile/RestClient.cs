using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Widget;
using System.Json;
using Android.Net;
using System.Net;
using System.IO;
using System;
using Android.Util;

namespace CollectingMobile
{
    class RestClient
    {
        //TODO: use shared preferences instead of hardcoded strings
        public static string serverLoginURL = @"https://jimsrv.no-ip.info/LabTest/_invoke/Login";
        public static string getRequestsURL = @"http://jimsrv.no-ip.info/LabTest/ResourceService.ashx?type=samplingrequest&username=";
        public static string getSpecimensURL = @"http://jimsrv.no-ip.info/LabTest/ResourceService.ashx?type=samplingrequestitems&samplingrequestid=";
        public static string postSpecimensURL = @"http://jimsrv.no-ip.info/LabTest/ResourceService.ashx?type=samplingresults";

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

        public static List<Request> GetRequestsFromServer()
        {
            List<Request> requests = GetRequestsOnly();

            foreach (Request request in requests)
            {
                request.Specimens = GetSpecimensForRequest(request.ID);
            }

            return requests;
        }

        private static List<Request> GetRequestsOnly()
        {
            string requestsURLForUser = getRequestsURL + ActiveUser.User.Name;

            string requestJSON = "";
            byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);

            HttpWebRequest requestWeb = SetRequestWebJSON(requestsURLForUser, dataJSON);
            HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;

            string responseContent = new System.IO.StreamReader(responseWeb.GetResponseStream()).ReadToEnd();
            JsonValue responseJSON = JsonValue.Parse(responseContent);

            return RequestsFactory.GetRequestsFromJSON(responseJSON);
        }

        private static List<Specimen> GetSpecimensForRequest(int requestID)
        {
            string specimensURLForRequest = getSpecimensURL + requestID;

            string requestJSON = "";
            byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);

            HttpWebRequest requestWeb = SetRequestWebJSON(specimensURLForRequest, dataJSON);
            HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;

            string responseContent = new System.IO.StreamReader(responseWeb.GetResponseStream()).ReadToEnd();
            JsonValue responseJSON = JsonValue.Parse(responseContent);

            return RequestsFactory.GetSpecimensFromJSON(responseJSON);
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
                Console.WriteLine();
                
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
            foreach(Specimen spec in specimens)
            {
                spec.uploaded = true;
            }
            return true;
            //string specimensJSON = "[";
            //foreach (Specimen s in specimens)
            //{
            //    specimensJSON += CreateSpecimenJSON(context, s);
            //    specimensJSON += ",";
            //}
            //specimensJSON = specimensJSON.TrimEnd(',');
            //specimensJSON += "]";          

            //byte[] dataJSON = new ASCIIEncoding().GetBytes(specimensJSON);

            //HttpWebRequest requestWeb = SetRequestWebJSON(postSpecimensURL, dataJSON);
            //HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;

            //string responseContent = new System.IO.StreamReader(responseWeb.GetResponseStream()).ReadToEnd();
            //JsonValue responseJSON = JsonValue.Parse(responseContent);

            //return responseJSON["d"];
        }

        private static string CreateSpecimenJSON(Context context, Specimen specimen)
        {
            string specimenJSONTemplate;
            using (StreamReader r = new StreamReader(context.Assets.Open("specimenJSONTemplate.json")))
            {
                specimenJSONTemplate = r.ReadToEnd();
            }

            JsonValue specimenJSON = JsonValue.Parse(specimenJSONTemplate);
            specimenJSON["ID"] = specimen.ID;
            specimenJSON["Description"] = specimen.Description;
            specimenJSON["MaterialTypeID"] = specimen.MaterialTypeID;
            specimenJSON["Location"] = specimen.Location;
            specimenJSON["SamplingPosition"] = specimen.SamplingPosition;

            return specimenJSON.ToString();
        }
    }
}



