using AS.Admin.Validators;
using AS.Domain.Entities;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    [Validator(typeof(UserModelValidator))]
    public class UserModel : ASModelBase
    {
        public MultiSelectList RoleSelectList { get; set; }
        public int? Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? LastActivity { get; set; }
        public DateTime? LastLogin { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string PasswordRepeat { get; set; }

        public List<string> Roles { get; set; }

        public UserModel()
        {
        }

        public UserModel(IUser user)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.Email = user.Email;
            this.CreatedBy = user.CreatedBy;
            this.CreatedOn = user.CreatedOn;
            this.ModifiedBy = user.ModifiedBy;
            this.ModifiedOn = user.ModifiedOn;
            this.LastActivity = user.LastActivity;
            this.LastLogin = user.LastLogin;
        }
    }
}