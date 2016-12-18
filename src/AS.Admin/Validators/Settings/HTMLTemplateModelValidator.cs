using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web.Mvc;
using FluentValidation;

namespace AS.Admin.Validators
{
    public class HTMLTemplateModelValidator : ValidatorBase<HTMLTemplateModel>
    {
        public HTMLTemplateModelValidator(ISettingManager settingManager, IResourceManager resourceManager)
         : base(settingManager, false)
        {
            RuleFor(m => m.Name)
                .NotEmpty()
                .WithMessage(resourceManager.GetString("HTMLTemplateModel_NameRequired"));
            RuleFor(m => m.Subject)
                .NotEmpty()
                .WithMessage(resourceManager.GetString("HTMLTemplateModel_SubjectRequired"));
            RuleFor(m => m.BodyFilePath)
                .NotEmpty()
                .WithMessage(resourceManager.GetString("HTMLTemplateModel_BodyRequred"));
        }
    }
}