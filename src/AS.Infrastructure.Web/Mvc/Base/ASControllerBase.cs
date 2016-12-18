using Mapster;
using System.Collections;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Controller base class.
    /// </summary>
    public abstract class ASControllerBase : Controller
    {
        /// <summary>
        /// If the action is unknown , directs to index
        /// </summary>
        /// <param name="actionName">Unknown action name</param>
        protected override void HandleUnknownAction(string actionName)
        {
            RedirectToAction("Index").ExecuteResult(this.ControllerContext);
        }

        /// <summary>
        /// Returns json resultview in the format  that  datatable API (jquery table plugin)
        /// </summary>
        /// <param name="data">List of objects</param>
        /// <param name="totalCount">Total count in original data(without pagination=</param>
        /// <param name="draw"></param>
        /// <returns>Returns json result</returns>
        protected JsonNetResult DataTableResult(IList data, int totalCount, int draw)
        {
            DataTableModel model = new DataTableModel();
            model.data = data as IList;
            model.TotalCount = totalCount;
            model.Draw = draw;

            return new JsonNetResult(model);
        }

        /// <summary>
        /// Returns json resultview in the format  that  datatable API (jquery table plugin)
        /// </summary>
        /// <param name="data">List of objects</param>
        /// <returns>Returns json resultview in the format  that  datatable API (jquery table plugin)</returns>
        protected JsonNetResult DataTableResult(IList data)
        {
            DataTableModel model = new DataTableModel();
            model.data = data;
            model.TotalCount = data.Count;

            return new JsonNetResult(model);
        }

        /// <summary>
        /// Maps one object to another by using FastMapper
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Mapped object</returns>
        protected TDestination Map<TDestination>(object entity)
        {
            return TypeAdapter.Adapt<TDestination>(entity);
        }
    }
}