using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EMailController : ASControllerBase
    {
        private readonly IMailService _service;
        private readonly ISettingManager _settingManager;

        public EMailController(IMailService service,ISettingManager settingManager)
        {
            this._service = service;
            this._settingManager = settingManager;
        }

        private bool IsDemo
        {
            get
            {
                return _settingManager.GetContainer<AppSetting>().Contains("IsDemo") && 
                    bool.Parse(_settingManager.GetContainer<AppSetting>()["IsDemo"].Value);
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(EMailListModel model)
        {
            if (Request.IsAjaxRequest())
            {
                IPagedList<EMail> list = _service.GetMails(model.PageIndex, model.PageSize, model.Ordering, model.From.Value,
                                                           model.To.Value, model.Receiver);
                List<EMailModel> modelList = new List<EMailModel>();

                foreach (EMail email in list)
                {
                    EMailModel emailModel = Map<EMailModel>(email);

                    if (this.IsDemo)
                    {
                        //E-Mail Addresses are masked on Demo site for security reasons
                        //Eventhough ,they are random randomly generated , some users might send e-mails
                        //to themselves for test purposes
                        emailModel.Receivers = RegexHelper.MaskEmailAddress(emailModel.Receivers);
                    }
                    modelList.Add(emailModel);
                }
                return DataTableResult(modelList, list.TotalCount, model.Draw);
            }
            return View(model);
        }

        public ActionResult ViewDetail()
        {
            EMailModel model = Map<EMailModel>(_service.GetById(int.Parse(Request.QueryString["id"])));

            if (this.IsDemo)
            {
                model.Receivers = RegexHelper.MaskEmailAddress(model.Receivers);
            }
            return View(model);
        }

        public ActionResult Resend(int id)
        {
            EMail mail = _service.GetById(id);
            mail.JobStatus = JobStatus.Queued;
            mail.TryCount = 0;
            _service.Enqueue(mail);

            return new EmptyResult();
        }
    }
}