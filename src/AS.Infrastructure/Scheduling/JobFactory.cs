using AS.Domain.Interfaces;
using FluentScheduler;
using System;

namespace AS.Infrastructure
{
    /// <summary>
    /// Custom Job Factory for FluentScheduler.
    /// </summary>
    public class JobFactory : IJobFactory
    {
        private static IServiceLocator _resolver;

        public static void SetResolver(IServiceLocator resolver)
        {
            _resolver = resolver;
        }

        public IJob GetJobInstance<T>() where T : IJob
        {
            if (_resolver == null || !_resolver.IsRegistered<T>())
                throw new ArgumentNullException("_resolver");

            return _resolver.Resolve<T>();
        }
    }
}