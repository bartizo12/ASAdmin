using AS.Domain.Settings;
using AS.Infrastructure.Web.Optimization;
using System.Web.Mvc;
using System.Web.Optimization;

namespace AS.Admin
{
    internal static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //css bundles
            BundleTable.Bundles.Add(new StyleBundle("~/content/asCss")
            .Include("~/Content/bootstrap/dist/css/bootstrap.css", new ASCssTransform())
            .Include("~/Content/bootstrap3-dialog/dist/css/bootstrap-dialog.css", new ASCssTransform())
            .Include("~/Content/admin-lte/dist/css/AdminLTE.css", new ASCssTransform())
            .Include("~/Content/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.css", new ASCssTransform())
            .Include("~/Content/fancybox/source/jquery.fancybox.css", new ASCssTransform())
            .Include("~/Content/font-awesome/css/font-awesome.css", new ASCssTransform())
            .Include("~/Content/admin-lte/dist/css/skins/skin-blue.css", new ASCssTransform())
            .Include("~/Content/iCheck/skins/square/blue.css", new ASCssTransform())
            .Include("~/Content/bootstrap-select/dist/css/bootstrap-select.css", new ASCssTransform())
            .Include("~/Content/DataTables/datatables.css", new ASCssTransform())
            .Include("~/Content/AS/css/styles.css", new ASCssTransform())
            );

            //js bundles
            BundleTable.Bundles.Add(new ScriptBundle("~/content/thirdPartyJs")
            .Include("~/Content/jquery/dist/jquery.js")
            .Include("~/Content/moment/moment.js")
            .Include("~/Content/bootstrap/dist/js/bootstrap.js")
            .Include("~/Content/bootstrap3-dialog/dist/js/bootstrap-dialog.js")
            .Include("~/Content/admin-lte/dist/js/app.js")
            .Include("~/Content/eonasdan-bootstrap-datetimepicker/src/js/bootstrap-datetimepicker.js")
            .Include("~/Content/fancybox/source/jquery.fancybox.js")
            .Include("~/Content/iCheck/icheck.js")
            .Include("~/Content/bootstrap-select/dist/js/bootstrap-select.js")
            .Include("~/Content/pace/pace.js")

            .Include("~/Content/DataTables/datatables.min.js")
           );

            BundleTable.Bundles.Add(new ScriptBundle("~/content/asJs")
            .Include("~/Content/AS/script/asframe.js"));
            BundleTable.Bundles.FileExtensionReplacementList.Clear();

            ISettingContainer<AppSetting> appSettings = DependencyResolver.Current
                                                                          .GetService<ISettingManager>()
                                                                          .GetContainer<AppSetting>();        

            if (appSettings.Contains("BundlingEnabled"))
            {
                BundleTable.EnableOptimizations = bool.Parse(appSettings["BundlingEnabled"].Value);
            }
            else
            {
#if DEBUG
                BundleTable.EnableOptimizations = false;
#else
                BundleTable.EnableOptimizations = true;
#endif
            }
            appSettings.SettingCacheLoaded += BundleConfig_SettingCacheLoaded;
        }

        private static void BundleConfig_SettingCacheLoaded(object sender, SettingsCacheLoadedEventArgs e)
        {
            if (e.SettingValues.Contains("BundlingEnabled"))
            {
                BundleTable.EnableOptimizations = bool.Parse((e.SettingValues["BundlingEnabled"] as AppSetting).Value);
            }
        }
    }
}