using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class ConfigurationModelValidator : ValidatorBase<ConfigurationModel>
    {
        public ConfigurationModelValidator(ISettingManager settingManager, IResourceManager resourceManager)
            : base(settingManager, false)
        {
            RuleFor(m => m.ConnectionString)
                   .NotEmpty()
                   .WithMessage(resourceManager.GetString("Installer_ConnectionStringRequired"));

            RuleFor(m => m.DataProvider)
                 .NotEmpty()
                 .WithMessage(resourceManager.GetString("Installer_ProviderRequired"));
        }
    }
}