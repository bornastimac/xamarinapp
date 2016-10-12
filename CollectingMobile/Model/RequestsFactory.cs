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
using System.Json;

namespace CollectingMobile
{
    class RequestsFactory
    {
        private static int id = 0;

        public static List<Request> GetRequestsFromJSON(JsonValue responseJSON)
        {
            List<Request> requests = new List<Request>();

            //init array of requests
            for (int i = 0; i < responseJSON.Count; i++)
            {
                JsonValue requestJSON = responseJSON[i];
                
                requests.Add(new Request(
                    requestJSON["Code"], 
                    requestJSON["ID"].ToString(), 
                    requestJSON["Description"], 
                    requestJSON["AssignedTo"],
                    new DateTime(),//new DateTime(Convert.ToInt64(requestJSON["DateCreated"].ToString().Where(c => Char.IsDigit(c)).ToString())), 
                    GetSpecimensByID(requestJSON["ID"].ToString())));
            }
            return requests;
        }

        public static List<Specimen> GetSpecimensFromJSON(JsonValue responseJSON)
        {
            List<Specimen> specimens = new List<Specimen>();
            
            for (int i = 0; i < responseJSON.Count; i++)
            {
                JsonValue specimenJSON = responseJSON[i];
                specimens.Add(new Specimen(
                    specimenJSON["MaterialTypesName"]));
            }

            return specimens;
        }

        public static List<Request> GetMockRequestsForUser(string username, int numberOfRequests)
        {
            List<Request> mockRequests = new List<Request>();

            for (int i = 0; i < numberOfRequests; i++)
            {
                mockRequests.Add(new Request("code" + (++id).ToString(), (++id).ToString(), "description_" + id, username, DateTime.Now, GetMockSpecimens(new Random().Next(1, 6))));
            }
            return mockRequests;
        }

        private static List<Specimen> GetSpecimensByID(string requestID)
        {
            List<Specimen> specimens = new List<Specimen>();



            return specimens;
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