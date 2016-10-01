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

namespace CollectingMobile
{
    [Activity(Label = "CollectingMobile", MainLauncher = true, Icon = "@drawable/icon",ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
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
                var progressDialog = ProgressDialog.Show(this, "Please wait..", "Authenticating...", true);

                new Thread(new ThreadStart(delegate
                {
                    if (RestClient.IsLoginOk(etUsername.Text, etPassword.Text))
                    {

                        StartActivity(typeof(SpecimenRequestsActivity));
                    }
                    else
                    {
                        RunOnUiThread(() => etUsername.Text = "");
                        RunOnUiThread(() => etPassword.Text = "");
                        RunOnUiThread(() => Toast.MakeText(ApplicationContext, "Incorrect Credentials", ToastLength.Long).Show());
                    }
                    RunOnUiThread(() => progressDialog.Hide());

                })).Start();

                

            };


        }
    }
}

