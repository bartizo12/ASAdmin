using System;
using System.Collections.Generic;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Base class of the all the entities.Contains crucial properties of the entity.Other entity classes
    /// shall inherit from either this class or <seealso cref="TrackableEntityBase{TId}"/>.
    /// Based on the article : http://enterprisecraftsmanship.com/2014/11/08/domain-object-base-class/
    /// </summary>
    /// <typeparam name="TId">Generic type of ID</typeparam>
    [Serializable]
    public abstract partial class EntityBase<TId> : IEntity where TId : struct
    {
        /// <summary>
        /// ID of the entity
        /// </summary>
        public virtual TId Id { get; set; }

        /// <summary>
        /// When entity is created/inserted
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        /// <summary>
        /// Authorized user who created/inserted the entity
        /// </summary>
        public virtual string CreatedBy { get; set; }

        protected virtual Type RealType
        {
            get
            {
                return GetType();
            }
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as EntityBase<TId>;

            if (ReferenceEquals(compareTo, null))
                return false;

            if (ReferenceEquals(this, compareTo))
                return true;

            if (this.RealType != compareTo.RealType)
                return false;

            if (!IsTransient() && !compareTo.IsTransient() && EqualityComparer<TId>.Default.Equals(Id, compareTo.Id))
                return true;

            return false;
        }

        public virtual bool IsTransient()
        {
            return EqualityComparer<TId>.Default.Equals(Id, default(TId));
        }

        public override int GetHashCode()
        {
            return (RealType.ToString() + Id).GetHashCode();
        }

        public static bool operator ==(EntityBase<TId> a, EntityBase<TId> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityBase<TId> x, EntityBase<TId> y)
        {
            return !(x == y);
        }
    }
}