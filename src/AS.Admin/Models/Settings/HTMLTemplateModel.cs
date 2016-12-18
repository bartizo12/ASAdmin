using AS.Admin.Validators;
using AS.Domain.Entities;
using AS.Infrastructure;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    [Validator(typeof(HTMLTemplateModelValidator))]
    public class HTMLTemplateModel : ASModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string BodyFilePath { get; set; }

        [Optional]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public int SettingDefinitionID { get; set; }
        public MultiSelectList TemplateFileList { get; set; }

        public static explicit operator HTMLTemplateModel(SettingValue value)
        {
            HTMLTemplateModel model = new HTMLTemplateModel();
            model.Id = value.Id;
            model.SettingDefinitionID = value.SettingDefinitionID;
            model.CreatedOn = value.CreatedOn;
            model.CreatedBy = value.CreatedBy;
            model.ModifiedBy = value.ModifiedBy;
            model.ModifiedOn = value.ModifiedOn;

            model.Name = value.Field1;
            model.Subject = value.Field2;
            model.BodyFilePath = value.Field3;
            model.Comment = value.Field4;

            return model;
        }
    }
}