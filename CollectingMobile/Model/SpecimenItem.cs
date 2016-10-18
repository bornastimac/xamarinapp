using System;

namespace CollectingMobile
{ 
    [Serializable]
    public class SpecimenItem
    {
        public int ID;
        //public DateTime taken;
        //image?
        //desc?
        //gps
        //datetime

        public SpecimenItem(int id)
        {
            this.ID = id;
        }
    }
}