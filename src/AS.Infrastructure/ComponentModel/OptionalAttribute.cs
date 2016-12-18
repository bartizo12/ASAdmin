using System;

namespace AS.Infrastructure
{
    /// <summary>
    /// Marks property as "Optional" , which means user may leave the field empty.
    /// Used for indicating  "Optional" fields on UI visually.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionalAttribute : Attribute
    {
    }
}