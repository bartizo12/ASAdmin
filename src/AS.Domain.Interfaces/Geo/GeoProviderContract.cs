using AS.Domain.Entities;
using System;
using System.Diagnostics.Contracts;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Contract class for <seealso cref="IGeoProvider"/> interface
    /// </summary>
    [ContractClassFor(typeof(IGeoProvider))]
    internal abstract class GeoProviderContract : IGeoProvider
    {
        public Country GetCountryInfo(string ipAddress)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(ipAddress), "IPAddress cannot be null or empty.");

            return default(Country);
        }
    }
}