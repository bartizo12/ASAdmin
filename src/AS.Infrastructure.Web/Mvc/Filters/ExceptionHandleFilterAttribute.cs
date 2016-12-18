using AS.Domain.Interfaces;
using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Custom exception filter that handles application exceptions. Logs and redirect to
    /// error page
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ExceptionHandleFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private readonly ILogger _logger;

        public ExceptionHandleFilterAttribute()
        {
        }

        public ExceptionHandleFilterAttribute(ILogger logger)
        {
            this._logger = logger;
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                this._logger.Error(filterContext.Exception);
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = filterContext.Exception.Message,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Error", action = "FiveOO" }));
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                }
                filterContext.ExceptionHandled = true;
            }
        }
    }
}