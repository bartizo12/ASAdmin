using System;
using System.Web;

namespace AS.Infrastructure.Web
{
    /// <summary>
    /// Cookie helper class
    /// </summary>
    internal static class CookieHelper
    {
        public static string GetValue(HttpContextBase context, string cookieName, string key)
        {
            if (string.IsNullOrEmpty(cookieName))
                throw new ArgumentNullException("cookieName");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!context.IsRequestAvailable())
                return null;

            HttpCookie cookie = context.Request.Cookies[cookieName];

            if (cookie == null || !cookie.HasKeys)
                return null;

            return cookie[key];
        }

        public static void SetValue(HttpContextBase context, string cookieName, string key, string value)
        {
            if (string.IsNullOrEmpty(cookieName))
                throw new ArgumentNullException("cookieName");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (!context.IsRequestAvailable())
                return;

            HttpCookie cookie = context.Response.Cookies[cookieName];

            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                cookie[key] = value;
            }
            cookie.Expires = DateTime.Now.AddYears(10);//Never expire please
            context.Response.Cookies.Add(cookie);
        }
    }
}