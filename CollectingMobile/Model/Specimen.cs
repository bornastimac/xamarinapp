using System;
using System.Collections.Generic;

namespace CollectingMobile
{
    [Serializable]
    public class Specimen
    {
        public readonly int ID;
        public readonly string Description;
        public readonly int MaterialTypeID;
        public readonly int Count;

        public string Location;
        public string SamplingPosition;
        public bool Uploaded = false;

        public Specimen(int id, string description, int materialTypeID, int count)
        {
            this.ID = id;
            this.Description = description;
            this.MaterialTypeID = materialTypeID;
            this.Count = count;
        }

    }
}