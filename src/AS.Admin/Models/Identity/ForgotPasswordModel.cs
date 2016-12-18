using AS.Admin.Validators;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;

namespace AS.Admin.Models
{
    [Validator(typeof(ForgotPasswordValidator))]
    public class ForgotPasswordModel : ASModelBase
    {
        public string UserNameOrEmail { get; set; }
    }
}