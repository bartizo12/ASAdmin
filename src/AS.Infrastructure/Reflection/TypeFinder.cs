using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AS.Infrastructure.Reflection
{
    /// <summary>
    /// Iterates through all filtered assemblies and finds types.
    /// Note that, default assembly name filter is "^AS." , which means only assemblies whose name starts with
    /// "AS." will be seeked.
    /// Implementation is taken from nopCommerce and simplified.
    /// https://github.com/nopSolutions/nopCommerce/blob/075a2a57fdb53cac5599db2cdea8d64fce578ba4/src/Libraries/Nop.Core/Infrastructure/WebAppTypeFinder.cs
    /// </summary>
    public class TypeFinder : ITypeFinder
    {
        private bool _areAssembliesLoaded = false;
        public string AssemblyFilter { get; set; }

        protected virtual string Dir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public TypeFinder()
        {
            this.AssemblyFilter = "^AS."; //Default filter
        }

        /// <summary>
        ///  Searches the types that are assignable to T type
        /// </summary>
        /// <typeparam name="T">Generic type to be searched</typeparam>
        /// <param name="onlyConcreteClasses">If  True,  only concerete classes will be returned  otherwise
        /// all classes that are assignable to <paramref name="type"/> will be returned </param>
        /// <returns>List of the found types</returns>
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        /// <summary>
        /// Returns the liste of assemblies in bin folder
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            if (!_areAssembliesLoaded)
            {
                foreach (string dllPath in Directory.GetFiles(this.Dir, "*.dll"))
                {
                    var assemblyName = AssemblyName.GetAssemblyName(dllPath);

                    if (Regex.IsMatch(assemblyName.Name, this.AssemblyFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled))
                    {
                        AppDomain.CurrentDomain.Load(assemblyName);
                    }
                }
                _areAssembliesLoaded = true;
            }
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Searches the types that are assignable to <paramref name="type"/>
        /// </summary>
        /// <param name="type">Type to be searched</param>
        /// <param name="onlyConcreteClasses">If  True,  only concerete classes will be returned  otherwise
        /// all classes that are assignable to <paramref name="type"/> will be returned </param>
        /// <returns>List of the found types</returns>
        public IEnumerable<Type> FindClassesOfType(Type type, bool onlyConcreteClasses = true)
        {
            List<Type> typeList = new List<Type>();

            foreach (Assembly assembly in this.GetAssemblies())
            {
                if (!Regex.IsMatch(assembly.FullName, this.AssemblyFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled))
                {
                    continue;
                }
                typeList.AddRange(assembly.GetTypes()
                    .Where(aType => !aType.IsInterface &&
                    ((aType.IsClass && onlyConcreteClasses) || !aType.IsClass)
                    && !aType.IsAbstract && (type.IsAssignableFrom(aType)
                    || DoesTypeImplementOpenGeneric(aType, type))));
            }

            return typeList;
        }

        /// <summary>
        /// Checks if <paramref name="type"/> implements  <paramref name="openGeneric"/>.
        /// Kind of a helper function for generic type checking.
        /// </summary>
        /// <param name="type">Type to be checked</param>
        /// <param name="openGeneric">Generic Type To Be Matched</param>
        /// <returns>True if <paramref name="type"/> implements <paramref name="openGeneric"/></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            if (!openGeneric.IsGenericTypeDefinition)
                return false;
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                for (Type tempType = type.BaseType; tempType != null; tempType = tempType.BaseType)
                {
                    if (tempType.IsGenericType && tempType.GetGenericTypeDefinition() == openGeneric)
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}