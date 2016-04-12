using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Events;
using Mozu.Api.ToolKit.Events;
using Mozu.Api.ToolKit.Handlers;
using Mozu.Api.ToolKit.Models;
using Newtonsoft.Json;



namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class EventService : IEventService
    {
        
        private readonly ILog _logger = LogManager.GetLogger(typeof(EventService));
        private readonly IComponentContext _container;
        private readonly IEmailHandler _emailHandler;

        public EventService(IComponentContext container, IEmailHandler emailHandler)
        {
            _container = container;
            _emailHandler = emailHandler;
        }

        public async Task ProcessEventAsync(IApiContext apiContext, Event eventPayLoad)
        {
            try
            {
                Trace.CorrelationManager.ActivityId = !String.IsNullOrEmpty(apiContext.CorrelationId)
                    ? Guid.Parse(apiContext.CorrelationId)
                    : Guid.NewGuid();

                _logger.Info(String.Format("Got Event {0} for tenant {1}", eventPayLoad.Topic, apiContext.TenantId));


                var eventType = eventPayLoad.Topic.Split('.');
                var topic = eventType[0];

                if (String.IsNullOrEmpty(topic))
                    throw new ArgumentException("Topic cannot be null or empty");
                Api.ToolKit.Events.IEventProcessor eventProcessor;
                if (topic.Equals("productinventory"))
                {
                    eventProcessor = new ProductInventoryEventProcessor();
                }
                else
                {
                    var eventCategory = (EventCategory) (Enum.Parse(typeof (EventCategory), topic, true));
                    eventProcessor = _container.ResolveKeyed<Api.ToolKit.Events.IEventProcessor>(eventCategory);
                }
                await eventProcessor.ProcessAsync(_container, apiContext, eventPayLoad);
            }
            catch (Exception exc)
            {
                _emailHandler.SendErrorEmail(new ErrorInfo { Message = "Error Processing Event : " + JsonConvert.SerializeObject(eventPayLoad), Context = apiContext, Exception = exc });
                throw;
            }
        }

    }
}
