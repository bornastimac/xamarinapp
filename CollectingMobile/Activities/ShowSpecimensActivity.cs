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
    [Activity(Label = "RequestDetailsActivity")]
    public class ShowSpecimensActivity : ListActivity
    {      
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            LoadSpecimenList();
            SetToolbar();
        }

        private void LoadSpecimenList()
        {
            List<string> specimenNames = new List<string>();
            string requestId = Intent.GetStringExtra("SelectedRequestId") ?? "Intent data not available";
            foreach (Specimen specimen in ActiveRequests.GetSpecimensForRequest(requestId))
            {
                specimenNames.Add(specimen.description);
            }
            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, specimenNames);
        }

        private void SetToolbar()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetActionBar(toolbar);
                //ActionBar.Title = "Uzorci";
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

    }
}