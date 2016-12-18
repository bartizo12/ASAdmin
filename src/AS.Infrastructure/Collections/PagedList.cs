using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.Infrastructure.Collections
{
    /// <summary>
    /// Paged List
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    [Serializable]
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            this.TotalCount = source.Count();
            this.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Total item count in the source ,before paginated
        /// </summary>
        public int TotalCount { get; private set; }
    }
}