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

using Java.IO;
using Android.Provider;
using Android.Net;
using System.IO;
using CollectingMobile.Model;
using Android.Media;

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

            //var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            //NetworkInfo activeConnection = cm.ActiveNetworkInfo;
            //if ((activeConnection != null) && activeConnection.IsConnected)
            //    LogoutHandler.LogMeOut(this);


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
            Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));
            Java.IO.File photoSpecimen = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimenSelected.ID + ".jpeg");
            if (photoSpecimen.Exists())
            {
                var resizedBitmap = (Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimenSelected.ID.ToString() + ".jpeg").LoadAndResizeBitmap(400, 300);
                FindViewById<ImageView>(Resource.Id.PhotoView).SetImageBitmap(resizedBitmap);
            }
            
        }


        private void InitViewValues()
        {
            Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));

            FindViewById<EditText>(Resource.Id.LocationText).Text = specimenSelected.Location == "" || specimenSelected.Location == null ? "-----, -----" : specimenSelected.Location;
            FindViewById<EditText>(Resource.Id.SamplingPositionText).Text = specimenSelected.SamplingPosition;
            FindViewById<EditText>(Resource.Id.QRText).Text = specimenSelected.Qrcode == "" || specimenSelected.Qrcode == null ? "----" : specimenSelected.Qrcode;
        }

        private void StartLocationSearch()
        {
            //string locationProvider;
            //Criteria locationCriteria = new Criteria();
            //locationCriteria.Accuracy = Accuracy.Coarse;
            //locationCriteria.PowerRequirement = Power.Medium;

            locMan = GetSystemService(LocationService) as LocationManager;
            //locationProvider = locMan.GetBestProvider(locationCriteria, true);

            if (locMan.IsProviderEnabled(LocationManager.GpsProvider))
            {

                FindViewById<TextView>(Resource.Id.LocationText).Text = ".";
                searchingLocationAnimationTimer = new Timer();
                searchingLocationAnimationTimer.Interval = 500;
                searchingLocationAnimationTimer.Elapsed += AnimateLocationText;
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
            RunOnUiThread(() =>
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
            );

        }

        private void InitButtons()
        {
            //save specimen

            FindViewById<ImageButton>(Resource.Id.SaveButton).Click += (object sender, EventArgs args) =>
            {
                ((Vibrator)GetSystemService(VibratorService)).Vibrate(50);
                Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));
                specimenSelected.Location = FindViewById<EditText>(Resource.Id.LocationText).Text;
                specimenSelected.SamplingPosition = FindViewById<EditText>(Resource.Id.SamplingPositionText).Text;
                specimenSelected.Qrcode = FindViewById<EditText>(Resource.Id.QRText).Text;
                SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                Toast.MakeText(this, Resources.GetText(Resource.String.Saved), ToastLength.Short).Show();
                Finish();
            };

            //get geolocation
            FindViewById<ImageButton>(Resource.Id.LocationButton).Click += (object sender, EventArgs args) =>
            {
                ((Vibrator)GetSystemService(VibratorService)).Vibrate(50);
                StartLocationSearch();
            };

            //QR scan
            FindViewById<ImageButton>(Resource.Id.QRButton).Click += (object sender, EventArgs args) =>
            {
                ((Vibrator)GetSystemService(VibratorService)).Vibrate(50);
                ScanQR();
            };

            //add image
            FindViewById<ImageButton>(Resource.Id.TakePhoto).Click += (object sender, EventArgs args) =>
            {
                ((Vibrator)GetSystemService(VibratorService)).Vibrate(50);
                Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));
                Java.IO.File photoSpecimen = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimenSelected.ID + ".jpeg");

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                Java.IO.File file = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/", specimenSelected.ID + ".jpeg");
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));
                StartActivityForResult(intent, 1777);
            };

            FindViewById<ImageView>(Resource.Id.PhotoView).Click += (object sender, EventArgs args) =>
             {
                 Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));
                 Java.IO.File photoSpecimen = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimenSelected.ID + ".jpeg");
                 if (photoSpecimen.Exists())
                 {
                     ((Vibrator)GetSystemService(VibratorService)).Vibrate(50);
                     Intent viewPhoto = new Intent(this, typeof(ViewPhotoActivity));
                     viewPhoto.PutExtra("photoPath", photoSpecimen.AbsolutePath);
                     StartActivity(viewPhoto);
                 }
             };
        }

        private async void ScanQR()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null)
            {
                RunOnUiThread(() => FindViewById<EditText>(Resource.Id.QRText).Text = result.Text);
            }
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                FindViewById<LinearLayout>(Resource.Id.toolbar).AddView(toolbar, 0);
                SetActionBar(toolbar);
                FindViewById<TextView>(Resource.Id.ToolbarText).Text = Resources.GetText(Resource.String.SpecimenInput);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {

                byte[] bitmapBytes;
                Specimen specimenSelected = ActiveRequests.GetRequestByID(Intent.GetIntExtra("SelectedRequestId", -1)).Specimens.Find(spec => spec.ID == Intent.GetIntExtra("SelectedSpecimenId", -1));
                Java.IO.File toDelete = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/", specimenSelected.ID + ".jpeg");
                specimenSelected.PhotoFileName = specimenSelected.ID + ".jpeg";
                SerializationHelper.SerializeRequests(this, ActiveRequests.Requests);
                var bmp = BitmapFactory.DecodeFile(toDelete.AbsolutePath);
                using (var ms = new MemoryStream())
                {
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, 75, ms);
                    bitmapBytes = ms.ToArray();
                }
                var resizedBitmap = (Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile/Pictures/" + specimenSelected.PhotoFileName).LoadAndResizeBitmap(400,300);
                FindViewById<ImageView>(Resource.Id.PhotoView).SetImageBitmap(resizedBitmap);
                toDelete.Delete();

                SaveImage(toDelete.AbsolutePath, bitmapBytes);     
            }
        }

        public static void SaveImage(string path, byte[] imageData)
        {
            Java.IO.File pictureFile = new Java.IO.File(path);
            using (FileOutputStream writer = new FileOutputStream(pictureFile))
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