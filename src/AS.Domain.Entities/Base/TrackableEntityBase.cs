using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Base class for trackable entities.In addition to <see cref="EntityBase{TId}"/>,
    /// Trackable entity contains ModifiedOn and ModifiedBy properties which are populated by DataAccess layer
    /// </summary>
    /// <typeparam name="TId">Generic ID type</typeparam>
    [Serializable]
    public abstract class TrackableEntityBase<TId> : EntityBase<TId>, ITrackableEntity
        where TId : struct
    {
        /// <summary>
        /// Datetime when entity is modified/updated
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Authorized user who modified/updated the entity
        /// </summary>
        public string ModifiedBy { get; set; }
    }
}