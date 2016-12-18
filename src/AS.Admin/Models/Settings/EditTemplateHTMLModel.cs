using AS.Infrastructure.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AS.Admin.Models
{
    public class EditTemplateHTMLModel : ASModelBase
    {
        public int Id { get; set; }
        public string BodyFilePath { get; set; }

        [DataType(DataType.Html)]
        [AllowHtml]
        public string Body { get; set; }
    }
}