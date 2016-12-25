using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Represents country.
    /// Key : ISO 3166 code of the country
    /// Value : Name of the country
    /// </summary>
    [Serializable]
    public class Country : Pair<string, string> 
    {
        public static Country Empty
        {
            get
            {
                return new Country(string.Empty, string.Empty);
            }
        }

        public Country(string key, string value)
            : base(key, value) { }

        public override string ToString()
        {
            return Key ?? string.Empty + ";" + Value ?? string.Empty;
        }
        public override bool Equals(object obj)
        {
            Country other = obj as Country;

            if (other == null)
                return false;

            return this.Key == other.Key;
        }
        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public static bool operator ==(Country first,Country second)
        {
            if (object.ReferenceEquals(first,null) || object.ReferenceEquals(second,null))
                return false;
            return first.Key == second.Key;
        }
        public static bool operator !=(Country first, Country second)
        {
            if (object.ReferenceEquals(first, null) || object.ReferenceEquals(second, null))
                return true;
            return first.Key != second.Key;
        }
    }
}