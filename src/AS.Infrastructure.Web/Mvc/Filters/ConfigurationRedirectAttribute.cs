using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Redirects client to the configuration page if configuration file does not exist.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ConfigurationRedirectAttribute : ActionFilterAttribute
    {
        private readonly IStorageManager<Configuration> _configurationStorageManager;

        public ConfigurationRedirectAttribute()
        {
        }

        public ConfigurationRedirectAttribute(IStorageManager<Configuration> configurationStorageManager)
        {
            this._configurationStorageManager = configurationStorageManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool filterDisabled = false;

            foreach (FilterAttribute attr in filterContext.ActionDescriptor.ControllerDescriptor.GetFilterAttributes(true))
            {
                ExcludeFilterAttribute excludeFilterAttr = (attr as ExcludeFilterAttribute);

                if (excludeFilterAttr != null && excludeFilterAttr.FilterType == this.GetType())
                {
                    filterDisabled = true;
                    break;
                }
            }

            if (filterDisabled)
                return;

            if (!this._configurationStorageManager.CheckIfExists())
            {
                filterContext.Controller.TempData["IsRedirectedToConfiguration"] = true;

                filterContext.Result = new RedirectToRouteResult(
        new RouteValueDictionary(new { controller = "Configuration", action = "Index" }));
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            }
        }
    }
}