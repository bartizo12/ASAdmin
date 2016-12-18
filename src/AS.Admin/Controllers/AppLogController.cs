using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections;
using System.Linq;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppLogController : ASControllerBase
    {
        private readonly ILogger _logger;
        private readonly ILoggingService _loggingService;

        public AppLogController(ILoggingService loggingService, ILogger logger)
        {
            this._logger = logger;
            this._loggingService = loggingService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult ViewDetail()
        {
            return View(Map<AppLogModel>(_loggingService.GetById(int.Parse(Request.QueryString["id"]))));
        }

        public ActionResult List(AppLogListModel model)
        {
            if (Request.IsAjaxRequest())
            {
                IPagedList<AppLog> list = _loggingService.GetLogs(model.SelectedLogLevels.ToList(),
                    model.From.Value, model.To.Value, model.PageIndex, model.PageSize, model.Ordering);

                return DataTableResult(list as IList, list.TotalCount, model.Draw);
            }
            return View(model);
        }
    }
}