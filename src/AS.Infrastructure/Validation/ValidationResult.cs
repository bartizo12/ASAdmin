using AS.Domain.Interfaces;
using System.Collections.Generic;

namespace AS.Infrastructure.Validation
{
    public class ValidationResult : IValidationResult
    {
        /// <summary>
        /// True if input is valid , otherwise false
        /// </summary>
        public bool Succeeded { get; private set; }
        /// <summary>
        /// Validation errors.
        /// </summary>
        public IEnumerable<string> Errors { get; private set; }

        public ValidationResult(bool succeeded, IEnumerable<string> errors)
        {
            this.Succeeded = succeeded;
            this.Errors = errors ?? new List<string>();
        }
    }
}
