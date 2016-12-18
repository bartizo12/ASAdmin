using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System;

namespace AS.Infrastructure.Logging
{
    public abstract class BaseLogger
    {
        private readonly ISettingManager _settingManager;

        public BaseLogger(ISettingManager settingManager)
        {
            this._settingManager = settingManager;
        }

        public LogLevel MinimumLogLevel
        {
            get
            {
                if (!_settingManager.GetContainer<AppSetting>().Contains("MinLogLevel"))
                    return LogLevel.Debug;

                return _settingManager.GetContainer<AppSetting>()["MinLogLevel"].Value.ToEnum(LogLevel.Debug);
            }
        }
    }
}