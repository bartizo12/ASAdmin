using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Collections;
using AS.Infrastructure.Data;
using AS.Infrastructure.Validation;
using AS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AS.Services
{
    /// <summary>
    ///  Provides all e-mail related functions
    /// </summary>
    public class MailService : IMailService
    {
        private const int DefaultMaxTryCount = 10;
        private readonly IDbContext _dbContext;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly IStorageManager<Configuration> _configurationStorageManager;
        private readonly IResourceManager _resourceManager;

        public MailService(IDbContextFactory dbContextFactory,
            IEncryptionProvider encryptionProvider,
            IStorageManager<Configuration> configurationStorageManager,
            IResourceManager resourceManager,
            IDbContext dbContext)
        {
            this._dbContextFactory = dbContextFactory;
            this._encryptionProvider = encryptionProvider;
            this._configurationStorageManager = configurationStorageManager;
            this._dbContext = dbContext;
            this._resourceManager = resourceManager;
        }

        /// <summary>
        /// Enqueue e-mail into the queue
        /// </summary>
        /// <param name="mail">E-Mail to be enqueued</param>
        public void Enqueue(EMail mail)
        {
            if (mail.Id > 0)
            {
                _dbContext.Entry(mail).State = EntityState.Modified;
            }
            else
            {
                _dbContext.Set<EMail>().Add(mail);
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Sends pending e-mails. An e-mail that failed to be sent is considered as pending until it reaches
        /// max trial count
        /// </summary>
        public void SendPendingEMails()
        {
            List<Task> mailTasks = new List<Task>();

            using (IDbContext dbContext = this._dbContextFactory.Create())
            {
                foreach (EMail email in GetPendingEmailsFromQueue(dbContext).ToList())
                {
                    try
                    {
                        email.JobStatus = JobStatus.Running;
                        email.SmtpPassword = _encryptionProvider.Decrypt(email.SmtpPassword,
                            _configurationStorageManager.Read().First().SymmetricKey);
                        dbContext.Entry(email).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        this.SendEMail(email);
                        email.JobStatus = JobStatus.Finished;
                    }
                    catch (Exception ex)
                    {
                        email.JobStatus = JobStatus.Failed;
                        email.ErrorMessage = ex.Message + " : " + ex.StackTrace;
                    }
                    finally
                    {
                        email.TryCount++;
                        email.LastExecutionTime = DateTime.UtcNow;
                        email.SmtpPassword = _encryptionProvider.Decrypt(email.SmtpPassword,
                            _configurationStorageManager.Read().First().SymmetricKey);
                        dbContext.Entry(email).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Gets e-mail by Id
        /// </summary>
        /// <param name="id">Id of the e-mail</param>
        /// <returns>Found e-mail or null</returns>
        public EMail GetById(int id)
        {
            return _dbContext.Set<EMail>().SingleOrDefault(email => email.Id == id);
        }

        /// <summary>
        /// Queries e-mails
        /// </summary>
        /// <param name="pageIndex">Pagination</param>
        /// <param name="pageSize">Pagination</param>
        /// <param name="ordering">OrderBy command</param>
        /// <param name="from">Sent DateTime Interval From</param>
        /// <param name="to">Sent DateTime Interval To</param>
        /// <param name="receiver">Filter by receiver e-mail</param>
        /// <returns>Paged list of e-mails</returns>
        public IPagedList<EMail> GetMails(int pageIndex, int pageSize, string ordering, DateTime from, DateTime to, string receiver = "")
        {
            var query = this._dbContext.Set<EMail>().AsNoTracking() as IQueryable<EMail>;
            query = query.Where(email => email.CreatedOn > from && email.CreatedOn < to);
            query = query.Where(email => string.IsNullOrEmpty(receiver) || email.Receivers.Contains(receiver));

            if (!string.IsNullOrEmpty(ordering))
            {
                query = query.OrderBy(ordering);
            }

            return new PagedList<EMail>(query, pageIndex, pageSize);
        }

        private IEnumerable<EMail> GetPendingEmailsFromQueue(IDbContext dbContext)
        {
            int maxTryCount = DefaultMaxTryCount;

            return dbContext.Set<EMail>().Where(mail => (mail.JobStatus == JobStatus.Queued ||
                                            (mail.JobStatus == JobStatus.Failed &&
                                            mail.TryCount < maxTryCount)) &&
                                            mail.CreatedBy != "Installer");
        }

        /// <summary>
        /// Sends e-mail to the receiver(s). Multiple receivers shall be seperated with ';'
        /// </summary>
        /// <param name="email">E-mail to be sent</param>
        public void SendEMail(EMail email)
        {
            var validator = new EmailAddressValidator(_resourceManager);

            using (SmtpClient smtpClient = new SmtpClient(email.SmtpHostAddress, email.SmtpPort))
            {
                smtpClient.Timeout = email.SmtpClientTimeOut;
                smtpClient.EnableSsl = email.SmtpEnableSsl;
                smtpClient.Credentials = new NetworkCredential(email.SmtpUserName, email.SmtpPassword);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(email.FromAddress, email.FromName);
                mailMessage.Body = email.Body;
                mailMessage.Subject = email.Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                foreach (string to in email.Receivers.Split(','))
                {
                    if (validator.Validate(to).Succeeded)
                    {
                        mailMessage.To.Add(to);
                    }
                    else
                    {
                        throw new ASException(string.Join(".", validator.Validate(to).Errors));
                    }
                }
                Task.Factory.StartNew(() => smtpClient.Send(mailMessage))
                    .TimeoutAfter(email.SmtpClientTimeOut)
                    .Wait();
            }
        }
    }
}