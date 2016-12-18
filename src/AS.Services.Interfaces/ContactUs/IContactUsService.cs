using AS.Domain.Entities;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Service interface for ContactUs Page
    /// </summary>
    public interface IContactUsService : IService
    {
        /// <summary>
        /// Sends contact us message to related receivers
        /// </summary>
        /// <param name="contactUs">ContactUs message</param>
        void Send(ContactUs contactUs);
    }
}