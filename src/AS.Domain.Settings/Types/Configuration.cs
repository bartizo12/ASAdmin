namespace AS.Domain.Settings
{
    /// <summary>
    /// Uneditable application configuration. The difference between this configuration
    /// and web.config is that , this configuration is created at the runtime when application
    /// has run for the first time.
    /// </summary>
    public class Configuration
    {
        public string ConnectionString { get; set; }
        public string DataProvider { get; set; }
        public bool IsDemo { get; set; }
        public string IPQueryApiKey { get; set; }
        public string RecaptchaPrivateKey { get; set; }
        public string RecaptchaPublicKey { get; set; }
        public string SymmetricKey { get; set; }

        /// <summary>
        /// Name of the settinng
        /// </summary>
        public string SMTPName { get; set; }

        /// <summary>
        /// SMTP Host Address .e.g smtp.gmail.com
        /// </summary>
        public string SMTPHost { get; set; }

        /// <summary>
        /// 25/587 or whatever the port number is
        /// </summary>
        public int SMTPPort { get; set; }

        public int SMTPTimeOut { get; set; }//In miliseconds

        /// <summary>
        /// If SMTP server requires SSL connection
        /// </summary>
        public bool SMTPEnableSsl { get; set; }

        /// <summary>
        /// Is SMTP server accepts Default Credenttials
        /// </summary>
        public bool SMTPDefaultCredentials { get; set; }

        /// <summary>
        /// Username on SMTP server
        /// </summary>
        public string SMTPUserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string SMTPPassword { get; set; }

        /// <summary>
        /// From Name that receiever see on mails
        /// </summary>
        public string SMTPFromDisplayName { get; set; }

        /// <summary>
        /// From address that receiever see on mails
        /// </summary>
        public string SMTPFromAddress { get; set; }

        public Configuration()
        {
        }

        public Configuration(string dataProvider, string connectionString)
        {
            this.DataProvider = dataProvider;
            this.ConnectionString = connectionString;
        }
    }
}