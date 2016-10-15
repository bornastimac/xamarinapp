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
    [Serializable]
    public class User
    {
        public readonly string Name;
        public readonly string Password;

        public User(string username, string password)
        {
            this.Name = username;
            this.Password = password;
        }
    }
}