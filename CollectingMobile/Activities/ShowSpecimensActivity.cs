using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading;

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

            InitSpecimensView(FindViewById<ListView>(Resource.Id.SpecimenslistView));
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
                PopupMenu menu = new PopupMenu(this, e.View);
                menu.Inflate(Resource.Menu.popupRequest);
                menu.MenuItemClick += (s, arg) =>
                {
                    ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.Uploading), true);

                    new Thread(new ThreadStart(delegate
                    {
                        if (RestClient.UploadSpecimen(this, ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens[e.Position]))
                        {
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
            };
        }

        private void LoadSpecimensList()
        {
            List<string> specimenNames = new List<string>();

            foreach (Specimen specimen in ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens ?? new List<Specimen>())
            {
                //TODO: remove string uploaded
                string uploaded =  ((specimen.Uploaded) ? " | UP" : "");
                specimenNames.Add(specimen.ID.ToString() + uploaded);
            }

            var specimensListView = FindViewById<ListView>(Resource.Id.SpecimenslistView);
            specimensListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemActivated1, specimenNames);
            //specimensListView.ChoiceMode = ChoiceMode.Multiple;
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