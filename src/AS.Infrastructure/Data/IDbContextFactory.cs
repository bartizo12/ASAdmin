namespace AS.Infrastructure.Data
{
    /// <summary>
    /// Interface for DbContext factory.
    /// Most of the time we'll leave IOC container to deal with lifetime of  dbcontext
    /// but sometimes we need to manage the lifetime of dbcontext by ourselves. (e.g asynchronous jobs)
    /// </summary>
    public interface IDbContextFactory
    {
        IDbContext Create();
    }
}