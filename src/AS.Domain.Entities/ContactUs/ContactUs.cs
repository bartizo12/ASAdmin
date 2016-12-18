using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Contact Us entity to be used store "Contact Us" messages.
    /// </summary>
    [Serializable]
    public class ContactUs : EntityBase<int>
    {
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string FullName { get; set; }
        public string IPAddress { get; set; }
        public string Country { get; set; }
    }
}