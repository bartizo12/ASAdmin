using System.Collections.Generic;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Paged list interface
    /// </summary>
    public interface IPagedList<T> : IList<T>
    {
        /// <summary>
        /// Total number of records in data source without pagination
        /// </summary>
        int TotalCount { get; }
    }
}