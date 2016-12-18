using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class StringResourceModelValidator : ValidatorBase<StringResourceModel>
    {
        public StringResourceModelValidator(ISettingManager settingManager, IResourceManager resourceManager)
           : base(settingManager, false)
        {
            RuleFor(m => m.Name)
                   .NotEmpty()
                   .WithMessage(resourceManager.GetString("StringResourceModel_NameRequired"));
            RuleFor(m => m.Value)
                  .NotEmpty()
                   .WithMessage(resourceManager.GetString("StringResourceModel_ValueRequired"));
        }
    }
}