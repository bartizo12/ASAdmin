using AS.Infrastructure.Web.Mvc;
using System;

namespace AS.Admin.Models
{
    public class AppLogModel : ASModelBase
    {
        public int Id { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Location { get; set; }
        public string MachineName { get; set; }
        public string AppDomain { get; set; }
        public string LoggerName { get; set; }
        public string ClientIP { get; set; }
    }
}