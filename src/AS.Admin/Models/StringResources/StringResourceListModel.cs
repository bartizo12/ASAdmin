using AS.Infrastructure.Web.Mvc;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    public class StringResourceListModel : DataTableModel
    {
        public string CultureCode { get; set; }
        public string NameOrValue { get; set; }
        public MultiSelectList CultureCodeList { get; set; }
    }
}