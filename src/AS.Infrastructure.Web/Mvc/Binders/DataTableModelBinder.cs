using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Model binder for DataTable(jquery table plugin)
    /// Reads datatable arguments and bind it model
    /// </summary>
    public class DataTableModelBinder : ASModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            DataTableModel model = base.BindModel(controllerContext, bindingContext) as DataTableModel;

            if (model != null)
            {
                if (!string.IsNullOrEmpty(controllerContext.HttpContext.Request.QueryString["order[0][dir]"]) &&
                    !string.IsNullOrEmpty(controllerContext.HttpContext.Request.QueryString["order[0][column]"]))
                {
                    int orderColIndex = int.Parse(controllerContext.HttpContext.Request.QueryString["order[0][column]"]);
                    string columnKey = "columns[" + orderColIndex.ToString() + "][data]";
                    model.Ordering = controllerContext.HttpContext.Request.QueryString[columnKey] + " " + controllerContext.HttpContext.Request.QueryString["order[0][dir]"].ToString().ToUpper();
                }
                if (!string.IsNullOrEmpty(controllerContext.HttpContext.Request.QueryString["start"]))
                {
                    model.start = int.Parse(controllerContext.HttpContext.Request.QueryString["start"]);
                }
                if (!string.IsNullOrEmpty(controllerContext.HttpContext.Request.QueryString["length"]))
                {
                    model.PageSize = int.Parse(controllerContext.HttpContext.Request.QueryString["length"]);
                }
            }
            return model;
        }
    }
}