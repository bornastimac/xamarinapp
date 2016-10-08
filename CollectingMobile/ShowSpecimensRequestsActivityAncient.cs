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
using System.Threading;

namespace CollectingMobile
{
    [Activity(Label = "Specimen Request", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ShowSpecimenRequestsActivity2 :ListActivity
    {
       // List<SpecimensRequest> specimensRequests;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
       //     SetContentView(Resource.Layout.SpecimenRequests);
            var clistAdapter = new CustomListAdapter(this);
            var requesListView = FindViewById<ListView>(Resource.Id.RequestDetailsListView);
            requesListView.Adapter = clistAdapter; 

            #region
            //cekanje na server
            // specimensRequests = RestClient.GetDataFromServer();
            //  var items = new string[] { "Lorem", "Ipsum", "Sit", "Dolorem", "dafuq", "12345" };
            //  new Thread(new ThreadStart(delegate
            //  {
            //  items = RestClient.GetDataFromServer();
            // RunOnUiThread(() => 
            //    ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items );

            //   })).Start();
            #endregion
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            StartActivity(typeof(RequestDetailsActivity));
        }
        [Android.Runtime.Register("onBackPressed", "()V", "GetOnBackPressedHandler")]
        public override void OnBackPressed() { }
    }

}
//id, description i date, zapisat u view