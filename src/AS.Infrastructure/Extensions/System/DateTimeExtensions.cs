namespace System
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts datetime object to javascript milliseconds. Makes it easy to convert
        /// .NET datetime to javascript datetime.
        /// Sample :
        /// var jsDate = new Date(@DateTime.Now.ToJavaScriptMilliseconds());
        /// </summary>
        /// <param name="dt">DateTime object</param>
        /// <returns>Total Milliseconds since epoch time to <typeparamref name="dt"/> </returns>
        public static long ToJavaScriptMilliseconds(this DateTime dt)
        {
            return Convert.ToInt64((dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        /// <summary>
        /// Converts datetime object to javascript milliseconds. Makes it easy to convert
        /// .NET datetime to javascript datetime.
        /// Sample :
        /// var jsDate = new Date(@DateTime.Now.ToJavaScriptMilliseconds());
        /// </summary>
        /// <param name="dt">DateTime object</param>
        /// <returns>Total Milliseconds since epoch time to <typeparamref name="dt"/>.
        /// 0 if <typeparamref name="dt"/> is null </returns>
        public static long ToJavaScriptMilliseconds(this DateTime? dt)
        {
            if (dt == null)
                return 0;
            else
                return (long)(dt.Value.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}