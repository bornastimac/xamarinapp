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

            LoadRequests();
            SetToolbar();
        }

        private void SetToolbar()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetActionBar(toolbar);
                ActionBar.Title = "Nalozi";
            }
        }

        private void LoadRequests()
        {
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        [Android.Runtime.Register("onBackPressed", "()V", "GetOnBackPressedHandler")]
        public override void OnBackPressed() { }
        
    }
}
