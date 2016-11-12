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

namespace CollectingMobile
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ViewPhotoActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PhotoLayout);
            
            File photoSpecimen = new File(Intent.GetStringExtra("photoPath"));
            if (photoSpecimen.Exists())
            {
                FindViewById<ImageView>(Resource.Id.PhotoFullView).SetImageBitmap(BitmapFactory.DecodeFile(photoSpecimen.AbsolutePath));
            }
        }


    }
}