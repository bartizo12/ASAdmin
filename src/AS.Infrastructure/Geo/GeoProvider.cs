using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace AS.Infrastructure
{
    /// <summary>
    /// Contains geolocation related functions
    /// </summary>
    public class GeoProvider : IGeoProvider
    {
        private readonly ISettingManager _settingManager;

        public GeoProvider(ISettingManager settingManaager)
        {
            this._settingManager = settingManaager;
        }

        /// <summary>
        /// Queries IP Address at ipinfo database. Note that, URL and API Key must be provided by settings
        /// </summary>
        /// <param name="ipAddress">IP address to be queried</param>
        /// <returns>Country info of the ipAddress</returns>
        public Country GetCountryInfo(string ipAddress)
        {
            Country countryInfo = Country.Empty;

            if (ipAddress == IPAddress.Loopback.ToString())
                return countryInfo;

            var urlAddresses = this._settingManager.GetContainer<UrlAddress>();

            if (!urlAddresses.Contains("IPCountryQueryUrl") ||
                string.IsNullOrEmpty(urlAddresses["IPCountryQueryUrl"].Address))
            {
                return countryInfo;
            }
            using (WebClient client = new WebClient())
            {
                string urlFormat = urlAddresses["IPCountryQueryUrl"].Address;
                string url = urlFormat.Replace("{{ip}}", ipAddress);
                string jsonResult = client.DownloadString(url);
                JObject ipCountryInfo = JObject.Parse(jsonResult);

                if (ipCountryInfo != null && ipCountryInfo["status"].ToString() == "fail")
                    throw new ASException(ipCountryInfo["message"].ToString());

                string code = ipCountryInfo["countryCode"].ToString();
                string country = ipCountryInfo["country"].ToString();
                countryInfo = new Country(code, country);
            }
            return countryInfo;
        }
    }
}