using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Password reset token that is created when user requests to reset his/her password
    /// </summary>
    public class PasswordResetToken : EntityBase<int>
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedOn { get; set; }
        public virtual EMail EMail { get; set; }
        public int EMailId { get; set; }

        public PasswordResetToken()
        {
            this.IsUsed = false;
        }
    }
}