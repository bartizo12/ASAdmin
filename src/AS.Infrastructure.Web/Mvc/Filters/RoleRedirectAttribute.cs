using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Redirects users according to their roles
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class RoleRedirectAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (ASControllerBase)filterContext.Controller;

            if (controller.User.Identity.IsAuthenticated)
            {
                if (controller.User.IsInRole("Admin"))
                {
                    filterContext.Result = new RedirectToRouteResult(
            new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                }
            }
        }
    }
}