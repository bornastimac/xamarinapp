using System;
using Android.Locations;

namespace CollectingMobile
{ 
    [Serializable]
    public class SpecimenItem //instance of specimen type
    {
        public readonly int ID;
        public readonly string Description;

        public string Location;
        public string SamplingPosition;
        public bool uploaded = false;
        //public DateTime Taken;
        //image?
        
        public SpecimenItem(int id, string description)
        {
            this.ID = id;
            this.Description = description;
        }
    }
}