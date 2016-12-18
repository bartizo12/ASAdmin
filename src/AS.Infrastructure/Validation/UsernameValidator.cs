using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System.Collections.Generic;
using System.Linq;

namespace AS.Infrastructure.Validation
{
    /// <summary>
    /// Username Policy
    /// --> Username cannot be empty
    /// --> Username length must be between 4,50
    /// --> Username must follow membership settings
    /// --> Allowed chracters for username ; abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+
    /// </summary>
    public class UsernameValidator : PropertyValidatorBase<string>
    {
        private const string AllowedUserNameCharacters  = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        private const int MinUsernameLength = 4;
        private const int MaxUsernameLength = 50;

        private readonly ISettingManager _settingManager;
        private readonly IResourceManager _resourceManager;

        public UsernameValidator(ISettingManager settingManager, IResourceManager resourceManager)
        {
            this._settingManager = settingManager;
            this._resourceManager = resourceManager;
        }
        public override IValidationResult Validate(string userName)
        {
            List<string> errors = new List<string>();
            bool isValid = true;
            
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
            {
                errors.Add(_resourceManager.GetString("UsernameCannotBeEmpty"));
                return new ValidationResult(false, errors);
            }
            if (userName.Length < MinUsernameLength || userName.Length > MaxUsernameLength)
            {
                isValid = false;
                errors.Add(string.Format(_resourceManager.GetString("UsernameLengthMustBeInRange"), 
                    MinUsernameLength,MaxUsernameLength));
            }
            MembershipSetting setting = _settingManager.GetContainer<MembershipSetting>().Default;

            if(setting != null)
            {
                if(setting.AllowOnlyAlphanumericUserNames && !userName.All(char.IsLetterOrDigit))
                {
                    isValid = false;
                    errors.Add(_resourceManager.GetString("UsernameCanBeOnlyAlphanumeric"));

                }
            }
            if(userName.Any(c => !AllowedUserNameCharacters.Contains(c)))
            {
                isValid = false;
                errors.Add(_resourceManager.GetString("InvalidUsername"));
            }

            return new ValidationResult(isValid, errors);
        }
    }
}