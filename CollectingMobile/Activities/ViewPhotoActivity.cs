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
using Android.Util;

namespace CollectingMobile
{
    [Activity]
    public class ViewPhotoActivity : Activity
    {
        public  Java.IO.File photoSpecimen;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PhotoLayout);
            var photoContainer = FindViewById<RelativeLayout>(Resource.Id.PhotoLayout);
            var photoSpecimen = new Java.IO.File(Intent.GetStringExtra("photoPath"));
            var opts = new BitmapFactory.Options();
            opts.InSampleSize = 4;
            FindViewById<ImageView>(Resource.Id.photoRetakeImageView).SetImageBitmap(BitmapFactory.DecodeFile(photoSpecimen.AbsolutePath,opts));
        }



        
    }
}