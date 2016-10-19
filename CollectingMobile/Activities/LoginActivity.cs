using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace CollectingMobile
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Locale)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Login);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {              
                SetToolbar();
            }

            Init();
        }

        private void Init()
        {
            Button btnLogin = FindViewById<Button>(Resource.Id.MyButton);
            EditText etUsername = FindViewById<EditText>(Resource.Id.username);
            EditText etPassword = FindViewById<EditText>(Resource.Id.password);
            List<User> listOfUsers = SerializationHelper.DeserializeUsers(this);

            etUsername.Text = "eugens1";
            etPassword.Text = "eugens1123%";

            btnLogin.Click += delegate
            {

#if !DEBUG
                ProgressDialog progressDialog = ProgressDialog.Show(this, "", Resources.GetText(Resource.String.Authenticating), true);

                if (listOfUsers.Exists(p => (p.Name == etUsername.Text && p.Password == etPassword.Text)))//TODO: user je pronađen u deserijaliziranoj listi?  exists vs any
                {
                    ActiveUser.User = new User(listOfUsers.First<User>(p => (p.Name == etUsername.Text && p.Password == etPassword.Text)));
                    StartActivity(typeof(ShowRequestsActivity));//prikazuje listu iz deserijaliziranog filea
                }
                else
                {


                    if (RestClient.AmIOnline(Application.Context))
                    {
                        new Thread(new ThreadStart(delegate
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
                }
#else
                ActiveUser.User = new User(etUsername.Text, etPassword.Text);
                StartActivity(typeof(ShowRequestsActivity));
#endif
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

