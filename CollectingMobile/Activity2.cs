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
using System.Threading;

namespace CollectingMobile
{
    [Activity(Label = "List of Orders", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SpecimenRequestsActivity : ListActivity
    {
        string[] items;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
         //   items = new string[] { "Lorem", "Ipsum", "Sit", "Dolorem", "dafuq", "12345" };
          //  new Thread(new ThreadStart(delegate
          //  {
                items = RestClient.GetDataFromServer();
               // RunOnUiThread(() => 
                ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);

         //   })).Start();
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = items[position];
            Toast.MakeText(this, t, ToastLength.Short).Show();
        }
    }
}