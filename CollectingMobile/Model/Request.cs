using System;
using System.Collections.Generic;

namespace CollectingMobile
{
    [Serializable]
    public class Request
    {
        public readonly int ID;
        public readonly string Code;
        public readonly string Description;
        public readonly string UsernameAssigned;
        public readonly DateTime Created;
        public List<Specimen> Specimens;

        public Request(int id, string code, string description, string usernameAssigned, DateTime created, List<Specimen> specimensRequested)
        {
            this.ID = id;
            this.Code = code;
            this.Description = description;
            this.UsernameAssigned = usernameAssigned;
            this.Created = created;
            this.Specimens = specimensRequested;           
        }

        public Specimen GetSpecimenByID(int specimenID)
        {
            return Specimens.Find(specimen => specimen.ID == specimenID);
        }

    }
}