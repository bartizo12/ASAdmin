using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Membership service Interface to deal with all membership related functionality
    /// Create/Delete/Update/Read users and roles
    /// </summary>
    public interface IMembershipService : IService
    {
        //Role Functions
        void CreateRole(string role, string note);

        void DeleteRole(string role);

        IList<IRole> GetRoles(string ordering);

        IRole GetRoleByName(string roleName);

        void UpdateRole(int id, string roleName, string note);

        //User Functions
        void CreateUser(string userName, string password, string email, List<string> roles);

        void DeleteUser(string userName);

        bool DoesUserNameExist(string userName);

        bool DoesEmailExist(string email);

        /// <summary>
        /// Starts "I forgot My Password" flow. Sends users e-mail address a link with a token to reset his/her password
        /// </summary>
        /// <param name="userNameOrEmail">Username or e-mail address of the user</param>
        /// <returns>Generated token</returns>
        PasswordResetToken StartForgotPasswordProcess(string userNameOrEmail);

        bool ResetPassword(string token, string newPassword);

        bool ResetPasswordWithoutToken(string userName, string newPassword); //For admin

        bool ValidateToken(string token);

        void ChangePassword(int userId, string currentPassword, string newPassword);

        bool Login(string userNameOrEmail, string password, bool isPersistent);

        void LogOut();

        IUser GetUserByUsername(string userName);

        IPagedList<UserActivity> GetUserActivities(int pageIndex, int pageSize, int userId);

        IPagedList<IUser> GetUsers(int pageIndex, int pageSize, string ordering, DateTime? lastActivityFrom, DateTime? lastActivityTo, string userName = null, string email = null);
    }
}