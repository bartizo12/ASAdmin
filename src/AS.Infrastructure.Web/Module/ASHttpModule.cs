using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace AS.Infrastructure.Web.Module
{
    /// <summary>
    /// HttpModule to log HTTP traffic and some events in the application
    /// </summary>
    public class ASHttpModule : IHttpModule, IRequiresSessionState
    {
        private readonly string[] StaticResourceExtensions = new string[] { ".axd", ".ashx", ".bmp", ".css", ".gif", ".htm", ".html", ".ico", ".jpeg", ".jpg", ".js", ".png", ".rar", ".zip", ".txt", ".xml", ".ts", ".woff2", ".woff" };
        private readonly ISettingManager _settingManager;
        private readonly IContextProvider _httpContextProvider;
        private readonly ILogger _logger;
        private readonly IRequestLogger _requestLogger;
        private readonly IGeoProvider _geoProvider;

        public ASHttpModule(ISettingManager settingManager,
            IContextProvider httpContextProvider,
            ILogger logger,
            IRequestLogger requestLogger,
            IGeoProvider geoProvider)
        {
            this._settingManager = settingManager;
            this._geoProvider = geoProvider;
            this._requestLogger = requestLogger;
            this._httpContextProvider = httpContextProvider;
            this._logger = logger;
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
            context.EndRequest += Context_EndRequest;
            context.PostRequestHandlerExecute += Context_PostRequestHandlerExecute;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            _logger.Warn("Current domain about to be unloaded");
        }

        private void Context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            _httpContextProvider.Items["SessionId"] = _httpContextProvider.SessionId;

            if (_httpContextProvider.CountryInfo == Country.Empty)
            {
                _httpContextProvider.CountryInfo = this._geoProvider.GetCountryInfo(_httpContextProvider.ClientIP);
            }
            _httpContextProvider.Items["CountryInfo"] = _httpContextProvider.CountryInfo;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            string extension = VirtualPathUtility.GetExtension(this._httpContextProvider.Url.LocalPath);

            if (StaticResourceExtensions.Contains(extension))
                return;

            if (_settingManager.GetContainer<AppSetting>().Contains("RequestLoggingEnabled")
                && bool.Parse(_settingManager.GetContainer<AppSetting>()["RequestLoggingEnabled"].Value))
            {
                Stopwatch stopwatch = new Stopwatch();
                _httpContextProvider.Items["Stopwatch"] = stopwatch;
                stopwatch.Start();
            }
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            string extension = VirtualPathUtility.GetExtension(this._httpContextProvider.Url.LocalPath);

            if (StaticResourceExtensions.Contains(extension))
                return;

            if (_settingManager.GetContainer<AppSetting>().Contains("RequestLoggingEnabled") &&
                bool.Parse(_settingManager.GetContainer<AppSetting>()["RequestLoggingEnabled"].Value))
            {
                Stopwatch stopwatch = (Stopwatch)_httpContextProvider.Items["Stopwatch"];
                if (stopwatch == null)
                    return;

                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                RequestLog requestLog = new RequestLog();
                requestLog.AbsolutePath = _httpContextProvider.Url.AbsolutePath;
                requestLog.BrowserType = _httpContextProvider.BrowserType;
                requestLog.ClientIP = _httpContextProvider.ClientIP;
                requestLog.CountryCode = ((Country)_httpContextProvider.Items["CountryInfo"]).Key;
                requestLog.CreatedBy = _httpContextProvider.UserName;
                requestLog.Duration = (int)ts.TotalMilliseconds;
                requestLog.HttpMethod = _httpContextProvider.HttpMethod;
                requestLog.QueryString = _httpContextProvider.Url.Query;
                requestLog.SessionID = (string)_httpContextProvider.Items["SessionId"];
                requestLog.UserAgent = _httpContextProvider.ServerVariables["HTTP_USER_AGENT"];
                _requestLogger.Log(requestLog);
                _httpContextProvider.Items["Stopwatch"] = null;
                _httpContextProvider.Items["SessionId"] = null;
                _httpContextProvider.Items["CountryInfo"] = null;
            }
        }

        public void Dispose()
        {
            this._logger.Warn("ASHttpModule is disposed");
        }
    }
}