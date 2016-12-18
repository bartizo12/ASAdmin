using AS.Admin.Validators;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AS.Admin.Models
{
    [Validator(typeof(ResetPasswordValidator))]
    public class ResetPasswordModel : ASModelBase
    {
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPasswordRepeat { get; set; }

        public string Token { get; set; }
    }
}