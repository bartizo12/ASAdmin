using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Collections;
using AS.Infrastructure.Data;
using AS.Infrastructure.Identity;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Identity;
using AS.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace AS.Services
{
    /// <summary>
    /// Membership service to deal with all membership related functionality
    /// Create/Delete/Update/Read users and roles
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private readonly IDbContext _dbContext;
        private readonly ASUserManager _userManager;
        private readonly ASRoleManager _roleManager;
        private readonly ASSignInManager _signInManager;
        private readonly ITemplateService _templateService;
        private readonly IMailService _mailService;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IContextProvider _contextProvider;
        private readonly IAppManager _appManager;
        private readonly IXmlSerializer _xmlSerializer;
        private readonly ISettingManager _settingManager;
        private readonly IResourceManager _resourceManager;

        public MembershipService(
            IDbContext dbContext,
            ASUserManager userManager,
            ASRoleManager roleManager,
            ASSignInManager signInManager,
            ITemplateService templateService,
            IMailService mailService,
            IDbContextFactory dbContextFactory,
            IContextProvider contextProvider,
            IDataProtectionProvider dataProtectionProvider,
            ISettingManager settingManager,
            IResourceManager resourceManager,
            IXmlSerializer xmlSerializer,
            IAppManager appManager)
        {
            this._resourceManager = resourceManager;
            this._dbContext = dbContext;
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._templateService = templateService;
            this._settingManager = settingManager;
            this._mailService = mailService;
            this._dbContextFactory = dbContextFactory;
            this._contextProvider = contextProvider;
            this._appManager = appManager;
            this._xmlSerializer = xmlSerializer;
            this._userManager.UserTokenProvider = new DataProtectorTokenProvider<ASUser, int>(
                dataProtectionProvider.Create("EmailConfirmation"));
        }

        /// <summary>
        /// Starts "I forgot My Password" flow. Sends users e-mail address a link with a token to reset his/her password
        /// Throws exception if user is not found
        /// </summary>
        /// <param name="userNameOrEmail">Username or e-mail address of the user</param>
        /// <returns>Generated token</returns>
        public PasswordResetToken StartForgotPasswordProcess(string userNameOrEmail)
        {
            ASUser user = _userManager.FindByEmail(userNameOrEmail);

            if (user == null)
            {
                user = _userManager.FindByName(userNameOrEmail);

                if (user == null)
                    throw new ASException(this._resourceManager.GetString("Membership_UserNotFound"));
            }
            if (this._settingManager.GetContainer<EMailSetting>().Default == null)
                throw new ASException(this._resourceManager.GetString("EMail_UserErrorMessage"));

            PasswordResetToken token = new PasswordResetToken();
            token.Token = this._userManager.GeneratePasswordResetToken(user.Id);
            token.UserId = user.Id;
            Dictionary<string, object> viewBag = new Dictionary<string, object>();
            viewBag.Add("UserName", user.UserName);
            viewBag.Add("ResetLink", string.Concat(this._contextProvider.RootAddress, "ResetPassword?token="
                , this._appManager.EncodeURL(token.Token)));
            EMail mail = new EMail();
            mail.Body = this._templateService.GetBody("ForgotPassword", viewBag);

            mail.EmailSettingName = this._settingManager.GetContainer<EMailSetting>().Default.Name;
            mail.FromAddress = this._settingManager.GetContainer<EMailSetting>().Default.FromAddress;
            mail.FromName = this._settingManager.GetContainer<EMailSetting>().Default.FromDisplayName;
            mail.Receivers = user.Email;
            mail.SmtpClientTimeOut = this._settingManager.GetContainer<EMailSetting>().Default.TimeOut;
            mail.SmtpEnableSsl = this._settingManager.GetContainer<EMailSetting>().Default.EnableSsl;
            mail.SmtpHostAddress = this._settingManager.GetContainer<EMailSetting>().Default.Host;
            mail.SmtpPassword = this._settingManager.GetContainer<EMailSetting>().Default.Password;
            mail.SmtpPort = this._settingManager.GetContainer<EMailSetting>().Default.Port;
            mail.SmtpUseDefaultCredentials = this._settingManager.GetContainer<EMailSetting>().Default.DefaultCredentials;
            mail.SmtpUserName = this._settingManager.GetContainer<EMailSetting>().Default.UserName;
            mail.Subject = this._templateService.GetSubject("ForgotPassword", viewBag);
            _mailService.Enqueue(mail);
            token.EMail = mail;
            _dbContext.Set<PasswordResetToken>().Add(token);

            UserActivity activity = new UserActivity();
            activity.UserId = user.Id;
            activity.UserActivityType = UserActivityType.PasswordResetRequest;
            _dbContext.Set<UserActivity>().Add(activity);

            _dbContext.SaveChanges();

            return token;
        }

        public bool ResetPasswordWithoutToken(string userName, string newPassword)
        {
            ASUser user = _userManager.FindByName(userName);
            string token = this._userManager.GeneratePasswordResetToken(user.Id);

            bool result = _userManager.ResetPassword(user.Id, token, newPassword) == IdentityResult.Success;

            if (result)
            {
                UserActivity activity = new UserActivity();
                activity.UserId = user.Id;
                activity.UserActivityType = UserActivityType.PasswordReset;
                _dbContext.Set<UserActivity>().Add(activity);

                _dbContext.SaveChanges();
            }
            return result;
        }

        public bool ResetPassword(string token, string newPassword)
        {
            PasswordResetToken tokenObj = (from t in _dbContext.Set<PasswordResetToken>()
                                           where t.Token == token
                                           select t).First();
            bool result = _userManager.ResetPassword(tokenObj.UserId, token, newPassword)
                 == IdentityResult.Success;

            if (result)
            {
                tokenObj.IsUsed = true;
                tokenObj.UsedOn = DateTime.UtcNow;
                _dbContext.Entry(tokenObj).State = EntityState.Modified;

                UserActivity activity = new UserActivity();
                activity.UserId = tokenObj.UserId;
                activity.UserActivityType = UserActivityType.PasswordReset;
                _dbContext.Set<UserActivity>().Add(activity);

                _dbContext.SaveChanges();
            }

            return result;
        }

        /// <summary>
        /// Validates password reset token. A token is valid if ;
        /// ** Token exists in our database
        /// ** Token is not used before
        /// ** Token is created less than last PasswordResetTokenExpireTimeInHours(by default 24)
        /// </summary>
        /// <param name="token">Token to be validated</param>
        /// <returns>True if valid, otherwise false.</returns>
        public bool ValidateToken(string token)
        {
            DateTime maxTime = DateTime.UtcNow.AddHours(this._settingManager.GetContainer<MembershipSetting>().Default.PasswordResetTokenExpireTimeInHours);

            return (from t in _dbContext.Set<PasswordResetToken>()
                    where t.Token == token && !t.IsUsed &&
                    t.CreatedOn < maxTime
                    select t).Any();
        }

        public void CreateUser(string userName, string password, string email, List<string> roles)
        {
            IdentityResult iResult;

            //Check if roles exists
            foreach (string role in roles)
            {
                if (!(_roleManager.RoleExistsAsync(role).Result))
                {
                    throw new ASException(this._resourceManager.GetString("Membership_RoleDoesNotExist"));
                }
            }
            //Now create user
            ASUser user = new ASUser();
            user.UserName = userName;
            user.Email = email;
            user.CreatedOn = DateTime.UtcNow;
            user.CreatedBy = this._contextProvider.UserName;
            iResult = _userManager.CreateAsync(user, password).Result;
            if (!iResult.Succeeded)
                throw new ASException(string.Join(";", iResult.Errors));

            foreach (string role in roles)
            {
                iResult = _userManager.AddToRole(user.Id, role);
                if (!iResult.Succeeded)
                    throw new ASException(string.Join(";", iResult.Errors));
            }

            Dictionary<string, object> viewBag = new Dictionary<string, object>();
            viewBag.Add("UserName", user.UserName);
            viewBag.Add("Url", this._contextProvider.RootAddress);
            EMail mail = new EMail();
            mail.Body = this._templateService.GetBody("Newuser", viewBag);

            if (this._settingManager.GetContainer<EMailSetting>().Default != null)
            {

                mail.EmailSettingName = this._settingManager.GetContainer<EMailSetting>().Default.Name;
                mail.FromAddress = this._settingManager.GetContainer<EMailSetting>().Default.FromAddress;
                mail.FromName = this._settingManager.GetContainer<EMailSetting>().Default.FromDisplayName;
                mail.Receivers = user.Email;
                mail.SmtpClientTimeOut = this._settingManager.GetContainer<EMailSetting>().Default.TimeOut;
                mail.SmtpEnableSsl = this._settingManager.GetContainer<EMailSetting>().Default.EnableSsl;
                mail.SmtpHostAddress = this._settingManager.GetContainer<EMailSetting>().Default.Host;
                mail.SmtpPassword = this._settingManager.GetContainer<EMailSetting>().Default.Password;
                mail.SmtpPort = this._settingManager.GetContainer<EMailSetting>().Default.Port;
                mail.SmtpUseDefaultCredentials = this._settingManager.GetContainer<EMailSetting>().Default.DefaultCredentials;
                mail.SmtpUserName = this._settingManager.GetContainer<EMailSetting>().Default.UserName;
                mail.Subject = this._templateService.GetSubject("NewUser", viewBag);
                _mailService.Enqueue(mail);
            }
            UserActivity activity = new UserActivity();
            activity.UserId = user.Id;
            activity.UserActivityType = UserActivityType.UserCreation;
            _dbContext.Set<UserActivity>().Add(activity);
            _dbContext.SaveChanges();
        }

        public void DeleteUser(string userName)
        {
            ASUser user = _userManager.FindByName(userName);

            if (user == null)
                return;

            IList<string> userRoles = _userManager.GetRoles(user.Id);

            foreach (ASUserLogin login in user.Logins)
                _userManager.RemoveLogin(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));

            foreach (string userRole in userRoles)
            {
                _userManager.RemoveFromRole(user.Id, userRole);
            }

            UserActivity activity = new UserActivity();
            activity.UserId = user.Id;
            activity.UserActivityType = UserActivityType.UserDeleted;
            IdentityResult iResult = _userManager.Delete(user);

            if (!iResult.Succeeded)
                throw new ASException(string.Join(";", iResult.Errors));

            _dbContext.Set<RecordAuditLog>().Add(GenerateAuditLog("DELETE", user));
            _dbContext.Set<UserActivity>().Add(activity);
            _dbContext.SaveChanges();
        }

        public bool DoesUserNameExist(string userName)
        {
            return _userManager.FindByName(userName) != null;
        }

        public bool DoesEmailExist(string email)
        {
            return _userManager.FindByEmail(email) != null;
        }

        public void LogOut()
        {
            UserActivity activity = new UserActivity();
            activity.UserId = this._contextProvider.UserId;
            activity.UserActivityType = UserActivityType.LogOut;

            _signInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            _dbContext.Set<UserActivity>().Add(activity);
            _dbContext.SaveChanges();
        }

        public bool Login(string userNameOrEmail, string password, bool isPersistent)
        {
            this.LogOut(); //LogOut first
            _contextProvider.LoginAttemptCount++;
            ASUser user = this._userManager.FindByName(userNameOrEmail);

            if (user == null)
                user = this._userManager.FindByEmail(userNameOrEmail);

            if (user == null)
                throw new ASException(this._resourceManager.GetString("Membership_UserNotFound"));

            SignInStatus status = _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent, false).Result;

            UserActivity activity;
            if (status != SignInStatus.Success)
            {
                activity = new UserActivity();
                activity.UserId = user.Id;
                activity.UserActivityType = UserActivityType.InvalidPasswordEntry;
                _dbContext.Set<UserActivity>().Add(activity);
                _dbContext.SaveChanges();
                throw new ASException(this._resourceManager.GetString("Membership_LoginFailed"));
            }
            var identity = this._userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            this._signInManager.AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

            user.LastLogin = DateTime.UtcNow;
            this._userManager.Update(user);

            activity = new UserActivity();
            activity.UserId = user.Id;
            activity.UserActivityType = UserActivityType.LogIn;
            _dbContext.Set<UserActivity>().Add(activity);
            _dbContext.SaveChanges();
            _contextProvider.LoginAttemptCount = 0;

            return true;
        }

        public Domain.Entities.IUser GetUserByUsername(string userName)
        {
            return _userManager.FindByName(userName);
        }

        public IPagedList<Domain.Entities.IUser> GetUsers(int pageIndex, int pageSize, string ordering, DateTime? lastActivityFrom, DateTime? lastActivityTo, string userName = null, string email = null)
        {
            var query = this._dbContext.Set<ASUser>().Include("Roles").AsNoTracking() as IQueryable<ASUser>;

            if (lastActivityFrom != null)
                query = query.Where(user => user.LastActivity >= lastActivityFrom.Value);
            if (lastActivityTo != null)
                query = query.Where(user => user.LastActivity <= lastActivityTo.Value);

            query = query.Where(user => string.IsNullOrEmpty(userName) || user.UserName.Contains(userName));
            query = query.Where(user => string.IsNullOrEmpty(email) || user.Email.Contains(email));

            if (!string.IsNullOrEmpty(ordering))
            {
                query = query.OrderBy(ordering);
            }

            return new PagedList<Domain.Entities.IUser>(query, pageIndex, pageSize);
        }

        public void ChangePassword(int userId, string currentPassword, string newPassword)
        {
            ASUser user = this._userManager.FindById(userId);

            SignInStatus status = _signInManager.PasswordSignInAsync(user.UserName, currentPassword, false, false).Result;

            if (status != SignInStatus.Success)
                throw new ASException(this._resourceManager.GetString("Membership_LoginFailed"));

            string token = _userManager.GeneratePasswordResetToken(userId);
            IdentityResult iResult = _userManager.ResetPassword(userId, token, newPassword);

            if (!iResult.Succeeded)
                throw new ASException(string.Join(";", iResult.Errors));

            UserActivity activity = new UserActivity();
            activity.UserId = user.Id;
            activity.UserActivityType = UserActivityType.PasswordChange;
            _dbContext.Set<UserActivity>().Add(activity);
        }

        //Role
        public void CreateRole(string role, string note)
        {
            ASRole asRole = new ASRole(role);
            asRole.Note = note;
            asRole.CreatedOn = DateTime.UtcNow;
            asRole.CreatedBy = this._contextProvider.UserName;

            IdentityResult iResult = _roleManager.CreateAsync(asRole).Result;

            if (!iResult.Succeeded)
                throw new ASException(string.Join(";", iResult.Errors));
        }

        public void DeleteRole(string role)
        {
            ASRole asRole = _roleManager.FindByName(role);

            if (asRole != null)
            {
                if (this._dbContext.Set<ASUserRole>().Any(u => u.RoleId == asRole.Id))
                {
                    throw new ASException(this._resourceManager.GetString("Roles_CannotBeDeletedRoleHasUsers"), role);
                }
                IdentityResult iResult = _roleManager.Delete(asRole);
                if (!iResult.Succeeded)
                    throw new ASException(string.Join(";", iResult.Errors));
                _dbContext.Set<RecordAuditLog>().Add(GenerateAuditLog("DELETE", asRole));
                _dbContext.SaveChanges();
            }
        }

        public Domain.Entities.IRole GetRoleByName(string roleName)
        {
            return _roleManager.FindByName(roleName);
        }

        public IList<Domain.Entities.IRole> GetRoles(string ordering)
        {
            var query = this._dbContext.Set<ASRole>().AsNoTracking() as IQueryable<Domain.Entities.IRole>;

            if (!string.IsNullOrEmpty(ordering))
            {
                query = query.OrderBy(ordering);
            }

            return query.ToList();
        }

        public void UpdateRole(int id, string roleName, string note)
        {
            ASRole role = _roleManager.FindById(id);

            if (role == null)
                throw new ASException(this._resourceManager.GetString("Roles_NotExists"));
            role.Name = roleName;
            role.Note = note;
            role.ModifiedOn = DateTime.UtcNow;
            role.ModifiedBy = this._contextProvider.UserName;

            IdentityResult iResult = _roleManager.Update(role);
            if (!iResult.Succeeded)
                throw new ASException(string.Join(";", iResult.Errors));
        }

        public IPagedList<UserActivity> GetUserActivities(int pageIndex, int pageSize, int userId)
        {
            var query = this._dbContext.Set<UserActivity>().AsNoTracking() as IQueryable<UserActivity>;
            query = query.Where(a => a.UserId == userId);
            query = query.OrderByDescending(a => a.CreatedOn);

            return new PagedList<UserActivity>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Normally we automatically generate this log in DbContext.SaveChanges  function.
        /// However, Identity Framework delete functions does not invoke our custom SaveChanges
        /// method.
        /// </summary>
        /// <param name="operation">Name of operation DELETE/UPDATE ..etc</param>
        /// <param name="entity">Entity to be logged</param>
        /// <returns>RecordAuditLog object to be inserted.</returns>
        private RecordAuditLog GenerateAuditLog(string operation, object entity)
        {
            RecordAuditLog log = new RecordAuditLog();
            log.EntityName = entity.GetType().Name;
            log.Operation = operation;
            log.Content = this._xmlSerializer.SerializeToXML(entity);
            log.CreatedBy = this._contextProvider.UserName;
            log.CreatedOn = DateTime.UtcNow;

            return log;
        }
    }
}