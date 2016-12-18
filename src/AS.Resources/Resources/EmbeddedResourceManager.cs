using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace AS.Resources
{
    public class EmbeddedResourceManager : IResourceManager
    {
        private readonly ResourceSet _resourceSet;

        public EmbeddedResourceManager()
        {
            _resourceSet = new ResourceManager(typeof(Resources))
                    .GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        }
        public void ClearCache()
        {

        }
        public string GetString(string name)
        {
            return _resourceSet.GetString(name);
        }
        public IEnumerable<StringResource> GetStringResources()
        {
            List<StringResource> list = new List<StringResource>();


            for (var resEnum = _resourceSet.GetEnumerator(); resEnum.MoveNext();)
            {
                StringResource stringResource = new StringResource();
                stringResource.CultureCode = CultureInfo.CurrentUICulture.Name;
                stringResource.Value = resEnum.Value.ToString().Replace("\r\n", "").Replace("\"", "\\\"");
                stringResource.AvaliableOnClientSide = resEnum.Key.ToString().StartsWith("UI_");

                list.Add(stringResource);
            }

            return list;
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
                if(this._resourceSet != null)
                {
                    this._resourceSet.Dispose();
                }
            }
            _disposed = true;
        }
        #endregion
    }
}
