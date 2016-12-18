using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class LoginValidator : ValidatorBase<LoginModel>
    {
        public LoginValidator(ISettingManager settingManager, IResourceManager resourceManager)
          : base(settingManager, false)
        {
            RuleFor(m => m.UserNameOrEmail)
                .NotEmpty()
                .WithMessage(resourceManager.GetString("Login_UsernameOrEmailRequired"));

            RuleFor(m => m.Password)
                .NotEmpty()
                .WithMessage(resourceManager.GetString("Login_PasswordRequired"));
        }
    }
}