using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : ASControllerBase
    {
        private readonly ISettingManager _settingManager;
        private readonly IStatisticsProvider _statisticsProvider;
        private readonly IResourceManager _resourceManager;

        public HomeController(IStatisticsProvider statisticsProvider,
            IResourceManager resourceManager,
            ISettingManager settingManager)
        {
            this._resourceManager = resourceManager;
            this._statisticsProvider = statisticsProvider;
            this._settingManager = settingManager;
        }

        public JsonNetResult GetAdminNotifications()
        {
            List<string> notifications = new List<string>();

            if (_settingManager.GetContainer<EMailSetting>().Default == null)
            {
                notifications.Add(this._resourceManager.GetString("Admin_EMailSettingMissingNotification"));
            }
            if (!_settingManager.GetContainer<AppSetting>().Contains("IPInfoDbApiKey"))
            {
                notifications.Add(this._resourceManager.GetString("Admin_IPQueryApiKeyMissing"));
            }
            return new JsonNetResult(notifications);
        }

        public JsonNetResult GetTotalJobCount()
        {
            return new JsonNetResult(_statisticsProvider.TotalJobCount);
        }

        public JsonNetResult GetTotalLogCount()
        {
            return new JsonNetResult(_statisticsProvider.TotalLogCount);
        }

        public JsonNetResult GetTotalMailCount()
        {
            return new JsonNetResult(_statisticsProvider.TotalMailCount);
        }

        public JsonNetResult GetTotalUserCount()
        {
            return new JsonNetResult(_statisticsProvider.TotalUserCount);
        }

        public ActionResult Index()
        {
            var model = new HomeModel();

            var emailSettings = _settingManager.GetContainer<EMailSetting>();

            if (emailSettings.Default == null || string.IsNullOrEmpty(emailSettings.Default.Host)
                || emailSettings.Default.Port == 0)
            {
                model.SMTPSettingIsMissing = true;
            }

            return View(model);
        }
    }
}