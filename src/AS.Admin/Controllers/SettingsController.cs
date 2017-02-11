using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Logging;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : ASControllerBase
    {
        private readonly ISettingsService _settingService;
        private readonly ITemplateService _templateService;
        private readonly ICacheService _cacheService;
        private readonly IAppManager _appManager;
        private readonly IConfigurationService _configurationService;
        private readonly ISettingManager _settingManager;
        private readonly IResourceManager _resourceManager;

        public SettingsController(ISettingsService settingService,
            ITemplateService templateService,
            IAppManager appManager,
            ICacheService cacheService,
            IResourceManager resourceManager,
            ISettingManager settingManager,
            IConfigurationService configurationService)
        {
            this._settingManager = settingManager;
            this._resourceManager = resourceManager;
            this._settingService = settingService;
            this._cacheService = cacheService;
            this._appManager = appManager;
            this._templateService = templateService;
            this._configurationService = configurationService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult Edit()
        {
            string id = Request.QueryString["id"];

            if (string.IsNullOrEmpty(id))
                return View(new SettingValueModel());
            SettingValue settingVal = _settingService.GetSettingValueById(int.Parse(id));

            if (settingVal.IsHiddenFromUser)
                return View(new SettingValueModel());

            SettingValueModel model = Map<SettingValueModel>(_settingService.GetSettingValueById(int.Parse(id)));

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SettingValueModel model)
        {
            try
            {
                SettingValue value = Map<SettingValue>(model);
                value.SettingDefinition = null;
                var dbValue = _settingService.GetSettingValueById(model.Id);
                value.SettingDefinitionID = dbValue.SettingDefinitionID;
                value.CreatedBy = dbValue.CreatedBy;
                value.CreatedOn = dbValue.CreatedOn;

                if (dbValue.IsHiddenFromUser)
                    throw new ASException(this._resourceManager.GetString("ErrorMessage_UnableToUpdate"));

                _settingService.UpdateSettingValue(value);

                TempData["ResultType"] = MessageType.Success;
                TempData["ResultMessage"] = this._resourceManager.GetString("SettingValueUpdateSuccess");
                TempData["ResultModel"] = model;
                _cacheService.Clear();

                return RedirectToAction("Result", "Shared");
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.SettingDefinition = Map<SettingDefinitionModel>(_settingService.GetSettingDefinitionById(model.SettingDefinitionID));
            }

            return View(model);
        }

        public ActionResult List(SettingsListModel model)
        {
            if (model == null)
                model = new SettingsListModel();

            if (Request.IsAjaxRequest())
            {
                IList<SettingValue> values = _settingService.SelectSettingValues(model.Ordering, model.SettingDefId.Value).
                    Where(value => value.IsHiddenFromUser == false).ToList();
                SettingDefinition settingDef = _settingService.GetSettingDefinitionById(model.SettingDefId.Value);

                foreach (SettingValue settingVal in values)
                {
                    for (int i = 1; i <= 15; i++)
                    {
                        FormInputType type = (FormInputType)typeof(SettingDefinition).GetProperty("FieldInputType" + i.ToString()).GetValue(settingDef, null);

                        if (type == FormInputType.Password)
                        {
                            typeof(SettingValue).GetProperty("Field" + i.ToString()).SetValue(settingVal, "******");
                        }
                    }
                }
                return DataTableResult(values as IList);
            }
            else
            {
                IEnumerable<SettingDefinition> settingDefList = _settingService.SelectAllSettingDefinitions();
                //Some settings has their own maangement page
                model.SettingDefinitionSelectList = settingDefList
                    .Where(p => p.Name != AvailableSettings.EMailSetting &&
                                p.Name != AvailableSettings.HTMLTemplate &&
                                p.Name != AvailableSettings.MembershipSetting &&
                                p.Name != AvailableSettings.AppSetting)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name + " (" + p.Description + ")"
                    }).ToList();
                model.SettingDefinitions = settingDefList;

                return View(model);
            }
        }

        #region Templates

        [HttpPost]
        public ActionResult EditTemplate(HTMLTemplateModel model)
        {
            model.TemplateFileList = new MultiSelectList(_templateService.FetchTemplateFileList());

            if (model.Id <= 0)
                ModelState.AddModelError(string.Empty, this._resourceManager.GetString("ErrorMessage_UnableToUpdate"));

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                SettingValue value = new SettingValue();
                value.Field1 = model.Name;
                value.Field2 = model.Subject;
                value.Field3 = model.BodyFilePath;
                value.Field4 = model.Comment;
                value.Id = model.Id;
                SettingValue dbValue = _settingService.GetSettingValueById(model.Id);

                if (dbValue.IsHiddenFromUser)
                    throw new ASException(this._resourceManager.GetString("ErrorMessage_UnableToUpdate"));

                value.SettingDefinitionID = dbValue.SettingDefinitionID;
                value.CreatedBy = dbValue.CreatedBy;
                value.CreatedOn = dbValue.CreatedOn;
                _settingService.UpdateSettingValue(value);

                TempData["ResultType"] = MessageType.Success;
                TempData["ResultMessage"] = this._resourceManager.GetString("SettingValueUpdateSuccess");
                TempData["ResultModel"] = model;
                _cacheService.Clear();

                return RedirectToAction("Result", "Shared");
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        public JsonNetResult GetHTML(string bodyFilePath)
        {
            string html = System.IO.File.ReadAllText(this._appManager.MapPhysicalFile(bodyFilePath));

            return new JsonNetResult(html);
        }

        public ActionResult EditTemplate()
        {
            string idStr = Request.QueryString["id"];
            int id = 0;

            if (string.IsNullOrEmpty(idStr) || !int.TryParse(idStr, out id))
            {
                ModelState.AddModelError(string.Empty, this._resourceManager.GetString("RecordDoesNotExist"));

                return View(new HTMLTemplateModel());
            }
            HTMLTemplateModel model = (HTMLTemplateModel)_settingService.GetSettingValueById(id);

            model.TemplateFileList = new MultiSelectList(_templateService.FetchTemplateFileList());
            return View(model);
        }

        public ActionResult ListTemplates(SettingsListModel model)
        {
            if (model == null)
                return View(new SettingsListModel());

            if (Request.IsAjaxRequest())
            {
                SettingDefinition settingDef = _settingService.GetSettingDefinitionByName("HTMLTemplate");
                List<HTMLTemplateModel> modelList = new List<HTMLTemplateModel>();

                foreach (SettingValue settingVal in _settingService.SelectSettingValues(model.Ordering, settingDef.Id))
                {
                    modelList.Add((HTMLTemplateModel)settingVal);
                }
                return DataTableResult(modelList);
            }
            return View(model);
        }

        #endregion Templates

        #region EMailSettings

        [HttpPost]
        public ActionResult TestSMTPConnection(EMailSettingModel model)
        {
            ASConfiguration config = new ASConfiguration();
            config.SMTPDefaultCredentials = model.DefaultCredentials;
            config.SMTPEnableSsl = model.EnableSsl;
            config.SMTPFromAddress = model.FromAddress;
            config.SMTPFromDisplayName = model.FromDisplayName;
            config.SMTPHost = model.Host;
            config.SMTPName = model.Name;
            config.SMTPPassword = model.Password;
            config.SMTPPort = model.Port;
            config.SMTPTimeOut = model.TimeOut;
            config.SMTPUserName = model.UserName;

            return new JsonNetResult(_configurationService.CanConnectSMTPServer(config));
        }

        [HttpPost]
        public ActionResult EditEMailSettings(EMailSettingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                ASConfiguration config = new ASConfiguration();
                config.SMTPDefaultCredentials = model.DefaultCredentials;
                config.SMTPEnableSsl = model.EnableSsl;
                config.SMTPFromAddress = model.FromAddress;
                config.SMTPFromDisplayName = model.FromDisplayName;
                config.SMTPHost = model.Host;
                config.SMTPName = model.Name;
                config.SMTPPassword = model.Password;
                config.SMTPPort = model.Port;
                config.SMTPTimeOut = model.TimeOut;
                config.SMTPUserName = model.UserName;

                string result = _configurationService.CanConnectSMTPServer(config);

                if (!string.IsNullOrEmpty(result))
                {
                    throw new ASException(result);
                }

                SettingValue value = (SettingValue)model;

                if (model.Id > 0)
                {
                    SettingValue dbValue = _settingService.GetSettingValueById(model.Id);

                    if (dbValue.IsHiddenFromUser)
                        throw new ASException(this._resourceManager.GetString("ErrorMessage_UnableToUpdate"));

                    value.SettingDefinitionID = dbValue.SettingDefinitionID;
                    value.CreatedBy = dbValue.CreatedBy;
                    value.CreatedOn = dbValue.CreatedOn;
                    _settingService.UpdateSettingValue(value);

                    TempData["ResultType"] = MessageType.Success;
                    TempData["ResultMessage"] = this._resourceManager.GetString("Resources.SettingValueUpdateSuccess");
                }
                else
                {
                    value.SettingDefinitionID = _settingService.GetSettingDefinitionByName("EMailSetting").Id;
                    _settingService.AddSettingValue(value);
                    TempData["ResultType"] = MessageType.Success;
                    TempData["ResultMessage"] = this._resourceManager.GetString("SettingValueUpdateSuccess");
                }
                _cacheService.Clear();

                TempData["ResultModel"] = model;
                return RedirectToAction("Result", "Shared");
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        public ActionResult EditEMailSettings()
        {
            string id = Request.QueryString["id"];
            EMailSettingModel model;

            if (string.IsNullOrEmpty(id))
            {
                model = new EMailSettingModel();
                return View(model);
            }
            model = (EMailSettingModel)_settingService.GetSettingValueById(int.Parse(id));
            model.UserName = string.Empty;
            model.Password = string.Empty;

            return View(model);
        }

        public ActionResult ListEMailSettings(SettingsListModel model)
        {
            if (model == null)
                return View(new SettingsListModel());

            if (Request.IsAjaxRequest())
            {
                SettingDefinition settingDef = _settingService.GetSettingDefinitionByName("EMailSetting");
                List<EMailSettingModel> modelList = new List<EMailSettingModel>();

                foreach (SettingValue settingVal in _settingService.SelectSettingValues(model.Ordering, settingDef.Id))
                {
                    EMailSettingModel modelItem = (EMailSettingModel)settingVal;
                    modelItem.UserName = RegexHelper.MaskEmailAddress(modelItem.UserName);
                    modelItem.Password = RegexHelper.Mask(modelItem.Password);

                    modelList.Add(modelItem);
                }
                return DataTableResult(modelList);
            }
            return View(model);
        }

        #endregion EMailSettings

        #region ApplicationSettings

        public ActionResult ApplicationSettings()
        {
            ApplicationSettingsModel model = new ApplicationSettingsModel();
            var appSettings = this._settingManager.GetContainer<AppSetting>();

            model.ApplicationDefaultTitle = appSettings["ApplicationDefaultTitle"].Value;
            model.BundlingEnabled = bool.Parse(appSettings["BundlingEnabled"].Value);
            model.DbQueryLogEnable = bool.Parse(appSettings["DbQueryLogEnable"].Value);
            model.RequestLoggingEnabled = bool.Parse(appSettings["RequestLoggingEnabled"].Value);
            model.LogLevels = new MultiSelectList(LogLevels.All);
            model.MetaDescription = appSettings["MetaDescription"].Value;
            model.MetaKeywords = appSettings["MetaKeywords"].Value;
            model.MinLogLevel = appSettings["MinLogLevel"].Value;
            model.RecaptchaDisplayCount = int.Parse(appSettings["RecaptchaDisplayCount"].Value);

            return View(model);
        }

        public ActionResult UpdateApplicationSetting(string name, string newValue)
        {
            SettingDefinition settingDef = _settingService.GetSettingDefinitionByName("AppSetting");
            SettingValue settingValue = _settingService.SelectSettingValues(null, settingDef.Id)
                                        .Where(s => s.Get(settingDef, "Name") == name)
                                        .First();
            settingValue.Set(settingDef, "Value", newValue);
            _settingService.UpdateSettingValue(settingValue);
            _cacheService.Clear();

            return new EmptyResult();
        }

        #endregion ApplicationSettings

        #region MembershipSettings

        public ActionResult MembershipSettings()
        {
            return View(Map<MembershipSettingModel>(this._settingManager.GetContainer<MembershipSetting>().Default));
        }

        public ActionResult UpdateMembershipSetting(string fieldName, string newValue)
        {
            SettingDefinition settingDef = _settingService.GetSettingDefinitionByName("MembershipSetting");
            SettingValue settingVal = _settingService.SelectSettingValues(string.Empty, settingDef.Id).First();

            settingVal.Set(settingDef, fieldName, newValue);
            _settingService.UpdateSettingValue(settingVal);
            _cacheService.Clear();

            return new EmptyResult();
        }

        #endregion MembershipSettings
    }
}