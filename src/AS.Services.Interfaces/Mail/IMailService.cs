using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Provides all e-mail related functions
    /// </summary>
    public interface IMailService : IService
    {
        /// <summary>
        /// Enqueue e-mail into the queue
        /// </summary>
        /// <param name="mail">E-Mail to be enqueued</param>
        void Enqueue(EMail mail);

        /// <summary>
        /// Sends pending e-mails
        /// </summary>
        void SendPendingEMails();

        /// <summary>
        /// Gets e-mail by Id
        /// </summary>
        /// <param name="id">Id of the e-mail</param>
        /// <returns>Found e-mail or null</returns>
        EMail GetById(int id);

        /// <summary>
        /// Sends e-mail to the receiver(s). Multiple receivers shall be seperated with ';'
        /// </summary>
        /// <param name="email">E-mail to be sent</param>
        void SendEMail(EMail email);

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
        IPagedList<EMail> GetMails(int pageIndex, int pageSize, string ordering, DateTime from, DateTime to, string receiver = "");
    }
}