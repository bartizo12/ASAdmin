using AS.Admin.Models;
using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Web.Mvc;
using AS.Services.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AS.Admin.Controllers
{
    public class NotificationsController : ASControllerBase
    {
        private const int MaxNotificationCount = 100;

        private readonly INotificationService _notificationService;
        private readonly IContextProvider _contextProvider;

        public NotificationsController(INotificationService notificationService, IContextProvider contextProvider)

        {
            this._notificationService = notificationService;
            this._contextProvider = contextProvider;
        }
        public JsonNetResult GetUnseenCount()
        {
            return new JsonNetResult(_notificationService.GetUnseenCount(this._contextProvider.UserId));
        }
        public ActionResult List(NotificationListModel model)
        {
            IList<Notification> notificationList = _notificationService.GetNotifications(this._contextProvider.UserId, MaxNotificationCount);
            model.Notifications = Map<IList<NotificationModel>>(notificationList);

            IEnumerable<int> unseenIdList = notificationList.Where(n => !n.IsSeen).Select(n => n.Id);

            if (unseenIdList != null && unseenIdList.Any())
            {
                _notificationService.UpdateAsSeen(unseenIdList);
            }

            return View(model);
        }
    }
}