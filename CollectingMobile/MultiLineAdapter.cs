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
    class MultiLineAdapter : BaseAdapter<SpecimensRequest>
    {
        List<SpecimensRequest> items;
        Activity context;
        public MultiLineAdapter(Activity context, List<SpecimensRequest> items) : base()
        {
            this.context = context;
            this.items = items;
        }

       
        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override SpecimensRequest this[int position]
        {
            get
            {
                return items[position];
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.CustomLayout, null);
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.description;
            view.FindViewById<TextView>(Resource.Id.Text2).Text = item.created.ToString() ;
            view.FindViewById<TextView>(Resource.Id.Text3).Text = item.id;

            return view;
        }
    }
}