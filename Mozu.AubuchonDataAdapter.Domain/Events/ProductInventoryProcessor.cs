

using System;
using System.Threading.Tasks;
using Autofac;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Events;
using Mozu.Api.ToolKit.Events;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
   public class ProductInventoryEventProcessor : EventProcessorBase, Api.ToolKit.Events.IEventProcessor
    {
       private readonly ILog _logger = LogManager.GetLogger(typeof(ProductInventoryEventProcessor));
        public async Task ProcessAsync(IComponentContext container, IApiContext apiContext, Event eventPayLoad)
        {
            EventPayLoad = eventPayLoad;
            ApiContext = apiContext;
            Container = container;
            _logger.Info("Processing Product Inventory event");
            var orderEvents = Container.Resolve<IProductInventoryEvents>();
            if (orderEvents == null) throw new ArgumentNullException("IProductInventoryEvents is not registered");
            await ExecuteAsync(orderEvents);
        }

    }

}
