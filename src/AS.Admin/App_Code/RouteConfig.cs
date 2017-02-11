using System.Web.Mvc;
using System.Web.Routing;

namespace AS.Admin
{
    internal static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("tsconfig.json");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.Ignore("tsconfig.json");
            routes.MapRoute(
                "ForgotPassword",
                "ForgotPassword/",
                new { controller = "Identity", action = "ForgotPassword" });
            routes.MapRoute(
                "Login",
                "Login/",
                new { controller = "Identity", action = "Login" });

            routes.MapRoute(
                name: "Google API Sign-in",
                url: "login-google",
                defaults: new { controller = "Identity", action = "GoogleLoginCallbackRedirect" });

            routes.MapRoute(
                "Logout",
                "Logout/",
             new { controller = "Identity", action = "Logout" });
            routes.MapRoute(
                "ResetPassword",
                "ResetPassword/",
                new { controller = "Identity", action = "ResetPassword" });

            routes.MapRoute(
                "404",
                "404/",
                new { controller = "Error", action = "FourOFour" });
            routes.MapRoute(
               "500",
               "500/",
               new { controller = "Error", action = "FiveOO" });
            routes.MapRoute(
                "Ping",
                "Ping/",
                new { controller = "Shared", action = "Ping" });

            routes.MapRoute(
                "Default", //Route Name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Identity", action = "Login", id = UrlParameter.Optional },
                new[] { "AS.Admin.Controllers" }
            );
        }
    }
}