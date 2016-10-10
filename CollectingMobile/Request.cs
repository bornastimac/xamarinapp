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
    class Request
    {
        public readonly string id;
        public readonly string description;
        public readonly string usernameAssigned;
        public readonly DateTime created;
        public readonly List<Specimen> specimensRequested;

        public Request(string id, string description, string usernameAssigned, DateTime created, List<Specimen> specimensRequested)
        {
            this.id = id;
            this.description = description;
            this.usernameAssigned = usernameAssigned;
            this.created = created;
            this.specimensRequested = specimensRequested;
            
        }
    }
}