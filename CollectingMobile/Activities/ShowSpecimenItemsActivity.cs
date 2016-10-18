using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace CollectingMobile
{
    [Activity]
    public class ShowSpecimenItemsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SpecimenItems);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {           
                SetToolbar();
            }

            LoadSpecimenItemsList();          
        }

        private void LoadSpecimenItemsList()
        {
            List<string> specimenItemsNames = new List<string>();

            foreach (SpecimenItem specimenItem in ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).GetSpecimenByID(Intent.GetIntExtra("SelectedSpecimenId", -1)).Items ?? new List<SpecimenItem>())
            {
                specimenItemsNames.Add(specimenItem.ID.ToString());
            }

            var specimenItemsListView = FindViewById<ListView>(Resource.Id.SpecimenItemslistView);
            specimenItemsListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, specimenItemsNames);
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.RootLoginActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.Requests);
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