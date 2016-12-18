using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Validation;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class ResetUserPasswordModelValidator : ValidatorBase<ResetUserPasswordModel>
    {
        public ResetUserPasswordModelValidator(ISettingManager settingManager, IResourceManager resourceManager)
                : base(settingManager, false)
        {
            RuleFor(m => m.NewPassword)
                .SetValidator(new PasswordValidator(settingManager, resourceManager));                
            RuleFor(m => m.NewPasswordRepeat)
                .SetValidator(new PasswordValidator(settingManager, resourceManager));
            RuleFor(m => m.NewPasswordRepeat)
                .Equal(m => m.NewPassword)
                .WithMessage(resourceManager.GetString("ResetPassword_PasswordsDontMatch"));
        }
    }
}