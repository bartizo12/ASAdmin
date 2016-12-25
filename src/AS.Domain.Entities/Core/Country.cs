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
    }
}