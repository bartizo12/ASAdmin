using AS.Domain.Settings;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Interface for configuration class
    /// </summary>
    public interface IConfigurationService : IService
    {
        /// <summary>
        /// Check if config exists.
        /// </summary>
        /// <returns>True if exists, false otherwise.</returns>
        bool CheckIfConfigExists();

        /// <summary>
        /// Saves config to a file
        /// </summary>
        /// <param name="settings"></param>
        void SaveConfig(Configuration settings);

        /// <summary>
        /// Reads config from file
        /// </summary>
        /// <returns>Read config</returns>
        Configuration ReadConfig();

        /// <summary>
        /// Check if input database setting is valid and connect database.
        /// </summary>
        /// <param name="settings">Db configuration</param>
        /// <returns>Error message if there is an error while connecting database.
        /// Otherwise returns emtpy string</returns>
        string CanConnectDatabase(Configuration settings);

        /// <summary>
        /// Checks if input SMTP server setting is valid and can connect SMTP
        /// </summary>
        /// <param name="setting">SMTP configuration</param>
        /// <returns>Error message if there is an error while connecting SMTP Server.
        /// Otherwise returns emtpy string</returns>
        string CanConnectSMTPServer(Configuration setting);
    }
}