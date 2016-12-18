using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Validation;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class UserModelValidator : ValidatorBase<UserModel>
    {
        private readonly IMembershipService _service;

        public UserModelValidator(IMembershipService service,
            IResourceManager resourceManager,
            ISettingManager settingManager)
                 : base(settingManager, false)
        {
            this._service = service;
            RuleFor(m => m.UserName)
              .SetValidator(new UsernameValidator(settingManager, resourceManager))
              .Must(this.UserNameNotInUse)
              .WithMessage(resourceManager.GetString("Users_UserNameExists"));

            RuleFor(m => m.Email)
              .NotEmpty()
              .WithMessage(resourceManager.GetString("Users_EmailAddressRequired"))
              .EmailAddress()
              .WithMessage(resourceManager.GetString("Users_EmailAddressInvalid"));

            RuleFor(m => m.Email)
                .Must(this.EmailNotInUse)
                .WithMessage(resourceManager.GetString("Users_EmailAddressExists"))
                .When(m => m.Id == null)
                .Length(0, 300)
                .WithMessage(resourceManager.GetString("MaxLen_ErrorMessage"), "E -Mail", 300);

            RuleFor(m => m.Roles)
                .NotEmpty()
                .WithMessage(resourceManager.GetString("Users_UserRoleRequired"));

            RuleFor(m => m.Password)
                 .SetValidator(new PasswordValidator(settingManager, resourceManager));

            RuleFor(m => m.PasswordRepeat)
               .SetValidator(new PasswordValidator(settingManager, resourceManager));

            RuleFor(m => m.PasswordRepeat)
                .Equal(m => m.Password)
                .WithMessage(resourceManager.GetString("ResetPassword_PasswordsDontMatch"));
        }

        private bool UserNameNotInUse(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return true;
            return !this._service.DoesUserNameExist(userName);
        }

        private bool EmailNotInUse(string email)
        {
            if (string.IsNullOrEmpty(email))
                return true;

            return !this._service.DoesEmailExist(email);
        }
    }
}