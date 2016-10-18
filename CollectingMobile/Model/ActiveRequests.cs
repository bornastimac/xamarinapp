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
                return requests;
            }
            set
            {
                requests = value;
            }
        }

        public static Request GetRequestByID(int requestId)
        {
            return requests.Find(x => x.ID == requestId);
        }
        
    }
}