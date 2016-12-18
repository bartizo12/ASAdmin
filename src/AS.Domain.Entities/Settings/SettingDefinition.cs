using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Dynamic structure of setting/config.It is similar to ConfigSection. A setting structure can have maximum 15 fields(can be extended if needed)
    /// For further information  about Settings,  please read AS.Domain.Settings ReadMe file.
    /// </summary>
    [Serializable]
    public class SettingDefinition : TrackableEntityBase<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Name of the Field1.
        /// </summary>
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

        /// <summary>
        /// Field1 Required/NotRequired
        /// </summary>
        public bool FieldRequired1 { get; set; }

        public bool FieldRequired2 { get; set; }
        public bool FieldRequired3 { get; set; }
        public bool FieldRequired4 { get; set; }
        public bool FieldRequired5 { get; set; }
        public bool FieldRequired6 { get; set; }
        public bool FieldRequired7 { get; set; }
        public bool FieldRequired8 { get; set; }
        public bool FieldRequired9 { get; set; }
        public bool FieldRequired10 { get; set; }
        public bool FieldRequired11 { get; set; }
        public bool FieldRequired12 { get; set; }
        public bool FieldRequired13 { get; set; }
        public bool FieldRequired14 { get; set; }
        public bool FieldRequired15 { get; set; }

        /// <summary>
        /// InputType/DataType of the Field1. For difference InputTypes , we generate different input element
        /// </summary>
        public FormInputType FieldInputType1 { get; set; }

        public FormInputType FieldInputType2 { get; set; }
        public FormInputType FieldInputType3 { get; set; }
        public FormInputType FieldInputType4 { get; set; }
        public FormInputType FieldInputType5 { get; set; }
        public FormInputType FieldInputType6 { get; set; }
        public FormInputType FieldInputType7 { get; set; }
        public FormInputType FieldInputType8 { get; set; }
        public FormInputType FieldInputType9 { get; set; }
        public FormInputType FieldInputType10 { get; set; }
        public FormInputType FieldInputType11 { get; set; }
        public FormInputType FieldInputType12 { get; set; }
        public FormInputType FieldInputType13 { get; set; }
        public FormInputType FieldInputType14 { get; set; }
        public FormInputType FieldInputType15 { get; set; }

        [XmlIgnore]
        public virtual ICollection<SettingValue> SettingValues { get; set; }
    }
}