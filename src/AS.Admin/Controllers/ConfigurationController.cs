using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using AS.Services.Interfaces;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.SqlServerCompact;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [AllowAnonymous]
    [ExcludeFilter(typeof(ConfigurationRedirectAttribute))]
    public class ConfigurationController : ASControllerBase
    {
        private readonly List<string> _avaliableProviders = new List<string>();
        private readonly IConfigurationService _configurationService;
        private readonly IAppManager _appManager;
        private readonly IResourceManager _resourceManager;

        public ConfigurationController(IConfigurationService configurationService,
             IResourceManager resourceManager,
             IAppManager appManager)
        {
            this._configurationService = configurationService;
            this._appManager = appManager;
            this._resourceManager = resourceManager;
            _avaliableProviders.Add(MySqlProviderInvariantName.ProviderName);
            _avaliableProviders.Add(SqlProviderServices.ProviderInvariantName);
            _avaliableProviders.Add(SqlCeProviderServices.ProviderInvariantName);
        }

        public ActionResult Index()
        {
            if (!TempData.ContainsKey("IsRedirectedToConfiguration") || TempData["IsRedirectedToConfiguration"] == null ||
                !Convert.ToBoolean(TempData["IsRedirectedToConfiguration"]))
                return RedirectToAction("Login", "Identity");

            if (_configurationService.CheckIfConfigExists())
                return RedirectToAction("Index", "Installer", new { isRedirected = true });

            ConfigurationModel model = new ConfigurationModel();
            model.IsDemo = true;
            model.DataProviderSelectList = new MultiSelectList(_avaliableProviders);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public JsonNetResult CanConnectDb(string provider, string connectionString)
        {
            ASConfiguration config = new ASConfiguration();
            config.ConnectionString = connectionString;
            config.DataProvider = provider;
            string connectionResult = _configurationService.CanConnectDatabase(config);

            if (!string.IsNullOrEmpty(connectionResult))
                connectionResult = string.Format(this._resourceManager.GetString("Installer_CannotConnectDatabase"),
                    connectionResult);

            return new JsonNetResult(connectionResult);
        }

        [ValidateAntiForgeryToken]
        public JsonNetResult CanConnectSMTP(ConfigurationModel model)
        {
            string connectionResult = _configurationService.CanConnectSMTPServer(Map<ASConfiguration>(model));

            if (!string.IsNullOrEmpty(connectionResult))
                connectionResult = string.Format(this._resourceManager.GetString("Installer_CannotConnectSMTP"),
                    connectionResult);

            return new JsonNetResult(connectionResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ConfigurationModel model)
        {
            model.DataProviderSelectList = new MultiSelectList(_avaliableProviders);
            string canConnectResult = this._configurationService.CanConnectDatabase(Map<ASConfiguration>(model));

            if (!string.IsNullOrEmpty(canConnectResult))
            {
                ModelState.AddModelError(string.Empty,
                    string.Format(this._resourceManager.GetString("Installer_CannotConnectDatabase"), canConnectResult));
                return View(model);
            }
            _configurationService.SaveConfig(Map<ASConfiguration>(model));
            _appManager.RestartApplication();

            return RedirectToAction("Index", "Installer", new { isRedirected = true });
        }
    }
}