using System.Collections.Generic;

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