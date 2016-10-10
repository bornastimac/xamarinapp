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
    class RequestsFactory
    {
        private static int id = 0;
        private static Random rnd = new Random();

        public static List<Request> GetMockSpecimensRequestsForUser(string username, int numberOfSpecimensRequests)
        {
            List<Request> mockSpecimensRequests = new List<Request>();

            for (int i = 0; i < numberOfSpecimensRequests; i++)
            {
                mockSpecimensRequests.Add(new Request((++id).ToString(), "description_" + id, username, DateTime.Now, GetMockSpecimens(rnd.Next(1, 6))));
            }
            return mockSpecimensRequests;
        }

        private static List<Specimen> GetMockSpecimens(int numberOfSpecimens)
        {
            List<Specimen> mockSpecimens = new List<Specimen>();

            for (int i = 0; i < numberOfSpecimens; i++)
            {
                mockSpecimens.Add(new Specimen("SpecDescription_" + i));
            }
            return mockSpecimens;
        }
    }
}