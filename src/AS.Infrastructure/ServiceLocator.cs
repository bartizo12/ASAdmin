using AS.Domain.Interfaces;

namespace AS.Infrastructure
{
    /// <summary>
    /// Common Service Locator
    /// Note that there can be only one active Service Locator in the application.
    /// </summary>
    public static class ServiceLocator
    {
        private static IServiceLocator _serviceLocator;

        public static IServiceLocator Current
        {
            get
            {
                return _serviceLocator;
            }
        }

        /// <summary>
        /// Sets service locator
        /// </summary>
        public static void SetServiceLocator(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }
    }
}