using AS.Domain.Interfaces;
using System;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Action Filter to log last activity time of online users
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class LastActivityUpdateActionFilter : ActionFilterAttribute
    {
        private readonly ILastActivityTimeUpdater _lastActivityTimeUpdater;

        public LastActivityUpdateActionFilter()
        {
        }

        public LastActivityUpdateActionFilter(ILastActivityTimeUpdater lastActivityTimeUpdater)
        {
            this._lastActivityTimeUpdater = lastActivityTimeUpdater;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            _lastActivityTimeUpdater.Update(filterContext.HttpContext.User);
        }
    }
}