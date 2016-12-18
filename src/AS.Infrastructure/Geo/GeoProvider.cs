using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
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
            var appSettings = this._settingManager.GetContainer<AppSetting>();

            if (!urlAddresses.Contains("IPCountryQueryUrl") ||
                string.IsNullOrEmpty(urlAddresses["IPCountryQueryUrl"].Address) ||
                !appSettings.Contains("IPInfoDbApiKey") ||
                string.IsNullOrEmpty(appSettings["IPInfoDbApiKey"].Value))
            {
                return countryInfo;
            }
            using (WebClient client = new WebClient())
            {
                string urlFormat = urlAddresses["IPCountryQueryUrl"].Address;
                string url = urlFormat.Replace("{{apiKey}}", appSettings["IPInfoDbApiKey"].Value).Replace("{{ip}}", ipAddress);
                string queryResult = client.DownloadString(url);
                string[] parts = queryResult.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 1 && parts[0] == "ERROR")
                    throw new ASException(parts[1]);

                countryInfo = new Country(parts[parts.GetLength(0) - 2], parts[parts.GetLength(0) - 1]);
            }
            return countryInfo;
        }
    }
}