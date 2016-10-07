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

namespace CollectingMobile
{
    [Activity(Label = "RequestDetailsActivity")]
    public class RequestDetailsActivity : Activity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RequestDetails);
            Button btnBack = FindViewById<Button>(Resource.Id.Back);
            Button btnMenu = FindViewById<Button>(Resource.Id.Menu);

            btnMenu.Click += (s, arg) =>
            {
                PopupMenu menu = new PopupMenu(this, btnMenu);
                menu.Inflate(Resource.Menu.menu);

                menu.MenuItemClick += (s1, arg1) =>
                {
                    Toast.MakeText(this, arg1.Item.TitleFormatted, ToastLength.Short).Show();
                };
                menu.Show();
            };

            btnBack.Click += delegate
            {
                base.OnBackPressed();
            };
            // Create your application here
        }

        
    }
}