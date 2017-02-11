using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Data;
using AS.Infrastructure.Identity;
using AS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace AS.Services
{
    /// <summary>
    /// Installs initial/test data
    /// </summary>
    public class InstallerService : IInstallerService
    {
        private readonly object _lockObj = new object();
        private readonly IAppManager _appManager;
        private readonly ISchedulerService _schedulerService;
        private readonly ICacheService _cacheService;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ILogger _logger;
        private readonly IDatabase _database;
        private readonly Func<ASConfiguration> _configurationFactory;
        private readonly IEncryptionProvider _encryptionProvider;

        public InstallerService(ISchedulerService schedulerService,
            IAppManager appManager,
            ICacheService cacheService,
            ILogger logger,
            IDbContextFactory dbContextFactory,
            IDatabase database,
            IEncryptionProvider encryptionProvider,
            Func<ASConfiguration> configurationFactory)
        {
            this._appManager = appManager;
            this._schedulerService = schedulerService;
            this._cacheService = cacheService;
            this._dbContextFactory = dbContextFactory;
            this._logger = logger;
            this._database = database;
            this._encryptionProvider = encryptionProvider;
            this._configurationFactory = configurationFactory;
        }

        private bool IsDemo
        {
            get { return this._configurationFactory().IsDemo; }
        }

        public void Install()
        {
            if (_configurationFactory() == null)
                return;

            lock (_lockObj)
            {
                this._schedulerService.Stop();
                using (IDbContext _dbContext = this._dbContextFactory.Create())
                {
                    _dbContext.AuditLoggingEnabled = false;
                    _dbContext.AutoDetectChangesEnabled = false;
                    _dbContext.ValidateOnSaveEnabled = false;
                    ASConfiguration config = _configurationFactory();

                    Stopwatch sw = Stopwatch.StartNew();

                    try
                    {
                        _database.ExecuteNonQuery("DeleteAllData", CommandType.StoredProcedure, null);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }

                    DataGenerator dataGenerator = new DataGenerator(this.IsDemo);
                    //String Resources
                    _dbContext.Set<StringResource>().AddRange(dataGenerator.GenerateStringResources());

                    #region Setting Definitions

                    SettingDefinition appSettingDef = new SettingDefinition()
                    {
                        Name = "AppSetting",
                        Description = "Application Settings in Name-Value structure." +
                                      " Contains simple settings just as <appSettings> in web.config.",
                        Field1 = "Name",
                        Field2 = "Value",
                        Field3 = "Comment",
                        FieldRequired1 = true,
                        FieldRequired2 = true,
                        FieldRequired3 = false
                    };
                    _dbContext.Set<SettingDefinition>().Add(appSettingDef);
                    SettingDefinition urlAddressSettingDef = new SettingDefinition()
                    {
                        Name = "UrlAddress",
                        Description = "Contains URL addresses to be used in the application in Name-Addres structure",
                        Field1 = "Name",
                        Field2 = "Address",
                        Field3 = "Comment",
                        FieldRequired1 = true,
                        FieldRequired2 = true,
                        FieldRequired3 = false
                    };
                    _dbContext.Set<SettingDefinition>().Add(urlAddressSettingDef);
                    SettingDefinition eMailAddressSettingDef = new SettingDefinition()
                    {
                        Name = "EMailAddress",
                        Description = "Contains E-Mail addresses to be used in the application in Name-Addres structure",
                        Field1 = "Name",
                        Field2 = "Address",
                        Field3 = "Comment",
                        FieldRequired1 = true,
                        FieldRequired2 = true,
                        FieldRequired3 = false
                    };
                    _dbContext.Set<SettingDefinition>().Add(eMailAddressSettingDef);
                    SettingDefinition htmlTemplateSettingDef = new SettingDefinition()
                    {
                        Name = "HTMLTemplate",
                        Description = "HTML Templates to be used in the application.For now , it is used" +
                                      " to generate user e-mails from templates however in future this setting" +
                                      " can be used for other purposes as well.",
                        Field1 = "Name",
                        Field2 = "Subject",
                        Field3 = "BodyFilePath",
                        Field4 = "Comment",
                        FieldRequired1 = true,
                        FieldRequired2 = true,
                        FieldRequired3 = true,
                        FieldRequired4 = false
                    };
                    _dbContext.Set<SettingDefinition>().Add(htmlTemplateSettingDef);
                    SettingDefinition emailSettingDef = new SettingDefinition()
                    {
                        Name = "EMailSetting",
                        Description = "E-Mail/SMTP settings that are used in the application.Provides user to manage " +
                                      "SMTP settings at run time and even store and use multiple SMTP settings in the " +
                                      " application.Differen e-mail settings can be used to send different types of e-mails",
                        Field1 = "Name",
                        Field2 = "Host",
                        Field3 = "Port",
                        Field4 = "TimeOut",
                        Field5 = "EnableSsl",
                        Field6 = "DefaultCredentials",
                        Field7 = "UserName",
                        Field8 = "Password",
                        Field9 = "FromDisplayName",
                        Field10 = "FromAddress",
                        Field11 = "Comment",
                        FieldRequired1 = true,
                        FieldRequired2 = true,
                        FieldRequired3 = true,
                        FieldRequired4 = true,
                        FieldRequired5 = true,
                        FieldRequired6 = true,
                        FieldRequired7 = true,
                        FieldRequired8 = true,
                        FieldRequired9 = true,
                        FieldRequired10 = true,
                        FieldRequired11 = false,
                        FieldInputType3 = FormInputType.DigitOnly,
                        FieldInputType5 = FormInputType.Checkbox,
                        FieldInputType6 = FormInputType.Checkbox,
                        FieldInputType8 = FormInputType.Password,
                        FieldInputType10 = FormInputType.Email
                    };
                    _dbContext.Set<SettingDefinition>().Add(emailSettingDef);
                    SettingDefinition membershipSettingDef = new SettingDefinition()
                    {
                        Name = "MembershipSetting",
                        Description = "Settings that are related with users and roles and other membership management",
                        Field1 = "Name",
                        Field2 = "PasswordResetTokenExpireTimeInHours",
                        Field3 = "LastActivityTimeUpdateIntervalInSeconds",
                        Field4 = "CookieValidationIntervalInMinutes",
                        Field5 = "RequireUniqueEmailForUsers",
                        Field6 = "AllowOnlyAlphanumericUserNames",
                        Field7 = "RequireDigitInPassword",
                        Field8 = "MinimumPasswordRequiredLength",
                        Field9 = "RequireLowercaseInPassword",
                        Field10 = "RequireNonLetterOrDigitInPassword",
                        Field11 = "RequireUppercaseInPassword",
                        Field12 = "Comment",
                        FieldRequired1 = true,
                        FieldRequired2 = true,
                        FieldRequired3 = true,
                        FieldRequired4 = true,
                        FieldRequired5 = true,
                        FieldRequired6 = true,
                        FieldRequired7 = true,
                        FieldRequired8 = true,
                        FieldRequired9 = true,
                        FieldRequired10 = true,
                        FieldRequired11 = true,
                        FieldRequired12 = false,
                        FieldInputType1 = FormInputType.Text,
                        FieldInputType2 = FormInputType.DigitOnly,
                        FieldInputType3 = FormInputType.DigitOnly,
                        FieldInputType4 = FormInputType.DigitOnly,
                        FieldInputType5 = FormInputType.Checkbox,
                        FieldInputType6 = FormInputType.Checkbox,
                        FieldInputType7 = FormInputType.Checkbox,
                        FieldInputType8 = FormInputType.DigitOnly,
                        FieldInputType9 = FormInputType.Checkbox,
                        FieldInputType10 = FormInputType.Checkbox,
                        FieldInputType11 = FormInputType.Checkbox,
                        FieldInputType12 = FormInputType.MultiLine
                    };
                    _dbContext.Set<SettingDefinition>().Add(membershipSettingDef);
                    _dbContext.SaveChanges();

                    #endregion Setting Definitions

                    #region Setting Values

                    SettingValue settingVal = new SettingValue()
                    {
                        Field1 = "ApplicationDefaultTitle",
                        Field2 = "AS.Admin",
                        Field3 = "Default page title to be used in HTML <title></title> tag",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "MetaDescription",
                        Field2 = "AS.Admin MVC.NET Admin Panel",
                        Field3 = "HTML meta description. Will be place in  <meta name='description'>",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "MetaKeywords",
                        Field2 = "AS.Admin,Back office,Admin Screens",
                        Field3 = "HTML meta keywords. Will be place in  <meta name='keywords'>",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "IsDemo",
                        Field2 = config.IsDemo.ToString(),
                        Field3 = "Indicates if current application is demo or not.If application is DEMO ," +
                                 " some actions are restricted for security purposes.",
                        IsHiddenFromUser = true,
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "RecaptchaPublickey",
                        Field2 = config.RecaptchaPublicKey,
                        Field3 = "Public API Key for recaptcha component.",
                        IsHiddenFromUser = true,
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "RecaptchaPrivateKey",
                        Field2 = config.RecaptchaPrivateKey,
                        Field3 = "Private API Key for recaptcha component.",
                        IsHiddenFromUser = true,
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "RecaptchaDisplayCount",
                        Field2 = "3",
                        Field3 = "Invalid trial limit to activate captcha verification.",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "DbQueryLogEnable",
                        Field2 = "False",
                        Field3 = "Flag for enabling/disabling entity framework command logging. Note that, " +
                                 "Command Logging (Interceptor Logging) really slows down the application. " +
                                 "Therefore,it is suggested to keep disabled unless it is really needed to " +
                                 "log entity framework db commands.",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "BundlingEnabled",
                        Field2 = bool.TrueString,
                        Field3 = "Flag for enabling/disabling bundling&minification of resource files.",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "RequestLoggingEnabled",
                        Field2 = bool.TrueString,
                        Field3 = "Flat for enabling/disabling HTTP request logging",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "MinLogLevel",
                        Field2 = "Debug",
                        Field3 = "Minimum Logging Level",
                        SettingDefinitionID = appSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "IPCountryQueryUrl",
                        Field2 = "http://ip-api.com/json/{{ip}}",
                        Field3 = "URL that we use for fetching country of the client by the ip adress." +
                        "API key must also be provided at 'Configuration' step when application run for the first time",
                        SettingDefinitionID = urlAddressSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "RecaptchaUrl",
                        Field2 = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}",
                        Field3 = "Recaptcha validation url",
                        SettingDefinitionID = urlAddressSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "PingUrl",
                        Field2 = "http://asadmindemo.com/Ping",
                        Field3 = "Ping url of our application",
                        SettingDefinitionID = urlAddressSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "SmtpConnectionTestEmailAddress",
                        Field2 = "test@gmail.com",
                        Field3 = "Test e-mail address to test SMTP connectivity",
                        SettingDefinitionID = eMailAddressSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "AdminEmailAddress",
                        Field2 = "admin@asadmindemo.com",
                        Field3 = "Admin e-mail address",
                        SettingDefinitionID = eMailAddressSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "ForgotPassword",
                        Field2 = "[AS.Admin]Please Reset Your Password",
                        Field3 = @"Templates\ForgotPassword.html",
                        Field4 = "HTML template of 'Forgot My Password' flow. This template is sent as e-mail " +
                                  "to the user.",
                        SettingDefinitionID = htmlTemplateSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);
                    settingVal = new SettingValue()
                    {
                        Field1 = "NewUser",
                        Field2 = "[AS.Admin]Your User Info",
                        Field3 = @"Templates\NewUser.html",
                        Field4 = "HTML template of 'New User'. This template is sent as e-mail to the user.",
                        SettingDefinitionID = htmlTemplateSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);

                    settingVal = new SettingValue()
                    {
                        Field1 = config.SMTPName,
                        Field2 = config.SMTPHost,
                        Field3 = config.SMTPPort.ToString(),
                        Field4 = config.SMTPTimeOut.ToString(),
                        Field5 = config.SMTPEnableSsl.ToString(),
                        Field6 = config.SMTPDefaultCredentials.ToString(),
                        Field7 = config.SMTPUserName,
                        Field8 = config.SMTPPassword, 
                        Field9 = config.SMTPFromDisplayName,
                        Field10 = config.SMTPFromAddress,
                        Field11 = string.Empty,
                        SettingDefinitionID = emailSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);

                    settingVal = new SettingValue()
                    {
                        Field1 = "MembershipSettings",
                        Field2 = "24",
                        Field3 = "60",
                        Field4 = "60",
                        Field5 = bool.TrueString,
                        Field6 = bool.FalseString,
                        Field7 = bool.FalseString,
                        Field8 = "4",
                        Field9 = bool.FalseString,
                        Field10 = bool.FalseString,
                        Field11 = bool.FalseString,
                        Field12 = string.Empty,
                        SettingDefinitionID = membershipSettingDef.Id
                    };
                    _dbContext.Set<SettingValue>().Add(settingVal);

                    #endregion Setting Values

                    #region Scheduled Jobs

                    JobDefinition jobDef = new JobDefinition();
                    jobDef.JobStatus = JobStatus.Queued;
                    jobDef.JobTypeName = "AS.Jobs.MailSendingJob, AS.Jobs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    jobDef.Name = "Mail Job";
                    jobDef.RunInterval = 5; //Every 5 seconds
                    jobDef.Comment = "Asynchronous job that is executed to send pending e-mails to the receivers.";
                    jobDef.CreatedBy = "Installer";
                    _dbContext.Set<JobDefinition>().Add(jobDef);

                    if (IsDemo)
                    {
                        jobDef = new JobDefinition();
                        jobDef.JobStatus = JobStatus.Queued;
                        jobDef.JobTypeName = "AS.Jobs.DemoResetJob, AS.Jobs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                        jobDef.Name = "Demo Environment Reset Job";
                        jobDef.RunInterval = 120 * 60;//Every 2 hours
                        jobDef.CreatedBy = "Installer";
                        jobDef.Comment = "Asynchronous job that is executed to reset demo application test data";
                        _dbContext.Set<JobDefinition>().Add(jobDef);
                    }
                    jobDef = new JobDefinition();
                    jobDef.JobStatus = JobStatus.Queued;
                    jobDef.JobTypeName = "AS.Jobs.PingJob, AS.Jobs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    jobDef.Name = "Ping Job";
                    jobDef.RunInterval = 60;//Every 1 minute
                    jobDef.CreatedBy = "Installer";
                    jobDef.Comment = "Asynchronous job that is executed to ping our own application to keep it up and running";
                    _dbContext.Set<JobDefinition>().Add(jobDef);

                    #endregion Scheduled Jobs
                    
                    // E-Mails
                    _dbContext.Set<EMail>().AddRange(dataGenerator.GenerateEMails());
                    //Roles
                    _dbContext.Set<ASRole>().AddRange(dataGenerator.GenerateRoles());
                    // Users
                    _dbContext.Set<ASUser>().AddRange(dataGenerator.GenerateUsers());
                    _dbContext.SaveChanges();

                    List<int> roleIdList = new List<int>(_dbContext.Set<ASRole>().Select(a => a.Id));
                    List<int> userIdList = new List<int>(_dbContext.Set<ASUser>().Select(a => a.Id));

                    //User Roles
                    _dbContext.Set<ASUserRole>().AddRange(dataGenerator.GenerateUserRoles(roleIdList, userIdList));
                    // UserActivities
                    _dbContext.Set<UserActivity>().AddRange(dataGenerator.GenerateActivities(userIdList));
                    // Notifications
                    _dbContext.Set<Notification>().AddRange(dataGenerator.GenerateNotifications(userIdList));

                    _dbContext.SaveChanges();
                    this._schedulerService.Initialize();

                    sw.Stop();
                    _logger.Warn(string.Format("Installer service took {0} seconds to install", sw.ElapsedMilliseconds / 1000));
                }
            }
        }
    }
}