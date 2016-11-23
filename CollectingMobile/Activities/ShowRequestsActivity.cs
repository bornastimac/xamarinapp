using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CollectingMobile
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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
            if (FindViewById<ListView>(Resource.Id.RequestsListView).Adapter == null)//only happens once at first load. Not in onCreate() so it doesnt block UI thread
            {
                LoadRequests(FindViewById<ListView>(Resource.Id.RequestsListView));
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (!RestClient.AmIOnline((ConnectivityManager)GetSystemService(ConnectivityService)))
                {
                    FindViewById<ImageButton>(Resource.Id.NoConnectionButton).SetImageResource(Resource.Drawable.ic_signal_wifi_off_white_18dp);
                }
                else
                {
                    FindViewById<ImageButton>(Resource.Id.NoConnectionButton).SetImageResource(0);
                }
            }
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.RootRequestsActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                FindViewById<TextView>(Resource.Id.ToolbarText).Text = Resources.GetText(Resource.String.Requests);
                FindViewById<ImageButton>(Resource.Id.NoConnectionButton).Click += delegate
                {
                    if (RestClient.AmIOnline((ConnectivityManager)GetSystemService(ConnectivityService)))
                    {
                        FindViewById<ImageButton>(Resource.Id.NoConnectionButton).SetImageResource(0);
                        Toast.MakeText(this, Resources.GetText(Resource.String.Connected), ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, Resources.GetText(Resource.String.CheckNetwork), ToastLength.Long).Show();
                    }

                };
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

            requestsView.ItemLongClick += delegate (object sender, AdapterView.ItemLongClickEventArgs e)
            {
                if (RestClient.AmIOnline((ConnectivityManager)GetSystemService(ConnectivityService)))
                {
                    if (requestsView.Adapter.GetType() == typeof(RequestsListAdapter))
                    {
                        PopupMenu menu = new PopupMenu(this, e.View);
                        menu.Inflate(Resource.Menu.popupRequest);
                        menu.MenuItemClick += (s, arg) =>
                        {
                            ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.Uploading), true);

                            new Thread(new ThreadStart(delegate
                            {
                                if (RestClient.UploadSpecimens(this, ActiveRequests.Requests[e.Position].Specimens))
                                {
                                    foreach (Specimen specimen in ActiveRequests.Requests[e.Position].Specimens)
                                    {
                                        specimen.Uploaded = true;
                                        RestClient.UploadImage(specimen);
                                    }
                                    ActiveRequests.Requests.RemoveAt(e.Position);
                                    SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                                    RunOnUiThread(() => requestsView.Adapter = new RequestsListAdapter(this, ActiveRequests.Requests));
                                    RunOnUiThread(() => Toast.MakeText(this, Resources.GetText(Resource.String.UploadSuccess), ToastLength.Short).Show());
                                }
                                else
                                {
                                    RunOnUiThread(() => Toast.MakeText(this, Resources.GetText(Resource.String.UploadError), ToastLength.Long).Show());
                                }
                                RunOnUiThread(() => progressDialog.Hide());
                            })).Start();
                        };
                        menu.Show();
                    }
                }
            };
        }

        private void LoadRequests(ListView requestsView)
        {
            ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.LoadingRequests), true);
            Dialog dialog = new AlertDialog.Builder(this)
                        .SetMessage(Resources.GetText(Resource.String.WebException))
                        .SetCancelable(false)
                        .SetNeutralButton(Resources.GetText(Resource.String.OK), (senderAlert, args) => {
                            Finish();
                        }).Create();

            if (RestClient.AmIOnline((ConnectivityManager)GetSystemService(ConnectivityService)))
            {
                new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        ActiveRequests.Requests = RestClient.GetRequestsFromServer();
                        SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                        RunOnUiThread(() => requestsView.Adapter = new RequestsListAdapter(this, ActiveRequests.Requests));
                        RunOnUiThread(() => progressDialog.Hide());
                    }
                    catch (Exception ex) when (ex is WebException || ex is UriFormatException)
                    {                     
                        RunOnUiThread(() => progressDialog.Hide());
                        RunOnUiThread(() => dialog.Show());
                    }
                })).Start();
            }
            else
            {
                Toast.MakeText(this, Resources.GetText(Resource.String.CheckNetwork) + "\n" + Resources.GetText(Resource.String.OfflineMode), ToastLength.Long).Show();
                ActiveRequests.Requests = SerializationHelper.DeserializeRequests(this);
                requestsView.Adapter = new RequestsListAdapter(this, ActiveRequests.Requests);
                progressDialog.Hide();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuRequests, menu);
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
                    StartActivity(typeof(ViewPhotoActivity));
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
