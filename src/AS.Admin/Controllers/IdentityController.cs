using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using AS.Services.Interfaces;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [AllowAnonymous]
    public class IdentityController : ASControllerBase
    {
        private const int DefaultRecaptchaDisplayCount = 3;

        private readonly IContextProvider _contextProvider;
        private readonly IMembershipService _service;
        private readonly ISettingManager _settingManager;
        private readonly IResourceManager _resourceManager;

        public IdentityController(IMembershipService service,
            IContextProvider contextProvider,
            IResourceManager resourceManager,
            ISettingManager settingManager)
        {
            this._service = service;
            this._resourceManager = resourceManager;
            this._contextProvider = contextProvider;
            this._settingManager = settingManager;
        }

        private short RecaptchaDisplayCount
        {
            get
            {
                short count;

                if (_settingManager.GetContainer<AppSetting>().Contains("RecaptchaDisplayCount") &&
                    short.TryParse(_settingManager.GetContainer<AppSetting>()["RecaptchaDisplayCount"].Value, out count))
                {
                    return count;
                }
                return DefaultRecaptchaDisplayCount;
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            this._service.LogOut();

            return RedirectToAction("Login", "Identity");
        }

        [RoleRedirect]
        public ActionResult ResetPassword()
        {
            ResetPasswordModel model = new ResetPasswordModel();

            if (!_service.ValidateToken(Request.QueryString["token"]))
            {
                return RedirectToAction("Login");
            }
            model.Token = Request.QueryString["token"];

            return View(model);
        }

        [HttpPost]
        [RoleRedirect]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                _service.ResetPassword(model.Token, model.NewPassword);
                TempData["ResultType"] = MessageType.Success;
                TempData["ResultMessage"] = this._resourceManager.GetString("ResetPassword_Successful");
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }

        [RoleRedirect]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordModel model = new ForgotPasswordModel();

            if (_settingManager.GetContainer<AppSetting>().Contains("RecaptchaPublickey") &&
                _settingManager.GetContainer<AppSetting>().Contains("RecaptchaPrivateKey"))
            {
                model.DisplayCaptcha = true;
                model.CaptchaPublicKey = _settingManager.GetContainer<AppSetting>()["RecaptchaPublickey"].Value;
            }
            return View(model);
        }

        [HttpPost]
        [RoleRedirect]
        [ValidateAntiForgeryToken]
        [ValidateRecaptcha]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (_settingManager.GetContainer<AppSetting>().Contains("RecaptchaPublickey") &&
                _settingManager.GetContainer<AppSetting>().Contains("RecaptchaPrivateKey"))
            {
                model.DisplayCaptcha = true;
                model.CaptchaPublicKey = _settingManager.GetContainer<AppSetting>()["RecaptchaPublickey"].Value;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                _service.StartForgotPasswordProcess(model.UserNameOrEmail);
                TempData["ResultType"] = MessageType.Info;
                TempData["ResultMessage"] = this._resourceManager.GetString("ForgotPassword_Success");
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        [RoleRedirect]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginModel model = new LoginModel();
            model.RememberMe = true;

            if (_contextProvider.LoginAttemptCount > this.RecaptchaDisplayCount &&
                _settingManager.GetContainer<AppSetting>().Contains("RecaptchaPublickey") &&
                _settingManager.GetContainer<AppSetting>().Contains("RecaptchaPrivateKey"))
            {
                model.DisplayCaptcha = true;
                model.CaptchaPublicKey = _settingManager.GetContainer<AppSetting>()["RecaptchaPublickey"].Value;
            }

            return View(model);
        }

        public ActionResult SetLanguageCode(string languageCode)
        {
            _contextProvider.LanguageCode = languageCode;

            return Login(string.Empty);
        }

        [HttpPost]
        [RoleRedirect]
        [ValidateAntiForgeryToken]
        [ValidateRecaptcha]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (_contextProvider.LoginAttemptCount > this.RecaptchaDisplayCount &&
                _settingManager.GetContainer<AppSetting>().Contains("RecaptchaPublickey") &&
                _settingManager.GetContainer<AppSetting>().Contains("RecaptchaPrivateKey"))
            {
                model.DisplayCaptcha = true;
                model.CaptchaPublicKey = _settingManager.GetContainer<AppSetting>()["RecaptchaPublickey"].Value;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                bool result = _service.Login(model.UserNameOrEmail, model.Password, model.RememberMe);

                if (result)
                {
                    return RedirectToAction(string.Empty, "Home");
                }
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }
    }
}