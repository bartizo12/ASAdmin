using AS.Infrastructure.Web.Mvc;
using System.Collections.Generic;

namespace AS.Admin.Models
{
    public class NotificationListModel : ASModelBase
    {
        public IList<NotificationModel> Notifications { get; set; }
    }
}
