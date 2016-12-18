using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface to find request types in entire application dynamically.
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// Searches the types that are assignable to <paramref name="type"/>
        /// </summary>
        /// <param name="type">Type to be searched</param>
        /// <param name="onlyConcreteClasses">If  True,  only concerete classes will be returned  otherwise
        /// all classes that are assignable to <paramref name="type"/> will be returned </param>
        /// <returns>List of the found types</returns>
        IEnumerable<Type> FindClassesOfType(Type type, bool onlyConcreteClasses = true);

        /// <summary>
        ///  Searches the types that are assignable to T type
        /// </summary>
        /// <typeparam name="T">Generic type to be searched</typeparam>
        /// <param name="onlyConcreteClasses">If  True,  only concerete classes will be returned  otherwise
        /// all classes that are assignable to <paramref name="type"/> will be returned </param>
        /// <returns>List of the found types</returns>
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        /// <summary>
        /// Returns the liste of assemblies in bin folder
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
        Justification = "This method involves time-consuming operations", Scope = "member")]
        IEnumerable<Assembly> GetAssemblies();
    }
}