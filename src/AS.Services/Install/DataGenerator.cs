using AS.Domain.Entities;
using AS.Infrastructure.Identity;
using AS.Infrastructure.Logging;
using Faker;
using Faker.Extensions;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.Services
{
    /// <summary>
    /// Generates initial and test data
    /// </summary>
    internal class DataGenerator
    {
        private const int TestEmailCount = 250;
        private const int TestAppLogCount = 500;
        private const int TestUserCount = 100;
        private const int TestMaxActivityCount = 60;
        private const int TestMaxReadNotificationCount = 30;
        private const int TestMaxUnreadNotificationCount = 10;
        private readonly Random random = new Random();
        private readonly bool _isDemo;

        public DataGenerator(bool isDemo)
        {
            _isDemo = isDemo;
        }

        public List<EMail> GenerateEMails()
        {
            List<EMail> mails = new List<EMail>();

            if (!_isDemo)
                return mails;

            var maxJobStatus = Enum.GetValues(typeof(JobStatus)).Cast<JobStatus>().Max();

            for (int i = 0; i < TestEmailCount; i++)
            {
                EMail mail = new EMail()
                {
                    Body = Lorem.Paragraph(),
                    CreatedBy = "Installer",
                    CreatedOn = DateTime.UtcNow.AddDays(-random.Next(400)),
                    EmailSettingName = "Gmail",
                    FromAddress = "noreply@asadmin.com",
                    FromName = "ASAdmin",
                    JobStatus = (JobStatus)random.Next((int)maxJobStatus),
                    Receivers = Internet.Email(),
                    SmtpClientTimeOut = 1000,
                    SmtpEnableSsl = true,
                    SmtpHostAddress = "smtp.gmail.com",
                    SmtpPassword = Internet.UserName(),
                    SmtpPort = 587,
                    SmtpUseDefaultCredentials = true,
                    SmtpUserName = "asadmin@gmail.com",
                    Subject = Lorem.Sentence(),
                    TryCount = random.Next(10)
                };

                if (mail.JobStatus != JobStatus.Queued)
                {
                    mail.LastExecutionTime = mail.CreatedOn.AddMilliseconds(random.Next(100000));
                }
                else
                {
                    mail.TryCount = 0;
                }
                if (mail.JobStatus == JobStatus.Failed)
                {
                    mail.ErrorMessage = Lorem.Paragraph();
                }
                else
                {
                    mail.ErrorMessage = string.Empty;
                }

                mails.Add(mail);
            }

            return mails;
        }

        public List<AppLog> GenerateAppLogs()
        {
            List<AppLog> logs = new List<AppLog>();

            if (!_isDemo)
                return logs;

            string[] levels = LogLevels.All.ToArray();

            for (int i = 0; i < TestAppLogCount; i++)
            {
                logs.Add(new AppLog()
                {
                    AppDomain = "TestDomain",
                    ClientIP = GetRandomIp(),
                    CreatedBy = Internet.UserName(),
                    CreatedOn = DateTime.UtcNow.AddDays(-random.Next(400)),
                    Level = levels.Random(),
                    Location = Lorem.Sentence(),
                    LoggerName = "AS.Infrastructure.Logging.NLogger",
                    MachineName = Environment.MachineName,
                    Message = Lorem.Paragraph()
                });
            }
            return logs;
        }

        public List<UserActivity> GenerateActivities(List<int> userIdList)
        {
            List<UserActivity> activities = new List<UserActivity>();

            if (!_isDemo)
                return activities;

            foreach (int userId in userIdList)
            {
                UserActivity act = new UserActivity()
                {
                    CreatedOn = DateTime.UtcNow.AddDays(-random.Next(700)).AddDays(-365),
                    UserId = userId,
                    UserActivityType = UserActivityType.UserCreation,
                    CreatedBy = "Installer"
                };
                activities.Add(act);
                var maxActType = Enum.GetValues(typeof(UserActivityType)).Cast<UserActivityType>().Max();

                for (int i = 0; i < random.Next(TestMaxActivityCount); i++)
                {
                    act = new UserActivity()
                    {
                        UserId = userId,
                        UserActivityType = (UserActivityType)(1 + random.Next((int)maxActType)),
                        CreatedOn = DateTime.UtcNow.AddDays(-random.Next(365)),
                        CreatedBy = "Installer"
                    };
                    activities.Add(act);
                }
            }

            return activities;
        }

        public List<ASRole> GenerateRoles()
        {
            List<ASRole> roles = new List<ASRole>();

            ASRole role = new ASRole()
            {
                Name = "Admin",
                CreatedBy = "Installer",
                CreatedOn = DateTime.UtcNow.AddDays(-random.Next(365)),
                ModifiedOn = null
            };
            roles.Add(role);

            if (!_isDemo)
            {
                return roles;
            }

            string[] items = new string[] { "User", "Moderator", "Visitor", "System User", "Super Admin", "Temp", "Test Role" };

            for (int i = 0; i < items.GetLength(0); i++)
            {
                role = new ASRole()
                {
                    Name = items[i],
                    CreatedBy = "Installer",
                    CreatedOn = DateTime.UtcNow.AddDays(-random.Next(365)),
                    ModifiedOn = null
                };
                roles.Add(role);
            }

            return roles;
        }

        public List<ASUserRole> GenerateUserRoles(List<int> roleIds, List<int> userIds)
        {
            List<ASUserRole> userRoles = new List<ASUserRole>();

            userRoles.Add(new ASUserRole()
            {
                RoleId = roleIds.First(),
                UserId = userIds.First(),
                CreatedOn = DateTime.UtcNow.AddDays(-random.Next(700)),
                CreatedBy = "Installer"
            });

            if (!_isDemo)
                return userRoles;
            for (int i = 1; i < userIds.Count; i++)
            {
                List<int> tempRolesIds = new List<int>(roleIds);
                int randRoleCount = random.Next(tempRolesIds.Count);

                for (int j = 0; j < randRoleCount; j++)
                {
                    int randRoleIndex = random.Next(tempRolesIds.Count);

                    userRoles.Add(new ASUserRole()
                    {
                        RoleId = tempRolesIds[randRoleIndex],
                        UserId = userIds[i],
                        CreatedOn = DateTime.UtcNow.AddDays(-random.Next(700)),
                        CreatedBy = "Installer"
                    });
                    tempRolesIds.RemoveAt(randRoleIndex);
                }
            }

            return userRoles;
        }

        public List<ASUser> GenerateUsers()
        {
            List<ASUser> users = new List<ASUser>();

            ASUser user = new ASUser()
            {
                UserName = "admin",
                Email = "admin@asadmindemo.com",
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "Installer",
                LastActivity = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                PasswordHash = new PasswordHasher().HashPassword("123456"),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            users.Add(user);

            if (!_isDemo)
                return users;

            for (int i = 0; i <= TestUserCount; i++)
            {
                user = new ASUser()
                {
                    UserName = Internet.UserName(),
                    Email = Internet.Email(),
                    CreatedOn = DateTime.UtcNow.AddDays(-random.Next(700)),
                    CreatedBy = "Installer",
                    LastActivity = DateTime.UtcNow.AddDays(-random.Next(30)),
                    LastLogin = DateTime.UtcNow.AddDays(-random.Next(40)),
                    PasswordHash = new PasswordHasher().HashPassword("123456"),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                if (!users.Any(u => u.UserName == user.UserName))
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public List<Notification> GenerateNotifications(List<int> userIdList)
        {
            List<Notification> notifications = new List<Notification>();

            if (!_isDemo)
                return notifications;

            foreach (int userId in userIdList)
            {
                for (int i = 0; i < random.Next(TestMaxReadNotificationCount); i++)
                {
                    Notification notification = new Notification();
                    notification.UserId = userId;
                    notification.Message = Lorem.Sentence();
                    notification.Url = "#";
                    notification.IsSeen = true;
                    notification.SeenOn = DateTime.UtcNow;
                    notification.CreatedOn = DateTime.UtcNow.AddYears(-1).AddDays(-random.Next(300));
                    notification.CreatedBy = "Installer";
                    notifications.Add(notification);
                }
                for (int i = 0; i < random.Next(TestMaxUnreadNotificationCount); i++)
                {
                    Notification notification = new Notification();
                    notification.UserId = userId;
                    notification.Message = Lorem.Sentence();
                    notification.Url = "#";
                    notification.IsSeen = false;
                    notification.CreatedOn = DateTime.UtcNow.AddDays(-random.Next(300));
                    notification.CreatedBy = "Installer";
                    notifications.Add(notification);
                }
            }
            return notifications;
        }

        public List<StringResource> GenerateStringResources()
        {
            List<StringResource> resourceList = new List<StringResource>();

            #region EN-US

            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Activities",
                Value = "Activities"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Activity_Activity",
                Value = "Activity"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Activity_Time",
                Value = "Activity Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Admin_EMailSettingMissingNotification",
                Value = "There is no  e-mail setting found . Please add one.Otherwise e-mails will not be send."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Admin_IPQueryApiKeyMissing",
                Value = "IPInfoDbApiKey  value is missing in config AppSettings. Client country info will not be provided until this key is provided."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Admin_MenuTitle",
                Value = "Main Menu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Application_Settings",
                Value = "Application Settings"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationLogs",
                Value = "Application Logs"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettings_AddNew",
                Value = "Add New"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_AppDomain",
                Value = "AppDomain"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_ClientIP",
                Value = "Client IP"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_CreatedBy",
                Value = "User"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_CreatedOn",
                Value = "Log Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_Level",
                Value = "Level"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_Location",
                Value = "Location"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_LoggerName",
                Value = "Logger Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_MachineName",
                Value = "Machine Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLog_Message",
                Value = "Message"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AppLogListModel_SelectedLogLevels",
                Value = "Log Levels"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Captcha_InvalidValidationCode",
                Value = "Captcha validation failed.Please try again"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Complete",
                Value = "Complete"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Configuration",
                Value = "AS Admin Configuration"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfigurationModel_ConnectionString",
                Value = "Connection String"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfigurationModel_DataProvider",
                Value = "Data Provider"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfigurationModel_IPQueryApiKey",
                Value = "IpInfoDb API Key"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfigurationModel_IPQueryApiKey_Hint",
                Value = "API key to be used to query clients country by its IP address via IpInfoDb service. Application  logs clients country info if this key is provided."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfigurationModel_IsDemo",
                Value = "Is Demo Application"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfigurationModel_RecaptchaPrivateKey",
                Value = "ReCaptcha Private Key"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfigurationModel_RecaptchaPublicKey",
                Value = "ReCaptcha Public Key"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Copyright_Text",
                Value = "<strong>Copyright © 2016 <a href='http://www.asadmindemo.com/'>AS Admin</a>.</strong>"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "CreatedBy",
                Value = "Created By"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "CreatedOn",
                Value = "Created On"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Dashboard",
                Value = "Dashboard"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DateTimeInterval",
                Value = "Date/Time Interval"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Demo_Disabled_Message",
                Value = "Updating/Creating  is disabled for this page on demo application."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Demo_HTMLTemplateEditDisabled",
                Value = "Updating HTML Templates  is disabled demo application."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Description",
                Value = "Description"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Details",
                Value = "Details"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EditHTML",
                Value = "Edit HTML"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMail_DemoMaskMessage",
                Value = "E-Mail addresses are masked on demo application for security purposes."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMail_Settings",
                Value = "EMail Settings"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMail_Settings_New",
                Value = "New E-Mail Settings"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMail_UserErrorMessage",
                Value = "E-mail could not be sent.Please contact to system administration to fix the issue."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailListModel_LastExecutionTime",
                Value = "Last Execution Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailListModel_Receiver",
                Value = "Receivers"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailListModel_Status",
                Value = "Status"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailListModel_Subject",
                Value = "Subject"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_Body",
                Value = "Body"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_EmailSettingName",
                Value = "Selected Setting"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_ErrorMessage",
                Value = "Error Message"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_FromAddress",
                Value = "From Address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_FromName",
                Value = "From Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_JobStatus",
                Value = "Status"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_LastExecutionTime",
                Value = "Last Execution Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_Receivers",
                Value = "Receivers"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_SmtpClientTimeOut",
                Value = "Timeout Duration"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_SmtpEnableSsl",
                Value = "Enable SSL"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_SmtpHostAddress",
                Value = "Host Address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_SmtpPort",
                Value = "Port"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_SmtpUseDefaultCredentials",
                Value = "Use Default Credentials"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_SmtpUserName",
                Value = "SMTP Username"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_Subject",
                Value = "Subject"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailModel_TryCount",
                Value = "Send Trial Count"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Emails",
                Value = "E-Mails"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EmailSetting_TestConnection",
                Value = "Test SMTP Connection"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_Comment",
                Value = "Comment"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_DefaultCredentials",
                Value = "Use Default Credentials"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_EnableSsl",
                Value = "SSL Enabled"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_FromAddress",
                Value = "From E-Mail Address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_FromAddressInvalid",
                Value = "Invalid e-mail address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_FromDisplayName",
                Value = "From"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_Host",
                Value = "Host"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_HostRequired",
                Value = "SMTP  host required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_NameRequired",
                Value = "Name required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_Name",
                Value = "Setting Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_Password",
                Value = "Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_Port",
                Value = "Port"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_PortRequired",
                Value = "Port required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_TimeOut",
                Value = "Timeout"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "EMailSettingModel_UserName",
                Value = "Username"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ErrorMessage_PageNotFound",
                Value = "Oops! Page not found."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ErrorMessage_PageNotFoundPublicBody",
                Value = "We could not find the page you were looking for.<br><br>Meanwhile, you may <a href='../ Login'>return to login</a>."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ErrorMessage_UnableToUpdate",
                Value = "Unable to update"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ErrorMessage_UnexpectedError",
                Value = "Oops! Something went wrong."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ErrorMessage_UnexpectedErrorAdminBody",
                Value = "We will work on fixing that right away.Meanwhile, you may <a href=\"Home\">return to dashboard</a>."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ForgotPassword_Success",
                Value = "Password reset e-mail  was sent to your e-mail address.Please check your mail box."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ForgotPassword_Title",
                Value = "Forgot Password?"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Header_SessionTime",
                Value = "Session Time :"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HomePage",
                Value = "Home Page"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTML_Templates",
                Value = "HTML Templates"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplate_New",
                Value = "New HTML Template"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplateModel_BodyFilePath",
                Value = "Body File Path"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplateModel_BodyRequred",
                Value = "Template body is required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplateModel_Comment",
                Value = "Comment"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplateModel_Name",
                Value = "Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplateModel_NameRequired",
                Value = "Template name is required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplateModel_Subject",
                Value = "Subject"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "HTMLTemplateModel_SubjectRequired",
                Value = "Template subject is required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ID",
                Value = "ID"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Info",
                Value = "Info"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer",
                Value = "Installer"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_CannotConnectDatabase",
                Value = "Database connection could not be established.Please verify and test your connection.Error : {0}"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_CannotConnectSMTP",
                Value = "SMTP connection could not be established.Please verify and test your connection.Error : {0}"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_Completed",
                Value = "Completed"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_CompleteInfo",
                Value = "Everything is set and ready to go. Now complete the wizard. This may take a few seconds."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_ConnectionString",
                Value = "Connection String"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_ConnectionStringRequired",
                Value = "Please enter connection string"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_ConnectionSuccess",
                Value = "Connection has been established."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_Database",
                Value = "Database"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_DatabaseConnectionInfo",
                Value = "Set database connection"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_InitialSettings",
                Value = "Initial Settings"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_InitialSettingsInfo",
                Value = "Set initial settings that are required in the application"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_ProviderRequired",
                Value = "Please select provider"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_SMTP",
                Value = "SMTP"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_SMTPConnectionInfo",
                Value = "Set SMTP settings  of the application to be able send e-mails from the application It is optional.You can skip this step without setting"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_TestConnection",
                Value = "Test Connection"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Installer_Testing",
                Value = "Testing"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "InvalidPasswordFormat",
                Value = "Passwords must be between 4-20 characters in length and may contain letters (A-Z, a-z), numbers (0-9) and punctuation marks (^ _ * + #/\\ \" ?!=.{ } ~' &)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_InvalidRunInterval",
                Value = "Invalid run time interval"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_NameExists",
                Value = "A scheduled job with this name already exists."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_NameRequired",
                Value = "Please enter job name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_NewSucess",
                Value = "Job \"{0}\"  is succesfully created."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_NotExists",
                Value = "Job definition does not exist"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_RunSuccess",
                Value = "Job  <strong>{0}</strong> has triggered to run."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_TypeNameRequired",
                Value = "Please select job type"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinition_UpdateSuccess",
                Value = "Scheduled job definition has been successfully updated"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinitionModel_JobStatus",
                Value = "Status"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinitionModel_JobTypeName",
                Value = "Type Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinitionModel_LastExecutionTime",
                Value = "Last Execution Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinitionModel_Name",
                Value = "Job Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "JobDefinitionModel_RunInterval",
                Value = "Runtime Interval( sec )"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "LastActivitiyDateTimeRange",
                Value = "Last Activity Time From/To"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "List",
                Value = "List"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_EmailOrUsername",
                Value = "E-Mail Addres Or Username"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_FacebookLoginNotSupported",
                Value = "Login by using Facebook is not supported yet. Will be implemented in future"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_ForgotPassword",
                Value = "I forgot my password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_GooglePlusLoginNotSupported",
                Value = "Login by using Google Plus is not supported yet. Will be implemented in future"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_Login",
                Value = "Log in"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_LoginWithFb",
                Value = "Log in using Facebook"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_LoginWithGooglePlus",
                Value = "Log in using Google+"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_Password",
                Value = "Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_PasswordRequired",
                Value = "Please enter your password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_RememberMe",
                Value = "Remember Me"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_Title",
                Value = "ASAdmin Log In"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Login_UsernameOrEmailRequired",
                Value = "Please enter your username or e-mail address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Logout",
                Value = "Logout"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Mail",
                Value = "Mail"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MaxLen_ErrorMessage",
                Value = "The maximum length for  {0}   is {1} characters."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Membership_LoginFailed",
                Value = "Invalid username/e-mail address or password."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Membership_RoleDoesNotExist",
                Value = "Role does not exist"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Membership_UserNotFound",
                Value = "Invalid username/e-mail address  or password."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ModifiedBy",
                Value = "Modified By"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ModifiedOn",
                Value = "Modified On"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MoreInfo",
                Value = "More info"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Name",
                Value = "Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Next",
                Value = "Next"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Note",
                Value = "Note"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Optional",
                Value = "Optional"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "OR",
                Value = "OR"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Panel_Top_UserWelcome",
                Value = "Welcome , "
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Setting_InvalidSettingValue",
                Value = "Please fill required fields of the setting"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "SettingListModel_SettingDefId",
                Value = "Setting Type"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Settings",
                Value = "Settings"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Settings_Update",
                Value = "Settings Update"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "SettingValues",
                Value = "Setting Values"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "SettingValueUpdateSuccess",
                Value = "Settings  has been updated"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "RecordDoesNotExist",
                Value = "Record does not exist"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPage",
                Value = "Reset Page"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_InvalidToken",
                Value = "Invalid token!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_NewPassword",
                Value = "New Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_NewPasswordRe",
                Value = "Retype New Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_NewPasswordRequired",
                Value = "Please enter new password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_NewPasswordRetypeRequired",
                Value = "Please  enter new password again."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_PasswordsDontMatch",
                Value = "Passwords don't match."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_ResetPassword",
                Value = "Reset Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_Resetting",
                Value = "Resetting..."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_Successful",
                Value = "Your password succesfully has been changed.You can go <a href=\"Login\">Login Page</a> and authenticate.<br> "
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPassword_Title",
                Value = "Reset Your Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPasswordAdmin_Successful",
                Value = "Users password  has been reset succesfully"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPasswordModel_NewPassword",
                Value = "New Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetPasswordModel_NewPasswordRepeat",
                Value = "Retype New Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetUserPasswordModel_NewPassword",
                Value = "New Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ResetUserPasswordModel_NewPasswordRepeat",
                Value = "Retype New Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Result_Error",
                Value = "Error!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Result_Success",
                Value = "Succesful!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "RoleModel_Name",
                Value = "Role Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles",
                Value = "Roles"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles_AddNew",
                Value = "Add New Role"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles_CannotBeDeletedRoleHasUsers",
                Value = "Role  '<b>{0}</b>' cannot be deleted.There are users in this role"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles_Exists",
                Value = "Role  \"{0}\" already exists."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles_NewRoleSuccess",
                Value = "Role \"{0}\"  is succesfully created."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles_NotExists",
                Value = "Role does not exist."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles_RoleNameRequired",
                Value = "Please enter role name."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Roles_UpdateSuccess",
                Value = "Role is succesfully updated."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Save",
                Value = "Save"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ScheduledJobs",
                Value = "Scheduled Jobs"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ScheduledJobs_New",
                Value = "New Scheduled Job"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "SourceCodeInfo",
                Value = "Source code is avaliable at :"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Submit",
                Value = "Submit"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AjaxErrorTitle",
                Value = "Error!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTables_colvis",
                Value = "Show/Hide Columns"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTables_DeleteConfirmMessage",
                Value = "Are you sure to delete this record?"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfirmTitle",
                Value = "Confirm"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTables_reload",
                Value = "Reload"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Delete",
                Value = "Delete"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Edit",
                Value = "Edit"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Executing",
                Value = "Executing"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Failed",
                Value = "Failed"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "No",
                Value = "No"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Ok",
                Value = "OK"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Queued",
                Value = "Queued"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Run",
                Value = "Run"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Successful",
                Value = "Successful"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Update",
                Value = "Update"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "View",
                Value = "View"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Yes",
                Value = "Yes"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivity_UserDeleted",
                Value = "User Deleted"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_InvalidPasswordEntry",
                Value = "Invalid login attempt"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_LogIn",
                Value = "Succesful Login"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ConfirmResendEmail",
                Value = "This e-mail will be resent to the receiver(s). Do you approve?"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_LogOut",
                Value = "User Logout"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_PasswordChange",
                Value = "Password Changed"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_PasswordReset",
                Value = "Password reset completed"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_PasswordResetRequest",
                Value = "Password reset requested"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_UserActivation",
                Value = "User activated"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_UserCreation",
                Value = "User Created"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserActivityType_UserDeactivation",
                Value = "User deactivated"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserListModel_EMail",
                Value = "E-Mail Address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserListModel_LastActivityFrom",
                Value = "Last Activity From"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserListModel_LastActivityTo",
                Value = "Last Activity To"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserListModel_UserName",
                Value = "Username"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserModel_Email",
                Value = "E-Mail Address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserModel_LastActivity",
                Value = "Last Activity Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserModel_LastLogin",
                Value = "Last Login Time"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserModel_PasswordRepeat",
                Value = "Retype Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserModel_SelectedRoles",
                Value = "Roles"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UserModel_UserName",
                Value = "Username"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users",
                Value = "Users"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_AddNew",
                Value = "Add New User"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_CreateSuccess",
                Value = "User with username :  {0} is  successfully created!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_EmailAddressExists",
                Value = "This e-mail address is already in use"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_EmailAddressInvalid",
                Value = "Invalid e-mail address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_EmailAddressRequired",
                Value = "E-mail address is reqired"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_PasswordRequired",
                Value = "Password is required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_UserNameExists",
                Value = "This username is already in use."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_UserNameInvalid",
                Value = "Invalid username.Username  must be between [4-50] characters; can contain letters( A-Z,a-z) , digits(0-9), dashes(-), underscores (_) and periods(.)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_UserNameRequired",
                Value = "Username is required"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Users_UserRoleRequired",
                Value = "User must have at least one role"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Version",
                Value = "<b>Version</b>"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ErrorMessage_PageNotFoundAdminBody",
                Value = ""
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ErrorMessage_UnexpectedErrorPublicBody",
                Value = "Internal server error.Something went wrong. <br><br>We will have a look at it. Meanwhile, you can  <a href=\"../ Login\">go back and try to login</a>."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResources",
                Value = "String Resources"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResource_AddNew",
                Value = "Add New"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceModel_CultureCode",
                Value = "Language Code"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceModel_Name",
                Value = "Name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceModel_Value",
                Value = "Value"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceListModel_CultureCode",
                Value = "Language Code"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceListModel_NameOrValue",
                Value = "Name/Value"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResource_NotExists",
                Value = "String Resource does not exist"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceModel_AvailableOnClientSide",
                Value = "Available On Client"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceModel_AvailableOnClientSide_Hint",
                Value = "If true, this string resource will be available to javascript/client side."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResource_Exists",
                Value = "String resource  \"{0}\" -   \"{1}\"  already exists."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResources_SaveSuccess",
                Value = "String resource  \"{0}\" -   \"{1}\"  successfully saved"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceModel_NameRequired",
                Value = "Please enter name"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "StringResourceModel_ValueRequired",
                Value = "Please enter value"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_EmptyTable",
                Value = "No data available in table"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_Info",
                Value = "Showing _START_ to _END_ of _TOTAL_ entries"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_InfoEmpty",
                Value = "Showing 0 to 0 of 0 entries"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_InfoFiltered",
                Value = "(filtered from _MAX_ total entries)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_LengthMenu",
                Value = "Show _MENU_ entries"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_LoadingRecords",
                Value = "Loading..."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_Processing",
                Value = "Processing..."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_ZeroRecords",
                Value = "No matching records found"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_SortAscending",
                Value = ": activate to sort column ascending"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_SortDescending",
                Value = ": activate to sort column descending"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_Next",
                Value = "Next"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_Previous",
                Value = "Prev"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_First",
                Value = "First"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "DataTable_Last",
                Value = "Last"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettings",
                Value = "Application Settings"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "Resend",
                Value = "Re-Send"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "SMTP_SettingMissing",
                Value = "SMTP setting is missing.Please configure your SMTP settings from <a href='../Settings/ListEMailSettings'>here</a>"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettings",
                Value = "Membership Settings"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_PasswordResetTokenExpireTimeInHours",
                Value = "Password Reset Token Expire Time(Hours)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_PasswordResetTokenExpireTimeInHours_Hint",
                Value = "User cannot use generated password reset token after it is expired."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_LastActivityTimeUpdateIntervalInSeconds",
                Value = "User Last Activity Time Update Interval(Seconds)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_LastActivityTimeUpdateIntervalInSeconds_Hint",
                Value = "System updates last activity time of a logged in user in configured time interval.Immediate update is avoided for performance reasons"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_CookieValidationIntervalInMinutes",
                Value = "Cookie Validation Interval(Minutes)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_CookieValidationIntervalInMinutes_Hint",
                Value = "Application validates browser cookie of logged in user in every specified minutes.In case users password or role has changed, cookie is removed and user is forced to login again"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_MinimumPasswordRequiredLength",
                Value = "Minimum Password Length"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_MinimumPasswordRequiredLength_Hint",
                Value = "Minimum required password length for user when new user is created/registered"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireUniqueEmailForUsers",
                Value = "Unique User E-Mail Address"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireUniqueEmailForUsers_Hint",
                Value = "Indicates whether an e-mail address can be used by only one user or multiple users.This flag is checked upon new user registration"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_AllowOnlyAlphanumericUserNames",
                Value = "Allow Only Alpha-Numeric Username"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_AllowOnlyAlphanumericUserNames_Hint",
                Value = "If this setting flag is checked a username must be alphanumeric only(cannot contain symbols)."
            });

            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_ApplicationDefaultTitle",
                Value = "Default Page Title"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_ApplicationDefaultTitle_Hint",
                Value = "Default page title to be used in HTML <title></title> tag"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_MetaDescription",
                Value = "Meta Description"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_MetaDescription_Hint",
                Value = "HTML meta description. Will be place in  <meta name='description'>"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_MetaKeywords",
                Value = "Meta Keywords"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_MetaKeywords_Hint",
                Value = "HTML meta keywords. Will be place in  <meta name='keywords'>"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_RecaptchaDisplayCount",
                Value = "Recaptcha Display Count"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_RecaptchaDisplayCount_Hint",
                Value = "Invalid trial limit to activate captcha verification."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_DbQueryLogEnable",
                Value = "Db Query Logging"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_DbQueryLogEnable_Hint",
                Value = "Flag for enabling/disabling entity framework command logging. Note that, " +
                         "Command Logging (Interceptor Logging) really slows down the application. " +
                         "Therefore,it is suggested to keep disabled unless it is really needed to " +
                         "log entity framework db commands."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_BundlingEnabled",
                Value = "Resource Bundling"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_Bundling_Hint",
                Value = "Flag for enabling/disabling bundling&minification of resource files."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "ApplicationSettingsModel_MinLogLevel",
                Value = "Minimum Logging Level"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireDigitInPassword",
                Value = "Require Digit In Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireDigitInPassword_Hint",
                Value = "Check if digit is required in users passwords"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireLowercaseInPassword",
                Value = "Require Lowercase Letter In Passwword"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireLowercaseInPassword_Hint",
                Value = "Check if at least 1 lowercase leter shall be required in users password upon user registration"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireNonLetterOrDigitInPassword",
                Value = "Require Non-Letter or Digit In Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireNonLetterOrDigitInPassword_Hint",
                Value = "Check if the password requires a non-letter or digit character."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireUppercaseInPassword",
                Value = "Require Uppercase Letter In Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "MembershipSettingModel_RequireUppercaseInPassword_Hint",
                Value = "Check if the password requires an uppercase letter"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "InvalidEmailAddress",
                Value = "E-mail address is not valid."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "PasswordTooShort",
                Value = "Passwords must be at least {0} characters"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "PasswordCannotBeEmpty",
                Value = "Password cannot be empty"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "PasswordRequiresDigit",
                Value = "Passwords must have at least one digit ('0'-'9')"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "PasswordRequiresLower",
                Value = "Passwords must have at least one lowercase ('a'-'z')"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "PasswordRequiresNonAlphanumeric",
                Value = "Passwords must have at least one non alphanumeric character"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "PasswordRequiresUpper",
                Value = "Passwords must have at least one uppercase ('A'-'Z')"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UsernameCannotBeEmpty",
                Value = "Username cannot be empty"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UsernameLengthMustBeInRange",
                Value = "Username length must be between {0} - {1}"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "UsernameCanBeOnlyAlphanumeric",
                Value = "Username can contain only alphanumeric characters"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "InvalidUsername",
                Value = "Invalid username"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "RecaptchaInvalidResponse",
                Value = "Please verify that you are not a robot"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "RecaptchaSettingsMissing",
                Value = "Recaptcha validation  setting is missing.Please contact to the system admin"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "RecaptchaSystemError",
                Value = "An unexpected error occured while validating the code.Please contact to the system admin"
            });

            #endregion EN-US

            #region tr-TR

            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettings",
                Value = "Kimlik Ayarları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Activities",
                Value = "Aktiviteler"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Activity_Activity",
                Value = "Aktivite"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Activity_Time",
                Value = "İşlem Zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Admin_EMailSettingMissingNotification",
                Value = "Tanımlı  E-Posta(SMTP) ayarı bulunmuyor. E-Posta gönderimi yapabilmek için lütfen bu tanımı yapın"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Admin_IPQueryApiKeyMissing",
                Value = "AppSettings  ayarlarında   IPInfoDbApiKey  değeri eksik. Bu değer sağlanmadığı takdirde sisteme bağlanan kullanıcıların ülke bilgisi kaydedilemez."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Admin_MenuTitle",
                Value = "Ana Menü"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Application_Settings",
                Value = "Uygulama Ayarları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationLogs",
                Value = "Uygulama Logları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettings_AddNew",
                Value = "Yeni Ekle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_AppDomain",
                Value = "AppDomain"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_ClientIP",
                Value = "IP Adresi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_CreatedBy",
                Value = "Kullanıcı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_CreatedOn",
                Value = "Kayıt Zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_Level",
                Value = "Seviye"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_Location",
                Value = "Konum"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_LoggerName",
                Value = "Logger Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_MachineName",
                Value = "Makine Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLog_Message",
                Value = "Mesaj"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AppLogListModel_SelectedLogLevels",
                Value = "Log Seviyeleri"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Captcha_InvalidValidationCode",
                Value = "Captcha doğrulama başarısız. Lütfen tekrar deneyiniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Complete",
                Value = "Tamamla"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Configuration",
                Value = "AS Admin  Yapılandırma"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfigurationModel_ConnectionString",
                Value = "Connection String"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfigurationModel_DataProvider",
                Value = "Veritabanı Tedarikçisi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfigurationModel_IPQueryApiKey",
                Value = "IpInfoDb API anahtarı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfigurationModel_IPQueryApiKey_Hint",
                Value = "IPInfoDb servisine gönderilecek API anahtarı. IPInfoDb sitesinden temin edebilirsiniz. Uygulamaya bağlanan kullanıcıların ülke bilgisini almak için kullanacağız."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfigurationModel_IsDemo",
                Value = "Demo Uygulama"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfigurationModel_RecaptchaPrivateKey",
                Value = "ReCaptcha Gizli Anahtarı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfigurationModel_RecaptchaPublicKey",
                Value = "ReCaptcha Açık Anahtarı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Copyright_Text",
                Value = "<strong>Copyright © 2016 <a href='http://www.asadmindemo.com/'>AS Admin</a>.</strong> "
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "CreatedBy",
                Value = "Oluşturan"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "CreatedOn",
                Value = "Oluşturulma Tarihi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Dashboard",
                Value = "Dashboard"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DateTimeInterval",
                Value = "Tarih/Zaman Aralığı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Demo_Disabled_Message",
                Value = "Demo uygulamasında bu sayfa için güncelleme/oluşturma işlemleri devre dışı bırakılmıştır."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Demo_HTMLTemplateEditDisabled",
                Value = "HTML şablolarının güncellenmesi Demo uygulamada devre dışı bırakılmıştır."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Description",
                Value = "Tanım"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Details",
                Value = "Detaylar"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EditHTML",
                Value = "HTML Düzenle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMail_DemoMaskMessage",
                Value = "Demo uygulamada e-posta adresleri güvenlik gerekçesiyle masklenmiştir."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMail_Settings",
                Value = "E-Posta Ayarları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMail_Settings_New",
                Value = "Yeni E-Posta Ayarı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMail_UserErrorMessage",
                Value = "E-posta gönderilemedi. Lütfen bu sorunun sistem yöneticisine bildiriniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailListModel_LastExecutionTime",
                Value = "Son Çalıştırılma Zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailListModel_Receiver",
                Value = "Alıcılar"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailListModel_Status",
                Value = "Durum"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailListModel_Subject",
                Value = "Konu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_Body",
                Value = "İçerik"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_EmailSettingName",
                Value = "Ayar"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_ErrorMessage",
                Value = "Hata Mesajı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_FromAddress",
                Value = "Kimden ( Adres )"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_FromName",
                Value = "Kimden"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_JobStatus",
                Value = "Durumu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_LastExecutionTime",
                Value = "Son Çalıştırılma Zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_Receivers",
                Value = "Alıcıları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_SmtpClientTimeOut",
                Value = "Zaman Aşım(timeout)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_SmtpEnableSsl",
                Value = "SSL Aktif"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_SmtpHostAddress",
                Value = "Host Adresi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_SmtpPort",
                Value = "Port"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_SmtpUseDefaultCredentials",
                Value = "Varsayılan Kimlik Bilgilerini Kullan"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_SmtpUserName",
                Value = "SMTP Kullanıcı Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_Subject",
                Value = "Konu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailModel_TryCount",
                Value = "Gönderim Deneme Sayısı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Emails",
                Value = "E-Postalar"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EmailSetting_TestConnection",
                Value = "SMTP Bağlantı Test Et"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_Comment",
                Value = "Yorum"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_DefaultCredentials",
                Value = "Varsayılan Kimlik Bilgilerini Kullan"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_EnableSsl",
                Value = "SSL Aktif"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_FromAddress",
                Value = "Kimden(Adres)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_FromAddressInvalid",
                Value = "Geçersiz e-posta adresi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_FromDisplayName",
                Value = "Kimden"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_Host",
                Value = "Host Adresi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_HostRequired",
                Value = "Host adresi zorunludur."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_NameRequired",
                Value = "İsimi zorunludur"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_Name",
                Value = "Ayar ismi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_Password",
                Value = "Parola"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_Port",
                Value = "Port"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_PortRequired",
                Value = "Port zorunludur."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_TimeOut",
                Value = "Zaman Aşım(timeout)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "EMailSettingModel_UserName",
                Value = "Kullanıcı Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ErrorMessage_PageNotFound",
                Value = "Sayfa bulunamadı."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ErrorMessage_PageNotFoundPublicBody",
                Value = "Aradığınız sayfayı bulamadık..<br><br><a href='../ Login'>Buradan</a> oturum açma sayfasına giderek oturum açabilirsiniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ErrorMessage_UnableToUpdate",
                Value = "Güncellenemedi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ErrorMessage_UnexpectedError",
                Value = "Beklenmeyen bir hata oluştu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ErrorMessage_UnexpectedErrorAdminBody",
                Value = "Hatayı en kısa sürede düzelteceğiz.<a href='Home'>Buradan</a> dashboard anasayfasına geri dönebilirsiniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ForgotPassword_Success",
                Value = "Parola sıfırlama işlemi için  e-posta adresinize  bir e-posta attık. Lütfen e-posta adresinizi kontrol ediniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ForgotPassword_Title",
                Value = "Parolamı Unuttum"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Header_SessionTime",
                Value = "Yerel Zaman :"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HomePage",
                Value = "Ana Sayfa"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTML_Templates",
                Value = "HTML Şablonları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplate_New",
                Value = "Yeni HTML Şablonu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplateModel_BodyFilePath",
                Value = "İçerik Dosya Yolu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplateModel_BodyRequred",
                Value = "Şablon  içerik dosyası zorunludur."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplateModel_Comment",
                Value = "Yorum"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplateModel_Name",
                Value = "İsim"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplateModel_NameRequired",
                Value = "Şablon adı zorunludur"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplateModel_Subject",
                Value = "Konu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "HTMLTemplateModel_SubjectRequired",
                Value = "Şablon konusu zorunludur"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ID",
                Value = "ID"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Info",
                Value = "Bilgi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer",
                Value = "Installer"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_CannotConnectDatabase",
                Value = "Veritabanı bağlantısı kurulamadı. Lütfen veritabanı bilgilerinizi tekrar kontrol ediniz. Hata : {0}"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_CannotConnectSMTP",
                Value = "SMTP  bağlantısı kurulamadı. Lütfen SMTP bilgilerinizi tekrar kontrol ediniz. Hata : {0}"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_Completed",
                Value = "Tamamlandı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_CompleteInfo",
                Value = "Yapılandırma tamamlandı ve uygulama çalışmaya hazır. Şimdi sihirbazı tamamlayınız. Bu işlem biraz zaman alabilir."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_ConnectionString",
                Value = "Connection String"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_ConnectionStringRequired",
                Value = "Lütfen Connection String değerini giriniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_ConnectionSuccess",
                Value = "Bağlantı kuruldu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_Database",
                Value = "Veritabanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_DatabaseConnectionInfo",
                Value = "Veritabanı bağlantısını oluştur"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_InitialSettings",
                Value = "Uygulama başlangıç ayarları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_InitialSettingsInfo",
                Value = "Uygulamada gerekli başlangıç ayarlarını yapınız"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_ProviderRequired",
                Value = "Lütfen  Sağlayıcı(Provider) seçiniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_SMTP",
                Value = "SMTP"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_SMTPConnectionInfo",
                Value = "Uygulama içerisinden e-posta gönderimi yapabilmek için SMTP(E-Posta) sunucu ayarlarını yapmanız gerekiyor. Bu adım opsiyoneldir. Atlayabilirsiniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_TestConnection",
                Value = "Bağlantıyı Test Et"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Installer_Testing",
                Value = "Test Ediliyor"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "InvalidPasswordFormat",
                Value = "Parola 4-20 karakter arasında olmalı  ve  harf(A-Z,a-z) , rakam (0-9) veya  sembollerden  (^ _ * + #/\\ \" ?!=.{ } ~' &) oluşabilir."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_InvalidRunInterval",
                Value = "Geçersiz çalışma aralık değeri"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_NameExists",
                Value = "Bu isimde zamanlanmış bir iş  zaten bulunmakta."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_NameRequired",
                Value = "Lütfen iş  adını giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_NewSucess",
                Value = "\"{0}\"  iş  başarıyla oluşturuldu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_NotExists",
                Value = "İş tanımı bulunamadı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_RunSuccess",
                Value = " <strong>{0}</strong>  işi başarıyla tetiklendi."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_TypeNameRequired",
                Value = "Lütfen iş tipini seçiniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinition_UpdateSuccess",
                Value = "Zamanlanmış iş tanımı başarıyla kaydedildi."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinitionModel_JobStatus",
                Value = "Durum"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinitionModel_JobTypeName",
                Value = "İş Tipi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinitionModel_LastExecutionTime",
                Value = "Son Çalışma Zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinitionModel_Name",
                Value = "İş Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "JobDefinitionModel_RunInterval",
                Value = "Çalışma Aralığı(saniye)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "LastActivitiyDateTimeRange",
                Value = "Son Etkinlik Zaman Aralığı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "List",
                Value = "Listele"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_EmailOrUsername",
                Value = "E-Posta Adresi veya Kullanıcı Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_FacebookLoginNotSupported",
                Value = "Facebook ile oturum açma henüz desteklenmiyor. İleri zamanlarda geliştirilecek"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_ForgotPassword",
                Value = "Parolamı Unuttum"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_GooglePlusLoginNotSupported",
                Value = "Google+ ile oturum açma henüz desteklenmiyor. İleri zamanlarda geliştirilecek"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_Login",
                Value = "Oturum Aç"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_LoginWithFb",
                Value = "Facebook ile Oturum Aç"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_LoginWithGooglePlus",
                Value = "Google+ ile Oturum Aç"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_Password",
                Value = "Parola"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_PasswordRequired",
                Value = "Lütfen parolanızı giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_RememberMe",
                Value = "Beni Hatırla"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_Title",
                Value = "ASAdmin Oturum Aç"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Login_UsernameOrEmailRequired",
                Value = "Lütfen kullanıcı adınızı veya e-posta adresini giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Logout",
                Value = "Çıkış"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Mail",
                Value = "Posta"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MaxLen_ErrorMessage",
                Value = "{0} için en fazla {1} karakter girebilirsiniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Membership_LoginFailed",
                Value = "Geçersiz  kullanıcı adı/e-posta adresi veya parola"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Membership_RoleDoesNotExist",
                Value = "Rol bulunamadı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Membership_UserNotFound",
                Value = "Geçersiz  kullanıcı adı/e-posta adresi veya parola"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ModifiedBy",
                Value = "Son Düzenleyen"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ModifiedOn",
                Value = "Son Düzenlenme Zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MoreInfo",
                Value = "Daha Fazla"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Name",
                Value = "İsim"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Next",
                Value = "Sonraki"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Note",
                Value = "Not"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Optional",
                Value = "Opsiyonel"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "OR",
                Value = "Veya"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Panel_Top_UserWelcome",
                Value = "Hoşgeldiniz,"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Setting_InvalidSettingValue",
                Value = "Lütfen zorunlu alanları doldurunuz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "SettingListModel_SettingDefId",
                Value = "Ayar tipi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Settings",
                Value = "Ayarlar"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Settings_Update",
                Value = "Ayar Güncelleme"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "SettingValues",
                Value = "Ayar Değerleri"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "SettingValueUpdateSuccess",
                Value = "Ayarlar güncellendi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "RecordDoesNotExist",
                Value = "Kayıt bulunamadı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPage",
                Value = "Temizle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_InvalidToken",
                Value = "Geçersiz Token"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_NewPassword",
                Value = "Yeni Parola"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_NewPasswordRe",
                Value = "Yeni Parola(Tekrar)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_NewPasswordRequired",
                Value = "Lütfen yeni parolayı giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_NewPasswordRetypeRequired",
                Value = "Lütfen yeni parolayı tekrar giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_PasswordsDontMatch",
                Value = "Parola uyuşmadı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_ResetPassword",
                Value = "Parola Sıfırla"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_Resetting",
                Value = "Sıfırlanıyor..."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_Successful",
                Value = "Parolanız başarıyla güncellendi.<a href=\"Login\"> Oturum Açma</a>  sayfasına giderek oturum açabilirsiniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPassword_Title",
                Value = "Parola Sıfırla"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPasswordAdmin_Successful",
                Value = "Kullanıcının parolası başarıyla sıfırlandı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPasswordModel_NewPassword",
                Value = "Yeni Parola"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetPasswordModel_NewPasswordRepeat",
                Value = "Yeni Parola(Tekrar)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetUserPasswordModel_NewPassword",
                Value = "Yeni Parola"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ResetUserPasswordModel_NewPasswordRepeat",
                Value = "Yeni Parola(Tekrar)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Result_Error",
                Value = "Hata!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Result_Success",
                Value = "Başarılı!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "RoleModel_Name",
                Value = "Rol Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles",
                Value = "Roller"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles_AddNew",
                Value = "Yeni Rol Ekle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles_CannotBeDeletedRoleHasUsers",
                Value = " '<b>{0}</b>'  rolü silinemez. Çünkü bu rolde olan kullanıcılar bulunmakta."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles_Exists",
                Value = "\"{0}\"  rolü zaten tanımlanmış."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles_NewRoleSuccess",
                Value = "\"{0}\"  rolü başarıyla tanımlandı."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles_NotExists",
                Value = "Rol bulunamadı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles_RoleNameRequired",
                Value = "Lütfen rol adını giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Roles_UpdateSuccess",
                Value = "Rol başarıyla güncellendi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Save",
                Value = "Kaydet"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ScheduledJobs",
                Value = "Zamanlanmış İşler"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ScheduledJobs_New",
                Value = "Yeni Zamanlanmış İş"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "SourceCodeInfo",
                Value = "Projenin kaynak kod adresi :"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Submit",
                Value = "Gönder"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "AjaxErrorTitle",
                Value = "Hata!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTables_colvis",
                Value = "Kolon Aç/Kapa"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTables_DeleteConfirmMessage",
                Value = "Bu kaydı silmek istediğinize emin misiniz?"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfirmTitle",
                Value = "Onay"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTables_reload",
                Value = "Yenile"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Delete",
                Value = "Sil"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Edit",
                Value = "Düzenle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Executing",
                Value = "Çalıştırılıyor"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Failed",
                Value = "Başarızı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "No",
                Value = "Hayır"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Ok",
                Value = "Tamam"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Queued",
                Value = "Kuyruğa Alındı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Run",
                Value = "Çalıştır"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Successful",
                Value = "Başarılı!"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Update",
                Value = "Güncelle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "View",
                Value = "Görüntüle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Yes",
                Value = "Evet"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivity_UserDeleted",
                Value = "Kullanıcı Silindi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_InvalidPasswordEntry",
                Value = "Geçersiz oturum açma girişimi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_LogIn",
                Value = "Oturum Açma"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_LogOut",
                Value = "Çıkış"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_PasswordChange",
                Value = "Parola Değişimi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_PasswordReset",
                Value = "Parola Sıfırlama"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_PasswordResetRequest",
                Value = "Parola Sıfırlama İsteği"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_UserActivation",
                Value = "Kullanıcı Aktifleştirme"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_UserCreation",
                Value = "Kullanıcı Yaratma"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserActivityType_UserDeactivation",
                Value = "Kullanıcı Pasifleştirme"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserListModel_EMail",
                Value = "E-Posta Adresi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserListModel_LastActivityFrom",
                Value = "Son İşlem Zaman Başlangıç"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserListModel_LastActivityTo",
                Value = "Son İşlem Zaman Sonu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserListModel_UserName",
                Value = "Kullanıcı Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserModel_Email",
                Value = "E-Posta Adresi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserModel_LastActivity",
                Value = "Son Etkinlik Zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserModel_LastLogin",
                Value = "Son Oturum açma zamanı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserModel_PasswordRepeat",
                Value = "Retype Password"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserModel_SelectedRoles",
                Value = "Roller"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UserModel_UserName",
                Value = "Kullanıcı Adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users",
                Value = "Kullanıcılar"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_AddNew",
                Value = "Yeni Kullanıcı Ekle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_CreateSuccess",
                Value = "{0} kullanıcı ismi ile yeni kullanıcı başarıyla oluşturuldu."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_EmailAddressExists",
                Value = "Bu e-posta adresi zaten kullanılmakta"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_EmailAddressInvalid",
                Value = "Geçersiz e-posta adresi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_EmailAddressRequired",
                Value = "E-Posta adresi zorunludur"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_PasswordRequired",
                Value = "Parola zorunludur"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_UserNameExists",
                Value = "Bu kullanıcı adı  başka bir kullanıcı tarafından kullanılıyor"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_UserNameInvalid",
                Value = "Geçersiz kullanıcı adı. Kullanıcı adı ; 4-50 karakter arasında harf(A-Z,a-z) rakam(0-9) veya  (-_.) karaktlerini içerebilir"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_UserNameRequired",
                Value = "Kullanıcı adı zorunludur"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Users_UserRoleRequired",
                Value = "Kullanıcının en az bir rolü olmak zorunda."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Version",
                Value = "<b>Versiyon</b>"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ErrorMessage_PageNotFoundAdminBody",
                Value = ""
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ErrorMessage_UnexpectedErrorPublicBody",
                Value = "Sunucu  uygulama hatası. Sistemde bir hata oluştu. Sistem yöneticisi bu  hatayı inceleyecektir. Bu arada sizde  <a href=\"../ Login\">Oturum Açma</a>. sayfasına gidip , tekrar oturum açmayı deneyebilirsiniz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResources",
                Value = "Çoklu Dil"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResource_AddNew",
                Value = "Yeni Ekle"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceModel_CultureCode",
                Value = "Dil Kodu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceModel_Name",
                Value = "İsim"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceModel_Value",
                Value = "Değer"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceListModel_CultureCode",
                Value = "Dil Kodu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceListModel_NameOrValue",
                Value = "İsim/Değer"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResource_NotExists",
                Value = "Böyle bir çoklu dili tanımı bulunamadı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceModel_AvailableOnClientSide",
                Value = "Arayüzden erişilebilir"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceModel_AvailableOnClientSide_Hint",
                Value = "Javascript den bu  tanıma erişilebilir/erişilemez "
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResource_Exists",
                Value = " \"{0}\" -   \"{1}\"   tanımı zaten mevcut."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResources_SaveSuccess",
                Value = " \"{0}\" - \"{1}\"   tanımı başarıyla kaydedildi."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceModel_NameRequired",
                Value = "Lütfen isim giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "StringResourceModel_ValueRequired",
                Value = "Lütfen değer giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_EmptyTable",
                Value = "Veri yok"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_Info",
                Value = "Toplam _TOTAL_ kayıttan ,  _START_  -  _END_  arasındaki kayıtlar gösteriliyor"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_InfoEmpty",
                Value = "0 kayıttan 0 - 0 arasındakiler gösteriliyor"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_InfoFiltered",
                Value = "(_MAX_  kayıttan filtrelenen)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_LengthMenu",
                Value = "_MENU_  kayıt göster"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_LoadingRecords",
                Value = "Yükleniyor..."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_Processing",
                Value = "Yükleniyor..."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_ZeroRecords",
                Value = "Kayıt bulunamadı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_SortAscending",
                Value = ": activate to sort column ascending"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_SortDescending",
                Value = ": activate to sort column descending"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettings",
                Value = "Uygulama Ayarları"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "Resend",
                Value = "Tekrar Gönder"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ConfirmResendEmail",
                Value = "Bu e-posta ilgili alıcıya tekrar gönderilecektir. Onaylıyor musunuz?"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "SMTP_SettingMissing",
                Value = "SMTP ayarları eksik.E-posta gönderimlerinin yapılabilmesi için <a href='../Settings/ListEMailSettings'>SMTP ayarlarının</a> yapılması gerekmektedir."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_PasswordResetTokenExpireTimeInHours",
                Value = "Parola Sıfırlama Belirteç Geçerlilik Süresi(Saat)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_PasswordResetTokenExpireTimeInHours_Hint",
                Value = "Bu süre aşıldıkdan sonra parola sıfırlama belirteci artık kullanılamaz(geçerli olmaz)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_LastActivityTimeUpdateIntervalInSeconds",
                Value = "Kullanıcı Son Aktivite Zamanı Güncelleme(Saniye)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_LastActivityTimeUpdateIntervalInSeconds_Hint",
                Value = "Kullanıcıların uygulamadaki son aktivite zamanları anında olmaz belli bir zaman aralığı ile olur.Performans sebebi ile belli zaman aralıklarında güncelleme yapılmaktadır."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_CookieValidationIntervalInMinutes",
                Value = "Cookie Doğrulama Zaman Aralığı(Dakika)"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_CookieValidationIntervalInMinutes_Hint",
                Value = "Kullanıcının parolası veya rolü değişti/değişmedi kontrolünü yapabilmek için cookie doğrulaması gerekmektedir.Bu doğrulamanın zaman aralığını belirtir."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_MinimumPasswordRequiredLength",
                Value = "Parola Minimum Karakter Sayısı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_MinimumPasswordRequiredLength_Hint",
                Value = "Kullanıcı kayıt işleminde kullanıcı için belirlenecek parolanın minimum karakter sayısı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_ApplicationDefaultTitle",
                Value = "Sayfa Başlığı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_ApplicationDefaultTitle_Hint",
                Value = "<title></title> etiketine yazılan varsayılan başlık"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_MetaDescription",
                Value = "Meta Tanımı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_MetaDescription_Hint",
                Value = "<meta name='description'> içine yazılan HTML meta tanımı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_MetaKeywords",
                Value = "Meta Anahtar Kelimeleri"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_MetaKeywords_Hint",
                Value = "<meta name='keywords'> içine yazılan HTML meta anahtar kelimeleri"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_RecaptchaDisplayCount",
                Value = "Recaptcha Doğrulama için Hatalı Giriş Sayısı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_RecaptchaDisplayCount_Hint",
                Value = "Captcha doğrulamanın aktif hale gelmesi için gereken hatalı giriş sayısı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_DbQueryLogEnable",
                Value = "Sorguları Logla"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_DbQueryLogEnable_Hint",
                Value = "Sorgu loglamayı açmak uygulamayı yavaşlatacaktır."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_BundlingEnabled",
                Value = "Kaynak Sıkıştırma"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_Bundling_Hint",
                Value = "Js ve css kaynak dosyalarını birleştirmek ve küçültmeye yarar."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "ApplicationSettingsModel_MinLogLevel",
                Value = "Minimum Log Seviyesi"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_AllowOnlyAlphanumericUserNames",
                Value = "Kullanıcı Adı Sadece Rakam veya Harf Olmalı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_AllowOnlyAlphanumericUserNames_Hint",
                Value = "Bu ayar seçildiği takdirde yeni oluşturulan kullanıcıların kullanıcı isimlerinde yalnızca rakam veya harfe izin verilecek."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireDigitInPassword",
                Value = "Parolada En Az 1 Rakam Zorunlu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireDigitInPassword_Hint",
                Value = "Eğer kullanıcı oluşturma admınında kullanıcı parolalarında rakam zorunlu ise bu ayarı seçiniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireLowercaseInPassword",
                Value = "Parolada  En Az 1 Küçük Harf Zorunlu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireLowercaseInPassword_Hint",
                Value = "Eğer kullanıcı oluşturma admınında kullanıcı parolalarında en az 1 küçük harf zorunlu ise bu ayarı seçiniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireNonLetterOrDigitInPassword",
                Value = "Parolada Harf-Rakam Harici 1 Karakter Zorunlu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireNonLetterOrDigitInPassword_Hint",
                Value = "Eğer kullanıcı oluşturma admınında kullanıcı parolalarında en az 1  harf-rakam dışında karakter zorunlu ise bu ayarı seçiniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireUppercaseInPassword",
                Value = "Parolada  En Az 1 Büyük Harf Zorunlu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "MembershipSettingModel_RequireUppercaseInPassword_Hint",
                Value = "Eğer kullanıcı oluşturma admınında kullanıcı parolalarında en az 1 büyük harf zorunlu ise bu ayarı seçiniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "InvalidEmailAddress",
                Value = "E-posta adresi geçersiz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "PasswordTooShort",
                Value = "Parola en az {0} karakter uzunluğunda olmalı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "PasswordRequiresDigit",
                Value = "Parolan en az bir adet rakam ('0'-'9') içermeli"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "PasswordRequiresLower",
                Value = "Parolan en az bir adet küçük harf ('a'-'z') içermeli"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "PasswordRequiresNonAlphanumeric",
                Value = "Parola en az bir adet harf veya rakam olmayan karaketer içermeli."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "PasswordRequiresUpper",
                Value = "Parolan en az bir adet büyük harf ('A'-'Z') içermeli"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UsernameCannotBeEmpty",
                Value = "Kullanıcı adı boş olamaz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UsernameLengthMustBeInRange",
                Value = "Kullanıcı adı uzunluğu {0} - {1} arasında olmalı."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "UsernameCanBeOnlyAlphanumeric",
                Value = "Kullanıcı adı sadece harf ve rakamlardan oluşabilir."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "InvalidUsername",
                Value = "Geçersiz kullanıcı adı"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "RecaptchaInvalidResponse",
                Value = "Geçersiz doğrulama kodu"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "RecaptchaSettingsMissing",
                Value = "Recaptcha doğrulama ayarları yapılmamış."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "RecaptchaSystemError",
                Value = "Doğrulama esnasında beklenmedik bir hata oluştu.Lütfen sistem yöneticisi ile görüşünüz."
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = false,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "PasswordCannotBeEmpty",
                Value = "Lütfen parola giriniz"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_Next",
                Value = "Sonraki"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_Previous",
                Value = "Önceki"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_First",
                Value = "İlk"
            });
            resourceList.Add(new StringResource()
            {
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "tr-TR",
                Name = "DataTable_Last",
                Value = "Son"
            });
            #endregion tr-TR

            return resourceList;
        }

        private string GetRandomIp()
        {
            return string.Format("{0}.{1}.{2}.{3}", random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }
    }
}