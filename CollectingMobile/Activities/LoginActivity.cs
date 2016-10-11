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

namespace CollectingMobile
{
    [Activity(Label = "CollectingMobile", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                SetContentView(Resource.Layout.Login);
                SetToolbar();
            }
            else
                SetContentView(Resource.Layout.LoginNoToolbar);

            Init();

        }

        private void Init()
        {
            Button btnLogin = FindViewById<Button>(Resource.Id.MyButton);
            EditText etUsername = FindViewById<EditText>(Resource.Id.username);
            EditText etPassword = FindViewById<EditText>(Resource.Id.password);

#if DEBUG
            etUsername.Text = "eugens1";
            etPassword.Text = "eugens1123%";
#endif

            btnLogin.Click += delegate
            {
                if (RestClient.IsServerReachable(Application.Context))
                {

                    var progressDialog = ProgressDialog.Show(this, "", "Authenticating...", true);

                    new Thread(new ThreadStart(delegate
                    {
                        if (RestClient.IsLoginOk(etUsername.Text, etPassword.Text))

                        {
                            ActiveUser.Username = etUsername.Text;
                            StartActivity(typeof(ShowRequestsActivity));
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
            };
        }

        private void SetToolbar()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetActionBar(toolbar);
                ActionBar.Title = "Collecting Mobile";
            }
        }
    }
}

