using AS.Infrastructure.Web.Mvc;
using System;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    public class AppLogListModelBinder : DataTableModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;
            AppLogListModel model = (AppLogListModel)base.BindModel(controllerContext, bindingContext);

            if (!string.IsNullOrEmpty(request.QueryString["SelectedLogLevels"]))
            {
                model.SelectedLogLevels = request.QueryString["SelectedLogLevels"]
                    .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            return model;
        }
    }
}