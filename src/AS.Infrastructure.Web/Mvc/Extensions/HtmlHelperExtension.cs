using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure;
using AS.Infrastructure.Web;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// Renders a multiple-selection dropdown list.
        /// Adds "multipleSelect" css class to the element.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString FormLineDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class='form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(expression, new { @class = "col-sm-2 control-label" }).ToHtmlString());
            sb.AppendLine("<div class='col-sm-10'>");

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attrs.Add("multiple", string.Empty);

            sb.AppendLine(htmlHelper.ListBoxFor(expression, selectList, attrs).ToHtmlString());
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='col-sm-offset-2 validation-wrapper'>");
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Renders drop-down list
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public static MvcHtmlString FormLineDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class='form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(expression, new { @class = "col-sm-2 control-label" }).ToHtmlString());
            sb.AppendLine("<div class='col-sm-10 form-group-input'>");
            sb.AppendLine(htmlHelper.DropDownListFor(expression, selectList, new { @class = "form-control" }).ToHtmlString());
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='col-sm-offset-2 validation-wrapper'>");
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Renders visual job status by using bootstrap classes and fontawesome icons
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString FormLineJobStatusFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class='form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(expression, new { @class = "col-sm-2 control-label" }).ToHtmlString());
            sb.AppendLine("<div class='col-sm-10 form-group-input'>");
            JobStatus value = (JobStatus)(int)ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;

            if (value == JobStatus.Queued)
            {
                sb.AppendLine("<span style='display: inline-block;' class='label label-warning' data-placement='right'  data-toggle='tooltip' title='" + ResMan.GetString("Queued") + "'><i class='fa fa-clock-o fa-3x'></i></label>");
            }
            else if (value == JobStatus.Failed)
            {
                sb.AppendLine("<span style='display: inline-block;' class='label label-danger' data-placement='right'  data-toggle='tooltip' title='" + ResMan.GetString("Failed") + "'><i class='fa fa-times fa-3x'></i></label>");
            }
            else if (value == JobStatus.Finished)
            {
                sb.AppendLine("<span style='display: inline-block;' class='label label-success' data-placement='right'  data-toggle='tooltip' title='" + ResMan.GetString("Successful") + "'><i class='fa fa-check-circle fa-3x'></i></label>");
            }
            else
            {
                sb.AppendLine("<span style='display: inline-block;' class='label label-info' data-placement='right'  data-toggle='tooltip' title='" + ResMan.GetString("Executing") + "'><i class='fa fa-circle-o-notch fa-3x fa-spin'></i></label>");
            }
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Renders an input line on the form.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString FormLineEditorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class='form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(expression, new { @class = "col-sm-2 control-label" }).ToHtmlString());
            var modelMetaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            if (expression.IsOptional())
            {
                int index = sb.ToString().LastIndexOf("</label>");
                sb.Insert(index, "<i style='font-weight:normal'>(" + ServiceLocator.Current.Resolve<IResourceManager>().GetString("Optional") + ")</i>");
            }
            if (!string.IsNullOrEmpty(modelMetaData.Description))
            {
                int index = sb.ToString().LastIndexOf("</label>");
                sb.Insert(index, "<a class='btn btn-link' href='#' data-toggle='tooltip'" +
                    "data-original-title='" +
                    HttpContext.Current.Server.HtmlEncode(modelMetaData.Description) + "'" +
                    " style='padding: 0px;float:right;'><i style='position:absolute;top: 0px;' class='fa fa-question-circle-o'></i></a>");
            }
            sb.AppendLine("<div class='col-sm-10 form-group-input'>");
            sb.AppendLine(htmlHelper.EditorFor(expression, htmlAttributes).ToHtmlString());
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='col-sm-offset-2 validation-wrapper'>");
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Renders an uneditable boolean line.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString FormLineBooleanFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class='form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(expression, new { @class = "col-sm-2 control-label" }).ToHtmlString());
            sb.AppendLine("<div class='col-sm-10 form-group-input'>");
            bool value = (bool)ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;

            if (value)
            {
                sb.AppendLine("<span style='display: inline-block;margin-top:10px;' class='label label-success' data-placement='right' data-toggle='tooltip' title='' data-original-title='True'><i class='fa  fa-check fa-2x'></i></span>");
            }
            else
            {
                sb.AppendLine("<span style='display: inline-block;margin-top:10px;' class='label label-danger' data-placement='right' data-toggle='tooltip' title='' data-original-title='True'><i class='fa  fa-times fa-2x'></i></span>");
            }
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Render an icheck checkbox ( http://icheck.fronteed.com/)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString FormLineCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class='form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(expression, new { @class = "col-sm-2 control-label" }).ToHtmlString());
            sb.AppendLine("<div class='col-sm-10 form-group-input'>");
            sb.AppendLine("<div class='checkbox icheck'>");
            sb.AppendLine(htmlHelper.CheckBoxFor(expression).ToHtmlString());
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }
    }
}