using System.ComponentModel;

namespace AS.Domain.Settings
{
    /// <summary>
    /// HTMLTemplate is used for generating an HTML output or HTML based E-Mail from a common template.
    /// E.g. Generating e-mail from template , to send to the user.
    /// </summary>
    [TypeConverter(typeof(HTMLTemplateTypeConverter))]
    public class HTMLTemplate : SettingBase
    {
        /// <summary>
        /// Name of the template
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Subject of the template (if it is an-email template it has one)
        /// </summary>
        public string Subject { get; internal set; }

        /// <summary>
        /// Physical path of the body of the template.
        /// </summary>
        public string BodyFilePath { get; internal set; }
    }
}