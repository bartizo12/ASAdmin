using System.Collections.Generic;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Instead of using ViewBag in views, better use this class to store common view data
    /// such as variables to be used in  HTML Header
    /// (Do not use ViewBag unless you really really need to)
    /// </summary>
    public class ViewModelHeader
    {
        public string Title { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public bool IsDemo { get; set; }
        public Dictionary<string, string> ClientResources { get; set; }
        public bool PopupPage { get; set; }
        public bool DisplayDisabledOnDemoMessage { get; set; }
        public string Icon { get; set; }
        public List<SelectListItem> LanguageList { get; set; }
    }
}