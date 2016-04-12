using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Mozu.Api.WebToolKit;

namespace Mozu.AubuchonDataAdapter.Web
{
    public class Bootstrapper : AbstractWebApiBootstrapper
    {
        public override void InitializeContainer(ContainerBuilder containerBuilder)
        {
            //Mozu.Api.MozuConfig.EventTimeoutInSeconds = 3600; //TEMP to increase the timeout of Order request. REMOVE when moving to Prod

            base.InitializeContainer(containerBuilder);

            containerBuilder.RegisterControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            Domain.Bootstrapper.Register(containerBuilder);
            Mozu.Integration.Scheduler.Bootstrapper.Register(containerBuilder);
            
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
           
        }

    }
}