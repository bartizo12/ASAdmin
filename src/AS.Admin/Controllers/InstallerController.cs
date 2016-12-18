using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using AS.Services.Interfaces;
using System.Net;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [AllowAnonymous]
    [ExcludeFilter(typeof(ConfigurationRedirectAttribute))]
    public class InstallerController : ASControllerBase
    {
        private readonly IInstallerService _service;

        public InstallerController(IInstallerService service)
        {
            this._service = service;
        }

#if DEBUG

        public ActionResult Index()
        {
            _service.Install();
            return RedirectToAction("Login", "Identity");
        }

#else
        public ActionResult Index(bool? isRedirected)
        {
            if (isRedirected == null || isRedirected == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (isRedirected != null && isRedirected.Value)
            {
                _service.Install();
            }
            return RedirectToAction("Login", "Identity");
        }
#endif
    }
}