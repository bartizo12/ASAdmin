using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Domain.Settings;
using AS.Infrastructure;
using AS.Infrastructure.Data;
using AS.Infrastructure.Data.EF;
using AS.Infrastructure.Identity;
using AS.Infrastructure.IO;
using AS.Infrastructure.Logging;
using AS.Infrastructure.Web;
using AS.Infrastructure.Web.Identity;
using AS.Infrastructure.Web.Module;
using AS.Infrastructure.Web.Mvc;
using AS.Infrastructure.Web.Mvc.Filters;
using AS.Services;
using AS.Services.Interfaces;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace AS.Admin
{
    public sealed class CompositionRoot : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationStorageManager>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.Register(c =>
            {
                return c.Resolve<ConfigurationStorageManager>().GetConfigurationFactory();
            })
            .AsSelf()
            .SingleInstance();

            builder.Register(c =>
            {
                return c.Resolve<ConfigurationStorageManager>().GetConfigurationFactory() as Func<DbConnectionConfiguration>;
            })
            .AsSelf()
            .SingleInstance();

            builder.RegisterType<NLogCustomTarget>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<ASXmlSerializer>()
                .As<IXmlSerializer>()
                .SingleInstance();
            builder.RegisterType<ASDatabaseInitializer<ASDbContext>>()
                 .As<IDatabaseInitializer<ASDbContext>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ASDbContext>()
                .As<IDbContext>()
                .As<DbContext>()
                .InstancePerRequest();
            builder.RegisterType<SettingDataProvider>()
                 .As<ISettingDataProvider>()
                 .InstancePerLifetimeScope();
            builder.RegisterType<SettingManager>()
                .As<ISettingManager>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<WebContextProvider>()
                .As<IContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<WebAppManager>()
                .As<IAppManager>()
                .As<IFileManager>()
                .SingleInstance();
            builder.Register(c => (new HttpContextWrapper(HttpContext.Current) as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerRequest();
            builder.RegisterType<GeoProvider>()
                .As<IGeoProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ASDbContextFactory>()
                .As<IDbContextFactory>().InstancePerLifetimeScope();
            builder.RegisterType<NLogger>()
                .As<ILogger>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RequestDbLogger>()
               .As<IRequestLogger>()
               .InstancePerRequest();
            builder.RegisterType<ASHttpModule>()
                .As<IHttpModule>();
            builder.RegisterType<ASUserStore>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<ASUserManager>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<ASRoleStore>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<ASRoleManager>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<ASSignInManager>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<ASDataProtectionProvider>()
                .As<IDataProtectionProvider>()
               .InstancePerLifetimeScope();
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication)
                .As<IAuthenticationManager>()
                .InstancePerLifetimeScope();
            builder.RegisterType<Infrastructure.Data.Database>()
                    .As<IDatabase>()
                    .InstancePerRequest();
            builder.RegisterType<StatisticsProvider>()
                    .As<IStatisticsProvider>()
                    .InstancePerRequest();
            builder.RegisterType<LastActivityTimeUpdater>()
              .As<ILastActivityTimeUpdater>()
              .InstancePerRequest();
            builder.RegisterType<ASDbCommandInterceptor>()
                .AsSelf()
                .InstancePerRequest();
            builder.RegisterType<RecaptchaValidator>()
                .AsSelf()
                .InstancePerRequest();
            builder.RegisterType<ResourceManager>()
                .As<IResourceManager>()
                .InstancePerRequest();
            builder.RegisterType<AESEncryptionProvider>()
                .As<IEncryptionProvider>()
                .InstancePerRequest();

            //Register Services
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                   .Where(t => typeof(IService).IsAssignableFrom(t))
                   .AsImplementedInterfaces();
            //FluentValidation Validators
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType
                       && i.GetGenericTypeDefinition() == typeof(FluentValidation.IValidator<>)))
               .AsImplementedInterfaces()
               .AsSelf()
               .InstancePerLifetimeScope();
            //MVC Specific Registration
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.Register(c => new GlobalValuesActionFilterAttribute(c.Resolve<ISettingManager>(),
               c.Resolve<IContextProvider>(), c.Resolve<IResourceManager>()))
                            .AsActionFilterFor<Controller>().InstancePerRequest();
            builder.Register(c => new ConfigurationRedirectAttribute(c.Resolve<Func<ASConfiguration>>()))
                          .AsActionFilterFor<Controller>().InstancePerRequest();
            builder.Register(c => new LastActivityUpdateActionFilter(c.Resolve<ILastActivityTimeUpdater>()))
                          .AsActionFilterFor<Controller>().InstancePerRequest();
            builder.Register(c => new ExceptionHandleFilterAttribute(c.Resolve<ILogger>()))
                          .AsActionFilterFor<Controller>().InstancePerRequest();

            builder.RegisterFilterProvider();
            builder.RegisterSource(new ViewRegistrationSource());
        }
    }
}