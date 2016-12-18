using AS.Domain.Entities;
using AS.Infrastructure.Data;
using AS.Services.Interfaces;

namespace AS.Services
{
    /// <summary>
    /// ContactUs service
    /// </summary>
    public class ContactUsService : IContactUsService
    {
        private readonly IDbContext _dbContext;

        public ContactUsService(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// Sends contact us message to related receivers
        /// </summary>
        /// <param name="contactUs">ContactUs message</param>
        public void Send(ContactUs contactUs)
        {
            _dbContext.Set<ContactUs>().Add(contactUs);
            _dbContext.SaveChanges();
        }
    }
}