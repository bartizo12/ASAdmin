using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace AS.Infrastructure.Web
{
    /// <summary>
    /// Contains  Web Applicationn related application.
    /// </summary>
    public sealed class WebAppManager : IAppManager, IFileManager
    {
        public WebAppManager()
        {
        }

        /// <summary>
        /// Searches and returns the list of the files in format of  "Folder\FileName.extension".e.g : "Templates\Test.html"
        /// </summary>
        /// <param name="folderPath">Path of the folder to be searched</param>
        /// <param name="extensions">Extensions to be matched</param>
        /// <param name="includeSubFolders">True if search shall include subfolders/directories</param>
        /// <returns></returns>
        public List<string> FindFiles(string folderPath, string[] extensions, bool includeSubFolders)
        {
            List<string> list = new List<string>();
            folderPath = this.MapPhysicalFile(folderPath);
            if (!Directory.Exists(folderPath))
                return list;

            int trimLen = HttpContext.Current.Request.PhysicalApplicationPath.Length;
            return (from file in Directory.GetFiles(folderPath)
                    where extensions.Contains(Path.GetExtension(file))
                    select file.Substring(trimLen, file.Length - trimLen)).ToList();
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPhysicalFile(string path)
        {
            if (!path.StartsWith(@"~\") && !path.Contains("/"))
            {
                path = @"~\" + path;
            }

            return HostingEnvironment.MapPath(path);
        }

        public string EncodeURL(string url)
        {
            return HttpContext.Current.Server.UrlEncode(url);
        }

        /// <summary>
        /// Restarts the Web Application
        /// Requires either Full Trust (HttpRuntime.UnloadAppDomain)
        /// or Write access to web.config.
        /// Taken From ;
        /// https://weblog.west-wind.com/posts/2006/Oct/08/Recycling-an-ASPNET-Application-from-within
        /// </summary>
        public bool RestartApplication()
        {
            bool Error = false;
            try
            {
                // *** This requires full trust so this will fail
                // *** in many scenarios
                HttpRuntime.UnloadAppDomain();
            }
            catch
            {
                Error = true;
            }

            if (!Error)
                return true;

            // *** Couldn't unload with Runtime - let's try modifying web.config
            string ConfigPath = HttpContext.Current.Request.PhysicalApplicationPath + "\\web.config";

            try
            {
                File.SetLastWriteTimeUtc(ConfigPath, DateTime.UtcNow);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}