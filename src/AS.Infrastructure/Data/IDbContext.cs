using AS.Domain.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace AS.Infrastructure.Data
{
    /// <summary>
    /// Interface for DbContext. (UnitOfWork)
    /// </summary>
    public interface IDbContext : IDisposable
    {
        bool IsInitialized { get; }
        bool AuditLoggingEnabled { set; }
        bool AutoDetectChangesEnabled { get; set; }
        bool ValidateOnSaveEnabled { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges();

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        void RemoveById<TEntity, TID>(TID id) where TID : struct where TEntity : EntityBase<TID>, new();
    }
}