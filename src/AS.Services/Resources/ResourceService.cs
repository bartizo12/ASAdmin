using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Collections;
using AS.Infrastructure.Data;
using AS.Services.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;

namespace AS.Services
{
    /// <summary>
    /// Handles management of  string resources.
    /// String Resources are stored in the database.
    /// </summary>
    public class ResourceService : IResourceService
    {
        private readonly IDbContext _dbContext;

        public ResourceService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds/Inserts new string resource definition
        /// </summary>
        /// <param name="resource">String Resource to be inserted</param>
        public void Insert(StringResource resource)
        {
            _dbContext.Set<StringResource>().Add(resource);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Updates string resource
        /// </summary>
        /// <param name="resource">Modified string resource entity</param>
        public void Update(StringResource resource)
        {
            StringResource existing = _dbContext.Set<StringResource>().Where(sr => sr.Id == resource.Id)
                                                                      .Single();
            existing.Value = resource.Value;
            existing.AvailableOnClientSide = resource.AvailableOnClientSide;

            _dbContext.Entry(existing).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Deletes string resource by its ID
        /// </summary>
        /// <param name="id">ID of the string resource to be deleted</param>
        public void DeleteById(int id)
        {
            _dbContext.RemoveById<StringResource, int>(id);
        }

        /// <summary>
        /// Gets all string resource definitions from db
        /// </summary>
        /// <returns>All string resources</returns>
        public IList<StringResource> FetchAll()
        {
            if (_dbContext.IsInitialized)
            {
                return _dbContext.Set<StringResource>()
                    .AsNoTracking()
                    .ToList();
            }
            else
            {
                return this.GetPredefinedResources();
            }
        }

        /// <summary>
        /// Gets string resource by its ID
        /// </summary>
        /// <param name="id">ID of the string resource to be fetched</param>
        /// <returns>Found string resource record or null if not found</returns>
        public StringResource GetResourceById(int id)
        {
            return _dbContext.Set<StringResource>()
                             .SingleOrDefault(sr => sr.Id == id);
        }

        /// <summary>
        ///  Retruns paged list of string resources after filtering and sorting applied
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="ordering">Order By string</param>
        /// <param name="cultureCode">Culture Code</param>
        /// <param name="nameOrValue">Name or Value</param>
        /// <returns>Paged list of found results.</returns>
        public IPagedList<StringResource> GetStringResources(int pageIndex, int pageSize,
            string ordering, string cultureCode, string nameOrValue)
        {
            var query = this._dbContext.Set<StringResource>().AsNoTracking() as IQueryable<StringResource>;

            if (!string.IsNullOrEmpty(cultureCode))
                query = query.Where(sr => sr.CultureCode == cultureCode);
            if (!string.IsNullOrEmpty(nameOrValue))
                query = query.Where(sr => sr.Name.Contains(nameOrValue) || sr.Value.Contains(nameOrValue));

            if (!string.IsNullOrEmpty(ordering))
            {
                query = query.OrderBy(ordering);
            }
            return new PagedList<StringResource>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Returns list of the cultures avaliable on the system. List is ordered by
        /// its
        /// </summary>
        /// <returns>Returns list of the cultures avaliable on the system</returns>
        public IList<string> FetchCultureList()
        {
            IList<string> cultureList = this.FetchAvailableCultureList();

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                if (!cultureList.Contains(culture.Name))
                {
                    cultureList.Add(culture.Name);
                }
            }
            return cultureList;
        }

        /// <summary>
        /// Returns list of the cultures avaliable in the database
        /// </summary>
        /// <returns>Returns list of the cultures in database</returns>
        public IList<string> FetchAvailableCultureList()
        {
            return _dbContext.Set<StringResource>()
                             .GroupBy(sr => sr.CultureCode)
                             .OrderByDescending(sr => sr.Count())
                             .Select(sr => sr.Key)
                             .ToList();
        }

        /// <summary>
        /// Gets string resource by cultureCode and name
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <param name="name"></param>
        /// <returns>Found string resource record or null if not found</returns>
        public StringResource GetResourceByNameAndCulture(string cultureCode, string name)
        {
            return _dbContext.Set<StringResource>()
                        .SingleOrDefault(sr => sr.CultureCode == cultureCode && sr.Name == name);
        }

        #region Private

        /// <summary>
        /// Before the application is configured there will be no database therefore no string resources
        /// we need to have some hard-coded predefined resources only to operate Configuration step
        /// </summary>
        /// <returns></returns>
        private IList<StringResource> GetPredefinedResources()
        {
            List<StringResource> resourceList = new List<StringResource>();
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
                Value = "Set SMTP(E-Mail) settings  of the application to be able send e-mails within the application.It is optional.You can skip this step without setting"
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
                Name = "Successful",
                Value = "Successful"
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
                AvailableOnClientSide = true,
                CreatedBy = "Installer",
                CultureCode = "en-US",
                Name = "AjaxErrorTitle",
                Value = "Error!"
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
                Name = "ErrorMessage_UnexpectedErrorPublicBody",
                Value = "Internal server error.Something went wrong. <br><br>We will have a look at it. Meanwhile, you can  <a href=\"../ Login\">go back and try to login</a>."
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
                Name = "EMailSettingModel_Name",
                Value = "Name"
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
            return resourceList;
        }

        #endregion Private
    }
}