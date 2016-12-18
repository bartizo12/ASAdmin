using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class JobDefinitionModelValidator : ValidatorBase<JobDefinitionModel>
    {
        public JobDefinitionModelValidator(ISettingManager settingManager, IResourceManager resourceManager)
           : base(settingManager, true)
        {
            RuleFor(m => m.Name)
                   .NotEmpty()
                   .WithMessage(resourceManager.GetString("JobDefinition_NameRequired"));
            RuleFor(m => m.JobTypeName)
                   .NotEmpty()
                   .WithMessage(resourceManager.GetString("JobDefinition_TypeNameRequired"));
            RuleFor(m => m.RunInterval)
                   .GreaterThan(0)
                   .WithMessage(resourceManager.GetString("JobDefinition_InvalidRunInterval"));
        }
    }
}