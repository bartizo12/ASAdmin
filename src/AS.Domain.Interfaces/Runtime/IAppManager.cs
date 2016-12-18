namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Application manager that provides  runtime related functions/properties
    /// </summary>
    public interface IAppManager : IFileManager
    {
        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPhysicalFile(string filePath);

        /// <summary>
        /// URL-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="url">Url to be encoded</param>
        /// <returns>The encoded url</returns>
        string EncodeURL(string url);

        /// <summary>
        /// Restarts application
        /// </summary>
        /// <returns>True if it is restarted. False if it failed</returns>
        bool RestartApplication();
    }
}