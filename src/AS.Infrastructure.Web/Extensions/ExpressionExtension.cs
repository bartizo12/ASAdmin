using AS.Infrastructure;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Code Taken From : http://stackoverflow.com/a/1560950/6117242
    /// </summary>
    public static class ExpressionExtension
    {
        public static T GetAttribute<T>(this ICustomAttributeProvider provider)
            where T : Attribute
        {
            var attributes = provider.GetCustomAttributes(typeof(T), true);
            return attributes.Length > 0 ? attributes[0] as T : null;
        }

        /// <summary>
        /// Checks if expression body is optional or not.
        /// If optional, at on UI we can display that member/field is optional.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="expression">Expression to be checked</param>
        /// <returns>True if expression body has OptionalAttribute</returns>
        public static bool IsOptional<T, V>(this Expression<Func<T, V>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression");

            return memberExpression.Member.GetAttribute<OptionalAttribute>() != null;
        }
    }
}