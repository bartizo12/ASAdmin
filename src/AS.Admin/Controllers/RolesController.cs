using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : ASControllerBase
    {
        private readonly IMembershipService _service;
        private readonly IResourceManager _resourceManager;

        public RolesController(IMembershipService service,
            IResourceManager resourceManager)
        {
            this._service = service;
            this._resourceManager = resourceManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(DataTableModel model)
        {
            if (Request.IsAjaxRequest())
            {
                IList list = _service.GetRoles(model.Ordering) as IList;

                return DataTableResult(list);
            }
            return View(model);
        }

        public ActionResult Edit()
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
                return View(new RoleModel());

            string roleName = Request.QueryString["id"];
            IRole role = _service.GetRoleByName(roleName);

            if (role == null)
                ModelState.AddModelError(string.Empty, this._resourceManager.GetString("Roles_NotExists"));

            RoleModel roleModel = base.Map<RoleModel>(role);

            return View(roleModel);
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            _service.DeleteRole(id);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(RoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                TempData["ResultModel"] = model;
                if (model.Id > 0)
                {
                    IRole tempRole = _service.GetRoleByName(model.Name);

                    if (tempRole != null && tempRole.Id != model.Id)
                    {
                        ModelState.AddModelError("Name",
                           string.Format(this._resourceManager.GetString("Roles_Exists"), model.Name));
                    }
                    else
                    {
                        _service.UpdateRole(model.Id, model.Name, model.Note);
                        TempData["ResultType"] = MessageType.Success;
                        TempData["ResultMessage"] = string.Format(this._resourceManager.GetString("Roles_UpdateSuccess"),
                            model.Name);
                        
                        return RedirectToAction("Result", "Shared");
                    }
                }
                else
                {
                    if (_service.GetRoleByName(model.Name) != null)
                    {
                        ModelState.AddModelError("Name",
                            string.Format(this._resourceManager.GetString("Roles_Exists"), model.Name));
                    }
                    else
                    {
                        _service.CreateRole(model.Name, model.Note);
                        TempData["ResultType"] = MessageType.Success;
                        TempData["ResultMessage"] = string.Format(this._resourceManager.GetString("Roles_NewRoleSuccess"),
                            model.Name);
                        
                        return RedirectToAction("Result", "Shared");
                    }
                }
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }
    }
}