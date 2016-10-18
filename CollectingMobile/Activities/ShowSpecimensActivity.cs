using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
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
            SetContentView(Resource.Layout.Specimens);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SetToolbar();
            }

            InitSpecimensView();
            LoadSpecimensList();          
        }

        private void InitSpecimensView()
        {
            FindViewById<ListView>(Resource.Id.SpecimenslistView).ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                Intent showSpecimenItemsActivity = new Intent(this, typeof(ShowSpecimenItemsActivity));
                showSpecimenItemsActivity.PutExtra("SelectedRequestId", ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).ID);
                showSpecimenItemsActivity.PutExtra("SelectedSpecimenId", ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens[e.Position].ID);
                StartActivity(showSpecimenItemsActivity);
            };
        }

        private void LoadSpecimensList()
        {
            List<string> specimenNames = new List<string>();

            foreach (Specimen specimen in ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens ?? new List<Specimen>())
            {
                specimenNames.Add(specimen.Description);
            }

            var specimensListView = FindViewById<ListView>(Resource.Id.SpecimenslistView);
            specimensListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, specimenNames);
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.RootSpecimensActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.Specimens);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Logout:
                    LogoutHandler.LogMeOut(this);
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

    }
}