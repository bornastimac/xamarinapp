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
    [Activity]
    public class ShowSpecimensActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SetContentView(Resource.Layout.Specimens);
                SetToolbar();
            }
            else

                SetContentView(Resource.Layout.SpecimensNoToolbar);

            LoadSpecimensList();          
        }

        private void LoadSpecimensList()
        {
            List<string> specimenNames = new List<string>();
            string requestId = Intent.GetStringExtra("SelectedRequestId") ?? "Intent data not available";
            
            foreach (Specimen specimen in ActiveRequests.GetSpecimensForRequest(requestId) ?? new List<Specimen>())
            {
                specimenNames.Add(specimen.description);
            }

            var specimensListView = FindViewById<ListView>(Resource.Id.SpecimenslistView);
            specimensListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, specimenNames);
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetActionBar(toolbar);
                ActionBar.Title = "Uzorci";
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.TitleFormatted.ToString() == "Logout")
            {
                LogoutHandler.LogMeOut(this);
            }

            return base.OnOptionsItemSelected(item);
        }

    }
}