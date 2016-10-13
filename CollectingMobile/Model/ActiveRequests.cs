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
    class ActiveRequests
    {
        private static List<Request> requests;

        public static List<Request> Requests
        {
            get
            {
                //lazy init
                if(requests == null)
                {
                    requests = RestClient.GetDataFromServer();
                }
                return requests;
            }
            set
            {
                requests = value;
            }
        }

        public static Request GetRequestFromPosition(int position)
        {
            return requests[position];
        }
        
        public static List<Specimen> GetSpecimensForRequest(string requestId)
        {
            return requests.Find(x => x.ID == requestId).Specimens;
        }
    }
}