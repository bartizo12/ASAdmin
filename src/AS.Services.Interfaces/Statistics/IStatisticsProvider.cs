namespace AS.Services.Interfaces
{
    /// <summary>
    /// Provides statictis about the application
    /// </summary>
    public interface IStatisticsProvider : IService
    {
        /// <summary>
        /// Total user count in database
        /// </summary>
        int TotalUserCount { get; }

        /// <summary>
        /// Total application count in database
        /// </summary>
        int TotalLogCount { get; }

        /// <summary>
        /// Total mail count in database
        /// </summary>
        int TotalMailCount { get; }

        /// <summary>
        /// Total async job/task count
        /// </summary>
        int TotalJobCount { get; }
    }
}