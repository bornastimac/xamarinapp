using System;

namespace CollectingMobile
{
    [Serializable]
    class Specimen
    {
        public readonly string Description;

        public Specimen(string description)
        {
            this.Description = description;
        }
    }
}