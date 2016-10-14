using Android.Content;

namespace CollectingMobile
{
    class LogoutHandler
    {
        public static void LogMeOut(Context context)
        {
            ClearActiveData();

            Intent loginActivity = new Intent(context, typeof(LoginActivity));
            loginActivity.SetFlags(ActivityFlags.ClearTop);
            context.StartActivity(loginActivity);
        }

        private static void ClearActiveData()
        {
            ActiveUser.Username = null;
            ActiveRequests.Requests = null;
        }
    }
}