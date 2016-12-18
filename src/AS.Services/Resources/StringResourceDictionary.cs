using AS.Domain.Entities;
using System.Collections.Generic;

namespace AS.Services
{
    /// <summary>
    /// A multiple key dictionary  to keep string resources
    /// </summary>
    internal class StringResourceDictionary
    {
        private Dictionary<string, Dictionary<string, StringResource>> dict;

        public StringResourceDictionary()
        {
            dict = new Dictionary<string, Dictionary<string, StringResource>>();
        }

        public IEnumerable<string> GetAvaliableCultures()
        {
            return dict.Keys;
        }

        public IEnumerable<StringResource> GetAll(string cultureCode)
        {
            if (dict.ContainsKey(cultureCode))
            {
                return dict[cultureCode].Values;
            }
            else
            {
                return new List<StringResource>();
            }
        }

        public bool Contains(string cultureCode, string name)
        {
            if (!string.IsNullOrEmpty(cultureCode) && dict.ContainsKey(cultureCode))
            {
                var innerDict = dict[cultureCode];

                if (innerDict != null && !string.IsNullOrEmpty(name) && innerDict.ContainsKey(name))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetString(string cultureCode, string name)
        {
            if (!string.IsNullOrEmpty(cultureCode) && dict.ContainsKey(cultureCode))
            {
                var innerDict = dict[cultureCode];

                if (innerDict != null && !string.IsNullOrEmpty(name) && innerDict.ContainsKey(name))
                {
                    return innerDict[name].Value;
                }
            }
            return name;
        }

        public void Add(StringResource stringResource)
        {
            if (!this.dict.ContainsKey(stringResource.CultureCode))
            {
                this.dict.Add(stringResource.CultureCode, new Dictionary<string, StringResource>());
            }
            if (!this.dict[stringResource.CultureCode].ContainsKey(stringResource.Name))
            {
                this.dict[stringResource.CultureCode].Add(stringResource.Name, stringResource);
            }
        }
    }
}