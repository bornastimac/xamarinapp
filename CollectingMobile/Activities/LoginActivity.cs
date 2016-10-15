using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using System.Threading;

namespace CollectingMobile
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Locale)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
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
                
//#if !DEBUG
                ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.Authenticating) , true);

                if (RestClient.AmIOnline(Application.Context))
                {
                    new Thread(new ThreadStart(delegate
                    {
                        if (RestClient.IsLoginOk(etUsername.Text, etPassword.Text))
                        {
                            ActiveUser.User = new User(etUsername.Text, etPassword.Text);
                            StartActivity(typeof(ShowRequestsActivity));
                        }
                        else
                        {
                            if (RestClient.IsServerReachable(Application.Context))
                            {
                                RunOnUiThread(() => etUsername.Text = "");
                                RunOnUiThread(() => etPassword.Text = "");
                                RunOnUiThread(() => Toast.MakeText(ApplicationContext, "Incorrect Credentials", ToastLength.Long).Show());
                            }
                        }
                        RunOnUiThread(() => progressDialog.Hide());
                    })).Start();
                }
                else
                {
                    progressDialog.Hide();
                }
//#else
                //ActiveUser.Username = etUsername.Text;
                //StartActivity(typeof(ShowRequestsActivity)); 
//#endif
            };
        }

        private void SetToolbar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetActionBar(toolbar);
                ActionBar.Title = Resources.GetText(Resource.String.ApplicationName);
            }
        }
    }
}

