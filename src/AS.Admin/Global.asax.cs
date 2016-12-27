using AS.Infrastructure;
using AS.Infrastructure.Logging;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using AS.Services.Interfaces;
using FluentValidation.Mvc;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AS.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MiniProfilerEF6.Initialize();
            string rootPath = HostingEnvironment.MapPath("~/App_Data/");

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            MvcHandler.DisableMvcResponseHeader = true;
            MappingConfig.RegisterMappings();
            GlobalFilters.Filters.Add(new CompressContentAttribute());
            var serviceLocator = new ServiceLocatorWeb();
            ServiceLocator.SetServiceLocator(serviceLocator);
            LoggingConfigurator.ConfigureNLog(serviceLocator);
            ModelMetadataProviders.Current = new ASMetadataProvider();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            EFQueryLoggingConfig.Configure();

            //Fluent Validation
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new ValidatorFactory())
            {
                AddImplicitRequiredValidator = false
            });
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            DependencyResolver.Current.GetService<IInstallerService>().Install();
        }
        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}