namespace AS.Domain.Interfaces
{
    /// <summary>
    /// Simple Hashtable interface
    /// Key is string ,while Value is generic type.
    /// </summary>
    /// <typeparam name="TValue">Generic type of value</typeparam>
    public interface IHashTable<TValue>
    {
        TValue this[string key] { get; }
    }
}