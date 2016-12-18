using System;
using System.Globalization;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Model binder for DateTime. Deals with  UTC - Client TimeZone convertions
    /// Note that , HttpRequest must have a header "ClientTimeZone" that contains TimeZone info of the
    /// client
    /// </summary>
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            DateTime? date = (DateTime?)value.ConvertTo(typeof(DateTime), CultureInfo.CurrentUICulture);
            int clientTimeZone;

            if (int.TryParse(controllerContext.RequestContext.HttpContext.Request.Headers["ClientTimeZone"], out clientTimeZone)
                && date != null)
            {
                date = date.Value.AddMinutes(clientTimeZone);
            }
            return date;
        }
    }
}