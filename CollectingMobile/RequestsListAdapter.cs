using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

namespace CollectingMobile
{
    public class RequestsListAdapter : BaseAdapter
    {
        List<Request> _requestList;
        Activity _activity;

        public override int Count
        {
            get
            {
                return _requestList.Count();
            }
        }

        public RequestsListAdapter(Activity activity)//, bool isOnline)
        {
            _activity = activity;
            //TODO: offline/online logika
            _requestList = ActiveRequests.Requests;
            //_requestList = RequestsSerialization.DeserializeAll(activity);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return -1;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.RequestListItem, parent, false);
            var requestId = view.FindViewById<TextView>(Resource.Id.requestId);
            var requestDescription = view.FindViewById<TextView>(Resource.Id.requestDescription);
            var requestDate = view.FindViewById<TextView>(Resource.Id.requestDate);
            requestId.Text = _requestList[position].Code;
            requestDescription.Text = _requestList[position].Description;
            requestDate.Text = _requestList[position].Created.ToShortDateString();
            return view;
        }
    }
}