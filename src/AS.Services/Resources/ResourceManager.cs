using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace AS.Services
{
    /// <summary>
    /// Resource manager class to manage string resources.
    /// Caches strings into <typeparamref name="MemmoryCache"/>
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        private const string CacheKeyName = "StringResources";
        private readonly IContextProvider _contextProvider;
        private readonly IResourceService _resourceService;

        public ResourceManager(IContextProvider contextProvider,
            IResourceService resourceService)
        {
            _contextProvider = contextProvider;
            _resourceService = resourceService;
        }

        /// <summary>
        /// Check if string resource exists
        /// </summary>
        /// <param name="name">String resource to be checked</param>
        /// <returns>True if exists, false if not</returns>
        public bool Exists(string name)
        {
            return GetDictionary().Contains(_contextProvider.LanguageCode, name);
        }

        /// <summary>
        /// Gets string by name
        /// </summary>
        /// <param name="name">Name of the resources string</param>
        /// <returns>Value of the resource string</returns>
        public string GetString(string name)
        {
            return GetDictionary().GetString(_contextProvider.LanguageCode, name);
        }

        private StringResourceDictionary GetDictionary()
        {
            StringResourceDictionary stringResources = (StringResourceDictionary)
                          MemoryCache.Default.Get(CacheKeyName);
            string langCode = _contextProvider.LanguageCode;

            if (stringResources == null)
            {
                stringResources = new StringResourceDictionary();

                foreach (StringResource stringResource in _resourceService.FetchAll())
                {
                    stringResources.Add(stringResource);
                }
                MemoryCache.Default.Add(CacheKeyName, stringResources, DateTimeOffset.MaxValue);
            }
            return stringResources;
        }

        public IEnumerable<StringResource> GetStringResources()
        {
            return GetDictionary().GetAll(_contextProvider.LanguageCode);
        }

        /// <summary>
        /// Clears resource memmory cache
        /// </summary>
        public void ClearCache()
        {
            MemoryCache.Default.Remove(CacheKeyName);
        }

        /// <summary>
        /// Returns list of the cultures avaliable on the system
        /// </summary>
        /// <returns>Returns list of the cultures avaliable on the system</returns>
        public IList<string> GetAvailableCultureList()
        {
            return GetDictionary().GetAvaliableCultures().ToList();
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //Dispose objects here if any
            }
            _disposed = true;
        }

        #endregion IDisposable
    }
}