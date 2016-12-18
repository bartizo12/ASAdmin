using AS.Admin.Validators;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AS.Admin.Models
{
    [Validator(typeof(ResetUserPasswordModelValidator))]
    public class ResetUserPasswordModel : ASModelBase
    {
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPasswordRepeat { get; set; }

        public string UserName { get; set; }
        public bool Success { get; set; }

        public ResetUserPasswordModel()
        {
        }

        public ResetUserPasswordModel(string userName)
        {
            this.UserName = userName;
        }
    }
}