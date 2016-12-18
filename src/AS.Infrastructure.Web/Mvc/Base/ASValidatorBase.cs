using AS.Domain.Settings;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Base model validator class that inherits from FluentValidation <typeparamref name="AbstractValidator"/>
    /// </summary>
    /// <typeparam name="T">Generic model type</typeparam>
    public abstract class ValidatorBase<T> : AbstractValidator<T> where T : ASModelBase
    {
        private readonly ISettingManager _settingManager;

        public ValidatorBase(ISettingManager settingManager, bool updateDisabledOnDemo)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            this.UpdateDisabledOnDemo = updateDisabledOnDemo;
            this._settingManager = settingManager;
        }

        private bool UpdateDisabledOnDemo { get; set; }

        private bool IsDemo
        {
            get
            {
                bool isDemo = false;

                if (_settingManager.GetContainer<AppSetting>().Contains("IsDemo"))
                {
                    bool.TryParse(_settingManager.GetContainer<AppSetting>()["IsDemo"].Value, out isDemo);
                }

                return isDemo;
            }
        }

        public override ValidationResult Validate(ValidationContext<T> context)
        {
            if (this.UpdateDisabledOnDemo && IsDemo)
            {
                return new ValidationResult(new List<ValidationFailure>() {
                    new ValidationFailure(string.Empty, ResMan.GetString("Demo_Disabled_Message")) });
            }
            return base.Validate(context);
        }
    }
}