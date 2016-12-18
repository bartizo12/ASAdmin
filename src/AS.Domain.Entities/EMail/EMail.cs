using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// E-mail entity to keep e-mails to be sent by the application.
    /// </summary>
    [Serializable]
    public class EMail : EntityBase<int>
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// Multiple receivers shall be seperated with ';'
        /// </summary>
        public string Receivers { get; set; }

        /// <summary>
        /// e.g info@asadmindemo.com
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// e.g AS.Admin
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Number of trial count to send an e-mail. Sometimes an e-mail cannot be send for some
        /// network or SMTP setting problem ,in that case we shall try to resend it again. In that purposes
        /// we keep track of the trial counts.
        /// </summary>
        public int TryCount { get; set; }

        /// <summary>
        /// Status of the e-mail
        /// </summary>
        public JobStatus JobStatus { get; set; }

        /// <summary>
        /// Last execution/sent time ( failed or succeded) of the e-mail
        /// </summary>
        public DateTime? LastExecutionTime { get; set; }

        /// <summary>
        /// If any exception occured
        /// </summary>
        public string ErrorMessage { get; set; }

        public string EmailSettingName { get; set; }
        public string SmtpHostAddress { get; set; }
        public int SmtpPort { get; set; }
        public int SmtpClientTimeOut { get; set; }//In miliseconds
        public bool SmtpEnableSsl { get; set; }
        public bool SmtpUseDefaultCredentials { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; } //Encrypted

        public EMail()
        {
            this.TryCount = 0;
            this.JobStatus = JobStatus.Queued;
        }
    }
}