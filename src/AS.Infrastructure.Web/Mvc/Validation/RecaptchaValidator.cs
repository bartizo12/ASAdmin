using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure.Validation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Repatcha validator.
    /// Requires following values to be provied in AppSetting
    /// UrlAddress  --> RecaptchaUrl : Recaptcha Url to validate
    /// AppSetting --> RecaptchaPrivateKey : Recaptcha API Private Key
    /// Validation is done at recaptcha server side.
    /// </summary>
    public class RecaptchaValidator : IValidator<string>
    {
        private readonly ILogger _logger;
        private readonly ISettingManager _settingManager;
        private readonly IContextProvider _contextProvider;
        private readonly IResourceManager _resourceManager;

        public RecaptchaValidator(ISettingManager settingManager
            , ILogger logger
            , IResourceManager resourceManager
            , IContextProvider contextProvider)
        {
            this._settingManager = settingManager;
            this._logger = logger;
            this._contextProvider = contextProvider;
            this._resourceManager = resourceManager;
        }

        public IValidationResult Validate(string value)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(value))
            {
                errors.Add(_resourceManager.GetString("RecaptchaInvalidResponse"));
                return new ValidationResult(false,errors);
            }
            if (!_settingManager.GetContainer<UrlAddress>().Contains("RecaptchaUrl") || !_settingManager.GetContainer<AppSetting>().Contains("RecaptchaPrivateKey"))
            {
                errors.Add(_resourceManager.GetString("RecaptchaSettingsMissing"));
                return new ValidationResult(false, errors);
            }

            string url = string.Format(_settingManager.GetContainer<UrlAddress>()["RecaptchaUrl"].Address,
                _settingManager.GetContainer<AppSetting>()["RecaptchaPrivateKey"].Value, value, _contextProvider.ClientIP);

            HttpClient httpClient = new HttpClient();
            bool isValid = false;

            try
            {
                var serverResponse = httpClient.GetAsync(url).Result.EnsureSuccessStatusCode();
                string responseString = serverResponse.Content.ReadAsStringAsync().Result;

                var resultObject = JObject.Parse(responseString);

                if (resultObject.Value<bool>("success"))
                    isValid = true;

                if (resultObject.Value<JToken>("error-codes") != null &&
                    resultObject.Value<JToken>("error-codes").Values<string>().Any())
                {
                    string error = string.Join(",", resultObject.Value<JToken>("error-codes").Values<string>());
                    errors.Add(error);
                    _logger.Warn("Invalid recaptcha error : {0}", error);
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex);
                errors.Add(_resourceManager.GetString("RecaptchaSystemError"));
            }
            finally
            {
                httpClient.Dispose();
            }
            return new ValidationResult(isValid, errors);
        }
    }
}