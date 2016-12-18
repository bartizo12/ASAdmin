using AS.Infrastructure.Web.Mvc;
using System;

namespace AS.Admin.Models
{
    public class EMailListModel : DataTableModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Receiver { get; set; }
    }
}