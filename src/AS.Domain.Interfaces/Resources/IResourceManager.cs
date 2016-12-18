using AS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface to access string resources
    /// </summary>
    public interface IResourceManager : IDisposable
    {
        /// <summary>
        /// Check if string resource exists
        /// </summary>
        /// <param name="name">String resource to be checked</param>
        /// <returns>True if exists, false if not</returns>
        bool Exists(string name);

        /// <summary>
        /// Gets string by name
        /// </summary>
        /// <param name="name">Name of the resources string</param>
        /// <returns>Value of the resource string</returns>
        string GetString(string name);

        /// <summary>
        /// Returns IEnumerable  interface to all string resources
        /// </summary>
        /// <returns>StringResources</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
        Justification = "This method involves time-consuming operations", Scope = "member")]
        IEnumerable<StringResource> GetStringResources();

        /// <summary>
        /// Clears resource memmory cache
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Returns list of the cultures avaliable on the system
        /// </summary>
        /// <returns>Returns list of the cultures avaliable on the system</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
        Justification = "This method involves time-consuming operations", Scope = "member")]
        IList<string> GetAvailableCultureList();
    }
}