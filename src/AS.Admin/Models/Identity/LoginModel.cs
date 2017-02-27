using AS.Admin.Validators;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AS.Admin.Models
{
    [Validator(typeof(LoginValidator))]
    public class LoginModel : ASModelBase
    {
        public string UserNameOrEmail { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}