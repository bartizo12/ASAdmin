namespace AS.Domain.Entities
{
    /// <summary>
    /// Interface to mark  safe to delete entities. A "Safe to Delete" entity is the entity
    /// that is deleted without logging. Non-SafeToDelete entities are serialized and logged
    /// to avoid data loss .In case of wrong delete, these entities can be deserialized back and
    /// restored
    /// <seealso cref="RecordAuditLog"/>
    /// </summary>
    public interface ISafeToDeleteEntity : IEntity
    {
    }
}