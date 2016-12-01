using System.Collections.Generic;
using System.Text;
using Android.Content;
using System.Json;
using Android.Net;
using System.Net;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using Android.Graphics;

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
                byte[] dataJSON = new ASCIIEncoding().GetBytes(requestJSON);           
                HttpWebRequest requestWeb = WebRequest.Create(serverLoginURL) as HttpWebRequest;
                requestWeb.Method = "POST";
                requestWeb.ContentType = "application/json";
                requestWeb.ContentLength = dataJSON.Length;
                requestWeb.CookieContainer = new CookieContainer();
                requestWeb.Expect = "application/json";
                requestWeb.Proxy = null;
                requestWeb.GetRequestStream().Write(dataJSON, 0, dataJSON.Length);

                HttpWebResponse responseWeb = requestWeb.GetResponse() as HttpWebResponse;
                ActiveUser.cookies.Add(responseWeb.Cookies);
                string responseContent = new StreamReader(responseWeb.GetResponseStream()).ReadToEnd();

                JsonValue responseJSON = JsonValue.Parse(responseContent);
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

        private static string PostAndGetResponseWebContent(string JSON, string URL)
        {
            byte[] dataJSON = new ASCIIEncoding().GetBytes(JSON);
            try
            {
                HttpWebRequest requestWeb = WebRequest.Create(URL) as HttpWebRequest;
                requestWeb.CookieContainer = new CookieContainer();
                requestWeb.CookieContainer.Add(ActiveUser.cookies);
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
            string postSpecimensURL = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingresults&username=" + ActiveUser.User.Name;

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
            string postSpecimensURL = "http://" + serverDomain + "/LabTest/ResourceService.ashx?type=samplingresults&username=" + ActiveUser.User.Name;
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

        public async static void UploadImage(Specimen specimen)
        {
            string url = @"http://" + serverDomain + "/LabTest/Blob.ashx?PhotoPhotoHandler=u|" + specimen.ID + "&_v=2";

            Java.IO.File pictureFile = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimen.ID + ".jpeg");

            if (pictureFile.Exists())
            {
                Bitmap bitmap = BitmapFactory.DecodeFile(pictureFile.AbsolutePath);

                byte[] bitmapBytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    bitmapBytes = stream.ToArray();
                }

                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(new MemoryStream(bitmapBytes)), "slika", specimen.ID + ".jpeg");
                    await client.PostAsync(url, content);
                }
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



