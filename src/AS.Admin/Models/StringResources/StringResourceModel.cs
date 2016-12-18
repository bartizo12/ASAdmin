using AS.Admin.Validators;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    [Validator(typeof(StringResourceModelValidator))]
    public class StringResourceModel : ASModelBase
    {
        public int Id { get; set; }
        public string CultureCode { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool AvailableOnClientSide { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }

        public MultiSelectList CultureCodeList { get; set; }
    }
}