using AS.Admin.Models;
using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [AllowAnonymous]
    [ExcludeFilter(typeof(ConfigurationRedirectAttribute))]
    public class ErrorController : ASControllerBase
    {
        public ActionResult FiveOO()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(new ErrorModel());
            }
            return RedirectToAction("FiveOOPublic");
        }

        public ActionResult FiveOOPublic()
        {
            return View(new ErrorModel());
        }

        public ActionResult FourOFour()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(new ErrorModel());
            }
            return RedirectToAction("FourOFourPublic");
        }

        public ActionResult FourOFourPublic()
        {
            return View(new ErrorModel());
        }
    }
}