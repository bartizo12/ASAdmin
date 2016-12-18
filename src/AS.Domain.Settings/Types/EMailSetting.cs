using System.ComponentModel;

namespace AS.Domain.Settings
{
    /// <summary>
    /// EMailSetting contains SMTP connection settings to be executed when an en-mail is sent.
    /// There might be multiple SMTP connection settings to be applied for different e-mails. Also user
    /// might want to change settings without editing the config file.
    /// </summary>
    [TypeConverter(typeof(EMailSettingTypeConverter))]
    public class EMailSetting : SettingBase
    {
        /// <summary>
        /// Name of the settinng
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// SMTP Host Address .e.g smtp.gmail.com
        /// </summary>
        public string Host { get; internal set; }

        /// <summary>
        /// 25,587 or whatever
        /// </summary>
        public int Port { get; internal set; }

        public int TimeOut { get; internal set; }//In miliseconds

        /// <summary>
        /// If SMTP server requires SSL connection
        /// </summary>
        public bool EnableSsl { get; internal set; }

        /// <summary>
        /// Is SMTP server accepts Default Credenttials
        /// </summary>
        public bool DefaultCredentials { get; internal set; }

        /// <summary>
        /// Username on SMTP server
        /// </summary>
        public string UserName { get; internal set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; internal set; }

        /// <summary>
        /// From Name that receiever see on mails
        /// </summary>
        public string FromDisplayName { get; internal set; }

        /// <summary>
        /// From address that receiever see on mails
        /// </summary>
        public string FromAddress { get; internal set; }
    }
}