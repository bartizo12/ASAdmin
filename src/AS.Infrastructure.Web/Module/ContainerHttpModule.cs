using AS.Infrastructure.Web.Module;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

[assembly: PreApplicationStartMethod(typeof(ContainerHttpModule), "Start")]

namespace AS.Infrastructure.Web.Module
{
    /// <summary>
    /// Registers modules and allows DI container to hook into creation of that one module.
    /// Taken from : http://haacked.com/archive/2011/06/03/dependency-injection-with-asp-net-httpmodules.aspx/
    /// </summary>
    public class ContainerHttpModule : IHttpModule
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(ContainerHttpModule));
        }

        private Lazy<IEnumerable<IHttpModule>> _modules = new Lazy<IEnumerable<IHttpModule>>(RetrieveModules);

        private static IEnumerable<IHttpModule> RetrieveModules()
        {
            return DependencyResolver.Current.GetServices<IHttpModule>();
        }

        public void Dispose()
        {
            var modules = _modules.Value;
            foreach (var module in modules)
            {
                var disposableModule = module as IDisposable;
                if (disposableModule != null)
                {
                    disposableModule.Dispose();
                }
            }
        }

        public void Init(HttpApplication context)
        {
            var modules = _modules.Value;
            foreach (var module in modules)
            {
                module.Init(context);
            }
        }
    }
}