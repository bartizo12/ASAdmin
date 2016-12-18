using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : ASControllerBase
    {
        private readonly IMembershipService _service;
        private readonly IResourceManager _resourceManager;

        public UsersController(IMembershipService service,
            IResourceManager resourceManager)
        {
            this._service = service;
            this._resourceManager = resourceManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(UserListModel model)
        {
            if (Request.IsAjaxRequest())
            {
                IPagedList<IUser> list = _service.GetUsers(model.PageIndex, model.PageSize, model.Ordering,
                    model.LastActivityFrom,
                    model.LastActivityTo, model.UserName, model.EMail);

                List<UserModel> modelList = new List<UserModel>();
                IList<IRole> roleList = _service.GetRoles(string.Empty);

                foreach (IUser user in list)
                {
                    UserModel uModel = new UserModel(user);
                    uModel.Roles = roleList.Where(r => user.RoleIds.Contains(r.Id)).Select(r => r.Name).ToList();
                    modelList.Add(uModel);
                }

                return DataTableResult(modelList, list.TotalCount, model.Draw);
            }
            return View(model);
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            _service.DeleteUser(id);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit()
        {
            UserModel model = null;
            model = new UserModel();
            model.RoleSelectList = new MultiSelectList(_service.GetRoles(string.Empty), "Name", "Name");

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserModel model)
        {
            model.RoleSelectList = new MultiSelectList(_service.GetRoles(string.Empty), "Name", "Name");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                _service.CreateUser(model.UserName, model.Password, model.Email, model.Roles);
                TempData["ResultType"] = MessageType.Success;
                TempData["ResultMessage"] = string.Format(ResMan.GetString("Users_CreateSuccess"),
                    model.UserName);
                return RedirectToAction("Result", "Shared");
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetUserPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }
            try
            {
                bool result = _service.ResetPasswordWithoutToken(model.UserName, model.NewPassword);

                if (result)
                {
                    model = new ResetUserPasswordModel();
                }
                model.Success = result;
            }
            catch (ASException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return PartialView(model);
        }

        public ActionResult ViewDetail(UserViewDetailModel model)
        {
            if (!Request.IsAjaxRequest())
            {
                IUser user = _service.GetUserByUsername(Request.QueryString["userName"]);
                UserModel userModel = new UserModel(user);
                IList<IRole> roleList = _service.GetRoles(string.Empty);
                userModel.Roles = roleList.Where(r => user.RoleIds.Contains(r.Id)).Select(r => r.Name).ToList();

                return View(userModel);
            }
            IPagedList<UserActivity> list = _service.GetUserActivities(model.PageIndex, model.PageSize, model.UserId);
            List<UserActivityModel> modelList = new List<UserActivityModel>();

            foreach (UserActivity ua in list)
            {
                UserActivityModel uaModel = new UserActivityModel();
                uaModel.CreatedOn = ua.CreatedOn;
                uaModel.Activity = _resourceManager.GetString("UserActivityType_" + ua.UserActivityType.ToString());

                modelList.Add(uaModel);
            }
            return DataTableResult(modelList, list.TotalCount, model.Draw);
        }
    }
}