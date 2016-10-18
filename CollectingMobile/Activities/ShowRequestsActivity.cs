using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
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
            SetContentView(Resource.Layout.Requests);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {              
                SetToolbar();
            }

            InitRequestsView(FindViewById<ListView>(Resource.Id.RequestsListView));
        }

        protected override void OnStart()
        {
            base.OnStart();
            if(FindViewById<ListView>(Resource.Id.RequestsListView).Adapter == null)//only happens once at first load. Not in onCreate() so it doesnt block UI thread
            {
                LoadRequests(FindViewById<ListView>(Resource.Id.RequestsListView));
            }
            
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {            
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.RootRequestsActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.Requests);
            }
        }

        private void InitRequestsView(ListView requestsView)
        {
            requestsView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                if (requestsView.Adapter.GetType() == typeof(RequestsListAdapter))
                {
                    Intent showSpecimensActivity = new Intent(this, typeof(ShowSpecimensActivity));
                    showSpecimensActivity.PutExtra("SelectedRequestId", ActiveRequests.Requests[e.Position].ID);
                    StartActivity(showSpecimensActivity);
                }
            };

            //FindViewById<SwipeRefreshLayout>(Resource.Id.SwipeRefresh)
        }

        private void LoadRequests(ListView requestsView)
        {
            ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.LoadingRequests), true);

            if (RestClient.AmIOnline(Application.Context))
            {
                new Thread(new ThreadStart(delegate
                {
                    ActiveRequests.Requests = RestClient.GetRequestsFromServer();
                    SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                    RunOnUiThread(() => requestsView.Adapter = new RequestsListAdapter(this, ActiveRequests.Requests));
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();
            }
            else
            {
                //TODO: notify user no internet connection. Maybe icon in toolbar for no internet connection?
                ActiveRequests.Requests = SerializationHelper.DeserializeRequests(this);
                new Thread(new ThreadStart(delegate
                {
                    RunOnUiThread(() => requestsView.Adapter = new RequestsListAdapter(this, ActiveRequests.Requests));
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();
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
                case Resource.Id.RefreshRequests:
                    LoadRequests(FindViewById<ListView>(Resource.Id.RequestsListView));
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed() { }

    }
}
