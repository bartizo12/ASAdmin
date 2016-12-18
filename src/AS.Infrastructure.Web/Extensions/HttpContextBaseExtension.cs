namespace System.Web
{
    public static class HttpContextBaseExtension
    {
        /// <summary>
        /// Checks if Request  avaliable in HttpContext.
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <returns>True if there is an http request avaliable</returns>
        public static bool IsRequestAvailable(this HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;
            try
            {
                if (httpContext.Request == null)
                    return false;
            }
            catch (HttpException)
            {
                return false;
            }
            return true;
        }
    }
}