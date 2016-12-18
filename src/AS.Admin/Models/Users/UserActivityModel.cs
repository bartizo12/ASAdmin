using AS.Infrastructure.Web.Mvc;
using System;

namespace AS.Admin.Models
{
    public class UserActivityModel : ASModelBase
    {
        public DateTime CreatedOn { get; set; }
        public string Activity { get; set; }
    }
}