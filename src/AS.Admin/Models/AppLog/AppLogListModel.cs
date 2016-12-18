using AS.Infrastructure.Web.Mvc;
using System;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    [ModelBinder(typeof(AppLogListModelBinder))]
    public class AppLogListModel : DataTableModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public string[] SelectedLogLevels { get; set; }
        public MultiSelectList LogLevels { get; set; }

        public AppLogListModel()
        {
            this.SelectedLogLevels = Infrastructure.Logging.LogLevels.All.ToArray();
            LogLevels = new MultiSelectList(Infrastructure.Logging.LogLevels.All);
        }
    }
}