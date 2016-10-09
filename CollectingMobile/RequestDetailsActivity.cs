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
                    if (string.Equals(arg1.Item.TitleFormatted.ToString(), "Logout")) //TODO: FIX THIS ASAP
                         {
                        ActiveUser.username = null;
                        var intent = new Intent(this, typeof(LoginActivity));
                        intent.SetFlags(ActivityFlags.ExcludeFromRecents);
                        intent.SetFlags(ActivityFlags.ClearTask);
                        StartActivity(intent);
                        this.Finish();
                    }
                    
                };
                menu.Show();
            };

            btnBack.Click += delegate
            {
                base.OnBackPressed();
            };
           
        }

        
    }
}