using AS.Domain.Entities;
using AS.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;

namespace AS.Infrastructure.Tests
{
    public class TestServiceLocator : IServiceLocator
    {
        private readonly Func<DbConnectionConfiguration> _configFactory;

        public TestServiceLocator()
        {
            var _connectionStringSettings = new ASConfiguration()
            {
                DataProvider = TestFixture.ProviderName,
                ConnectionString = TestFixture.ConnectionString
            };
            _configFactory = () =>
            {
                return _connectionStringSettings;
            };
        }

        public bool IsRegistered<T>()
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            if (typeof(T) == typeof(Func<DbConnectionConfiguration>))
            {
                return (T)(object)_configFactory;
            }
            return default(T);
        }

        public void Dispose()
        {
          
        }
    }
}