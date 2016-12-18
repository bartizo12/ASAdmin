using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class EMailSettingModelValidator : ValidatorBase<EMailSettingModel>
    {
        public EMailSettingModelValidator(ISettingManager settingManager, IResourceManager resourceManager)
         : base(settingManager, false)
        {
            RuleFor(m => m.Name)
             .NotEmpty()
             .WithMessage(resourceManager.GetString("EMailSettingModel_NameRequired"));
            RuleFor(m => m.Host)
                .NotEmpty()
                .WithMessage(resourceManager.GetString("EMailSettingModel_HostRequired"));
            RuleFor(m => m.Port)
                .GreaterThan(0)
                .WithMessage(resourceManager.GetString("EMailSettingModel_PortRequired"));
            RuleFor(m => m.FromAddress)
                .NotEmpty()
                .EmailAddress()
                .WithMessage(resourceManager.GetString("EMailSettingModel_FromAddressInvalid"));
        }
    }
}