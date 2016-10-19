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
using Android.Locations;

namespace CollectingMobile
{
    [Activity]
    public class SpecimenInputActivity : Activity, ILocationListener
    {
        LocationManager locMan;      

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SpecimenInput);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SetToolbar();
            }

            InitButtons();
            InitViewValues();
        }

        private void InitViewValues()
        {
            Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));

            FindViewById<TextView>(Resource.Id.LocationText).Text = specimenSelected.Location;
            FindViewById<EditText>(Resource.Id.SamplingPositionText).Text = specimenSelected.SamplingPosition;
            //FindViewById<TextView>(Resource.Id.LocationText).Text = ((specimenSelected.Location.Equals("null") || (specimenSelected.Location==null)) ? ("-------") : (specimenSelected.Location));
            //FindViewById<EditText>(Resource.Id.SamplingPositionText).Text = (specimenSelected.SamplingPosition.Equals("null") || specimenSelected.SamplingPosition == null) ? "-------" : specimenSelected.SamplingPosition;
        }

        private void StartLocationSearch()
        {
            string locationProvider;
            Criteria locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;

            locMan = GetSystemService(LocationService) as LocationManager;
            locationProvider = locMan.GetBestProvider(locationCriteria, true);

            if (locMan.IsProviderEnabled(locationProvider))
            {
                locMan.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 1, this);
                Toast.MakeText(this, Resources.GetText(Resource.String.SearchingLocation), ToastLength.Short).Show();
                FindViewById<ImageButton>(Resource.Id.LocationButton).Enabled = false;
            }
            else
            {
                Toast.MakeText(this, Resources.GetText(Resource.String.NoLocationService), ToastLength.Short).Show();
            }
        }

        private void InitButtons()
        {
            //save specimen
            FindViewById<ImageButton>(Resource.Id.SaveButton).Click += (object sender, EventArgs args) => {
                Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));
                specimenSelected.Location = FindViewById<TextView>(Resource.Id.LocationText).Text;
                specimenSelected.SamplingPosition = FindViewById<EditText>(Resource.Id.SamplingPositionText).Text;
                SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                Toast.MakeText(this, Resources.GetText(Resource.String.Saved), ToastLength.Short).Show();
                Finish();
            };

            //get geolocation
            FindViewById<ImageButton>(Resource.Id.LocationButton).Click += (object sender, EventArgs args) => {               
                StartLocationSearch();             
            };
        }



        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.RootSpecimenInputActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.SpecimenInput);
            }
        }

        public void OnLocationChanged(Location location)
        {
            FindViewById<ImageButton>(Resource.Id.LocationButton).Enabled = true;
            FindViewById<TextView>(Resource.Id.LocationText).Text = location.Latitude + ", " + location.Longitude;
            locMan.RemoveUpdates(this);
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
    }
}