using AS.Domain.Interfaces;
using AS.Domain.Settings;
using Moq;
using System;
using System.Collections.Generic;

namespace AS.Infrastructure.Tests
{
    public class TestServiceLocator : IServiceLocator
    {
        private readonly IStorageManager<Configuration> _configurationManager;

        public TestServiceLocator()
        {
            var _connectionStringSettings = new Configuration()
            {
                DataProvider = TestFixture.ProviderName,
                ConnectionString = TestFixture.ConnectionString
            };
            var mockConnectionStringManager = new Mock<IStorageManager<Configuration>>();
            mockConnectionStringManager.Setup(m => m.CheckIfExists()).Returns(true);
            mockConnectionStringManager.Setup(m => m.Read()).Returns(new List<Configuration>() { _connectionStringSettings });

            _configurationManager = mockConnectionStringManager.Object;
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
            if (typeof(T) == typeof(IStorageManager<Configuration>))
            {
                return (T)this._configurationManager;
            }
            return default(T);
        }

        public void Dispose()
        {
          
        }
    }
}