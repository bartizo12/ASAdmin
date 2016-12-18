namespace AS.Domain.Entities
{
    /// <summary>
    /// Represents a pair. Similar to <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}"/>
    /// except this one is a class.
    /// </summary>
    /// <typeparam name="TKey">Type of Key</typeparam>
    /// <typeparam name="TValue">Type of Value</typeparam>
    public class Pair<TKey, TValue>
    {
        public TKey Key { get; protected set; }
        public TValue Value { get; protected set; }

        public Pair()
        {
        }

        public Pair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}