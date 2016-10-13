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
        public readonly string Code;
        public readonly string ID;
        public readonly string Description;
        public readonly string UsernameAssigned;
        public readonly DateTime Created;
        public List<Specimen> Specimens;

        public Request(string code, string id, string description, string usernameAssigned, DateTime created, List<Specimen> specimensRequested)
        {
            this.Code = code;
            this.ID = id;
            this.Description = description;
            this.UsernameAssigned = usernameAssigned;
            this.Created = created;
            this.Specimens = specimensRequested;           
        }

    }
}