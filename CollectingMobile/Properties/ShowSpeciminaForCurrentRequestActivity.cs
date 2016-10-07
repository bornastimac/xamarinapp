using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CollectingMobile.Properties
{
    [Activity(Label = "ShowSpeciminaForCurrentRequestActivity")]
    public class ShowSpeciminaForCurrentRequestActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ShowSpecimenForCurrentRequest);
       //     Button back = FindViewById<Button>(Resource.Id.button1);
      //      back.Click += delegate
       //     {
      //          base.OnBackPressed();
        //    };
        }
    }
}