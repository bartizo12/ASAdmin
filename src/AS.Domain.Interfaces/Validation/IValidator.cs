namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Generic Validator Interface.
    /// </summary>
    /// <typeparam name="T">Type of the value to be validated</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates input and returns the validation result
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IValidationResult Validate(T value);
    }
}