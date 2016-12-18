using AS.Domain.Entities;
using AS.Domain.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace AS.Infrastructure.Web
{
    /// <summary>
    /// Provides web application runtime data
    /// </summary>
    public class WebContextProvider : IContextProvider
    {
        private readonly string DefaultLanguageCode = "en-US";

        public WebContextProvider()
        {
        }

        public short LoginAttemptCount
        {
            get
            {
                object loginAttemptCount = HttpContext.Current.Session["LoginAttemptCount"];

                if (loginAttemptCount == null)
                {
                    HttpContext.Current.Session["LoginAttemptCount"] = 0;
                    return 0;
                }
                return Convert.ToInt16(loginAttemptCount);
            }
            set
            {
                HttpContext.Current.Session["LoginAttemptCount"] = value;
            }
        }

        public string SessionId
        {
            get
            {
                if (HttpContext.Current.Session == null)
                    return string.Empty;
                return HttpContext.Current.Session.SessionID;
            }
        }

        public IDictionary Items
        {
            get
            {
                return HttpContext.Current.Items;
            }
        }

        public NameValueCollection ServerVariables
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables;
            }
        }

        public string HttpMethod
        {
            get
            {
                return HttpContext.Current.Request.HttpMethod;
            }
        }

        public string BrowserType
        {
            get
            {
                return HttpContext.Current.Request.Browser.Type;
            }
        }

        public Uri Url
        {
            get
            {
                return HttpContext.Current.Request.Url;
            }
        }

        public CultureInfo Culture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
        }

        public Country CountryInfo
        {
            get
            {
                if (HttpContext.Current == null)
                    return Country.Empty;
                HttpContextBase _httpContext = new HttpContextWrapper(HttpContext.Current);

                if (!_httpContext.IsRequestAvailable()
                    || _httpContext.Session == null
                    || _httpContext.Session["CountryCode"] == null)
                    return Country.Empty;

                return (Country)_httpContext.Session["CountryCode"];
            }
            set
            {
                HttpContextBase _httpContext = new HttpContextWrapper(HttpContext.Current);
                if (!_httpContext.IsRequestAvailable() || _httpContext.Session == null)
                    return;
                _httpContext.Session["CountryCode"] = value;
            }
        }

        public int UserId
        {
            get
            {
                if (HttpContext.Current == null)
                    return default(int);

                HttpContextBase _httpContext = new HttpContextWrapper(HttpContext.Current);

                if (!_httpContext.IsRequestAvailable())
                    return default(int);

                if (_httpContext.User != null
                    && _httpContext.User.Identity != null
                    && _httpContext.User.Identity.IsAuthenticated)
                {
                    return IdentityExtensions.GetUserId<int>(_httpContext.User.Identity);
                }
                return default(int);
            }
        }

        public string UserName
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;

                HttpContextBase _httpContext = new HttpContextWrapper(HttpContext.Current);

                if (!_httpContext.IsRequestAvailable())
                    return string.Empty;

                if (_httpContext.User != null
                    && _httpContext.User.Identity != null
                    && _httpContext.User.Identity.IsAuthenticated)
                {
                    return _httpContext.User.Identity.Name;
                }
                return string.Empty;
            }
        }

        public string ClientIP
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                HttpContextBase _httpContext = new HttpContextWrapper(HttpContext.Current);

                if (!_httpContext.IsRequestAvailable())
                    return string.Empty;

                var result = "";
                if (_httpContext.Request.Headers != null)
                {
                    //The X-Forwarded-For (XFF) HTTP header field is a de facto standard
                    //for identifying the originating IP address of a client
                    //connecting to a web server through an HTTP proxy or load balancer.
                    var forwardedHttpHeader = "X-FORWARDED-FOR";

                    //it's used for identifying the originating IP address of a client connecting to a web server
                    //through an HTTP proxy or load balancer.
                    string xff = _httpContext.Request.Headers.AllKeys
                        .Where(x => forwardedHttpHeader.Equals(x, StringComparison.InvariantCultureIgnoreCase))
                        .Select(k => _httpContext.Request.Headers[k])
                        .FirstOrDefault();

                    //if you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
                    if (!String.IsNullOrEmpty(xff))
                    {
                        string lastIp = xff.Split(new[] { ',' }).FirstOrDefault();
                        result = lastIp;
                    }
                }

                if (String.IsNullOrEmpty(result) && _httpContext.Request.UserHostAddress != null)
                {
                    result = _httpContext.Request.UserHostAddress;
                }

                //some validation
                if (result == "::1")
                    result = IPAddress.Loopback.ToString();
                //remove port
                if (!String.IsNullOrEmpty(result))
                {
                    int index = result.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                    if (index > 0)
                        result = result.Substring(0, index);
                }
                return result;
            }
        }

        public string LanguageCode
        {
            get
            {
                HttpContextBase _context = new HttpContextWrapper(HttpContext.Current);
                string languageCode = DefaultLanguageCode;
                string sessionLanguageCode = _context.Session["LanguageCode"] as string;

                if (string.IsNullOrEmpty(sessionLanguageCode))
                {
                    languageCode = CookieHelper.GetValue(_context, "AS", "LanguageCode") ?? languageCode;
                    _context.Session["LanguageCode"] = languageCode;
                }
                else
                {
                    languageCode = sessionLanguageCode;
                }
                return languageCode;
            }
            set
            {
                HttpContextBase _context = new HttpContextWrapper(HttpContext.Current);
                CookieHelper.SetValue(_context, "AS", "LanguageCode", value);
                _context.Session["LanguageCode"] = value;
            }
        }

        public string RootAddress
        {
            get
            {
                var request = HttpContext.Current.Request;
                var appUrl = HttpRuntime.AppDomainAppVirtualPath;

                if (appUrl != "/") appUrl += "/";

                var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

                return baseUrl;
            }
        }
    }
}