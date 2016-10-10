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
    [Activity(Label = "RequestDetailsActivity")]
    public class ShowSpecimensActivity : ListActivity
    {      
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            List<string> specimenNames = new List<string>();
            string requestId = Intent.GetStringExtra("SelectedRequestId") ?? "Intent data not available";
            foreach (Specimen specimen in ActiveRequests.GetSpecimensForRequest(requestId))
            {
                specimenNames.Add(specimen.description);
            }
            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, specimenNames);

        }

        
    }
}