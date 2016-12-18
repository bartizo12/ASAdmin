using System;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface for  Service Locator. Kept it as simple as possible.
    /// We will avoid to use this as much as possible.However, sometimes we will need this.
    /// </summary>
    public interface IServiceLocator : IDisposable
    {
        bool IsRegistered<T>();

        object Resolve(Type serviceType);

        T Resolve<T>();
    }
}