using AS.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Provides run-time context data. At Infrastructure and service layers, we better get runtime data through
    /// this interface to keep our layers clean and seperated
    /// </summary>
    public interface IContextProvider
    {
        Uri Url { get; }
        string BrowserType { get; }
        string HttpMethod { get; }
        NameValueCollection ServerVariables { get; }
        IDictionary Items { get; }
        string SessionId { get; }
        string UserName { get; }
        int UserId { get; }
        string ClientIP { get; }
        CultureInfo Culture { get; }
        Country CountryInfo { get; set; }
        string RootAddress { get; }
        short LoginAttemptCount { get; set; }
        string LanguageCode { get; set; }
    }
}