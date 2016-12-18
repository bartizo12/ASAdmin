using AS.Domain.Entities;
using AS.Infrastructure.Web.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    public class SettingsListModel : DataTableModel
    {
        public int? SettingDefId { get; set; }
        public IEnumerable<SelectListItem> SettingDefinitionSelectList { get; set; }
        public IEnumerable<SettingDefinition> SettingDefinitions { get; set; }
    }
}