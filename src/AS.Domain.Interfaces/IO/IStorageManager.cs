using System.Collections.Generic;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface to manage storage of objects in file.
    /// </summary>
    /// <typeparam name="T">Generic class type</typeparam>
    public interface IStorageManager<T> where T : class, new()
    {
        /// <summary>
        /// Checks if file exists
        /// </summary>
        /// <returns>True if file exists, False if not</returns>
        bool CheckIfExists();

        /// <summary>
        /// Saves <typeparamref name="items"/> to file.
        /// </summary>
        /// <param name="items">Items to be saved</param>
        void Save(List<T> items);

        /// <summary>
        /// Saves object to file/storage
        /// </summary>
        /// <param name="item">Object to be saved</param>
        void Save(T item);

        /// <summary>
        /// Reads List of objects from  file
        /// </summary>
        /// <returns></returns>
        List<T> Read();
    }
}