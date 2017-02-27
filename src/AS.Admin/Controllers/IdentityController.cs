using AS.Admin.Models;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using AS.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AS.Infrastructure.Web.Identity;

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
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ASSignInManager _signInManager;

        public IdentityController(IMembershipService service,
            IContextProvider contextProvider,
            IResourceManager resourceManager,
            ISettingManager settingManager,
            IAuthenticationManager authenticationManager,
            ASSignInManager signInManager)
        {
            this._service = service;
            this._resourceManager = resourceManager;
            this._contextProvider = contextProvider;
            this._settingManager = settingManager;
            this._authenticationManager = authenticationManager;
            this._signInManager = signInManager;
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Identity", new { ReturnUrl = returnUrl }));
        }
        [AllowAnonymous]
        public ActionResult ExternalLoginCallbackRedirect(string returnUrl)
        {
            return RedirectPermanent("/Identity/ExternalLoginCallback");
        }
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await this._authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            bool result = _service.LoginExternal(loginInfo.Email, loginInfo.Login.LoginProvider, loginInfo.Login.ProviderKey);

            if(result)
                return RedirectToAction(string.Empty, "Home");
            else
                return RedirectToAction("Login", "Identity");
        }
    }
}