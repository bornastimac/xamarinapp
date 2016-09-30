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
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate
            {
                RestClient.GetResponse();
            };


        }
    }
}

