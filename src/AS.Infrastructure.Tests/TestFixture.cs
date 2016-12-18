using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Data;
using AS.Infrastructure.Data.EF;
using AS.Infrastructure.Identity;
using AS.Infrastructure.Reflection;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlServerCe;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AS.Infrastructure.Tests
{
    public class TestFixture : IDisposable
    {
        public static readonly string ProviderName = "System.Data.SqlServerCe.4.0";
        public static readonly string ConnectionString;

        static TestFixture()
        {
            string dbFilepath = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                 + @"\ASTest.sdf";

            if (File.Exists(dbFilepath))
                File.Delete(dbFilepath);

            ConnectionString = "Data Source=" + (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                + @"\ASTest.sdf;Persist Security Info=False";
            SqlCeEngine en = new SqlCeEngine(ConnectionString);
            en.CreateDatabase();
            ServiceLocator.SetServiceLocator(new TestServiceLocator());
        }

        private readonly ASUserStore _userStore;
        private readonly ASUserManager _userManager;
        private readonly IXmlSerializer _xmlSerializer;
        private readonly IContextProvider _contextProvider;
        private readonly IDatabase _database;
        private readonly ISettingContainer<AppSetting> _appSettings;
        private readonly IDatabaseInitializer<ASDbContext> _dbInitializer;
        private readonly ASDbCommandInterceptor _dbCommandInterceptor;
        private readonly IStorageManager<Configuration> _configurationManager;
        private readonly List<Configuration> _configurations;
        private readonly ITypeFinder _typeFinder;
        private readonly IAppManager _appManager;
        private readonly DbContext _dbContext;
        private readonly IDbContextFactory _dbContextFactory;

        public ASUserManager UserManager
        {
            get { return this._userManager; }
        }

        public IDbContext DbContext
        {
            get { return this._dbContext as IDbContext; }
        }

        public IDbContextFactory DbContextFactory
        {
            get { return this._dbContextFactory; }
        }

        public ASDbCommandInterceptor DbCommandInterceptor
        {
            get { return this._dbCommandInterceptor; }
        }

        public TestFixture()
        {
            _configurations = new List<Configuration>() { new Configuration(ProviderName, ConnectionString) };
            var mockAppSettings = new Mock<ISettingContainer<AppSetting>>();
            var mockContextProvider = new Mock<IContextProvider>();
            var mockConnectionStringManager = new Mock<IStorageManager<Configuration>>();
            var mockAppManager = new Mock<IAppManager>();
            var mockMembershipSettings = new MembershipSettingContainer();
            var mockSettingManager = new Mock<ISettingManager>();

            mockContextProvider.Setup(m => m.BrowserType).Returns("Chrome");
            mockContextProvider.Setup(m => m.ClientIP).Returns("111.222.333.444");
            mockContextProvider.Setup(m => m.CountryInfo).Returns(new Country("tr-TR", "Turkey"));
            mockContextProvider.Setup(m => m.Culture).Returns(CultureInfo.DefaultThreadCurrentCulture);
            mockContextProvider.Setup(m => m.HttpMethod).Returns("GET");
            mockContextProvider.Setup(m => m.RootAddress).Returns(@"http://www.asadmindemo.com");
            mockContextProvider.Setup(m => m.SessionId).Returns("q1w2e3r4");
            mockContextProvider.Setup(m => m.Url).Returns(new Uri("http://www.asadmindemo.com/Home"));
            mockContextProvider.Setup(m => m.UserId).Returns(1);
            mockContextProvider.Setup(m => m.UserName).Returns("UnitTester");
            mockAppSettings.Setup(m => m["DbQueryLogEnable"]).Returns(new AppSetting() { Value = "True" });
            mockConnectionStringManager.Setup(m => m.CheckIfExists()).Returns(true);
            mockConnectionStringManager.Setup(m => m.Read()).Returns(_configurations);
            mockAppManager.Setup(m => m.MapPhysicalFile(It.IsAny<string>())).Returns("Script.sql");

            _appSettings = mockAppSettings.Object;
            _xmlSerializer = new ASXmlSerializer();
            _contextProvider = mockContextProvider.Object;
            _configurationManager = mockConnectionStringManager.Object;
            _database = new Infrastructure.Data.Database(_configurationManager);
            _appManager = mockAppManager.Object;
            _typeFinder = new TypeFinder();
            mockSettingManager.Setup(m => m.GetContainer<AppSetting>()).Returns(_appSettings);
            mockSettingManager.Setup(m => m.GetContainer<MembershipSetting>()).Returns(mockMembershipSettings);

            _dbInitializer = new ASDatabaseInitializer<ASDbContext>(_configurationManager, _appManager);
            _dbCommandInterceptor = new ASDbCommandInterceptor(_database, _contextProvider, mockSettingManager.Object);
            _dbContextFactory = new ASDbContextFactory(_xmlSerializer, _contextProvider, _dbInitializer,
                 _typeFinder, _configurationManager);
            _dbContext = new ASDbContext(_xmlSerializer, _contextProvider, _dbInitializer,
                 _typeFinder, _configurationManager);
            _userStore = new ASUserStore(_dbContext);
            _userManager = new ASUserManager(_userStore, mockSettingManager.Object);
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
                //Dispose here
            }
            _disposed = true;
        }

        #endregion IDisposable
    }
}