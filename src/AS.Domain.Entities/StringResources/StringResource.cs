using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// String resource to store string definitions in the application.
    /// </summary>
    [Serializable]
    public class StringResource : TrackableEntityBase<int>
    {
        /// <summary>
        /// Culture code e.g ;  en-US
        /// </summary>
        public string CultureCode { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }

        /// <summary>
        /// True : String resource will be avaliable on client side. Means, in javascript, we will be able
        /// to get this resource from as a variable on p without sending an extra request to server.
        /// False : String resource will only be avaliable on server side.
        /// </summary>
        public bool AvailableOnClientSide { get; set; }
    }
}