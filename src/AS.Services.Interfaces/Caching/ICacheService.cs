namespace AS.Services.Interfaces
{
    /// <summary>
    /// Caching Service Interface
    /// </summary>
    public interface ICacheService : IService
    {
        /// <summary>
        /// Clear all cache
        /// </summary>
        void Clear();
    }
}