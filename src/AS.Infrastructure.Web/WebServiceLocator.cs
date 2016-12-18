using AS.Domain.Interfaces;
using AS.Infrastructure.Web.Reflection;
using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AS.Infrastructure.Web
{
    /// <summary>
    /// Service locator(Dependeny Resolver) for web applications.
    /// Must be created only one instance(singleton) at runtime, and set it
    /// as default dependency resolver.
    /// </summary>
    public class WebServiceLocator : IServiceLocator
    {
        private readonly IContainer _container;

        public WebServiceLocator()
        {
            ContainerBuilder builder = new ContainerBuilder();
            var typeFinder = new WebTypeFinder();
            builder.RegisterInstance(typeFinder)
                .As<ITypeFinder>()
                .SingleInstance();
            builder.RegisterAssemblyModules(typeFinder.GetAssemblies().ToArray());
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            JobFactory.SetResolver(this);
        }

        public object Resolve(Type serviceType)
        {
            return this.Scope().Resolve(serviceType);
        }

        public T Resolve<T>()
        {
            return this.Scope().Resolve<T>();
        }

        public bool IsRegistered<T>()
        {
            return this.Scope().IsRegistered<T>();
        }

        private ILifetimeScope Scope()
        {
            try
            {
                if (HttpContext.Current != null)
                    return AutofacDependencyResolver.Current.RequestLifetimeScope;

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return _container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
            catch (Exception)
            {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return _container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _container.Dispose();
            }
            _disposed = true;
        }

        #endregion IDisposable
    }
}