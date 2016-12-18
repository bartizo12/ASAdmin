using AS.Domain.Interfaces;
using AS.Services.Interfaces;
using System.Threading;

namespace AS.Jobs
{
    /// <summary>
    /// Async Job  that sends pending e-mails via SMTP server(whatever is set)
    /// </summary>
    public class MailSendingJob : JobBase
    {
        private static readonly object _lockObj = new object();

        private readonly IMailService _mailService;

        public MailSendingJob(IMailService mailService, ILogger logger)
            : base(logger)
        {
            this._mailService = mailService;
        }

        protected override void OnExecute()
        {
            if (Monitor.TryEnter(_lockObj))
            {
                try
                {
                    this._mailService.SendPendingEMails();
                }
                finally
                {
                    Monitor.Exit(_lockObj);
                }
            }
        }
    }
}