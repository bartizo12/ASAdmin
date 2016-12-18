using AS.Admin.Validators;
using AS.Infrastructure;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace AS.Admin.Models
{
    [Validator(typeof(RoleValidator))]
    public class RoleModel : ASModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Optional]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public RoleModel()
        {
            this.Id = default(int);
        }
    }
}