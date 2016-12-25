using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Interface for entity classes. We will have CreatedOn and CreatedBy properties in all entities
    /// to keep basic details of the entities.
    /// Note that CreatedOn and CreatedBy fields will be automatically populated at data access layer for
    /// the classes that implements this interface.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Datetime when entity is created/inserted
        /// </summary>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// Authorized user who created/inserted the entity
        /// </summary>
        string CreatedBy { get; set; }
    }
}