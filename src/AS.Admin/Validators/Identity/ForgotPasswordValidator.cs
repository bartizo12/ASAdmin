using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class ForgotPasswordValidator : ValidatorBase<ForgotPasswordModel>
    {
        public ForgotPasswordValidator(ISettingManager settingManager, IResourceManager resourceManager)
            : base(settingManager, false)
        {
            RuleFor(m => m.UserNameOrEmail)
              .NotEmpty()
              .WithMessage(resourceManager.GetString("Login_UsernameOrEmailRequired"))
              .Length(0, 300)
              .WithMessage(resourceManager.GetString("MaxLen_ErrorMessage"), "UserNameOrEmail", 300);
        }
    }
}