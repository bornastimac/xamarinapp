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
    [Activity(Label = "ShowSpecimensRequestsActivity2")]
    public class ShowSpecimensRequestsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SpecimenRequests);
            var clistAdapter = new CustomListAdapter(this);
            var requestListView = FindViewById<ListView>(Resource.Id.RequestsListView);
            requestListView.Adapter = clistAdapter;
            requestListView.ItemClick += delegate
            {
                StartActivity(typeof(RequestDetailsActivity));
            };
        }

       
             [Android.Runtime.Register("onBackPressed", "()V", "GetOnBackPressedHandler")]
        public override void OnBackPressed() { }
        
    }
}
