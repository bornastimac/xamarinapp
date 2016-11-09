using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.IO;
using Android.Graphics;
using Android.Provider;
using Android.Content.PM;
using Android.Net;
using CollectingMobile.Model;


namespace CollectingMobile
{
    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }
    [Activity(Label = "CameraActivity")]
    public class CameraActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CameraLayout);

            if (IsThereAnAppToTakePictures())
            {

                TakeAPicture();
            }
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            
            if( resultCode == Result.Ok)
            {
                var bitmap = (Bitmap)data.Extras.Get("data");
                ImageView iw = FindViewById<ImageView>(Resource.Id.imageView1);
                iw.SetImageBitmap(bitmap);
                byte[] bitmapData;
                using(var stream = new System.IO.MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);  //moze ici i async
                    bitmapData = stream.ToArray();
                }
                
                //pošalji bitmapData u json-u
                
                
            }
          //  GC.Collect();
        }

        private void TakeAPicture()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            StartActivityForResult(intent, 0);
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CollectingMobilePictures");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

    }
}