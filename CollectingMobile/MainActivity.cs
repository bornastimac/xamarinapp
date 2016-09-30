using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Json;
using System.Threading.Tasks;
using System.Text;

namespace CollectingMobile
{
    [Activity(Label = "CollectingMobile", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.Main);
            Button btnLogin = FindViewById<Button>(Resource.Id.MyButton);
            EditText etUsername = FindViewById<EditText>(Resource.Id.username);
            EditText etPassword = FindViewById<EditText>(Resource.Id.password);
            btnLogin.Click += delegate
            {
                if(RestClient.IsLoginOk(etUsername.Text,etPassword.Text))
                {
                    
                    StartActivity(typeof(Activity2));
                }
                else
                {
                    Toast.MakeText(this, "Incorrect Credentials", ToastLength.Long).Show();
                    etUsername.Text = "";
                    etPassword.Text = "";
                }
            };


        }
    }
}

