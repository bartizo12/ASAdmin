using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Service interface for string resource management
    /// </summary>
    public interface IResourceService : IService
    {
        /// <summary>
        /// Adds/Inserts new string resource definition
        /// </summary>
        /// <param name="resource">String Resource to be inserted</param>
        void Insert(StringResource resource);

        /// <summary>
        /// Updates string resource
        /// </summary>
        /// <param name="resource">Modified string resource entity</param>
        void Update(StringResource resource);

        /// <summary>
        /// Deletes string resource by its ID
        /// </summary>
        /// <param name="id">ID of the string resource to be deleted</param>
        void DeleteById(int id);

        /// <summary>
        /// Gets all string resource definitions from db
        /// </summary>
        /// <returns>All string resources</returns>
        IList<StringResource> FetchAll();

        /// <summary>
        /// Gets string resource by  ID
        /// </summary>
        /// <param name="id">ID of the string resource to be fetched</param>
        /// <returns>Found string resource record or null if not found</returns>
        StringResource GetResourceById(int id);

        /// <summary>
        /// Gets string resource by cultureCode and name
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="name"></param>
        /// <returns>Found string resource record or null if not found</returns>
        StringResource GetResourceByNameAndCulture(string cultureCode, string name);

        /// <summary>
        /// Gets list of string resources from the datebase. Filtering, ORdering and pagination is applied
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="ordering">OrderBy value</param>
        /// <param name="cultureCode">Culture code to be filtered</param>
        /// <param name="nameOrValue">Name or value to be searched.</param>
        /// <returns>Paged list of string resources</returns>
        IPagedList<StringResource> GetStringResources(int pageIndex, int pageSize, string ordering, string cultureCode, string nameOrValue);

        /// <summary>
        /// Returns list of the system cultures
        /// </summary>
        /// <returns>Returns list of the cultures  on the system</returns>
        IList<string> FetchCultureList();

        /// <summary>
        /// Returns list of the cultures avaliable in the database
        /// </summary>
        /// <returns>Returns list of the cultures in database</returns>
        IList<string> FetchAvailableCultureList();
    }
}