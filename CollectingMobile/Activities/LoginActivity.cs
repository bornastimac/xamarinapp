using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Android.Net;
using Newtonsoft.Json;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;
using ZXing.Mobile;

namespace CollectingMobile
{
    [Activity(MainLauncher = true, Icon = "@drawable/inRebus", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Locale)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Login);
            Button btn = FindViewById<Button>(Resource.Id.Camera);
            btn.Click += delegate
             {
                 StartActivity(typeof(CameraActivity));
             };

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SetToolbar();
            }

            Init();
            RestClient.serverDomain = ConfigHelper.GetServerDomain();
        }

        private void Init()
        {
            MobileBarcodeScanner.Initialize(Application);

            Button btnLogin = FindViewById<Button>(Resource.Id.MyButton);
            EditText etUsername = FindViewById<EditText>(Resource.Id.username);
            EditText etPassword = FindViewById<EditText>(Resource.Id.password);
            List<User> listOfUsers = SerializationHelper.DeserializeUsers(this);

#if DEBUG
            etUsername.Text = "eugens1";
            etPassword.Text = "eugens1123%";
#endif

            btnLogin.Click += delegate
            {
                ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.Authenticating), true);

                if (listOfUsers.Exists(p => (p.Name == etUsername.Text && p.Password == etPassword.Text)))
                {
                    ActiveUser.User = new User(listOfUsers.First(p => (p.Name == etUsername.Text && p.Password == etPassword.Text)));
                    progressDialog.Hide();
                    StartActivity(typeof(ShowRequestsActivity));
                }
                else
                {
                    if (RestClient.AmIOnline((ConnectivityManager)GetSystemService(ConnectivityService)))
                    {
                        Dialog dialog = new AlertDialog.Builder(this)
                        .SetMessage(Resources.GetText(Resource.String.WebException))
                        .SetCancelable(false)
                        .SetNeutralButton(Resources.GetText(Resource.String.OK), (senderAlert, args) =>
                        {
                            Finish();
                        }).Create();

                        new Thread(new ThreadStart(delegate
                        {
                            try
                            {
                                if (RestClient.IsLoginOk(etUsername.Text, etPassword.Text))
                                {
                                    ActiveUser.User = new User(etUsername.Text, etPassword.Text);
                                    listOfUsers.Add(ActiveUser.User);
                                    SerializationHelper.SerializeUsers(this, listOfUsers);
                                    StartActivity(typeof(ShowRequestsActivity));
                                }
                                else
                                {
                                    if (RestClient.IsServerReachable())
                                    {
                                        RunOnUiThread(() => etUsername.Text = "");
                                        RunOnUiThread(() => etPassword.Text = "");
                                        RunOnUiThread(() => Toast.MakeText(ApplicationContext, "Incorrect Credentials", ToastLength.Long).Show());
                                    }
                                }
                                RunOnUiThread(() => progressDialog.Hide());
                            }
                            catch (Exception ex) when (ex is WebException || ex is UriFormatException)
                            {
                                RunOnUiThread(() => progressDialog.Hide());
                                RunOnUiThread(() => dialog.Show());
                            }
                        })).Start();
                    }
                    else
                    {
                        progressDialog.Hide();
                        Toast.MakeText(this, Resources.GetText(Resource.String.CheckNetwork), ToastLength.Long).Show();
                    }
                }
            };
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = (Toolbar)LayoutInflater.Inflate(Resource.Layout.toolbar, null);
                toolbar.LayoutParameters = new Android.Views.ViewGroup.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
                FindViewById<RelativeLayout>(Resource.Id.RootLoginActivity).AddView(toolbar, 0);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.Login);
            }
        }
    }
}

