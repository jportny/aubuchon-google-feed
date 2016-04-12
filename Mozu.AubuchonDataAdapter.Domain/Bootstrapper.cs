using System.Reflection;
using Autofac;
using Mozu.Api.Events;
using Mozu.AubuchonDataAdapter.Domain.Events;
using Quartz;
using Mozu.AubuchonDataAdapter.Domain.Handlers.Excel;
using IEventProcessor = Mozu.AubuchonDataAdapter.Domain.Events.IEventProcessor;
using OrderEventProcessor = Mozu.AubuchonDataAdapter.Domain.Events.OrderEventProcessor;

namespace Mozu.AubuchonDataAdapter.Domain
{
    public class Bootstrapper
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Events.EventService>().As<IEventService>();

            containerBuilder.RegisterType<ProductInventoryEventProcessor>().Named<Api.ToolKit.Events.IEventProcessor>("productinventory");
            
            //containerBuilder.RegisterType<ApplicationEvents>().As<IApplicationEvents>();
            //containerBuilder.RegisterType<ProductEvents>().As<IProductEvents>();
            //containerBuilder.RegisterType<CustomerAccountEvents>().As<ICustomerAccountEvents>();
            //containerBuilder.RegisterType<OrderEvents>().As<IOrderEvents>();
            //containerBuilder.RegisterType<AubuchonAccountHandler>().As<IAubuchonAccountHandler>();
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(x => typeof(IJob).IsAssignableFrom(x));
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(x => typeof(IInterruptableJob).IsAssignableFrom(x));
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(t => t.Name.EndsWith("Events"))
               .AsImplementedInterfaces();

            containerBuilder.RegisterType<ProductInventoryEvents>().As<IProductInventoryEvents>();
            containerBuilder.RegisterType<GenericEventProcessor>().As<IEventProcessor>();

            containerBuilder.RegisterType<AccountEventProcessor>().Named<IEventProcessor>("customeraccount.created");
            containerBuilder.RegisterType<AccountEventProcessor>().Named<IEventProcessor>("customeraccount.updated");
            containerBuilder.RegisterType<OrderEventProcessor>().Named<IEventProcessor>("order.opened");
            containerBuilder.RegisterType<OrderEventProcessor>().Named<IEventProcessor>("order.updated");
            
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(t => t.Name.EndsWith("Handler"))
               .AsImplementedInterfaces();

            containerBuilder.RegisterType<ImportExportLocationMZExtras>().As<IImportExportLocationMZExtras>();



            Integration.Scheduler.Bootstrapper.Register(containerBuilder);
        }
    }
}
