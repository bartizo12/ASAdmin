namespace AS.Domain.Entities
{
    /// <summary>
    /// Interface for Role entity. This implementation makes it easy to integrate a third party
    /// membership API/Framework(such as ASP.NET Identity Framework) to our application. It is used
    /// to make our Layers loosely coupled.
    /// </summary>
    public interface IRole : ITrackableEntity
    {
        /// <summary>
        /// Note/Comment
        /// </summary>
        string Note { get; set; }

        /// <summary>
        /// Name of the Role
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Id of the Role
        /// </summary>
        int Id { get; set; }
    }
}