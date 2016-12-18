using System.Collections.Generic;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface for validation result
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// True if input is valid , otherwise false
        /// </summary>
        bool Succeeded { get; }
        /// <summary>
        /// Validation errors.
        /// </summary>
        IEnumerable<string> Errors { get; }
    }
}
