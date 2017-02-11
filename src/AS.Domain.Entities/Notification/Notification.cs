using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// User notification.
    /// </summary>
    [Serializable]
    public class Notification : EntityBase<int>
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool IsSeen { get; set; }
        public string Url { get; set; }
        public DateTime? SeenOn { get; set; }

        public Notification()
        {
            this.IsSeen = false;
        }
        public Notification(int userId, string message, string url)
        {
            this.IsSeen = false;
            this.UserId = userId;
            this.Message = message;
            this.Url = url;
        }
    }
}