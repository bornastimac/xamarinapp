using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace CollectingMobile
{
    [Activity]
    public class ShowRequestsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SetContentView(Resource.Layout.Requests);
                SetToolbar();
            }
            else
                SetContentView(Resource.Layout.RequestsNoToolbar);


            InitRequestsView();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (FindViewById<ListView>(Resource.Id.RequestsListView).Adapter == null)
                LoadRequests(FindViewById<ListView>(Resource.Id.RequestsListView));

        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.Requests);
            }
        }

        private void InitRequestsView()
        {
            FindViewById<ListView>(Resource.Id.RequestsListView).ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                Intent showSpecimensActivity = new Intent(this, typeof(ShowSpecimensActivity));
                showSpecimensActivity.PutExtra("SelectedRequestId", ActiveRequests.GetRequestFromPosition(e.Position).ID);
                StartActivity(showSpecimensActivity);
            };
        }

        private void LoadRequests(ListView requestsView)
        {
            ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.LoadingRequests), true);

            if (RestClient.AmIOnline(Application.Context))
            {
                new Thread(new ThreadStart(delegate
                {
                    var clistAdapter = new RequestsListAdapter(this);
                    RunOnUiThread(() => requestsView.Adapter = clistAdapter);
                    RunOnUiThread(() => progressDialog.Hide());
                }
            )).Start();
            }
            else
            {
                RunOnUiThread(() => Toast.MakeText(this, Resources.GetText(Resource.String.CheckNetwork), ToastLength.Long).Show());
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
                case Resource.Id.Test:
                    RequestsSerialization.SerializeAll(this, ActiveRequests.Requests, ActiveUser.User.Name);
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed() { }

    }
}
