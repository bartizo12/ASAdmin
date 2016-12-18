using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Used to log client requests. In a web application, it is HTTP request.
    /// We keep track of HTTP request for analyzing that logs in the future.
    /// </summary>
    [Serializable]
    public class RequestLog : EntityBase<long>, ISafeToDeleteEntity
    {
        public string AbsolutePath { get; set; }
        public string QueryString { get; set; }
        public int Duration { get; set; } //In milliseconds
        public string UserAgent { get; set; }
        public string BrowserType { get; set; }
        public string HttpMethod { get; set; }
        public string ClientIP { get; set; }
        public string CountryCode { get; set; }
        public string SessionID { get; set; }
    }
}