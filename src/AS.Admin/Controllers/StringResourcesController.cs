using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System;
using System.Collections;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StringResourcesController : ASControllerBase
    {
        private readonly IResourceService _resourceService;
        private readonly IResourceManager _resourceManager;

        public StringResourcesController(IResourceService resourceService,
            IResourceManager resourceManager)
        {
            this._resourceService = resourceService;
            this._resourceManager = resourceManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(StringResourceListModel model)
        {
            if (Request.IsAjaxRequest())
            {
                IPagedList<StringResource> list = _resourceService.GetStringResources(model.PageIndex,
                    model.PageSize, model.Ordering, model.CultureCode, model.NameOrValue);

                return DataTableResult(list as IList, list.TotalCount, model.Draw);
            }
            model.CultureCodeList = new MultiSelectList(_resourceService.FetchAvailableCultureList());
            return View(model);
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            _resourceService.DeleteById(id);
            _resourceManager.ClearCache();
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit()
        {
            int id;
            StringResourceModel model = new StringResourceModel();

            if (!int.TryParse(Request.QueryString["id"], out id))
            {
                model.CultureCodeList = new MultiSelectList(_resourceService.FetchCultureList());
                return View(model);
            }
            var stringResource = _resourceService.GetResourceById(id);

            if (stringResource == null)
            {
                ModelState.AddModelError(string.Empty, ResMan.GetString("StringResource_NotExists"));
            }
            else
            {
                model = base.Map<StringResourceModel>(stringResource);
                model.CultureCodeList = new MultiSelectList(_resourceService.FetchCultureList());
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(StringResourceModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CultureCodeList = new MultiSelectList(_resourceService.FetchCultureList());
                return View(model);
            }
            try
            {
                if (model.Id > 0)
                {
                    StringResource tempStringResource = _resourceService
                        .GetResourceByNameAndCulture(model.CultureCode, model.Name);

                    if (tempStringResource != null && tempStringResource.Id != model.Id)
                    {
                        ModelState.AddModelError("Name",
                           string.Format(ResMan.GetString("StringResource_Exists"), model.Name));
                    }
                    else
                    {
                        _resourceService.Update(this.Map<StringResource>(model));
                        _resourceManager.ClearCache();
                        TempData["ResultType"] = MessageType.Success;
                        TempData["ResultMessage"] = string.Format(ResMan.GetString("StringResources_SaveSuccess"),
                            model.Name, model.CultureCode);
                        TempData["ResultModel"] = model;
                        return RedirectToAction("Result", "Shared");
                    }
                }
                else
                {
                    if (_resourceService
                        .GetResourceByNameAndCulture(model.CultureCode, model.Name) != null)
                    {
                        ModelState.AddModelError("Name",
                            string.Format(ResMan.GetString("StringResource_Exists"),
                            model.Name, model.CultureCode));
                    }
                    else
                    {
                        _resourceService.Insert(Map<StringResource>(model));
                        _resourceManager.ClearCache();
                        TempData["ResultType"] = MessageType.Success;
                        TempData["ResultMessage"] = string.Format(ResMan.GetString("StringResources_SaveSuccess"),
                            model.Name, model.CultureCode);
                        TempData["ResultModel"] = model;
                        return RedirectToAction("Result", "Shared");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            finally
            {
                model.CultureCodeList = new MultiSelectList(_resourceService.FetchCultureList());
            }
            return View(model);
        }
    }
}