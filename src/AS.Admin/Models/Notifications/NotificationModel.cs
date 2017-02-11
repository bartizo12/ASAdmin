using AS.Infrastructure.Web.Mvc;
using System;

namespace AS.Admin.Models
{
    public class NotificationModel : ASModelBase
    {
        public string Message { get; set; }
        public string Url { get; set; }
        public bool IsSeen { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
