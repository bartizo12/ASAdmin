using System;
using System.Xml.Serialization;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Dynamic setting/configuration value entity. This entity is aimed to be used to store all  setting types.
    /// Stored setting values are converted to  the regarding setting object at run time.
    /// For further information  about Settings,  please read AS.Domain.Settings ReadMe file.
    /// </summary>
    [Serializable]
    public class SettingValue : TrackableEntityBase<int>
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public string Field7 { get; set; }
        public string Field8 { get; set; }
        public string Field9 { get; set; }
        public string Field10 { get; set; }
        public string Field11 { get; set; }
        public string Field12 { get; set; }
        public string Field13 { get; set; }
        public string Field14 { get; set; }
        public string Field15 { get; set; }
        public int SettingDefinitionID { get; set; }
        public bool IsHiddenFromUser { get; set; }

        [XmlIgnore]
        public SettingDefinition SettingDefinition { get; set; }
    }
}