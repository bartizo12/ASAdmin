using AS.Admin.Validators;
using AS.Infrastructure;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    [Validator(typeof(JobDefinitionModelValidator))]
    public class JobDefinitionModel : ASModelBase
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public MultiSelectList JobTypeSelectList { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string JobTypeName { get; set; }
        public int RunInterval { get; set; }
        public int JobStatus { get; set; }

        [DataType(DataType.MultilineText)]
        public string Error { get; set; }

        [Optional]
        public string Comment { get; set; }

        public DateTime? LastExecutionTime { get; set; }
    }
}