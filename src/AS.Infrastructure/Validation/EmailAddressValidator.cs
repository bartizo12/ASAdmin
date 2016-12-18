using AS.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AS.Infrastructure.Validation
{
    /// <summary>
    /// E-Mail Address validator.
    /// Uses validation rules of <see cref="EmailAddressAttribute"/>
    /// </summary>
    public class EmailAddressValidator : PropertyValidatorBase<string>
    {
        private readonly IResourceManager _resourceManager;

        public EmailAddressValidator(IResourceManager resourceManager)
        {
            this._resourceManager = resourceManager;
        }

        /// <summary>
        /// Validates e-mail address
        /// </summary>
        /// <param name="emailAddress">E-mail address to  be validated</param>
        /// <returns>Validation Result</returns>
        public override IValidationResult Validate(string emailAddress)
        {
            bool valid = new EmailAddressAttribute().IsValid(emailAddress);
            List<string> errors = new List<string>();

            if (!valid)
                errors.Add(_resourceManager.GetString("InvalidEmailAddress"));

            return new ValidationResult(valid, errors);
        }
    }
}