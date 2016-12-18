namespace AS.Services.Interfaces
{
    /// <summary>
    /// Interface for installer service that installs initial/test data
    /// </summary>
    public interface IInstallerService : IService
    {
        /// <summary>
        /// Installs current application initial/test data
        /// </summary>
        void Install();
    }
}