using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Interface for Templating(HTML Templating)
    /// Mostly will be used for HTML formatted e-mail templating
    /// </summary>
    public interface ITemplateService : IService
    {
        /// <summary>
        /// Returns List of the templates in the application
        /// </summary>
        /// <returns>Returns List of the templates in the application</returns>
        List<string> FetchTemplateFileList();

        /// <summary>
        /// Applys templating to the subject
        /// </summary>
        /// <param name="templateName">Name of the template</param>
        /// <param name="values">Values to be used in template subject</param>
        /// <returns>Subject after values applied to the template</returns>
        string GetSubject(string templateName, Dictionary<string, object> values);

        /// <summary>
        /// Applys templating to the body
        /// </summary>
        /// <param name="templateName">Name of the template</param>
        /// <param name="values">Values to be used in template body</param>
        /// <returns>Body after values applied to the template</returns>
        string GetBody(string templateName, Dictionary<string, object> values);
    }
}