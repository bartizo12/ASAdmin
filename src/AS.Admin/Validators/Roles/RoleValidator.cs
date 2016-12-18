using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class RoleValidator : ValidatorBase<RoleModel>
    {
        public RoleValidator(ISettingManager settingManager, IResourceManager resourceManager)
          : base(settingManager, false)
        {
            RuleFor(m => m.Name)
              .NotEmpty()
              .WithMessage(resourceManager.GetString("Roles_RoleNameRequired"));
        }
    }
}