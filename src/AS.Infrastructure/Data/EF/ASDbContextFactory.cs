using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Data.Entity;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// DbContextFactory that creates DbContext typeof <seealso cref="ASDbContext"/>
    /// In some cases, we need to create DbContext on our own, rather than getting DbContext to be
    /// injected by DI framework.
    /// </summary>
    public class ASDbContextFactory : IDbContextFactory
    {
        private readonly IXmlSerializer _xmlSerializer;
        private readonly IContextProvider _contextProvider;
        private readonly ITypeFinder _typeFinder;
        private readonly IDatabaseInitializer<ASDbContext> _dbInitializer;
        private readonly Func<DbConnectionConfiguration> _dbConnectionConfigurationFactory;

        public ASDbContextFactory(IXmlSerializer xmlSerializer,
            IContextProvider contextProvider,
            IDatabaseInitializer<ASDbContext> dbInitializer,
            ITypeFinder typeFinder,
            Func<DbConnectionConfiguration> dbConnectionConfigurationFactory)
        {
            this._xmlSerializer = xmlSerializer;
            this._contextProvider = contextProvider;
            this._typeFinder = typeFinder;
            this._dbInitializer = dbInitializer;
            this._dbConnectionConfigurationFactory = dbConnectionConfigurationFactory;
        }

        public IDbContext Create()
        {
            return new ASDbContext(_xmlSerializer,
                _contextProvider,
                _dbInitializer,
                _typeFinder,
                _dbConnectionConfigurationFactory);
        }
    }
}