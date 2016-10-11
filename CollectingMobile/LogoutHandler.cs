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
    class LogoutHandler
    {
        public static void LogMeOut(Activity act)
        {
            ClearData();

            Intent loginActivity = new Intent(act, typeof(LoginActivity));
            loginActivity.SetFlags(ActivityFlags.ClearTop);
            act.StartActivity(loginActivity);
        }

        private static void ClearData()
        {
            ActiveUser.Username = null;
            ActiveRequests.Requests = null;
        }
    }
}