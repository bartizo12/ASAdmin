using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Services.Interfaces;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;

namespace AS.Services
{
    /// <summary>
    /// Configures application
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private const string TestEmailAddress = "test@gmail.com";

        private readonly IMailService _mailService;
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly ISettingManager _settingManager;
        private readonly IStorageManager<Configuration> _configurationStorageManager;
        private readonly ILogger _logger;

        public ConfigurationService(IStorageManager<Configuration> configurationStorageManager,
            ISettingManager settingManager,
            ILogger logger,
            IEncryptionProvider encryptionProvider,
            IMailService mailService)
        {
            this._configurationStorageManager = configurationStorageManager;
            this._mailService = mailService;
            this._encryptionProvider = encryptionProvider;
            this._settingManager = settingManager;
            this._logger = logger;
        }

        private string TestMailReceiver
        {
            get
            {
                if (_settingManager.GetContainer<EMailAddress>() == null ||
                    !_settingManager.GetContainer<EMailAddress>().Contains("SmtpConnectionTestEmailAddress"))
                    return TestEmailAddress;
                return _settingManager.GetContainer<EMailAddress>()["SmtpConnectionTestEmailAddress"].Address;
            }
        }

        /// <summary>
        /// Check if config exists.
        /// </summary>
        /// <returns>True if exists, false otherwise.</returns>
        public bool CheckIfConfigExists()
        {
            return this._configurationStorageManager.CheckIfExists();
        }

        /// <summary>
        /// Reads config from file
        /// </summary>
        /// <returns>Read config</returns>
        public Configuration ReadConfig()
        {
            return this._configurationStorageManager.Read().First();
        }

        /// <summary>
        /// Saves config to a file
        /// </summary>
        /// <param name="settings"></param>
        public void SaveConfig(Configuration config)
        {
            config.SymmetricKey = this._encryptionProvider.GenerateKey();
            this._configurationStorageManager.Save(config);
        }

        /// <summary>
        /// Check if input database setting is valid and connect database.
        /// </summary>
        /// <param name="settings">Db configuration</param>
        /// <returns>Error message if there is an error while connecting database.
        /// Otherwise returns emtpy string</returns>
        public string CanConnectDatabase(Configuration config)
        {
            var factory = DbProviderFactories.GetFactory(config.DataProvider);

            using (IDbConnection connection = factory.CreateConnection())
            {
                try
                {
                    connection.ConnectionString = config.ConnectionString;
                    connection.Open();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return ex.Message;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Checks if input SMTP server setting is valid and can connect SMTP
        /// </summary>
        /// <param name="setting">SMTP configuration</param>
        /// <returns>Error message if there is an error while connecting SMTP Server.
        /// Otherwise returns emtpy string</returns>
        public string CanConnectSMTPServer(Configuration config)
        {
            try
            {
                EMail testMail = new EMail();
                testMail.SmtpClientTimeOut = config.SMTPTimeOut;
                testMail.SmtpPort = config.SMTPPort;
                testMail.SmtpEnableSsl = config.SMTPEnableSsl;
                testMail.SmtpUserName = config.SMTPUserName;
                testMail.SmtpPassword = config.SMTPPassword;
                testMail.SmtpHostAddress = config.SMTPHost;
                testMail.FromAddress = config.SMTPFromAddress;
                testMail.FromName = config.SMTPFromDisplayName;
                testMail.Body = Faker.Lorem.Sentence();
                testMail.Subject = Faker.Lorem.Sentence();
                testMail.Receivers = TestMailReceiver;

                CheckSMTPConnection(config.SMTPHost,
                    config.SMTPPort,
                    config.SMTPTimeOut);
                _mailService.SendEMail(testMail);
            }
            catch (AggregateException ae)
            {
                string msg = string.Empty;

                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    msg += e.Message + Environment.NewLine;
                }
                return msg;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return ex.Message;
            }
            return string.Empty;
        }

        private void CheckSMTPConnection(string address, int port, int timeOut)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = socket.BeginConnect(address, port, null, null);
            // Two second timeout
            bool success = result.AsyncWaitHandle.WaitOne(timeOut, true);
            if (!success)
            {
                socket.Close();
                throw new Exception("Failed to connect server.");
            }
        }
    }
}