using System;
using System.Collections.Generic;
using System.Linq;
using System.Json;

namespace CollectingMobile
{
    class RequestsFactory
    {
        private static int idMock = 0;

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
                    new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToInt64(new string(responseJSON[0]["DateCreated"].ToString().Where(c => Char.IsDigit(c)).ToArray()))),   //Unix time to DateTime             
                    new List<Specimen>()));
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
                mockRequests.Add(new Request("code" + (++idMock).ToString(), (++idMock).ToString(), "description_" + idMock, username, DateTime.Now, GetMockSpecimens(new Random().Next(1, 6))));
            }
            return mockRequests;
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