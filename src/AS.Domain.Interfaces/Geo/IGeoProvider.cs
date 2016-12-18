using AS.Domain.Entities;
using System.Diagnostics.Contracts;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface for the class that provides geographical functions.
    /// e.g  ; Getting country info of the IP address
    /// </summary>
    [ContractClass(typeof(GeoProviderContract))]
    public interface IGeoProvider
    {
        /// <summary>
        /// Returns country info of the ip address
        /// </summary>
        /// <param name="ipAddress">IP address to be queried</param>
        /// <returns>Found country info as <seealso cref="Country"/> object</returns>
        Country GetCountryInfo(string ipAddress);
    }
}