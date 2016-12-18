using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Data.EF;
using System.Data.Entity.Infrastructure.Interception;

namespace AS.Admin
{
    internal static class EFQueryLoggingConfig
    {
        public static void Configure()
        {
            var appSettings = ServiceLocator.Current.Resolve<ISettingManager>().GetContainer<AppSetting>();
            appSettings.SettingCacheLoaded += AppSettings_SettingCacheLoaded;
        }

        private static void AppSettings_SettingCacheLoaded(object sender, SettingsCacheLoadedEventArgs e)
        {
            if (e.SettingValues.Contains("DbQueryLogEnable"))
            {
                ASDbCommandInterceptor interceptor = ServiceLocator.Current.Resolve<ASDbCommandInterceptor>();

                if (bool.Parse((e.SettingValues["DbQueryLogEnable"] as AppSetting).Value))
                {
                    DbInterception.Add(interceptor);
                }
                else
                {
                    DbInterception.Remove(interceptor);
                }
            }
        }
    }
}