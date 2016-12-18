using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SchedulerController : ASControllerBase
    {
        private readonly ISchedulerService _service;
        private readonly ISettingManager _settingManager;

        public SchedulerController(ISchedulerService service, ISettingManager settingManager)
        {
            this._service = service;
            this._settingManager = settingManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(DataTableModel model)
        {
            if (Request.IsAjaxRequest())
            {
                IList<JobDefinition> list = _service.FetchJobDefinitions(model.Ordering);

                return DataTableResult(list as IList);
            }
            return View(model);
        }

        public ActionResult ViewDetail()
        {
            JobDefinitionModel model = new JobDefinitionModel();

            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                int jobDefId = int.Parse(Request.QueryString["id"]);
                model = Map<JobDefinitionModel>(_service.GetById(jobDefId));
            }
            return View(model);
        }

        public ActionResult Edit()
        {
            JobDefinitionModel model;

            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                model = new JobDefinitionModel();
                model.JobTypeSelectList = new MultiSelectList(_service.FindJobTypes());

                return View(model);
            }
            int jobDefId = int.Parse(Request.QueryString["id"]);
            JobDefinition jobDef = _service.GetById(jobDefId);

            if (jobDef == null)
                ModelState.AddModelError(string.Empty, ResMan.GetString("JobDefinition_NotExists"));
            model = base.Map<JobDefinitionModel>(jobDef);

            model.JobTypeSelectList = new MultiSelectList(_service.FindJobTypes());
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(JobDefinitionModel model)
        {
            if (!ModelState.IsValid)
            {
                model.JobTypeSelectList = new MultiSelectList(_service.FindJobTypes());
                return View(model);
            }
            try
            {
                bool isDemo = false;

                if (_settingManager.GetContainer<AppSetting>().Contains("IsDemo"))
                {
                    bool.TryParse(_settingManager.GetContainer<AppSetting>()["IsDemo"].Value, out isDemo);
                }

                if (isDemo)
                    throw new ASException(ResMan.GetString("ErrorMessage_UnableToUpdate"));

                if (model.Id > 0)
                {
                    JobDefinition dbJobDef = _service.GetById(model.Id);
                    model.CreatedBy = dbJobDef.CreatedBy;
                    model.CreatedOn = dbJobDef.CreatedOn;
                    model.ModifiedBy = dbJobDef.ModifiedBy;
                    model.ModifiedOn = dbJobDef.ModifiedOn;
                    JobDefinition tempJobDef = _service.GetByName(model.Name);

                    if (tempJobDef.Id != model.Id)
                        throw new ASException(ResMan.GetString("JobDefinition_NameExists"));

                    _service.UpdateJobDefinition(Map<JobDefinition>(model));
                    TempData["ResultType"] = MessageType.Success;
                    TempData["ResultMessage"] = string.Format(ResMan.GetString("JobDefinition_UpdateSuccess"),
                        model.Name);

                    return RedirectToAction("Result", "Shared");
                }
                else
                {
                    if (_service.GetByName(model.Name) != null)
                        throw new ASException(ResMan.GetString("JobDefinition_NameExists"));
                    _service.CreateJobDefinition(Map<JobDefinition>(model));
                    TempData["ResultType"] = MessageType.Success;
                    TempData["ResultMessage"] = string.Format(ResMan.GetString("JobDefinition_NewSucess"),
                        model.Name);

                    return RedirectToAction("Result", "Shared");
                }
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            model.JobTypeSelectList = new MultiSelectList(_service.FindJobTypes());
            return View(model);
        }

        public ActionResult Run()
        {
            _service.Run(Request.QueryString["name"]);

            return View(new JobDefinitionModel());
        }
    }
}