using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CollectingMobile
{
    [Serializable]
    public class Specimen
    {
        [JsonIgnore]
        public bool Uploaded = false;

        public int? ID;
        public string Description;

        #region other fields
        public string LabCode;
        public int? SamplingFrequencyID;
        public int? SamplingMethodID;
        public int? CounstructionElementID;
        public int? SpecimenShapeID;
        public string Location;
        public string SamplingPosition;
        public int? SpecimenCount;
        public string Code;
        public int? MaterialTypeID;
        public string MaterialTypesName;
        public int? StandardCodeID;
        public string StandardCode;
        public int? SamplingRequestID;
        public int? ManufacturerID;
        public string Manufacturer;
        public int? MeasureUnitID;
        public string MeasureUnitCode;
        public float? Quantity;
        public string Status;
        public string Qrcode;
        public int? InstallerID;
        public string Installer;
        public int? TestingMethodID;
        public string TestingUserGuid;
        public string Tester;
        public string TestingMethodNormCode;
        public string SpecimenShapesName;
        public string ConstructionElementsName;
        public string TestingMethodsNormCode;
        public string Material;
        public string SamplingFrequency;
        public string PhotoFileName;
        #endregion

        #region public Specimen(params){params};
        public Specimen(
            int? ID,
            string Description,
            string LabCode,
            int? SamplingFrequencyID,
            int? SamplingMethodID,
            int CounstructionElementID,
            int? SpecimenShapeID,
            string Location,
            string SamplingPosition,
            int? SpecimenCount,
            string Code,
            int? MaterialTypeID,
            string MaterialTypesName,
            int? StandardCodeID,
            string StandardCode,
            int? SamplingRequestID,
            int? ManufacturerID,
            string Manufacturer,
            int? MeasureUnitID,
            string MeasureUnitCode,
            int? Quantity,
            string Status,
            string Qrcode,
            int? InstallerID,
            string Installer,
            int? TestingMethodID,
            string TestingUserGuid,
            string Tester,
            string TestingMethodNormCode,
            string SpecimenShapesName,
            string ConstructionElementsName,
            string TestingMethodsNormCode,
            string Material,
            string SamplingFrequency,
            string PhotoFileName)
        {
            this.ID = ID;
            this.Description = Description;
            this.LabCode = LabCode;
            this.SamplingFrequencyID = SamplingFrequencyID;
            this.SamplingMethodID = SamplingMethodID;
            this.CounstructionElementID = CounstructionElementID;
            this.SpecimenShapeID = SpecimenShapeID;
            this.Location = Location;
            this.SamplingPosition = SamplingPosition;
            this.SpecimenCount = SpecimenCount;
            this.Code = Code;
            this.MaterialTypeID = MaterialTypeID;
            this.MaterialTypesName = MaterialTypesName;
            this.StandardCodeID = StandardCodeID;
            this.StandardCode = StandardCode;
            this.SamplingRequestID = SamplingRequestID;
            this.ManufacturerID = ManufacturerID;
            this.Manufacturer = Manufacturer;
            this.MeasureUnitID = MeasureUnitID;
            this.MeasureUnitCode = MeasureUnitCode;
            this.Quantity = Quantity;
            this.Status = Status;
            this.Qrcode = Qrcode;
            this.InstallerID = InstallerID;
            this.Installer = Installer;
            this.TestingMethodID = TestingMethodID;
            this.TestingUserGuid = TestingUserGuid;
            this.Tester = Tester;
            this.TestingMethodNormCode = TestingMethodNormCode;
            this.SpecimenShapesName = SpecimenShapesName;
            this.ConstructionElementsName = ConstructionElementsName;
            this.TestingMethodsNormCode = TestingMethodsNormCode;
            this.Material = Material;
            this.SamplingFrequency = SamplingFrequency;
            this.PhotoFileName = PhotoFileName;
        }
        #endregion

        public Specimen() { }
    }
}