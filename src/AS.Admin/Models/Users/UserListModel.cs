using AS.Infrastructure.Web.Mvc;
using System;

namespace AS.Admin.Models
{
    public class UserListModel : DataTableModel
    {
        public string UserName { get; set; }
        public string EMail { get; set; }
        public DateTime? LastActivityFrom { get; set; }
        public DateTime? LastActivityTo { get; set; }
    }
}