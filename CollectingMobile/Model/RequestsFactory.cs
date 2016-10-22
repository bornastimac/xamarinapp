using System;
using System.Collections.Generic;
using System.Linq;
using System.Json;
using Newtonsoft.Json;

namespace CollectingMobile
{
    class RequestsFactory
    {
        private static int idRequestMock = 0;
        private static int idSpecimenMock = 0;

        public static List<Request> GetRequestsFromJSON(JsonValue responseJSON)
        {
            List<Request> requests = new List<Request>();

            //init array of requests
            for (int i = 0; i < responseJSON.Count; i++)
            {
                JsonValue requestJSON = responseJSON[i];

                requests.Add(new Request(
                    requestJSON["ID"],
                    requestJSON["Code"],
                    requestJSON["Description"],
                    requestJSON["AssignedTo"],
                    new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToInt64(new string(responseJSON[0]["DateCreated"].ToString().Where(c => Char.IsDigit(c)).ToArray()))),   //Unix time to DateTime             
                    new List<Specimen>()));
            }
            return requests;
        }

        public static List<Request> GetRequestsFromJSON(string responseJSON)
        {
            return JsonConvert.DeserializeObject<List<Request>>(responseJSON);
        }

        public static List<Specimen> GetSpecimensFromJSON(string responseJSON)
        {
            return JsonConvert.DeserializeObject<List<Specimen>>(responseJSON);
        }
       
        public static List<Request> GetMockRequestsForUser(string username, int numberOfRequests)
        {
            List<Request> mockRequests = new List<Request>();

            for (int i = 0; i < numberOfRequests; i++)
            {
                mockRequests.Add(new Request(++idRequestMock, "code" + (++idRequestMock).ToString(), "description_" + idRequestMock, username, DateTime.Now, GetMockSpecimens(new Random().Next(1, 6))));
            }
            return mockRequests;
        }

        private static List<Specimen> GetMockSpecimens(int numberOfSpecimens)
        {
            List<Specimen> mockSpecimens = new List<Specimen>();

            for (int i = 0; i < numberOfSpecimens; i++)
            {
                mockSpecimens.Add(new Specimen());
            }
            return mockSpecimens;
        }
    }
}