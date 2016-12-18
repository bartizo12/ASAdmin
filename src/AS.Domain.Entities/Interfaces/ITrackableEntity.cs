using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Marks a trackable entity.ITrackable entity contains ModifiedOn and ModifiedBy properties as addition.
    /// Trackable entity is extended from <see cref="IEntity"/>.
    /// Note that ModifiedOn and ModifiedBy fields will be automatically populated at data access layer for
    /// the classes that implements this interface.
    /// </summary>
    public interface ITrackableEntity : IEntity
    {
        /// <summary>
        /// Datetime when entity is modified/updated
        /// </summary>
        DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Authorized user who modified/updated the entity
        /// </summary>
        string ModifiedBy { get; set; }
    }
}