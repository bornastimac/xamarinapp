using System;
using System.Collections.Generic;

namespace CollectingMobile
{
    [Serializable]
    public class Specimen
    {
        public readonly int ID;
        public readonly string Description;
        public readonly int Count;
        public readonly List<SpecimenItem> Items;

        public Specimen(int id, string description, int count)
        {
            this.ID = id;
            this.Description = description;
            this.Count = count;

            Items = new List<SpecimenItem>();
            for (int i = 0; i < count; i++)
            {
                Items.Add(new SpecimenItem(ID, Description));
            }
        }

    }
}