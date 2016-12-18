using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Validation;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class ResetPasswordValidator : ValidatorBase<ResetPasswordModel>
    {
        public ResetPasswordValidator(ISettingManager settingManager, IResourceManager resourceManager)
            : base(settingManager, false)
        {
            RuleFor(m => m.NewPassword)
                .SetValidator(new PasswordValidator(settingManager, resourceManager));

            RuleFor(m => m.NewPasswordRepeat)
                .SetValidator(new PasswordValidator(settingManager, resourceManager))
                .Equal(m => m.NewPassword)
                .WithMessage(resourceManager.GetString("ResetPassword_PasswordsDontMatch"));
        }
    }
}