using AS.Infrastructure.Web.Mvc;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    public class ApplicationSettingsModel : ASModelBase
    {
        public string ApplicationDefaultTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int RecaptchaDisplayCount { get; set; }
        public bool DbQueryLogEnable { get; set; }
        public bool BundlingEnabled { get; set; }
        public bool RequestLoggingEnabled { get; set; }
        public string MinLogLevel { get; set; }
        public MultiSelectList LogLevels { get; set; }
    }
}