namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Taken From : http://stackoverflow.com/a/16104/6117242
        /// </summary>
        /// <typeparam name="TEnum">Generic enum type</typeparam>
        /// <param name="value">String value to be converted to Enum</param>
        /// <param name="defaultValue">Default enum value if convertion fails</param>
        /// <returns>Converted enum value</returns>
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue)
            where TEnum : struct, IFormattable, IComparable
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            TEnum result;
            return Enum.TryParse<TEnum>(value, true, out result) ? result : defaultValue;
        }
    }
}