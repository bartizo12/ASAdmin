using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AS.Infrastructure.Data.EF
{
    /// <summary>
    /// Custom Entity Framework DbContext
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    [DbConfigurationType(typeof(ASDbConfiguration))]
    public partial class ASDbContext : IdentityDbContext<ASUser, ASRole, int, ASUserLogin, ASUserRole, ASUserClaim>, IDbContext
    {
        private readonly IStorageManager<Configuration> _configurationStorageManager;
        private readonly IContextProvider _contextProvider;
        private readonly ITypeFinder _typeFinder;
        private readonly IXmlSerializer _xmlSerializer;

        /// <summary>
        /// We have defined parameterless constructor because Migration requires so. We pass a dummy
        /// connection string , since we dont have connection string when application is run at the
        /// first time. This is just a work-around
        /// </summary>
        public ASDbContext()
            : base("Data Source=;")
        {
            this.AuditLoggingEnabled = true;
        }

        // Passing "Server=;" as connection string is a dirty workaround
        public ASDbContext(IXmlSerializer xmlSerializer,
            IContextProvider contextProvider,
            IDatabaseInitializer<ASDbContext> dbInitializer,
            ITypeFinder typeFinder,
            IStorageManager<Configuration> configurationStorageManager) : base("Data Source=;")
        {
            this.AuditLoggingEnabled = true;
            this._configurationStorageManager = configurationStorageManager;
            this._xmlSerializer = xmlSerializer;
            this._contextProvider = contextProvider;
            this._typeFinder = typeFinder;

            if (!_configurationStorageManager.CheckIfExists())
            {
                return;
            }
            this.Database.Connection.ConnectionString = _configurationStorageManager.Read().First().ConnectionString;
            System.Data.Entity.Database.SetInitializer(dbInitializer);
            this.Configuration.ProxyCreationEnabled = false;
        }

        public bool AuditLoggingEnabled { get; set; }

        public bool AutoDetectChangesEnabled
        {
            get { return this.Configuration.AutoDetectChangesEnabled; }
            set { this.Configuration.AutoDetectChangesEnabled = value; }
        }

        public bool IsInitialized { get { return this.Database.Exists(); } }

        public bool ValidateOnSaveEnabled
        {
            get { return this.Configuration.ValidateOnSaveEnabled; }
            set { this.Configuration.ValidateOnSaveEnabled = value; }
        }

        /// <summary>
        /// Remove entity by ID
        /// </summary>
        /// <typeparam name="TEntity">Generic Entity Type</typeparam>
        /// <typeparam name="TID">Generic ID Type</typeparam>
        /// <param name="id">Id of the entity to be deleted</param>
        public void RemoveById<TEntity, TID>(TID id)
            where TID : struct
            where TEntity : EntityBase<TID>, new()
        {
            TEntity entity = new TEntity();
            entity.Id = id;
            TEntity existingEntity = this.Set<TEntity>().Local
                                          .FirstOrDefault(t => EqualityComparer<TID>.Default.Equals(t.Id, id));
            if (existingEntity == null)
            {
                this.Set<TEntity>().Attach(entity);
                this.Set<TEntity>().Remove(entity);
            }
            else
            {
                this.Entry<TEntity>(existingEntity).State = EntityState.Deleted;
            }
            this.SaveChanges();
        }

        public override int SaveChanges()
        {
            List<RecordAuditLog> auditLogs = new List<RecordAuditLog>();
            foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
            {
                ITrackableEntity trackableEntity = entry.Entity as ITrackableEntity;
                IEntity entity = entry.Entity as IEntity;

                if (entry.State == EntityState.Deleted && !(entry.Entity is ISafeToDeleteEntity))
                {
                    RecordAuditLog log = new RecordAuditLog();
                    log.EntityName = entry.Entity.GetType().Name;
                    log.Operation = entry.State.ToString();
                    log.Content = this._xmlSerializer.SerializeToXML(entry.Entity);
                    log.CreatedBy = this._contextProvider.UserName;
                    log.CreatedOn = DateTime.UtcNow;

                    auditLogs.Add(log);
                }
                else if (entry.State == EntityState.Modified && trackableEntity != null)
                {
                    trackableEntity.ModifiedOn = DateTime.UtcNow;
                    trackableEntity.ModifiedBy = this._contextProvider.UserName;
                }
                else if (entry.State == EntityState.Added && entity != null)
                {
                    if (string.IsNullOrEmpty(entity.CreatedBy))
                    {
                        entity.CreatedBy = this._contextProvider.UserName;
                    }
                    if (entity.CreatedOn == default(DateTime))
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                }
            }
            if (auditLogs.Count > 0 && AuditLoggingEnabled)
            {
                this.Set<RecordAuditLog>().AddRange(auditLogs);
            }
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (!_configurationStorageManager.CheckIfExists())
            {
                return;
            }
            IEnumerable<Type> typesToRegister = _typeFinder.FindClassesOfType(typeof(EntityTypeConfigurationBase<,>));

            foreach (Type type in typesToRegister)
            {
                if (!type.IsAbstract)
                {
                    dynamic configurationInstance = Activator.CreateInstance(type);
                    modelBuilder.Configurations.Add(configurationInstance);
                }
            }
            base.OnModelCreating(modelBuilder);
            // On mysql databases , if table has compact row format , it throws
            //  "Specified key was too long; max key length is 767 bytes" error.
            // To avoid this error,we've added following lines
            modelBuilder.Entity<ASRole>().Property(r => r.Name).HasMaxLength(128);
            modelBuilder.Entity<ASUser>().Property(r => r.UserName).HasMaxLength(128);
        }
    }
}