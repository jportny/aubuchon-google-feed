
using Autofac;
using System.Reflection;
using Quartz;
using Mozu.Api.ToolKit;

namespace Mozu.AubuchonDataAdapter.Service
{
    public class Bootstrapper : AbstractBootstrapper
    {
        public override void InitializeContainer(ContainerBuilder containerBuilder)
        {
            Domain.Bootstrapper.Register(containerBuilder);
            //containerBuilder.RegisterType<QuartzJobScheduler>().As<IJobScheduler>().SingleInstance();
            //containerBuilder.RegisterType<SqlLogHandler>().As<ILogHandler>();
            //containerBuilder.RegisterType<EntityLogHandler>().As<ILogHandler>();
            Integration.Scheduler.Bootstrapper.Register(containerBuilder);
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(x => typeof(IJob).IsAssignableFrom(x));
            //containerBuilder.RegisterType<TestManager>().As<ITestManager>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
        }
    }
}
