namespace AS.Domain.Entities
{
    /// <summary>
    /// User Activities to be used to audit activities of a user
    /// </summary>
    public enum UserActivityType
    {
        UserCreation = 1,
        LogIn,
        LogOut,
        InvalidPasswordEntry,
        PasswordResetRequest,
        PasswordReset,
        PasswordChange,
        UserActivation,
        UserDeactivation,
        UserDeleted
    }
}