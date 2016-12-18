using AS.Domain.Interfaces;
using System.Web.Mvc;

namespace AS.Infrastructure.Web
{
    /// <summary>
    /// Static Resource Manager. Implemented to have a global Getter to be used in whole application
    /// Makes it easy for views especially.
    /// </summary>
    public static class ResMan
    {
        private static IResourceManager RM
        {
            get { return DependencyResolver.Current.GetService<IResourceManager>(); }
        }

        /// <summary>
        /// Gets string by name
        /// </summary>
        /// <param name="name">Name of the resources string</param>
        /// <returns>Value of the resource string</returns>
        public static string GetString(string name)
        {
            return RM.GetString(name);
        }

        /// <summary>
        /// Check if string resource exists
        /// </summary>
        /// <param name="name">String resource to be checked</param>
        /// <returns>True if exists, false if not</returns>
        public static bool Exists(string name)
        {
            return RM.Exists(name);
        }
    }
}