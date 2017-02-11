using AS.Domain.Interfaces;
using AS.Infrastructure.Web.Mvc;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [AllowAnonymous]
    public class LanguageController : ASControllerBase
    {
        private readonly IContextProvider _contextProvider;

        public LanguageController(IContextProvider contextProvider)
        {
            this._contextProvider = contextProvider;
        }

        public ActionResult Set(string languageCode)
        {
            _contextProvider.LanguageCode = languageCode;

            return new EmptyResult();
        }
    }
}