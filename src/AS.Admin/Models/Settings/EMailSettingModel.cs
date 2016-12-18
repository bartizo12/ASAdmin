using AS.Admin.Validators;
using AS.Domain.Entities;
using AS.Infrastructure;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace AS.Admin.Models
{
    [Validator(typeof(EMailSettingModelValidator))]
    public class EMailSettingModel : ASModelBase
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int SettingDefinitionID { get; set; }

        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int TimeOut { get; set; }
        public bool EnableSsl { get; set; }
        public bool DefaultCredentials { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string FromDisplayName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string FromAddress { get; set; }

        [DataType(DataType.MultilineText)]
        [Optional]
        public string Comment { get; set; }

        public static explicit operator SettingValue(EMailSettingModel model)
        {
            SettingValue value = new SettingValue();
            value.Id = model.Id;
            value.SettingDefinitionID = model.SettingDefinitionID;
            value.CreatedOn = model.CreatedOn;
            value.CreatedBy = model.CreatedBy;

            value.Field1 = model.Name;
            value.Field2 = model.Host;
            value.Field3 = model.Port.ToString();
            value.Field4 = model.TimeOut.ToString();
            value.Field5 = model.EnableSsl.ToString();
            value.Field6 = model.DefaultCredentials.ToString();
            value.Field7 = model.UserName;
            value.Field8 = model.Password;
            value.Field9 = model.FromDisplayName;
            value.Field10 = model.FromAddress;
            value.Field11 = model.Comment;

            return value;
        }

        public static explicit operator EMailSettingModel(SettingValue value)
        {
            EMailSettingModel model = new EMailSettingModel();
            model.Id = value.Id;
            model.SettingDefinitionID = value.SettingDefinitionID;
            model.CreatedOn = value.CreatedOn;
            model.CreatedBy = value.CreatedBy;

            model.Name = value.Field1;
            model.Host = value.Field2;
            model.Port = int.Parse(value.Field3);
            model.TimeOut = int.Parse(value.Field4);
            model.EnableSsl = bool.Parse(value.Field5);
            model.DefaultCredentials = bool.Parse(value.Field6);
            model.UserName = value.Field7;
            model.Password = value.Field8;
            model.FromDisplayName = value.Field9;
            model.FromAddress = value.Field10;
            model.Comment = value.Field11;
            model.ModifiedBy = value.ModifiedBy;
            model.ModifiedOn = value.ModifiedOn;

            return model;
        }
    }
}