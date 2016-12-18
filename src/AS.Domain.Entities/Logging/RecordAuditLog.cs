using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Used to audit entity operations. At the present ,we only audit DELETEs.
    /// If an entity is deleted and it is not safe to delete , entity is serialized
    /// and stored as this log entity
    /// </summary>
    [Serializable]
    public class RecordAuditLog : EntityBase<long>, ISafeToDeleteEntity
    {
        /// <summary>
        /// Name of the operation. (DELETE...etc)
        /// </summary>
        public string Operation { get; set; }

        public string EntityName { get; set; }

        /// <summary>
        /// Serialized content of the entity
        /// </summary>
        public string Content { get; set; }
    }
}