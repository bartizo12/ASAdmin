using AS.Infrastructure.Web.Mvc;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    public class SharedController : ASControllerBase
    {
        public ActionResult Index()
        {
            return RedirectToAction("Result");
        }

        public ActionResult Result()
        {
            return View(TempData["ResultModel"] as ASModelBase);
        }

        public ActionResult Ping()
        {
            return new EmptyResult();
        }
    }
}