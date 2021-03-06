using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.Net;
using CollectingMobile.Model;

namespace CollectingMobile
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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

            InitSpecimensView(FindViewById<ListView>(Resource.Id.SpecimenslistView));
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (!RestClient.AmIOnline((ConnectivityManager)GetSystemService(ConnectivityService)))
                {
                    FindViewById<ImageButton>(Resource.Id.NoConnectionButton).SetImageResource(Resource.Drawable.ic_signal_wifi_off_white_24dp);
                }
                else
                {
                    FindViewById<ImageButton>(Resource.Id.NoConnectionButton).SetImageResource(0);
                }
            }
            LoadSpecimensList();
        }

        private void InitSpecimensView(ListView specimensView)
        {
            specimensView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                Intent specimenInputActivity = new Intent(this, typeof(SpecimenInputActivity));
                specimenInputActivity.PutExtra("SelectedRequestId", ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).ID);
                specimenInputActivity.PutExtra("SelectedSpecimenId", ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens[e.Position].ID ?? -1);
                StartActivity(specimenInputActivity);
            };

            specimensView.ItemLongClick += delegate (object sender, AdapterView.ItemLongClickEventArgs e)
            {
                if (ActiveUser.cookies.Count != 0 && RestClient.AmIOnline((ConnectivityManager)GetSystemService(ConnectivityService)))
                {
                    PopupMenu menu = new PopupMenu(this, e.View);
                    menu.Inflate(Resource.Menu.popupRequest);
                    menu.MenuItemClick += (s, arg) =>
                    {
                        ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.Uploading), true);

                        new Thread(new ThreadStart(delegate
                        {
                            if (RestClient.UploadSpecimen(this, ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens[e.Position]))
                            {
                                RestClient.UploadImage(ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens[e.Position]);
                                ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens[e.Position].Uploaded = true;
                                SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                                RunOnUiThread(() => LoadSpecimensList());
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
                else
                {
                    Toast.MakeText(this, Resources.GetText(Resource.String.PleaseRelog), ToastLength.Long).Show();
                }
            };
        }

        private void LoadSpecimensList()
        {
            List<string> specimenNames = new List<string>();
            int i = 0;
            foreach (Specimen specimen in ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens ?? new List<Specimen>())
            {
                string uploaded = ((specimen.Uploaded) ? " | UP" : "");
                specimenNames.Add(++i + "  " + ((specimen.Material != null) ? specimen.Material.ToString() : "?") + "  " + ((specimen.SamplingPosition != null) ? "\n(" + specimen.SamplingPosition + ")" : ""));
            }

            var specimensListView = FindViewById<ListView>(Resource.Id.SpecimenslistView);
            specimensListView.Adapter = new ArrayAdapter<string>(this, Resource.Layout.SpecimenItem, specimenNames);
            //specimensListView.ChoiceMode = ChoiceMode.Multiple;
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.RootSpecimensActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                FindViewById<TextView>(Resource.Id.ToolbarText).Text = Resources.GetText(Resource.String.Specimens);
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