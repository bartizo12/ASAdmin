using AS.Domain.Interfaces;
using AS.Domain.Settings;
using RazorEngine;
using RazorEngine.Templating;
using System.Collections.Generic;
using System.IO;

namespace AS.Services
{
    /// <summary>
    /// Templating service that handles HTML templating
    /// </summary>
    public class TemplateService : Interfaces.ITemplateService
    {
        private readonly IContextProvider _contextProvider;
        private readonly IAppManager _appManager;
        private readonly ISettingManager _settingManager;

        public TemplateService(ISettingManager settingManager,
            IContextProvider contextProvider,
            IAppManager appManager)
        {
            this._settingManager = settingManager;
            this._contextProvider = contextProvider;
            this._appManager = appManager;
        }

        /// <summary>
        /// Applys templating to the subject
        /// </summary>
        /// <param name="templateName">Name of the template</param>
        /// <param name="values">Values to be used in template subject</param>
        /// <returns>Subject after values applied to the template</returns>
        public string GetSubject(string templateName, Dictionary<string, object> values)
        {
            HTMLTemplate template = _settingManager.GetContainer<HTMLTemplate>()[templateName];
            return Engine.Razor.RunCompile(template.Subject, template.Name + "_Subject", null, values);
        }

        /// <summary>
        /// Applys templating to the body
        /// </summary>
        /// <param name="templateName">Name of the template</param>
        /// <param name="values">Values to be used in template body</param>
        /// <returns>Body after values applied to the template</returns>
        public string GetBody(string templateName, Dictionary<string, object> values)
        {
            HTMLTemplate template = _settingManager.GetContainer<HTMLTemplate>()[templateName];
            string body = File.ReadAllText(this._appManager.MapPhysicalFile(template.BodyFilePath));
            return Engine.Razor.RunCompile(body, template.Name + "_Body", null, new DynamicViewBag(values));
        }

        /// <summary>
        /// Fetches all ".html" files in Templates older
        /// </summary>
        /// <returns>List of html template files</returns>
        public List<string> FetchTemplateFileList()
        {
            return _appManager.FindFiles(@"~/Templates", new string[] { ".html" }, true);
        }
    }
}