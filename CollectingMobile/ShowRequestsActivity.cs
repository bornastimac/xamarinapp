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
    [Activity(Label = "Nalozi")]
    public class ShowRequestsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Requests);

            if (RestClient.AmIOnline(this))
            {
                var clistAdapter = new RequestsListAdapter(this);
                var requestListView = FindViewById<ListView>(Resource.Id.RequestsListView);
                requestListView.Adapter = clistAdapter;
                requestListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
                {
                    Intent showSpecimensActivity = new Intent(this, typeof(ShowSpecimensActivity));
                    showSpecimensActivity.PutExtra("SelectedRequestId", ActiveRequests.GetRequestFromPosition(e.Position).id);
                    StartActivity(showSpecimensActivity);
                };
            }
            else
            {
                Toast.MakeText(this, "Check your network connection", ToastLength.Long).Show();
            }

        }

       
             [Android.Runtime.Register("onBackPressed", "()V", "GetOnBackPressedHandler")]
        public override void OnBackPressed() { }
        
    }
}
