using Autofac;
using System.Reflection;

namespace AS.Jobs
{
    public sealed class CompositionRoot : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Register Jobs
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
              .Where(t => !t.IsAbstract && t.BaseType == typeof(JobBase))
              .AsSelf()
              .InstancePerLifetimeScope();
        }
    }
}