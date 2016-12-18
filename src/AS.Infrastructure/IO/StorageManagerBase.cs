using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace AS.Infrastructure.IO
{
    /// <summary>
    /// Base storage manager, that stores generic objects.
    /// Serializes object(s) to XML and saves to file. Reads and deserializes back to object list.
    /// </summary>
    /// <typeparam name="T">Generic class</typeparam>
    public abstract class StorageManagerBase<T> : IStorageManager<T> where T : class, new()
    {
        private readonly string FilePath;
        private readonly IXmlSerializer _xmlSerializer;
        private readonly IAppManager _appManager;
        private List<T> _data = null;
        private DateTime lastModifiedTime;

        public StorageManagerBase(IXmlSerializer xmlSerializer,
           IAppManager appManager, string fileName)
        {
            this._xmlSerializer = xmlSerializer;
            this._appManager = appManager;
            FilePath = _appManager.MapPhysicalFile(fileName);
        }

        protected bool EnableCache { private get; set; }

        /// <summary>
        /// Checks if file exists
        /// </summary>
        /// <returns>True if file exists, false if not</returns>
        public bool CheckIfExists()
        {
            return File.Exists(FilePath);
        }

        /// <summary>
        /// Saves object to file
        /// </summary>
        /// <param name="item"></param>
        public void Save(T item)
        {
            List<T> list = new List<T>();
            list.Add(item);
            Save(list);
        }

        /// <summary>
        /// Saves objects to file
        /// </summary>
        /// <param name="items">Objects to ve saved</param>
        public void Save(List<T> items)
        {
            File.WriteAllText(FilePath, _xmlSerializer.SerializeToXML(items));
        }

        /// <summary>
        /// Reads from file
        /// </summary>
        /// <returns>List of read objects</returns>
        public List<T> Read()
        {
            if (!File.Exists(FilePath))
                return new List<T>();
            if (!EnableCache)
            {
                return _xmlSerializer.DeserializeFromXML<List<T>>(File.ReadAllText(FilePath));
            }
            if (_data != null && lastModifiedTime == File.GetLastWriteTimeUtc(FilePath))
                return _data;
            _data = _xmlSerializer.DeserializeFromXML<List<T>>(File.ReadAllText(FilePath));
            lastModifiedTime = File.GetLastWriteTimeUtc(FilePath);
            return _data;
        }
    }
}