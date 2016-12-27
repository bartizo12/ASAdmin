using AS.Admin.Validators;
using AS.Infrastructure;
using AS.Infrastructure.Web.Mvc;
using FluentValidation.Attributes;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    [Validator(typeof(ConfigurationModelValidator))]
    public class ConfigurationModel : ASModelBase
    {
        public string ConnectionString { get; set; }
        public string DataProvider { get; set; }
        public MultiSelectList DataProviderSelectList { get; set; }
        public EMailSettingModel EMailSetting { get; set; }
        public bool IsDemo { get; set; }

        [Optional]
        public string RecaptchaPrivateKey { get; set; }

        [Optional]
        public string RecaptchaPublicKey { get; set; }
    }
}