using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Sets global/common variables for views
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class GlobalValuesActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ISettingManager _settingManager;
        private readonly IResourceManager _resourceManager;
        private readonly IContextProvider _contextProvider;

        public GlobalValuesActionFilterAttribute()
        {
        }

        public GlobalValuesActionFilterAttribute(ISettingManager settingManager,
            IContextProvider contextProvider,
            IResourceManager resourceManager)
        {
            this._settingManager = settingManager;
            this._contextProvider = contextProvider;
            this._resourceManager = resourceManager;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ASModelBase model = filterContext.Controller.ViewData.Model as ASModelBase;

            if (model != null)
            {
                var settings = _settingManager.GetContainer<AppSetting>();
                if (settings.Contains("ApplicationDefaultTitle"))
                    model.Header.Title = settings["ApplicationDefaultTitle"].Value;
                if (settings.Contains("MetaDescription"))
                    model.Header.MetaDescription = settings["MetaDescription"].Value;
                if (settings.Contains("MetaKeywords"))
                    model.Header.MetaKeywords = settings["MetaKeywords"].Value;

                bool isDemo = false;

                if (settings.Contains("IsDemo"))
                {
                    bool.TryParse(settings["IsDemo"].Value, out isDemo);
                }
                model.Header.IsDemo = isDemo;
                model.Header.ClientResources = _resourceManager.GetStringResources()
                                                               .Where(r => r.AvailableOnClientSide == true)
                                                               .ToDictionary(r => r.Name, r => r.Value);

                model.Header.LanguageList = new List<SelectListItem>();

                foreach (string langCode in _resourceManager.GetAvailableCultureList().OrderBy(s => s))
                {
                    model.Header.LanguageList.Add(new SelectListItem()
                    {
                        Value = langCode,
                        Selected = _contextProvider.LanguageCode == langCode,
                        Text = CultureInfo.GetCultureInfo(langCode).GetNativeNameWithoutCountryName()
                    });
                }
            }
        }
    }
}