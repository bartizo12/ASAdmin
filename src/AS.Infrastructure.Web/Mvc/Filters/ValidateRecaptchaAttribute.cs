using System;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Validates ReCatpcha code in the page
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ValidateRecaptchaAttribute : ActionFilterAttribute
    {
        private const string G_RESPONSE_FIELD_KEY = "g-recaptcha-response";

        public RecaptchaValidator RecaptchaValidator { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string response = filterContext.HttpContext.Request.Form[G_RESPONSE_FIELD_KEY];

            if (response != null)
            {
                var result = RecaptchaValidator.Validate(response);

                if(!result.Succeeded)
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError(string.Empty, string.Join(".",result.Errors) );
                }
            }
        }
    }
}