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
using System.Threading;
using Android.Net;
//TODO: 1. User se autorizira online i spremaju mu se podaci
//TODO: 2. Ako nema net, logira se pomoću tih podataka
//started working on toolbar
namespace CollectingMobile
{
    [Activity(Label = "CollectingMobile", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);
            Button btnLogin = FindViewById<Button>(Resource.Id.MyButton);
            EditText etUsername = FindViewById<EditText>(Resource.Id.username);
            EditText etPassword = FindViewById<EditText>(Resource.Id.password);
            

            btnLogin.Click += delegate
            {
              
                
                if (RestClient.AmIOnline(Application.Context))
                {
                    var progressDialog = ProgressDialog.Show(this, "", "Authenticating...", true);

                    new Thread(new ThreadStart(delegate
                    {
                        if (RestClient.IsLoginOk(etUsername.Text, etPassword.Text))

                        {
                            ActiveUser.username = etUsername.Text;
                            StartActivity(typeof(ShowSpecimensRequestsActivity));
                            Finish();
                        }
                        else
                        {
                            RunOnUiThread(() => etUsername.Text = "");
                            RunOnUiThread(() => etPassword.Text = "");
                            RunOnUiThread(() => Toast.MakeText(ApplicationContext, "Incorrect Credentials", ToastLength.Long).Show());
                        }
                        RunOnUiThread(() => progressDialog.Hide());

                    })).Start();
                }

                else
                {
                    Toast.MakeText(this, "Check your network connection", ToastLength.Long).Show();
                }

            };
        }

        [Android.Runtime.Register("onBackPressed", "()V", "GetOnBackPressedHandler")] //TODO: drugacije rijesiti ovo
        public override void OnBackPressed() { }
    }
}

