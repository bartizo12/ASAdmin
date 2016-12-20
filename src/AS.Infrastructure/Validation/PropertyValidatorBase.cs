using AS.Domain.Interfaces;
using FluentValidation.Validators;

namespace AS.Infrastructure.Validation
{
    /// <summary>
    /// Base class for fluent property validators
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PropertyValidatorBase<T> : PropertyValidator, IValidator<T>
    {
        public PropertyValidatorBase()
            : base("{error}")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            IValidationResult result = this.Validate((T)context.PropertyValue);
            context.MessageFormatter.AppendArgument("error", string.Join(".", result.Errors));

            return result.Succeeded;
        }

        public abstract IValidationResult Validate(T value);
    }
}