namespace AS.Domain.Entities
{
    /// <summary>
    /// User activity audit log entity
    /// </summary>
    public class UserActivity : EntityBase<int>
    {
        public int UserId { get; set; }
        public UserActivityType UserActivityType { get; set; }
    }
}