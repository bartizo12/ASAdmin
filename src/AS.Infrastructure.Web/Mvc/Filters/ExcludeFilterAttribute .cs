using System;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Code Taken From : http://blogs.microsoft.co.il/oric/2011/10/28/exclude-a-filter/
    /// Marks method/class to be excluded from a specific attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ExcludeFilterAttribute : FilterAttribute
    {
        public Type FilterType
        {
            get;
            private set;
        }

        public ExcludeFilterAttribute(Type filterType)
        {
            this.FilterType = filterType;
        }
    }
}