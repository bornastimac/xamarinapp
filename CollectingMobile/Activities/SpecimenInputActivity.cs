using System;
using System.Timers;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Graphics;
using System.Net.Http;
using Java.IO;
using Android.Provider;
using Android.Net;

namespace CollectingMobile
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SpecimenInputActivity : Activity, ILocationListener
    {
        LocationManager locMan;
        Timer searchingLocationAnimationTimer;

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

        private void InitViewValues()
        {
            Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));

            FindViewById<EditText>(Resource.Id.LocationText).Text = specimenSelected.Location == "" || specimenSelected.Location == null ? "-----, -----" : specimenSelected.Location;
            FindViewById<EditText>(Resource.Id.SamplingPositionText).Text = specimenSelected.SamplingPosition;
            FindViewById<EditText>(Resource.Id.QRText).Text = specimenSelected.Qrcode == "" || specimenSelected.Qrcode == null ? "----" : specimenSelected.Qrcode;

            File photoSpecimen = new File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimenSelected.ID + ".png");
            if (photoSpecimen.Exists())
            {
                FindViewById<ImageView>(Resource.Id.PhotoView).SetImageBitmap(BitmapFactory.DecodeFile(photoSpecimen.AbsolutePath));               
            }

            FindViewById<ImageView>(Resource.Id.PhotoView).Click += delegate
            {
                if (photoSpecimen.Exists())
                {
                    Intent viewPhoto = new Intent(this, typeof(ViewPhotoActivity));
                    viewPhoto.PutExtra("photoPath", photoSpecimen.AbsolutePath);
                    StartActivity(viewPhoto);
                }             
            };
        }

        private void StartLocationSearch()
        {
            searchingLocationAnimationTimer = new Timer();
            searchingLocationAnimationTimer.Interval = 500;
            searchingLocationAnimationTimer.Elapsed += AnimateLocationText;

            //string locationProvider;
            //Criteria locationCriteria = new Criteria();
            //locationCriteria.Accuracy = Accuracy.Coarse;
            //locationCriteria.PowerRequirement = Power.Medium;

            locMan = GetSystemService(LocationService) as LocationManager;
            //locationProvider = locMan.GetBestProvider(locationCriteria, true);

            if (locMan.IsProviderEnabled(LocationManager.GpsProvider))
            {
                locMan.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 1, this);
                Toast.MakeText(this, Resources.GetText(Resource.String.SearchingLocation), ToastLength.Short).Show();
                FindViewById<ImageButton>(Resource.Id.LocationButton).Enabled = false;
                searchingLocationAnimationTimer.Start();
            }
            else
            {
                Toast.MakeText(this, Resources.GetText(Resource.String.NoLocationService), ToastLength.Short).Show();
            }
        }

        private void AnimateLocationText(object sender, ElapsedEventArgs args)
        {
            TextView tw = FindViewById<TextView>(Resource.Id.LocationText);
            if (tw.Text.Length <= 18)
            {
                tw.Text += " . .";
            }
            else
            {
                tw.Text = ".";
            }
        }
        
        private void InitButtons()
        {
            //save specimen
            FindViewById<Button>(Resource.Id.SaveButton).Click += (object sender, EventArgs args) => {
                Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));
                specimenSelected.Location = FindViewById<EditText>(Resource.Id.LocationText).Text;
                specimenSelected.SamplingPosition = FindViewById<EditText>(Resource.Id.SamplingPositionText).Text;
                specimenSelected.Qrcode = FindViewById<EditText>(Resource.Id.QRText).Text;
                SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                Toast.MakeText(this, Resources.GetText(Resource.String.Saved), ToastLength.Short).Show();
                Finish();
            };

            //get geolocation
            FindViewById<ImageButton>(Resource.Id.LocationButton).Click += (object sender, EventArgs args) => {
                StartLocationSearch();             
            };

            //QR scan
            FindViewById<ImageButton>(Resource.Id.QRButton).Click += (object sender, EventArgs args) => {
                ScanQR();
            };

            //add image
            FindViewById<ImageButton>(Resource.Id.PhotoButton).Click += (object sender, EventArgs args) => {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(intent, 0);
            };
        }

        private async void ScanQR()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null)
            {
                RunOnUiThread(()=> FindViewById<EditText>(Resource.Id.QRText).Text = result.Text);
            }
        }


        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.RootSpecimenInputActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.SpecimenInput);
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                var bitmap = (Bitmap)data.Extras.Get("data");
                FindViewById<ImageView>(Resource.Id.PhotoView).SetImageBitmap(bitmap);
                byte[] bitmapData;

                using (var stream = new System.IO.MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    bitmapData = stream.ToArray();
                }

                Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));

                SaveImage(specimenSelected.ID.ToString(), bitmapData);
            }
        }

        private void SaveImage(string specimenID, byte[] imageData)
        {
            Java.IO.File picctureFile = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimenID + ".png");
            using (FileOutputStream writer = new FileOutputStream(picctureFile))
            {
                writer.Write(imageData);
            }
        }

        public void OnLocationChanged(Location location)
        {
            FindViewById<ImageButton>(Resource.Id.LocationButton).Enabled = true;
            searchingLocationAnimationTimer.Stop();
            FindViewById<EditText>(Resource.Id.LocationText).Text = location.Latitude + ", " + location.Longitude;
            locMan.RemoveUpdates(this);
        }

        public void OnProviderDisabled(string provider)
        {
            FindViewById<ImageButton>(Resource.Id.LocationButton).Enabled = true;
            Toast.MakeText(this, Resources.GetText(Resource.String.NoLocationService), ToastLength.Long).Show();

        }

        public void OnProviderEnabled(string provider)
        {
            FindViewById<ImageButton>(Resource.Id.LocationButton).Enabled = true;
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
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