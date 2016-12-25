using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System.Collections.Generic;
using System.Linq;

namespace AS.Infrastructure.Validation
{
    /// <summary>
    /// Password cannot be empty or cannot contain white spaces. Additionally Membership password validation settings are applied
    /// </summary>
    public class PasswordValidator : PropertyValidatorBase<string>
    {
        private readonly ISettingManager _settingManager;
        private readonly IResourceManager _resourceManager;

        public PasswordValidator(ISettingManager settingManager, IResourceManager resourceManager)
        {
            this._settingManager = settingManager;
            this._resourceManager = resourceManager;
        }

        /// <summary>
        /// Validates password according to the membership password validation settings.
        /// </summary>
        /// <param name="password">Password to be valited</param>
        /// <returns>ValidationResult</returns>
        public override IValidationResult Validate(string password)
        {
            List<string> errors = new List<string>();
            bool isValid = true;

            if (string.IsNullOrEmpty(password) || password.Any(char.IsWhiteSpace))
            {
                errors.Add(_resourceManager.GetString("PasswordCannotBeEmpty"));
                return new ValidationResult(false, errors);
            }
            MembershipSetting setting = _settingManager.GetContainer<MembershipSetting>().Default;

            if (setting != null)
            {
                if (setting.MinimumPasswordRequiredLength > password.Length)
                {
                    isValid = false;
                    errors.Add(string.Format(_resourceManager.GetString("PasswordTooShort"),
                        setting.MinimumPasswordRequiredLength));
                }
                if (setting.RequireDigitInPassword && !password.Any(char.IsDigit))
                {
                    isValid = false;
                    errors.Add(_resourceManager.GetString("PasswordRequiresDigit"));
                }
                if (setting.RequireLowercaseInPassword && !password.Any(char.IsLower))
                {
                    isValid = false;
                    errors.Add(_resourceManager.GetString("PasswordRequiresLower"));
                }
                if (setting.RequireLowercaseInPassword && !password.Any(char.IsLower))
                {
                    isValid = false;
                    errors.Add(_resourceManager.GetString("PasswordRequiresLower"));
                }
                if (setting.RequireNonLetterOrDigitInPassword && password.All(char.IsLetterOrDigit))
                {
                    isValid = false;
                    errors.Add(_resourceManager.GetString("PasswordRequiresNonAlphanumeric"));
                }
                if (setting.RequireUppercaseInPassword && !password.Any(char.IsUpper))
                {
                    isValid = false;
                    errors.Add(_resourceManager.GetString("PasswordRequiresUpper"));
                }
            }
            return new ValidationResult(isValid, errors);
        }
    }
}