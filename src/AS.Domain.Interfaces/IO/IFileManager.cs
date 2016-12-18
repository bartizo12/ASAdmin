using System.Collections.Generic;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Interface for file management
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Searches and returns the list of the files in format of  "Folder\FileName.extension".
        /// e.g : "Templates\Test.html"
        /// </summary>
        /// <param name="folderPath">Path of the folder to be searched</param>
        /// <param name="extensions">Extensions to be matched</param>
        /// <param name="includeSubFolders">True if search shall include subfolders/directories</param>
        /// <returns></returns>
        List<string> FindFiles(string folderPath, string[] extensions, bool includeSubFolders);
    }
}