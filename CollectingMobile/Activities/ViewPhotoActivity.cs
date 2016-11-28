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
using System.Net.Http;
using System.IO;

namespace CollectingMobile
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ViewPhotoActivity : Activity
    {
        public  Java.IO.File photoSpecimen;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PhotoLayout);
            Java.IO.File photoSpecimen = new Java.IO.File(Intent.GetStringExtra("photoPath"));
            FindViewById<ImageView>(Resource.Id.photoRetakeImageView).SetImageBitmap(BitmapFactory.DecodeFile(photoSpecimen.AbsolutePath));
            FindViewById<Button>(Resource.Id.photoRetakeButton).Click += (object sender, EventArgs args) =>
            {
                Intent intent = new Intent(MediaStore.ActionImageCapture);               
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(photoSpecimen));
                StartActivityForResult(intent, 1777);

            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {

                byte[] bitmapBytes;
                photoSpecimen = new Java.IO.File(Intent.GetStringExtra("photoPath"));
                var bmp = BitmapFactory.DecodeFile(photoSpecimen.AbsolutePath);
                using (var ms = new MemoryStream())
                {
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, 75, ms);
                    bitmapBytes = ms.ToArray();
                }
                var resizedBitmap = (photoSpecimen.AbsolutePath).LoadAndResizeBitmap(400, 300);
                
                photoSpecimen.Delete();
                SpecimenInputActivity.SaveImage(photoSpecimen.AbsolutePath, bitmapBytes);
                FindViewById<ImageView>(Resource.Id.photoRetakeImageView).SetImageBitmap(BitmapFactory.DecodeFile(photoSpecimen.AbsolutePath));

            }


        }
    }
}