using AS.Infrastructure.Web.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace AS.Admin.Models
{
    public class EMailModel : ASModelBase
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Subject { get; set; }

        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        public string Receivers { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public int TryCount { get; set; }
        public int JobStatus { get; set; }
        public DateTime? LastExecutionTime { get; set; }

        [DataType(DataType.MultilineText)]
        public string ErrorMessage { get; set; }

        public string EmailSettingName { get; set; }
        public string SmtpHostAddress { get; set; }
        public int SmtpPort { get; set; }
        public int SmtpClientTimeOut { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public bool SmtpUseDefaultCredentials { get; set; }
        public string SmtpUserName { get; set; }
    }
}